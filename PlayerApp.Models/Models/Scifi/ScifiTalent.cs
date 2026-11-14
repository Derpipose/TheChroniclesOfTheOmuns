using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class ScifiTalent {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Ability { get; private set; } = string.Empty;
    public string Perk { get; private set; } = string.Empty;
    public string PerkAmount { get; private set; } = string.Empty;

    public ScifiTalent(string name, string description, string ability, string perk = "", string perkAmount = "") {
        Name = name;
        Description = description;
        Ability = ability;
        Perk = perk;
        PerkAmount = perkAmount;
    }

    // Parameterless constructor for EF Core
    protected ScifiTalent() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

    public void SetName(string name) => Name = name;
    public void SetDescription(string description) => Description = description;
    public void SetAbility(string ability) => Ability = ability;
    public void SetPerk(string perk) => Perk = perk;
    public void SetPerkAmount(string perkAmount) => PerkAmount = perkAmount;
}

public class ScifiCharacterTalent {
    public int ScifiCharacterId { get; private set; }
    public int ScifiTalentId { get; private set; }
    public TalentSourceEnum SourceType { get; private set; }

    public ScifiTalent? ScifiTalent { get; private set; }
    public ScifiCharacter? ScifiCharacter { get; private set; }

    public ScifiCharacterTalent(int scifiCharacterId, int scifiTalentId, TalentSourceEnum sourceType) {
        ScifiCharacterId = scifiCharacterId;
        ScifiTalentId = scifiTalentId;
        SourceType = sourceType;
    }

    // Parameterless constructor for EF Core
    protected ScifiCharacterTalent() {
        // EF Core needs this to materialize the entity
    }
}