using PlayerApp.Models;
using PlayerApp.Models.Enums;
using NUnit.Framework;
using System.Collections.Generic;

namespace ChroniclesTest;

public class RaceStatBonusTests {

    [Test]
    public void FixedBonus_MustHaveStatId_AndNotBeSelectable() {
        var bonus = new RaceStatBonus {
            BonusValue = 2,
            StatId = (int)StatsType.Constitution,
            IsSelectable = false
        };

        Assert.Multiple(() => {
            Assert.That(bonus.StatId, Is.Not.Null);
            Assert.That(bonus.IsSelectable, Is.False);
        });
    }

    [Test]
    public void SelectableBonus_MayHaveNullStatId() {
        var bonus = new RaceStatBonus {
            BonusValue = 1,
            StatId = null,
            IsSelectable = true
        };

        Assert.Multiple(() => {
            Assert.That(bonus.IsSelectable, Is.True);
            Assert.That(bonus.StatId, Is.Null);
        });
    }

    [Test]
    public void Race_CanHaveMultipleBonuses_WithDifferentRules() {
        var race = new CharacterRace {
            Name = "Elf",
            RaceType = "Magical",
            Description = "Graceful and intelligent",
            RaceStatBonuses = new List<RaceStatBonus>()
        };

        race.RaceStatBonuses.Add(new RaceStatBonus {
            BonusValue = 2,
            StatId = (int)StatsType.Dexterity,
            IsSelectable = false
        });

        race.RaceStatBonuses.Add(new RaceStatBonus {
            BonusValue = 1,
            IsSelectable = true
        });

        Assert.Multiple(() => {
            Assert.That(race.RaceStatBonuses, Has.Count.EqualTo(2));
            Assert.That(race.RaceStatBonuses.Count(b => b.IsSelectable), Is.EqualTo(1));
            Assert.That(race.RaceStatBonuses.Count(b => !b.IsSelectable), Is.EqualTo(1));
        });
    }

    [Test]
    public void BonusValue_IsNotRestrictedToFixedSet() {
        var bonusValues = new[] { 1, 2, 3, 4 };

        foreach (var value in bonusValues) {
            var bonus = new RaceStatBonus { BonusValue = value };
            Assert.That(bonus.BonusValue, Is.EqualTo(value));
        }
    }
}
