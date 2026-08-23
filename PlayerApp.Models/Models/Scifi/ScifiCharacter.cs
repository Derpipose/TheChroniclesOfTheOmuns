using System.Diagnostics;
using PlayerApp.Models.Dice;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class ScifiCharacter {
    
    public int Id { get; private set; } = 0;
    public int CharacterId { get; private set; } = 0;
    public Character? Character { get; private set; } 
    public int? ScifiRaceId { get; private set; } 
    public int? ScifiSubraceId { get; private set; }
    public int? ScifiClassId { get; private set; }
    public int? ScifiBackgroundId { get; private set; }
    
    public ICollection<ScifiCharacterModifier> Modifiers { get; set; } = [];


    public ScifiRace? ScifiRace { get; private set; }
    public ScifiSubrace? ScifiSubrace { get; private set; }
    public ScifiClass? ScifiClass { get; private set; }
    public ScifiBackground? ScifiBackground { get; private set; }


    public ScifiCharacter(int characterid) {
        CharacterId = characterid;
    }

    // Parameterless constructor for EF Core
    protected ScifiCharacter() {
        // EF Core needs this to materialize entities
    }
    public void SetScifiRace(ScifiRace scifiRace, List<(StatType stat, int modifier)> modifiers, ScifiSubrace? scifiSubrace = null) {
        ClearScifiRace();
        if (scifiRace.Id == 0)
            throw new InvalidOperationException(
                "ScifiRace must be persisted before being assigned to a ScifiCharacter.");
        ScifiRaceId = scifiRace.Id;
        if (scifiSubrace != null) {
            if (scifiSubrace.Id == 0)
                throw new InvalidOperationException(
                    "ScifiSubrace must be persisted before being assigned to a ScifiCharacter.");
            if (scifiSubrace.ScifiRaceId != scifiRace.Id)
                throw new InvalidOperationException(
                    "ScifiSubrace must belong to the same ScifiRace as the ScifiRace being assigned to the ScifiCharacter.");
            ScifiSubraceId = scifiSubrace.Id;
        }

        SetRaceModifiers(modifiers, scifiRace, scifiSubrace);


    }

    private void SetRaceModifiers(List<(StatType stat, int modifier)> modifiers, ScifiRace? scifiRace = null, ScifiSubrace? scifiSubrace = null) {
    if (scifiRace == null && ScifiRace == null)
        throw new InvalidOperationException(
            "ScifiRace must be set before setting race modifiers.");

    bool subraceWins = scifiSubrace?.RaceStats != null &&
        ((scifiSubrace.RaceStats.Modifiers?.Count ?? 0) > 0 || (scifiSubrace.RaceStats.FreeStatPicks?.Count ?? 0) > 0);

        var (fixedSource, pickSource) = subraceWins
            ? (ModifierSource.Subrace, ModifierSource.SubracePick)
            : (ModifierSource.Race, ModifierSource.RacePick);

        var winningRaceStats = subraceWins ? scifiSubrace!.RaceStats : scifiRace?.RaceStats;

        if (winningRaceStats == null)
            return;

        ApplyModifiers(modifiers, winningRaceStats, fixedSource, pickSource);
    }

    private void ApplyModifiers(List<(StatType stat, int modifier)> picks, RaceStats raceStats, ModifierSource fixedSource, ModifierSource pickSource) {
        // Assign fixed racial modifiers unconditionally
        foreach (var stat in raceStats.Modifiers ?? Enumerable.Empty<RaceStatModifier>()) {
            var modType = ToModifierType(stat.Stat);
            Modifiers.Add(new ScifiCharacterModifier(Id, modType, stat.Modifier, fixedSource));
        }

        var pool = new List<int>(raceStats.FreeStatPicks ?? new List<int>());
        if (pool.Count == 0)
            return; // no picks to worry about at all

        // Check for any stats that are already set by fixed modifiers
        var alreadySetStats = (raceStats.Modifiers ?? new List<RaceStatModifier>())
            .Select(m => m.Stat)
            .ToHashSet();

        var seenStats = new HashSet<StatType>();

        foreach (var pick in picks) {
            if (alreadySetStats.Contains(pick.stat))
                throw new InvalidOperationException("Free stat picks cannot be assigned to a stat that already has a racial modifier.");

            if (!seenStats.Add(pick.stat))
                throw new InvalidOperationException("Free stat picks cannot be assigned to the same stat more than once.");

            if (!pool.Remove(pick.modifier))
                throw new InvalidOperationException("The submitted pick amount is not available in the free stat picks for this race/subrace.");

            var modType = ToModifierType(pick.stat);
            Modifiers.Add(new ScifiCharacterModifier(Id, modType, pick.modifier, pickSource));
        }

        // Whatever's left in the pool becomes Unassigned placeholders
        foreach (var amount in pool) {
            Modifiers.Add(new ScifiCharacterModifier(Id, ModifierType.Unassigned, amount, pickSource));
        }
    }


    public void SetScifiClass(int scifiClassId) {
        if (scifiClassId == 0)
            throw new InvalidOperationException(
                "ScifiClass must be persisted before being assigned to a ScifiCharacter.");
        ScifiClassId = scifiClassId;
    }

    public void SetScifiBackground(int scifiBackgroundId) {
        if (scifiBackgroundId == 0)
            throw new InvalidOperationException(
                "ScifiBackground must be persisted before being assigned to a ScifiCharacter.");
        ScifiBackgroundId = scifiBackgroundId;
    }

    public void UpdateHealth(CharacterStats stats, ScifiClass? scifiClass) {
        if (Character is null)
            throw new InvalidOperationException("ScifiCharacter must have an associated Character before health can be set.");
        
        if (scifiClass == null) {
            Character.SetMaxHealth(0);
            Character.SetCurrentHealth(0);
            return;
        }

        if (stats == null) 
            return;
        
        int hitDiceMax = scifiClass.HitDice.Sides();
        int conBonus = Character.GetModifierBonus(GetAllModifiers(), ModifierType.Constitution);
        int constitution = stats.Constitution + conBonus;
        int conMod = (constitution - 10) / 2;

        int health = constitution * 2 + hitDiceMax + conMod;

        Character.SetMaxHealth(health);
        Character.SetCurrentHealth(health);
    }

    public void ScifiCharacterSetBaseArmorClass(CharacterStats stats) {
        if (Character is null)
            throw new InvalidOperationException("ScifiCharacter must have an associated Character before AC can be set.");
        
        if (ScifiRace != null) {
            int dexmod = stats.GetStatModifier(StatType.Dexterity);
            int baseAC = 10 + dexmod;
            Character.SetBaseArmorClass(baseAC);
        }
    }

    public IEnumerable<ICharacterModifier> GetAllModifiers() {
        return Modifiers.Cast<ICharacterModifier>();
    }

    public void ClearScifiRace() {
        ScifiRaceId = null;
        ScifiSubraceId = null;
        var sourcesToClear = new[] { ModifierSource.Race, ModifierSource.Subrace, ModifierSource.RacePick, ModifierSource.SubracePick };
        Modifiers.Where(m => sourcesToClear.Contains(m.Source)).ToList().ForEach(m => Modifiers.Remove(m));
    }

    public void ClearScifiSubrace() {
        ScifiSubraceId = null;
    }

    public void ClearScifiClass() {
        ScifiClassId = null;
    }

    public void ClearScifiBackground() {
        ScifiBackgroundId = null;
    }

    private static ModifierType ToModifierType(StatType stat) => stat switch {
        StatType.Strength => ModifierType.Strength,
        StatType.Dexterity => ModifierType.Dexterity,
        StatType.Constitution => ModifierType.Constitution,
        StatType.Intelligence => ModifierType.Intelligence,
        StatType.Wisdom => ModifierType.Wisdom,
        StatType.Charisma => ModifierType.Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(stat), "Unknown stat type.")
    };

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

}