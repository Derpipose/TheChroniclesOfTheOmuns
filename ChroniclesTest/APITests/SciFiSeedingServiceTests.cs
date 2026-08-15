using System.Text.Json;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class SciFiSeedingServiceTests {
    [Fact]
    public async Task SeedScifiRacesFromFileAsync_SeedsAllRaces_FromRealSeedFile() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, new SciFiSubraceService(db), new ScifiWeaponService(db));

        await seedService.SeedRacesAsync();
        int expectedCount = 26;
        Assert.True(db.ScifiRaces.Any());
        Assert.Contains(db.ScifiRaces, r => r.Name == "Divaren");
        Assert.Equal(expectedCount, db.ScifiRaces.Count());
    }

    [Fact]
    public async Task SeedScifiRacesFromFileAsync_IsIdempotent_OnSecondRun() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(),
        raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();

        await seedService.SeedRacesAsync();
        var countAfterFirstRun = db.ScifiRaces.Count();

        await seedService.SeedRacesAsync();
        var countAfterSecondRun = db.ScifiRaces.Count();

        Assert.Equal(countAfterFirstRun, countAfterSecondRun);
    }

    [Fact]
    public async Task SeedScifiSubracesFromFileAsync_SeedsAllSubraces_FromRealSeedFile() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();
        await seedService.SeedSubracesAsync();

        int expectedCount = 38;

        Assert.Equal(expectedCount, db.ScifiSubraces.Count());
        Assert.Contains(db.ScifiSubraces, s => s.SubRaceName == "House Gears");
    }

    [Fact]
    public async Task SeedScifiSubracesFromFileAsync_IsIdempotent_OnSecondRun() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();
        await seedService.SeedSubracesAsync();
        var countAfterFirstRun = db.ScifiSubraces.Count();

        await seedService.SeedSubracesAsync();
        var countAfterSecondRun = db.ScifiSubraces.Count();

        Assert.Equal(countAfterFirstRun, countAfterSecondRun);
    }

    // [Fact]
    // public async Task SeedScifiSubracesFromFileAsync_Throws_WhenParentRaceNotYetSeeded() {
    //     await using var db = TestDb.Create();
    //     var raceService = new SciFiRaceService(db);
    //     var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, new SciFiSubraceService(db));

    //     await seedService.SeedRacesAsync();

    //     await Assert.ThrowsAsync<InvalidOperationException>(() => seedService.SeedSubracesAsync());
    // }

    [Fact]
    public async Task SeedScifiSubracesFromFileAsync_ParsesLockedStat_FromLPrefixedValue() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();
        await seedService.SeedRacesAsync();
        await seedService.SeedSubracesAsync();

        var knight = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstAsync(s => s.SubRaceName == "Knight", cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(knight.RaceStats);
        Assert.Equal(18, knight.RaceStats!.GetModifierFor(StatType.Strength));
        Assert.True(knight.RaceStats.IsStatLocked(StatType.Strength));
        Assert.Equal(18, knight.RaceStats.GetModifierFor(StatType.Constitution));
        Assert.True(knight.RaceStats.IsStatLocked(StatType.Constitution));
    }

    [Fact]
    public async Task SeedScifiSubracesFromFileAsync_ParsesCommaListPick_IntoFreeStatPicks() {
        await using var db = TestDb.Create();
        var raceService = new SciFiRaceService(db);
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();
        await seedService.SeedSubracesAsync();

        var highTech = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstAsync(s => s.SubRaceName == "High Tech", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(new List<int> { 1, 2, 2, 3 }, highTech.RaceStats!.FreeStatPicks);
    }

    [Fact]
    public async Task SeedScifiSubracesFromFileAsync_ParsesBareNumberPick_IntoFreeStatPicks() {
        await using var db = TestDb.Create();
        var subraceService = new SciFiSubraceService(db);
        var weaponService = new ScifiWeaponService(db);
        var raceService = new SciFiRaceService(db);
        var seedService = new ScifiSeedingService(db, new TestHttpClientFactory(), raceService, subraceService, weaponService);

        await seedService.SeedRacesAsync();

        await seedService.SeedSubracesAsync();

        var nilbog = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstAsync(s => s.SubRaceName == "Nilbog", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(new List<int> { 2 }, nilbog.RaceStats!.FreeStatPicks);
    }
}

internal sealed class TestHttpClientFactory : IHttpClientFactory {
    public HttpClient CreateClient(string name) => new();
}
