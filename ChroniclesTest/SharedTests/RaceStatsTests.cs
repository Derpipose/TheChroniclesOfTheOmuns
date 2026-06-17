using System;
using System.Collections.Generic;
using Xunit;
using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class RaceStatsTests {

    [Fact]
    public void BlankRaceStatsHasNoModifiersOrPicks() {
        RaceStats stats = new RaceStats();

        Assert.Empty(stats.Modifiers);
        Assert.Empty(stats.FreeStatPicks);
    }

    [Fact]
    public void RaceStatsStoresModifiers() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Strength, 2),
            new StatModifier(StatType.Constitution, 2)
        };

        RaceStats stats = new RaceStats(modifiers);

        Assert.Equal(2, stats.Modifiers.Count);
    }

    [Fact]
    public void RaceStatsStoresFreeStatPicks() {
        RaceStats stats = new RaceStats(freeStatPicks: new List<int> { 1, 2 });

        Assert.Equal(2, stats.FreeStatPicks.Count);
        Assert.Contains(1, stats.FreeStatPicks);
        Assert.Contains(2, stats.FreeStatPicks);
    }

    [Fact]
    public void GetModifierForReturnsCorrectValue() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Intelligence, 5)
        };
        RaceStats stats = new RaceStats(modifiers);

        Assert.Equal(5, stats.GetModifierFor(StatType.Intelligence));
    }

    [Fact]
    public void GetModifierForReturnsZeroWhenStatNotPresent() {
        RaceStats stats = new RaceStats();

        Assert.Equal(0, stats.GetModifierFor(StatType.Strength));
    }

    [Fact]
    public void GetModifierForDoesNotBleedIntoOtherStats() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Strength, 2),
            new StatModifier(StatType.Constitution, -3)
        };
        RaceStats stats = new RaceStats(modifiers);

        Assert.Equal(2, stats.GetModifierFor(StatType.Strength));
        Assert.Equal(-3, stats.GetModifierFor(StatType.Constitution));
        Assert.Equal(0, stats.GetModifierFor(StatType.Wisdom));
        Assert.Equal(0, stats.GetModifierFor(StatType.Charisma));
        Assert.Equal(0, stats.GetModifierFor(StatType.Intelligence));
        Assert.Equal(0, stats.GetModifierFor(StatType.Dexterity));
    }

    [Fact]
    public void IsStatLockedReturnsTrueForLockedStat() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Strength, 19, isLocked: true)
        };
        RaceStats stats = new RaceStats(modifiers);

        Assert.True(stats.IsStatLocked(StatType.Strength));
    }

    [Fact]
    public void IsStatLockedReturnsFalseForUnlockedStat() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Strength, 2, isLocked: false)
        };
        RaceStats stats = new RaceStats(modifiers);

        Assert.False(stats.IsStatLocked(StatType.Strength));
    }

    [Fact]
    public void IsStatLockedReturnsFalseForStatNotPresent() {
        RaceStats stats = new RaceStats();

        Assert.False(stats.IsStatLocked(StatType.Wisdom));
    }

    [Fact]
    public void CannotHaveDuplicateStatInModifiers() {
        var modifiers = new List<StatModifier> {
            new StatModifier(StatType.Strength, 2),
            new StatModifier(StatType.Strength, 3)
        };

        Assert.Throws<ArgumentException>(() => new RaceStats(modifiers));
    }

    [Fact]
    public void NullModifiersDefaultsToEmptyList() {
        RaceStats stats = new RaceStats(modifiers: null);

        Assert.NotNull(stats.Modifiers);
        Assert.Empty(stats.Modifiers);
    }

    [Fact]
    public void NullFreeStatPicksDefaultsToEmptyList() {
        RaceStats stats = new RaceStats(freeStatPicks: null);

        Assert.NotNull(stats.FreeStatPicks);
        Assert.Empty(stats.FreeStatPicks);
    }

    [Fact]
    public void IdDefaultsToZeroBeforePersist() {
        RaceStats stats = new RaceStats();

        Assert.Equal(0, stats.Id);
    }

    [Fact]
    public void SimulatePersistSetsId() {
        RaceStats stats = new RaceStats();
        stats.SimulatePersist(42);

        Assert.Equal(42, stats.Id);
    }
}