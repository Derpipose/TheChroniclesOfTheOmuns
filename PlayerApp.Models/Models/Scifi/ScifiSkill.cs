using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;


public class ScifiSkill {
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public StatType StatGovernor { get; private set; }
    public SkillType SkillType { get; private set; }
    public SkillKind SkillKind { get; private set; }

    public ScifiSkill(string name, StatType Governor, SkillType skillType, SkillKind skillKind) {
        Name = name;
        StatGovernor = Governor;
        SkillType = skillType;
        SkillKind = skillKind;
    }

    public void Update(string name, StatType Governor, SkillType skillType, SkillKind skillKind) {
        Name = name;
        StatGovernor = Governor;
        SkillType = skillType;
        SkillKind = skillKind;
    }

    // Parameterless constructor for EF Core
    protected ScifiSkill() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}



public class ScifiCharacterSkill {
    public int ScifiCharacterId { get; private set; }
    public int ScifiSkillId { get; private set; }
    public int SkillAmount { get; private set; } = 0;
    public SkillSource SourceType { get; private set; } = SkillSource.None;

    public ScifiSkill? ScifiSkill { get; private set; }
    public ScifiCharacter? ScifiCharacter { get; private set; }

    public ScifiCharacterSkill(int scifiCharacterId, int scifiSkillId, int skillAmount, SkillSource sourceType) {
        if (skillAmount == 0) {
            throw new ArgumentOutOfRangeException(nameof(skillAmount), "Skill amount cannot be 0.");
        }
        if (scifiCharacterId <= 0) {
            throw new ArgumentOutOfRangeException(nameof(scifiCharacterId), "Scifi character ID must be positive.");
        }
        if (scifiSkillId <= 0) {
            throw new ArgumentOutOfRangeException(nameof(scifiSkillId), "Scifi skill ID must be positive.");
        }
        ScifiCharacterId = scifiCharacterId;
        ScifiSkillId = scifiSkillId;
        SkillAmount = skillAmount;
        SourceType = sourceType;
    }

    public void UpdateSkillAmount(int newAmount) {
        SkillAmount = newAmount;
    }

    // Parameterless constructor for EF Core
    protected ScifiCharacterSkill() {
        // EF Core needs this to materialize the entity
    }
}




public enum SkillType {
    General, 
    Specialization
}

public enum SkillKind {
    Specialization,
    Practical,
    Secondary
}