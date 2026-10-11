using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class SciFiSubraceServiceTests {
    [Fact]
    public async Task CreateSciFiSubraceAsync_CreatesSubrace_WhenNameIsUnique() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Divaren",
            "A short squat bearded race similar to humans.",
            true,
            0,
            null,
            30,
            18,
            90));

        var result = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.",
            1,
            null));

        Assert.Equal("Hive-Mind", result.SubRaceName);
        Assert.Single(db.ScifiSubraces);
    }

    [Fact]
    public async Task CreateSciFiSubraceAsync_Throws_WhenNameAlreadyExists() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Divaren",
            "A short squat bearded race similar to humans.",
            true,
            0,
            null,
            30,
            18,
            90));

        var request = new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.",
            1,
            null);

        await subraceService.CreateScifiSubraceAsync(race.Id, request);

        await Assert.ThrowsAsync<ConflictException>(() => subraceService.CreateScifiSubraceAsync(race.Id, request));
    }

    [Fact]
    public async Task CreateSciFiSubraceAsync_Throws_WhenRaceDoesNotExist() {
        await using var db = TestDb.Create();
        var subraceService = CreateSubraceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => subraceService.CreateScifiSubraceAsync(999, new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.",
            1,
            null)));
    }

    [Fact]
    public async Task UpdateScifiSubrace_UpdatesSubrace_WhenIdExists() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Divaren",
            "A short squat bearded race similar to humans.",
            true,
            0,
            null,
            30,
            18,
            90));

        var subrace = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.",
            1,
            null));

        var result = await subraceService.UpdateSubrace(subrace.Id, new UpdateScifiSubraceDto(
            "Hive-Mind Updated",
            "A subrace of the Divaren, known for their collective consciousness. Updated.",
            1,
            null));

        Assert.Equal("Hive-Mind Updated", result.SubRaceName);
    }

    [Fact]
    public async Task UpdateScifiSubrace_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var subraceService = CreateSubraceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => subraceService.UpdateSubrace(999, new UpdateScifiSubraceDto(
            "Hive-Mind Updated",
            "A subrace of the Divaren, known for their collective consciousness. Updated.",
            1,
            null)));
    }

    [Fact]
    public async Task GetSubraceById_ReturnsSubrace_WhenSubraceExists() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        var blaze = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Null",
            "A Tasawii variant more attuned to the void than the material plane.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 })));

        var result = await subraceService.GetSubraceById(blaze.Id);

        Assert.NotNull(result);
        Assert.Equal(blaze.Id, result.Id);
        Assert.Equal(race.Id, result.ScifiRaceId);
        Assert.Equal("Blaze", result.SubRaceName);
    }

    [Fact]
    public async Task GetSubraceByRaceId_ReturnsSubraces_WhenSubracesExist() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Null",
            "A Tasawii variant more attuned to the void than the material plane.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 })));

        var result = await subraceService.GetSubraceByRaceId(race.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Blaze", result[0].SubRaceName);
        Assert.Equal("Null", result[1].SubRaceName);
    }

    [Fact]
    public async Task GetSubraceByIdWithStats_ReturnsSubraceWithStats() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        var blaze = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        var result = await subraceService.GetSubraceByIdWithStats(blaze.Id);

        Assert.Equal(blaze.Id, result.Id);
        Assert.NotNull(result.RaceStats);
#pragma warning disable CS8604
        var strMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Strength);
#pragma warning restore CS8604
        Assert.Equal(1, strMod.Modifier);
        Assert.Equal(new List<int> { 3 }, result.RaceStats.FreeStatPicks);
    }

    [Fact]
    public async Task GetSubraceByIdWithStats_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var subraceService = CreateSubraceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => subraceService.GetSubraceByIdWithStats(999));
    }

    [Fact]
    public async Task GetSubraceByRaceWithStats_ReturnsFirstSubraceForRace() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        var blaze = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Null",
            "A dampened, energy-suppressed Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 })));

        var result = await subraceService.GetSubraceByRaceWithStats(race.Id);

        Assert.Equal(race.Id, result[0].ScifiRaceId);
        Assert.NotNull(result[0].RaceStats);
        Assert.Equal(blaze.Id, result[0].Id);
    }

    

    [Fact]
    public async Task DeleteRaceAsync_RemovesRaceAndSubraces_AndClearsCharacterReferences() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        var subrace = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        var raceCharacterBase = new Character(1, "Zorak");
        raceCharacterBase.SetBaseHealth(10);
        raceCharacterBase.SetLevel(1);
        db.Characters.Add(raceCharacterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var raceOnlyCharacter = new ScifiCharacter(raceCharacterBase.Id);
        raceOnlyCharacter.SetScifiRace(race, []);
        db.ScifiCharacters.Add(raceOnlyCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var subraceCharacterBase = new Character(1, "Ember");
        subraceCharacterBase.SetBaseHealth(10);
        subraceCharacterBase.SetLevel(1);
        db.Characters.Add(subraceCharacterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var subraceCharacter = new ScifiCharacter(subraceCharacterBase.Id);
        subraceCharacter.SetScifiRace(race, [], subrace);
        db.ScifiCharacters.Add(subraceCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await subraceService.DeleteRaceAsync(race.Id);

        Assert.False(await db.ScifiRaces.AnyAsync(r => r.Id == race.Id, cancellationToken: TestContext.Current.CancellationToken));
        Assert.False(await db.ScifiSubraces.AnyAsync(s => s.Id == subrace.Id, cancellationToken: TestContext.Current.CancellationToken));

        var reloadedRaceOnly = await db.ScifiCharacters.FirstAsync(c => c.Id == raceOnlyCharacter.Id, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(reloadedRaceOnly.ScifiRaceId);
        Assert.Null(reloadedRaceOnly.ScifiSubraceId);

        var reloadedSubraceChar = await db.ScifiCharacters.FirstAsync(c => c.Id == subraceCharacter.Id, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(reloadedSubraceChar.ScifiRaceId);
        Assert.Null(reloadedSubraceChar.ScifiSubraceId);
    }

    [Fact]
    public async Task DeleteRaceAsync_Throws_WhenRaceDoesNotExist() {
        await using var db = TestDb.Create();
        var subraceService = CreateSubraceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => subraceService.DeleteRaceAsync(999));
    }

    [Fact]
    public async Task DeleteSubraceAsync_RemovesSubrace_AndClearsCharacterSubraceOnly() {
        await using var db = TestDb.Create();
        var raceService = CreateRaceService(db);
        var subraceService = CreateSubraceService(db);

        var race = await raceService.CreateSciFiRaceAsync(new CreateScifiRaceDto(
            "Alien",
            "Tasawii",
            "Energy Cloud in a suit of armor.",
            true,
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }),
            30,
            37,
            90));

        var subrace = await subraceService.CreateScifiSubraceAsync(race.Id, new CreateScifiSubraceDto(
            "Blaze",
            "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 })));

        var characterBase = new Character(1, "Ember");
        characterBase.SetBaseHealth(10);
        characterBase.SetLevel(1);
        db.Characters.Add(characterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var scifiCharacter = new ScifiCharacter(characterBase.Id);
        scifiCharacter.SetScifiRace(race, [(StatType.Dexterity, 3)], subrace);
        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await subraceService.DeleteSubraceAsync(subrace.Id);

        Assert.False(await db.ScifiSubraces.AnyAsync(s => s.Id == subrace.Id, cancellationToken: TestContext.Current.CancellationToken));
        Assert.True(await db.ScifiRaces.AnyAsync(r => r.Id == race.Id, cancellationToken: TestContext.Current.CancellationToken));

        var reloaded = await db.ScifiCharacters.FirstAsync(c => c.Id == scifiCharacter.Id, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(reloaded.ScifiSubraceId);
        Assert.Equal(race.Id, reloaded.ScifiRaceId);
    }

    [Fact]
    public async Task DeleteSubraceAsync_Throws_WhenSubraceDoesNotExist() {
        await using var db = TestDb.Create();
        var subraceService = CreateSubraceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => subraceService.DeleteSubraceAsync(999));
    }

    private static SciFiRaceService CreateRaceService(AppDbContext db) =>
        new(db);

    private static SciFiSubraceService CreateSubraceService(AppDbContext db) =>
        new(db);
}
