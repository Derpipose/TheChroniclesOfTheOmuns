using System.Text.Json;
using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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

#pragma warning disable CS8604 // Possible null reference argument.
        var strengthMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Strength);
#pragma warning restore CS8604 // Possible null reference argument.
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

#pragma warning disable CS8604 // Possible null reference argument.
        var dexMod = result.RaceStats.Modifiers.Single(m => m.Stat == StatType.Dexterity);
#pragma warning restore CS8604 // Possible null reference argument.
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
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Equal(StatType.Strength, resultFirst.RaceStats.Modifiers.Single().Stat);
#pragma warning restore CS8604 // Possible null reference argument.

        var resultSecond = result.Single(r => r.Id == secondRace.Id);
        Assert.NotNull(resultSecond.RaceStats);
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Equal(StatType.Dexterity, resultSecond.RaceStats.Modifiers.Single().Stat);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task GetAllRacesWithStats_ReturnsEmptyList_WhenNoRacesExist() {
        await using var db = TestDb.Create();
        var service = new SciFiRaceService(db);

        var result = await service.GetAllRacesWithStats();

        Assert.Empty(result);
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
