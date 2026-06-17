using System.Diagnostics;
using System.Linq;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class RaceStats {
    public int Id { get; private set; }
    public IReadOnlyList<StatModifier> Modifiers { get; private set; }
    public IReadOnlyList<int> FreeStatPicks { get; private set; }

    public RaceStats(List<StatModifier>? modifiers = null, List<int>? freeStatPicks = null) {
        modifiers ??= new List<StatModifier>();

        var duplicates = modifiers.GroupBy(m => m.Stat).Where(g => g.Count() > 1);
        if (duplicates.Any())
            throw new ArgumentException("A stat cannot appear more than once in Modifiers.");

        Modifiers = modifiers;
        FreeStatPicks = freeStatPicks ?? new List<int>();
    }

    public int GetModifierFor(StatType stat) {
        return Modifiers.FirstOrDefault(m => m.Stat == stat)?.Modifier ?? 0;
    }

    public bool IsStatLocked(StatType stat) {
        return Modifiers.Any(m => m.Stat == stat && m.IsLocked);
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}


public class StatModifier {
    public StatType Stat { get; private set; }
    public int Modifier { get; private set; }
    public bool IsLocked { get; private set; }

    public StatModifier(StatType stat, int modifier, bool isLocked = false) {
        Stat = stat;
        Modifier = modifier;
        IsLocked = isLocked;
    }
}