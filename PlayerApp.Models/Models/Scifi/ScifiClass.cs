using System.Diagnostics;
using PlayerApp.Models.Dice;

namespace PlayerApp.Models;

public class ScifiClass {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public DiceType HitDice { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public IReadOnlyList<ScifiClassTalent> ClassTalents { get; private set; } = new List<ScifiClassTalent>();

    public ScifiClass(string name, DiceType hitDice, string description = "In progress") {
        Name = name;
        HitDice = hitDice;
        Description = description;
    }

    // Parameterless constructor for EF Core
    protected ScifiClass() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

    public void SetName(string name) => Name = name;
    public void SetDescription(string description) => Description = description;
    public void SetHitDice(DiceType hitDice) => HitDice = hitDice;
}

public class ScifiClassTalent {
    public int ScifiClassId { get; private set; }
    public int ScifiTalentId { get; private set; }

    public ScifiClass? ScifiClass { get; private set; }
    public ScifiTalent? ScifiTalent { get; private set; }

    public ScifiClassTalent(int scifiClassId, int scifiTalentId) {
        ScifiClassId = scifiClassId;
        ScifiTalentId = scifiTalentId;
    }
    
    // Parameterless constructor for EF Core
    protected ScifiClassTalent() {
        // EF Core needs this to materialize the join entity
    }


}