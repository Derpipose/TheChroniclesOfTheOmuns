using System.Diagnostics;
using System.Linq;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class RaceStats {
    public int Id { get; private set; }
    public IReadOnlyList<RaceStatModifier>? Modifiers { get; private set; }
    public IReadOnlyList<int>? FreeStatPicks { get; private set; }

    private RaceStats() { }

    public RaceStats(List<RaceStatModifier>? modifiers = null, List<int>? freeStatPicks = null) {
        modifiers ??= new List<RaceStatModifier>();

        var duplicates = modifiers.GroupBy(m => m.Stat).Where(g => g.Count() > 1);
        if (duplicates.Any())
            throw new ArgumentException("A stat cannot appear more than once in Modifiers.");

        Modifiers = modifiers;
        FreeStatPicks = freeStatPicks ?? new List<int>();
    }

    public int GetModifierFor(StatType stat) =>
        (Modifiers ?? Enumerable.Empty<RaceStatModifier>()).FirstOrDefault(m => m.Stat == stat)?.Modifier ?? 0;

    public bool IsStatLocked(StatType stat) =>
        (Modifiers ?? Enumerable.Empty<RaceStatModifier>()).Any(m => m.Stat == stat && m.IsLocked);

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}


public class RaceStatModifier {
    public StatType Stat { get; private set; }
    public int Modifier { get; private set; }
    public bool IsLocked { get; private set; }

    public RaceStatModifier(StatType stat, int modifier, bool isLocked = false) {
        Stat = stat;
        Modifier = modifier;
        IsLocked = isLocked;
    }
}