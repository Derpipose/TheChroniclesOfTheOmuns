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
    private readonly List<ScifiCharacterTalent> _talents = [];
    public IReadOnlyList<ScifiCharacterTalent> Talents => _talents;


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
    public void SetScifiRace(ScifiRace scifiRace, List<(StatType stat, int modifier)> modifiers, ScifiSubrace? scifiSubrace = null, CharacterStats? stats = null) {
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
        stats?.EnsureAllMeetMinimum(GetAllModifiers());
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
            var modType = CharacterStats.ToModifierType(stat.Stat);
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

            var modType = CharacterStats.ToModifierType(pick.stat);
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

    public void AddTalent(ScifiTalent talent, TalentSourceEnum sourceType) {
        if (Character is null)
            throw new InvalidOperationException(
                "ScifiCharacter must have an associated Character before talents can be added.");
        
        if (Character.IsActive && (sourceType == TalentSourceEnum.Background || sourceType == TalentSourceEnum.Class))
            throw new InvalidOperationException(
                "Cannot add these talents for an active character.");
        
        if (talent.Id == 0)
            throw new InvalidOperationException(
                "ScifiTalent must be persisted before being assigned to a ScifiCharacter.");

        if (Id == 0)
            throw new InvalidOperationException(
                "ScifiCharacter must be persisted before a ScifiTalent can be assigned to it.");
        
        if (_talents.Any(t => t.ScifiTalentId == talent.Id ))
            throw new InvalidOperationException(
                "ScifiCharacter cannot have the same ScifiTalent assigned more than once.");
        _talents.Add(new ScifiCharacterTalent(Id, talent.Id, sourceType));
    }

    public void ClearTalentsBySource(TalentSourceEnum sourceType) {
        
        if(Character is null)
            throw new InvalidOperationException(
                "ScifiCharacter must have an associated Character before talents can be cleared.");
        if(Character.IsActive && (sourceType == TalentSourceEnum.Background || sourceType == TalentSourceEnum.Class
                                    || sourceType == TalentSourceEnum.LevelUp ))
            throw new InvalidOperationException(
                "Cannot remove these talents for an active character.");
        _talents.RemoveAll(t => t.SourceType == sourceType);
    }

    public void UpdateHealth(CharacterStats stats, ScifiClass? scifiClass) {
        if (Character is null)
            throw new InvalidOperationException("ScifiCharacter must have an associated Character before health can be set.");

        if (Character.IsActive)
            return;
        
        if (scifiClass == null) {
            Character.SetBaseHealth(0);
            Character.SetCurrentHealth(0);
            return;
        }

        if (stats == null) 
            return;
        
        int hitDiceMax = scifiClass.HitDice.Sides();
        var modifiers = GetAllModifiers();
        int constitution = stats.GetStat(StatType.Constitution, modifiers);
        int conMod = stats.GetStatModifier(StatType.Constitution, modifiers);

        int health = constitution * 2 + hitDiceMax + conMod;

        Character.SetBaseHealth(health);
        Character.SetCurrentHealth(health);
    }

    public int GetArmorClass(CharacterStats stats) {
        var modifiers = GetAllModifiers();

        int dexmod = stats.GetStatModifier(StatType.Dexterity, modifiers);
        int ACModifiers = modifiers.Where(m => m.ModifierType == ModifierType.ArmorClass).Sum(m => m.Amount);
        int totalAC = 10 + dexmod + ACModifiers;
        return totalAC;
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

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

    [Conditional("DEBUG")]
    public void SimulateSetCharacter(Character character) {
        Character = character;
    }

    [Conditional("DEBUG")]
    public void SimulateSetNavigations(ScifiRace? race, ScifiClass? scifiClass, ScifiBackground? background) {
        ScifiRace = race;
        ScifiClass = scifiClass;
        ScifiBackground = background;
    }
}