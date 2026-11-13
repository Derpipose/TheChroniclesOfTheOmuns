using System.Security.Claims;
using ChroniclesAPI.Controllers;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
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
        Assert.Equal(38, character.BaseHealth);
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
        Assert.Equal(42, finalCharacter.BaseHealth);
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

        Assert.Equal(0, character.BaseHealth);
    }
}

public class ScifiCharacterUpdateEndpointTests {
    private const int PlayerId = 1;

    private static ScifiCharacterController CreateController(AppDbContext db) {
        var context = new DefaultHttpContext {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, PlayerId.ToString())], "test"))
        };
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return new(db, new PlayerContext(accessor), new ScifiTalentService(db));
    }

    [Fact]
    public async Task Update_SameRaceSubraceAndPicksTwice_LeavesExactlyOneSetOfRaceModifiers() {
        await using var db = TestDb.Create();

        // Race: fixed Strength +2 and one free pick of +1. Subrace has no stats of its own, so the race's stats win.
        var raceStats = new RaceStats(new List<RaceStatModifier> { new(StatType.Strength, 2, false) }, new List<int> { 1 });
        var subraceStats = new RaceStats(null, null);
        db.RaceStats.AddRange(raceStats, subraceStats);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var race = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        db.ScifiRaces.Add(race);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var subrace = new ScifiSubrace(race.Id, "TestSubrace", "Test description", subraceStats.Id);
        db.ScifiSubraces.Add(subrace);

        var character = new Character(PlayerId, "Rook");
        db.Characters.Add(character);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sc = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(sc);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var request = new ScifiCharacterController.UpdateScifiCharacterDto(
            race.Id, subrace.Id, null, null,
            [new ScifiCharacterController.StatPickDto(StatType.Wisdom, 1)]);

        // Each call is a fresh request: new controller, nothing tracked
        for (var i = 0; i < 2; i++) {
            db.ChangeTracker.Clear();
            var result = await CreateController(db).Update(sc.Id, request);
            Assert.IsType<OkObjectResult>(result);
        }

        db.ChangeTracker.Clear();
        var stored = await db.ScifiCharacterModifiers.AsNoTracking()
            .Where(m => m.ScifiCharacterId == sc.Id)
            .ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, stored.Count);
        Assert.Single(stored, m => m.ModifierType == ModifierType.Strength && m.Amount == 2 && m.Source == ModifierSource.Race);
        Assert.Single(stored, m => m.ModifierType == ModifierType.Wisdom && m.Amount == 1 && m.Source == ModifierSource.RacePick);
    }
}
