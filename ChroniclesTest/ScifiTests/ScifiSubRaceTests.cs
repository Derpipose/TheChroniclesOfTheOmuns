using Xunit;
using PlayerApp.Models;

namespace ChroniclesTest;

public class ScifiSubraceTests {

    [Fact]
    public void CreateScifiSubrace() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Gears", description: "", raceStatsId: 10
        );

        Assert.Equal(1, subrace.ScifiRaceId);
        Assert.Equal("House Gears", subrace.SubRaceName);
        Assert.Equal("", subrace.Description);
        Assert.Equal(10, subrace.RaceStatsId);
    }

    [Fact]
    public void SubraceCanHaveADescription() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Thunder",
            description: "Known for their booming voices and short tempers.", raceStatsId: 11
        );

        Assert.Equal("Known for their booming voices and short tempers.", subrace.Description);
    }

    [Fact]
    public void ThrowsWhenScifiRaceIdIsZero() {
        Assert.Throws<InvalidOperationException>(() =>
            new ScifiSubrace(scifiRaceId: 0, subRaceName: "House Twilight", description: "", raceStatsId: 12));
    }

    [Fact]
    public void ThrowsWhenRaceStatsIdIsZero() {
        Assert.Throws<InvalidOperationException>(() =>
            new ScifiSubrace(scifiRaceId: 1, subRaceName: "House Remnants", description: "", raceStatsId: 0));
    }

    [Fact]
    public void IdDefaultsToZeroBeforePersist() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Gears", description: "", raceStatsId: 10
        );

        Assert.Equal(0, subrace.Id);
    }

    [Fact]
    public void SimulatePersistSetsId() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Gears", description: "", raceStatsId: 10
        );

        subrace.SimulatePersist(5);

        Assert.Equal(5, subrace.Id);
    }

    [Fact]
    public void RaceStatsIdLinksToCorrectRaceStats() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Remnants", description: "", raceStatsId: 42
        );

        Assert.Equal(42, subrace.RaceStatsId);
    }

    [Fact]
    public void ScifiRaceIdLinksToCorrectParentRace() {
        ScifiSubrace subrace = new ScifiSubrace(
            scifiRaceId: 7, subRaceName: "House Gears", description: "", raceStatsId: 10
        );

        Assert.Equal(7, subrace.ScifiRaceId);
    }

    [Fact]
    public void DifferentSubracesCanShareTheSameParentRace() {
        ScifiSubrace gears = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Gears", description: "", raceStatsId: 10
        );
        ScifiSubrace thunder = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Thunder", description: "", raceStatsId: 11
        );

        Assert.Equal(gears.ScifiRaceId, thunder.ScifiRaceId);
        Assert.NotEqual(gears.SubRaceName, thunder.SubRaceName);
        Assert.NotEqual(gears.RaceStatsId, thunder.RaceStatsId);
    }

    [Fact]
    public void DifferentSubracesCanShareTheSameRaceStats() {
        ScifiSubrace gears = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Gears", description: "", raceStatsId: 10
        );
        ScifiSubrace thunder = new ScifiSubrace(
            scifiRaceId: 1, subRaceName: "House Thunder", description: "", raceStatsId: 10
        );

        Assert.Equal(gears.RaceStatsId, thunder.RaceStatsId);
    }
}