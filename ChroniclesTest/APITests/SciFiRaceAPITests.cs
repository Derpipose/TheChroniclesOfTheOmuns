using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class SciFiRaceServiceTests {

    [Fact]
    public async Task CreateSciFiRaceAsync_CreatesRace_WhenNameIsUnique() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db); // IConfiguration unused by this method

        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                new List<RaceStatModifierDto> {
                    new(StatType.Strength, 2, true) // adjust enum values to match your StatType
                },
                null),
            30, 18, 90);

        var result = await service.CreateSciFiRaceAsync(request);

        Assert.Equal("Divaren", result.Name);
        Assert.Single(db.ScifiRaces);
    }

    [Fact]
    public async Task CreateSciFiRaceAsync_Throws_WhenNameAlreadyExists() {
        await using var db = TestDb.Create();

        var service = new SciFiRaceService(db);
        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "duplicate name", true, 0, null, 30, 18, 90);
        await service.CreateSciFiRaceAsync(request);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateSciFiRaceAsync(request));
    }

    [Fact]
    public async Task CreateSciFiSubraceAsync_CreatesSubrace_WhenNameIsUnique() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.", 1, null);

        var result = await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        Assert.Equal("Hive-Mind", result.SubRaceName);
        Assert.Single(db.ScifiSubraces);
    }

    [Fact]
    public async Task CreateSciFiSubraceAsync_Throws_WhenNameAlreadyExists() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.", 1, null);

        await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateScifiSubraceAsync(race.Id, subraceRequest));
    }
    [Fact]
    public async Task CreateSciFiSubraceAsync_Throws_WhenRaceDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var subraceRequest = new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.", 1, null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateScifiSubraceAsync(999, subraceRequest));
    }
    [Fact]
    public async Task UpdateScifiRace_UpdatesRace_WhenIdExists() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var updateRequest = new UpdateScifiRaceDto(
            "Alien", "NNotDivaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90, true);

        var result = await service.UpdateRace(race.Id, updateRequest);

        Assert.Equal("NNotDivaren", result.Name);
    }
    [Fact]
    public async Task UpdateScifiRace_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var updateRequest = new UpdateScifiRaceDto(
            "Alien", "NNotDivaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90, true);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateRace(999, updateRequest));
    }
    [Fact]
    public async Task UpdateScifiSubrace_UpdatesSubrace_WhenIdExists() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Hive-Mind",
            "A subrace of the Divaren, known for their collective consciousness.", 1, null);

        var subrace = await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        var updateRequest = new UpdateScifiSubraceDto(
            "Hive-Mind Updated",
            "A subrace of the Divaren, known for their collective consciousness. Updated.", 1, null);

        var result = await service.UpdateSubrace(subrace.Id, updateRequest);

        Assert.Equal("Hive-Mind Updated", result.SubRaceName);
    }

    [Fact]
    public async Task UpdateScifiSubrace_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var updateRequest = new UpdateScifiSubraceDto(
            "Hive-Mind Updated",
            "A subrace of the Divaren, known for their collective consciousness. Updated.", 1, null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateSubrace(999, updateRequest));
    }
    [Fact]
    public async Task GetAllRaces_ReturnsAllRaces() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);


        await service.CreateSciFiRaceAsync(request);

        var result = await service.GetAllRaces();

        Assert.Single(result);
        request = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            null, 30, 37, 90);
        await service.CreateSciFiRaceAsync(request);

        result = await service.GetAllRaces();

        Assert.Equal(2, result.Count);
        Assert.Equal(request.Name, result[1].Name);

    }

    [Fact]
    public async Task GetRaceById_ReturnsCorrectRace() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(request);

        var result = await service.GetRaceById(race.Id);

        Assert.Equal(race.Id, result.Id);
        Assert.Equal(request.Name, result.Name);

        request = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            null, 30, 37, 90);
        await service.CreateSciFiRaceAsync(request);
        var result2 = await service.GetRaceById(2);
        Assert.Equal(request.Name, result2.Name);

    }

    [Fact]
    public async Task GetRaceById_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetRaceById(999));
    }
    [Fact]
    public async Task GetRaceByIdWithStats_LoadsModifiersAndFreeStatPicks() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> {
                new(StatType.Strength, 2, true),
                new(StatType.Wisdom, -1, false)
                },
                FreeStatPicks: [1]
            ),
            30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(request);

        var result = await service.GetRaceByIdWithStats(race.Id);

        Assert.NotNull(result.RaceStats);

        var strengthMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Strength);
        Assert.Equal(2, strengthMod.Modifier);
        Assert.True(strengthMod.IsLocked);

        var wisdomMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Wisdom);
        Assert.Equal(-1, wisdomMod.Modifier);
        Assert.False(wisdomMod.IsLocked);

        Assert.Equal(new List<int> { 1 }, result.RaceStats.FreeStatPicks);
    }

    [Fact]
    public async Task GetRaceByIdWithStats_LoadsSharedRaceStats_WhenRaceStatsIdProvided() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        // First race creates a RaceStats row via the DTO path.
        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 18, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);
        var sharedStatsId = firstRace.RaceStatsId;

        // Second race reuses the same RaceStatsId instead of passing a RaceStatsDto.
        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, sharedStatsId,
            null,
            30, 37, 90);

        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        var result = await service.GetRaceByIdWithStats(secondRace.Id);

        Assert.NotNull(result.RaceStats);
        Assert.Equal(sharedStatsId, result.RaceStatsId);

        var dexMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Dexterity);
        Assert.Equal(3, dexMod.Modifier);
        Assert.True(dexMod.IsLocked);
    }
    [Fact]
    public async Task GetAllRacesWithStats_ReturnsAllRacesWithStatsLoaded() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 2, true) },
                FreeStatPicks: new List<int> { 1 }
            ),
            30, 18, 90);

        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);
        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        var result = await service.GetAllRacesWithStats();

        Assert.Equal(2, result.Count);

        var resultFirst = result.Single(r => r.Id == firstRace.Id);
        Assert.NotNull(resultFirst.RaceStats);
        Assert.Equal(StatType.Strength, resultFirst.RaceStats.Modifiers.Single().Stat);

        var resultSecond = result.Single(r => r.Id == secondRace.Id);
        Assert.NotNull(resultSecond.RaceStats);
        Assert.Equal(StatType.Dexterity, resultSecond.RaceStats.Modifiers.Single().Stat);
    }

    [Fact]
    public async Task GetAllRacesWithStats_ReturnsEmptyList_WhenNoRacesExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var result = await service.GetAllRacesWithStats();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSubraceById_ReturnsSubrace_WhenSubraceExists() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var blazeRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var nullRequest = new CreateScifiSubraceDto(
            "Null", "A Tasawii variant more attuned to the void than the material plane.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 }
            ));

        var blaze = await service.CreateScifiSubraceAsync(race.Id, blazeRequest);
        await service.CreateScifiSubraceAsync(race.Id, nullRequest);

        var result = await service.GetSubraceById(blaze.Id);

        Assert.NotNull(result);
        Assert.Equal(blaze.Id, result.Id);
        Assert.Equal(race.Id, result.ScifiRaceId);
        Assert.Equal("Blaze", result.SubRaceName);
    }

    [Fact]
    public async Task GetSubraceByRaceId_ReturnsSubraces_WhenSubracesExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var blazeRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var nullRequest = new CreateScifiSubraceDto(
            "Null", "A Tasawii variant more attuned to the void than the material plane.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 }
            ));

        await service.CreateScifiSubraceAsync(race.Id, blazeRequest);
        await service.CreateScifiSubraceAsync(race.Id, nullRequest);

        var result = await service.GetSubraceByRaceId(race.Id);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Blaze", result[0].SubRaceName);
        Assert.Equal("Null", result[1].SubRaceName);
    }
    [Fact]
    public async Task GetSubraceByIdWithStats_ReturnsSubraceWithStats() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var blazeRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var blaze = await service.CreateScifiSubraceAsync(race.Id, blazeRequest);

        var result = await service.GetSubraceByIdWithStats(blaze.Id);

        Assert.Equal(blaze.Id, result.Id);
        Assert.NotNull(result.RaceStats);
        var strMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Strength);
        Assert.Equal(1, strMod.Modifier);
        Assert.Equal(new List<int> { 3 }, result.RaceStats.FreeStatPicks);
    }

    [Fact]
    public async Task GetSubraceByIdWithStats_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetSubraceByIdWithStats(999));
    }

    [Fact]
    public async Task GetSubraceByRaceWithStats_ReturnsFirstSubraceForRace() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var blazeRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var nullRequest = new CreateScifiSubraceDto(
            "Null", "A dampened, energy-suppressed Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 }
            ));

        var blaze = await service.CreateScifiSubraceAsync(race.Id, blazeRequest);
        await service.CreateScifiSubraceAsync(race.Id, nullRequest);

        var result = await service.GetSubraceByRaceWithStats(race.Id);

        Assert.Equal(race.Id, result.ScifiRaceId);
        Assert.NotNull(result.RaceStats);
        // Only true because Blaze was created first and FirstOrDefaultAsync
        // has no explicit ordering — see note below.
        Assert.Equal(blaze.Id, result.Id);
    }

    [Fact]
    public async Task GetSubraceByRaceWithStats_Throws_WhenRaceHasNoSubraces() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            null, 30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetSubraceByRaceWithStats(race.Id));
    }

    [Fact]
    public async Task DeleteRaceAsync_RemovesRaceAndSubraces_AndClearsCharacterReferences() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var subrace = await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        // Character attached directly via race (no subrace)
        var raceCharacterBase = new Character(1, "Zorak");
        raceCharacterBase.SetMaxHealth(10);
        raceCharacterBase.SetLevel(1);
        db.Characters.Add(raceCharacterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var raceOnlyCharacter = new ScifiCharacter(raceCharacterBase.Id);
        raceOnlyCharacter.SetScifiRace(race.Id);
        db.ScifiCharacters.Add(raceOnlyCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Character attached via subrace
        var subraceCharacterBase = new Character(1, "Ember");
        subraceCharacterBase.SetMaxHealth(10);
        subraceCharacterBase.SetLevel(1);
        db.Characters.Add(subraceCharacterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var subraceCharacter = new ScifiCharacter(subraceCharacterBase.Id);
        subraceCharacter.SetScifiRace(race.Id);
        subraceCharacter.SetScifiSubrace(subrace.Id);
        db.ScifiCharacters.Add(subraceCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await service.DeleteRaceAsync(race.Id);

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
        var service = new SciFiRaceService(db);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteRaceAsync(999));
    }

    [Fact]
    public async Task DeleteSubraceAsync_RemovesSubrace_AndClearsCharacterSubraceOnly() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var subrace = await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        var characterBase = new Character(1, "Ember");
        characterBase.SetMaxHealth(10);
        characterBase.SetLevel(1);
        db.Characters.Add(characterBase);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var scifiCharacter = new ScifiCharacter(characterBase.Id);
        scifiCharacter.SetScifiRace(race.Id);
        scifiCharacter.SetScifiSubrace(subrace.Id);
        db.ScifiCharacters.Add(scifiCharacter);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        await service.DeleteSubraceAsync(subrace.Id);

        Assert.False(await db.ScifiSubraces.AnyAsync(s => s.Id == subrace.Id, cancellationToken: TestContext.Current.CancellationToken));
        Assert.True(await db.ScifiRaces.AnyAsync(r => r.Id == race.Id, cancellationToken: TestContext.Current.CancellationToken));

        var reloaded = await db.ScifiCharacters.FirstAsync(c => c.Id == scifiCharacter.Id, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(reloaded.ScifiSubraceId);
        Assert.Equal(race.Id, reloaded.ScifiRaceId); // race reference should survive
    }

    [Fact]
    public async Task DeleteSubraceAsync_Throws_WhenSubraceDoesNotExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteSubraceAsync(999));
    }

    [Fact]
    public async Task ResolveRaceStatsIdAsync_ReusesExistingStats_RegardlessOfModifierOrder() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        // First race establishes a RaceStats row with modifiers in one order
        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> {
                new(StatType.Strength, 2, true),
                new(StatType.Wisdom, -1, false)
                },
                FreeStatPicks: new List<int> { 1, 2 }
            ),
            30, 37, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);

        // Second race submits the SAME modifiers/picks but in reversed order
        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> {
                new(StatType.Wisdom, -1, false),
                new(StatType.Strength, 2, true)
                },
                FreeStatPicks: new List<int> { 2, 1 }
            ),
            30, 18, 90);

        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedFirst = await db.ScifiRaces.FirstAsync(r => r.Id == firstRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedSecond = await db.ScifiRaces.FirstAsync(r => r.Id == secondRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        // Same RaceStats row should be reused, not duplicated, despite the different order.
        // This kills the OrderBy -> OrderByDescending mutants: without the OrderBy call,
        // order-sensitive comparison would treat these as different and create a duplicate row.
        Assert.Equal(reloadedFirst.RaceStatsId, reloadedSecond.RaceStatsId);

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var statsCount = await db.RaceStats.CountAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Equal(1, statsCount);
    }

    [Fact]
    public async Task ResolveRaceStatsIdAsync_CreatesNewStats_WhenModifiersDiffer() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 2, true) },
                FreeStatPicks: new List<int> { 1 }
            ),
            30, 37, 90);

        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 2, true) }, // different stat
                FreeStatPicks: new List<int> { 1 } // same picks
            ),
            30, 18, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);
        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedFirst = await db.ScifiRaces.FirstAsync(r => r.Id == firstRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedSecond = await db.ScifiRaces.FirstAsync(r => r.Id == secondRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        // Modifiers differ but FreeStatPicks match. If the SequenceEqual check used ||
        // instead of &&, this pair would incorrectly be treated as a match.
        Assert.NotEqual(reloadedFirst.RaceStatsId, reloadedSecond.RaceStatsId);

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var statsCount = await db.RaceStats.CountAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Equal(2, statsCount);
    }

    [Fact]
    public async Task ResolveRaceStatsIdAsync_CreatesNewStats_WhenFreeStatPicksDiffer() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 2, true) }, // same modifiers
                FreeStatPicks: new List<int> { 1 }
            ),
            30, 37, 90);

        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 2, true) },
                FreeStatPicks: new List<int> { 2 } // different picks
            ),
            30, 18, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);
        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedFirst = await db.ScifiRaces.FirstAsync(r => r.Id == firstRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedSecond = await db.ScifiRaces.FirstAsync(r => r.Id == secondRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        Assert.NotEqual(reloadedFirst.RaceStatsId, reloadedSecond.RaceStatsId);
    }

    [Fact]
    public async Task ResolveRaceStatsIdAsync_TreatsNullModifiersAndPicks_AsEmptyLists() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        // First race has explicit empty lists
        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto>(),
                FreeStatPicks: new List<int>()
            ),
            30, 37, 90);

        // Second race has null lists instead - this kills the "?? new List<>()" mutants,
        // since a null-coalescing removal would throw an NRE on .Select()/.OrderBy() below
        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: null,
                FreeStatPicks: null
            ),
            30, 18, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);
        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedFirst = await db.ScifiRaces.FirstAsync(r => r.Id == firstRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedSecond = await db.ScifiRaces.FirstAsync(r => r.Id == secondRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        // null and empty-list should be treated as equivalent, resulting in stats reuse
        Assert.Equal(reloadedFirst.RaceStatsId, reloadedSecond.RaceStatsId);
    }

    [Fact]
    public async Task ResolveRaceStatsIdAsync_ReturnsFallbackId_WhenRaceStatsDtoIsNull() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        // Create a real RaceStats row via the DTO path first
        var firstRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 2, true) },
                FreeStatPicks: new List<int> { 1 }
            ),
            30, 37, 90);

        var firstRace = await service.CreateSciFiRaceAsync(firstRequest);

        // Second race passes RaceStats: null and instead reuses the existing id via fallback
        var secondRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, firstRace.RaceStatsId,
            null,
            30, 18, 90);

        var secondRace = await service.CreateSciFiRaceAsync(secondRequest);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedSecond = await db.ScifiRaces.FirstAsync(r => r.Id == secondRace.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        Assert.Equal(firstRace.RaceStatsId, reloadedSecond.RaceStatsId);

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var statsCount = await db.RaceStats.CountAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Equal(1, statsCount); // confirms no new row was created
    }

    [Fact]
    public async Task DeleteRaceAsync_PersistsRemovalOfRaceAndSubraces() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var subraceRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var subrace = await service.CreateScifiSubraceAsync(race.Id, subraceRequest);

        var characterBase = new Character(1, "Zorak");
        characterBase.SetMaxHealth(10);
        characterBase.SetLevel(1);
        db.Characters.Add(characterBase);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        var scifiCharacter = new ScifiCharacter(characterBase.Id);
        scifiCharacter.SetScifiRace(race.Id);
        scifiCharacter.SetScifiSubrace(subrace.Id);
        db.ScifiCharacters.Add(scifiCharacter);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        await service.DeleteRaceAsync(race.Id);

        // Detach everything so these queries can't be served from the change tracker's
        // cached state - this is what kills the "delete the SaveChangesAsync() line" mutant.
        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.False(await db.ScifiRaces.AnyAsync(r => r.Id == race.Id));
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.False(await db.ScifiSubraces.AnyAsync(s => s.Id == subrace.Id));
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedCharacter = await db.ScifiCharacters.FirstAsync(c => c.Id == scifiCharacter.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Null(reloadedCharacter.ScifiRaceId);
        Assert.Null(reloadedCharacter.ScifiSubraceId);
    }

    [Fact]
    public async Task DeleteRaceAsync_ClearsOnlyAffectedCharacters_NotUnrelatedOnes() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var unrelatedRaceRequest = new CreateScifiRaceDto(
            "Alien", "Divaren", "A short squat bearded race similar to humans.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 }
            ),
            30, 18, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);
        var unrelatedRace = await service.CreateSciFiRaceAsync(unrelatedRaceRequest);

        var characterBase = new Character(1, "Zorak");
        characterBase.SetMaxHealth(10);
        characterBase.SetLevel(1);
        db.Characters.Add(characterBase);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        // This character belongs to a DIFFERENT race and should NOT be touched
        var unrelatedCharacter = new ScifiCharacter(characterBase.Id);
        unrelatedCharacter.SetScifiRace(unrelatedRace.Id);
        db.ScifiCharacters.Add(unrelatedCharacter);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        await service.DeleteRaceAsync(race.Id);

        db.ChangeTracker.Clear();

        // This kills mutants that flip the Where clause's || to && or negate the
        // subrace-id-contains check: those mutations would incorrectly sweep in
        // this unrelated character too.
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedUnrelated = await db.ScifiCharacters.FirstAsync(c => c.Id == unrelatedCharacter.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Equal(unrelatedRace.Id, reloadedUnrelated.ScifiRaceId);
    }

    [Fact]
    public async Task DeleteSubraceAsync_PersistsRemoval_AndClearsOnlyAffectedCharacters() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var raceRequest = new CreateScifiRaceDto(
            "Alien", "Tasawii", "Energy Cloud in a suit of armor.",
            true, 0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Dexterity, 3, true) },
                FreeStatPicks: new List<int> { 2 }
            ),
            30, 37, 90);

        var race = await service.CreateSciFiRaceAsync(raceRequest);

        var blazeRequest = new CreateScifiSubraceDto(
            "Blaze", "A volatile, fire-attuned Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Strength, 1, true) },
                FreeStatPicks: new List<int> { 3 }
            ));

        var nullRequest = new CreateScifiSubraceDto(
            "Null", "A dampened, energy-suppressed Tasawii variant.",
            0,
            new RaceStatsDto(
                Modifiers: new List<RaceStatModifierDto> { new(StatType.Wisdom, 1, true) },
                FreeStatPicks: new List<int> { 4 }
            ));

        var blaze = await service.CreateScifiSubraceAsync(race.Id, blazeRequest);
        var nullSubrace = await service.CreateScifiSubraceAsync(race.Id, nullRequest);

        var blazeCharBase = new Character(1, "Ember");
        blazeCharBase.SetMaxHealth(10);
        blazeCharBase.SetLevel(1);
        db.Characters.Add(blazeCharBase);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        var blazeCharacter = new ScifiCharacter(blazeCharBase.Id);
        blazeCharacter.SetScifiRace(race.Id);
        blazeCharacter.SetScifiSubrace(blaze.Id);
        db.ScifiCharacters.Add(blazeCharacter);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        // A different character tied to the OTHER subrace - should survive untouched
        var nullCharBase = new Character(1, "Frost");
        nullCharBase.SetMaxHealth(10);
        nullCharBase.SetLevel(1);
        db.Characters.Add(nullCharBase);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        var nullCharacter = new ScifiCharacter(nullCharBase.Id);
        nullCharacter.SetScifiRace(race.Id);
        nullCharacter.SetScifiSubrace(nullSubrace.Id);
        db.ScifiCharacters.Add(nullCharacter);
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        await db.SaveChangesAsync();
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

        await service.DeleteSubraceAsync(blaze.Id);

        db.ChangeTracker.Clear();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.False(await db.ScifiSubraces.AnyAsync(s => s.Id == blaze.Id));
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.True(await db.ScifiSubraces.AnyAsync(s => s.Id == nullSubrace.Id));
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedBlazeChar = await db.ScifiCharacters.FirstAsync(c => c.Id == blazeCharacter.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Null(reloadedBlazeChar.ScifiSubraceId);
        Assert.Equal(race.Id, reloadedBlazeChar.ScifiRaceId);

        // This kills the "== subraceId" -> "!= subraceId" mutant: that mutation
        // would incorrectly clear THIS character's subrace instead.
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        var reloadedNullChar = await db.ScifiCharacters.FirstAsync(c => c.Id == nullCharacter.Id);
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.Equal(nullSubrace.Id, reloadedNullChar.ScifiSubraceId);
    }

}

public static class TestDb {
    public static AppDbContext Create() {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique per test = isolated
            .Options;

        return new AppDbContext(options);
    }
}
