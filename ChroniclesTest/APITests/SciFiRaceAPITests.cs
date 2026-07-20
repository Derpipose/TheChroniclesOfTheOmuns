using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
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
        db.ScifiRaces.Add(new ScifiRace("Alien", "Divaren", "desc", true, 1, 30, 18, 90));
        await db.SaveChangesAsync();

        var service = new SciFiRaceService(db);
        var request = new CreateScifiRaceDto(
            "Alien", "Divaren", "duplicate name", true, 0, null, 30, 18, 90);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateSciFiRaceAsync(request));
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
