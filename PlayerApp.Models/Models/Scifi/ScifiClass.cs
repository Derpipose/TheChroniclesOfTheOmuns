using System.Diagnostics;
using PlayerApp.Models.Dice;

namespace PlayerApp.Models;

public class ScifiClass {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; }
    public DiceType HitDice { get; private set; }
    public string Description { get; private set; }

    public IReadOnlyList<ScifiClassTalent> ClassTalents { get; private set; } = new List<ScifiClassTalent>();

    public ScifiClass(string name, DiceType hitDice, string description = "In progress") {
        Name = name;
        HitDice = hitDice;
        Description = description;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
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
}