using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Dice;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Database;

namespace ChroniclesTest;

public class ScifiCharacterHealthTests {

    private static async Task<(ScifiClass classA, ScifiClass classB)> SeedTwoClasses(AppDbContext db) {
        var classA = new ScifiClass("Trooper", DiceType.D8);
        var classB = new ScifiClass("Vanguard", DiceType.D12);
        db.ScifiClasses.AddRange(classA, classB);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        return (classA, classB);
    }

    [Fact]
    public async Task UpdateHealth_UsesAssignedClass_WhenClassSetAndSavedInSameContext() {
        await using var db = TestDb.Create();
        var (classA, _) = await SeedTwoClasses(db);

        var character = new Character(1, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var stats = new CharacterStats(character.Id);
        stats.SetStat(StatType.Constitution, 14, character);
        db.CharacterStats.Add(stats);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var scifiCharacter = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        scifiCharacter.SetScifiClass(classA.Id);
        scifiCharacter.UpdateHealth(stats, classA);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // conMod for CON 14 = (14-10)/2 = 2; health = 14*2 + 8 (D8) + 2 = 38
        Assert.Equal(38, character.MaxHealth);
    }

    [Fact]
    public async Task UpdateHealth_ReflectsNewClass_AfterReloadFromDatabase() {
        // This is the case that matters: does SetScifiClass's FK-only assignment
        // survive a real reload, or does UpdateHealth silently see a stale/null ScifiClass?
        await using var db = TestDb.Create();
        var (classA, classB) = await SeedTwoClasses(db);

        var character = new Character(1, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var stats = new CharacterStats(character.Id);
        stats.SetStat(StatType.Constitution, 14, character);
        db.CharacterStats.Add(stats);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var scifiCharacter = new ScifiCharacter(character.Id);
        scifiCharacter.SetScifiClass(classA.Id);
        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Simulate a fresh request: clear tracked state, reload from "the database"
        db.ChangeTracker.Clear();

        var reloaded = await db.ScifiCharacters
            .Include(sc => sc.Character)
            .Include(sc => sc.ScifiClass)
            .FirstAsync(sc => sc.Id == scifiCharacter.Id, TestContext.Current.CancellationToken);

        var reloadedStats = await db.CharacterStats
            .FirstAsync(cs => cs.CharacterId == reloaded.CharacterId, TestContext.Current.CancellationToken);

        // Change class to B, same as the controller's Update flow
        reloaded.SetScifiClass(classB.Id);
        reloaded.UpdateHealth(reloadedStats, classB);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        db.ChangeTracker.Clear();
        var finalCharacter = await db.Characters
            .FirstAsync(c => c.Id == character.Id, TestContext.Current.CancellationToken);

        // conMod = 2; health = 14*2 + 12 (D12) + 2 = 42, NOT the D8 value (38)
        Assert.Equal(42, finalCharacter.MaxHealth);
    }

    [Fact]
    public async Task UpdateHealth_DoesNothing_WhenScifiClassIsNull() {
        await using var db = TestDb.Create();

        var character = new Character(1, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var stats = new CharacterStats(character.Id);
        db.CharacterStats.Add(stats);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var scifiCharacter = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        scifiCharacter.UpdateHealth(stats, null);

        Assert.Equal(0, character.MaxHealth);
    }
}