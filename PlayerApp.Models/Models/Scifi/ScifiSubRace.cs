using System;
using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiSubrace {
    public int Id { get; private set; }
    public int ScifiRaceId { get; private set; }
    public string SubRaceName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int RaceStatsId { get; private set; }

    public ScifiSubrace(int scifiRaceId, string subRaceName, string description, int raceStatsId) {
        if (scifiRaceId == 0)
            throw new InvalidOperationException(
                "ScifiRace must be persisted before creating a ScifiSubrace.");
        if (raceStatsId == 0)
            throw new InvalidOperationException(
                "RaceStats must be persisted before creating a ScifiSubrace.");

        ScifiRaceId = scifiRaceId;
        SubRaceName = subRaceName;
        Description = description;
        RaceStatsId = raceStatsId;
    }

    // Parameterless constructor for EF Core
    protected ScifiSubrace() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}