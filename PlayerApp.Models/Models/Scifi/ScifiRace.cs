using System.Diagnostics;

public class ScifiRace {
    public int Id { get; private set; }
    public string SubType { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsStarterRace { get; private set; }
    public int RaceStatsId { get; private set; }
    public int Speed { get; private set; }
    public int AdventuringAgeStart { get; private set; }
    public int AgeMax { get; private set; }
    public bool IsPlayable { get; private set; }

    // public List<ScifiSubrace> Subraces { get; private set; } = new();
    // public bool HasSubrace => Subraces.Any();

    public ScifiRace(string subType, string name, string description, bool isStarterRace,
                     int raceStatsId, int speed, int adventuringAgeStart,
                     int ageMax) {
        SubType = subType;
        Name = name;
        Description = description;
        IsStarterRace = isStarterRace;
        RaceStatsId = raceStatsId;
        Speed = speed;
        AdventuringAgeStart = adventuringAgeStart;
        AgeMax = ageMax;
    }

    public void Update(string subType, string name, string description, bool isStarterRace,
                    int raceStatsId, int speed, int adventuringAgeStart,
                    int ageMax, bool isPlayable) {
        SubType = subType;
        Name = name;
        Description = description;
        IsStarterRace = isStarterRace;
        RaceStatsId = raceStatsId;
        Speed = speed;
        AdventuringAgeStart = adventuringAgeStart;
        AgeMax = ageMax;
        IsPlayable = isPlayable;
    }

    // Parameterless constructor for EF Core
    protected ScifiRace() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}
