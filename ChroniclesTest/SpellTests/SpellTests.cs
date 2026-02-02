using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

[TestFixture]
public class SpellTests {

    [SetUp]
    public void Setup() {
    }

    [Test]
    public void ManaCostCanBeCreated() {
        ManaCost manaCost = new ManaCost {
            ManaMin = 1,
            ManaMax = 1,
            ManaRule = ManaRuleEnum.Fixed,
            Base = null
        };
        Assert.That(manaCost.ManaMin, Is.EqualTo(1));
        Assert.That(manaCost.ManaMax, Is.EqualTo(1));
        Assert.That(manaCost.ManaRule, Is.EqualTo(ManaRuleEnum.Fixed));
        Assert.That(manaCost.Base, Is.EqualTo(null));
    }

    [Test]
    public void DamageProfileCanBeCreated() {
        DamageProfile damageProfile = new DamageProfile {
            InitialDiceCount = 2,
            DiceType = DiceType.D6,
            SecondaryDiceCount = null,
            SecondaryDiceType = null,
            SecondaryRule = SecondaryDiceRule.None
        };

        Assert.That(damageProfile.InitialDiceCount, Is.EqualTo(2));
        Assert.That(damageProfile.DiceType, Is.EqualTo(DiceType.D6));
        Assert.That(damageProfile.SecondaryDiceType, Is.EqualTo(null));
        Assert.That(damageProfile.SecondaryRule, Is.EqualTo(SecondaryDiceRule.None));

        DamageProfile secondDamageProfile = new DamageProfile {
            InitialDiceCount = 4,
            DiceType = DiceType.D6,
            SecondaryDiceCount = null,
            SecondaryDiceType = null,
            SecondaryRule = SecondaryDiceRule.None
        };

        Assert.That(secondDamageProfile.InitialDiceCount, Is.EqualTo(4));
        Assert.That(secondDamageProfile.DiceType, Is.EqualTo(DiceType.D6));
        Assert.That(secondDamageProfile.SecondaryDiceType, Is.EqualTo(null));
        Assert.That(secondDamageProfile.SecondaryRule, Is.EqualTo(SecondaryDiceRule.None));

    }

    [Test]
    [Order(1)]
    public void SpellCanBeCreated() {
        Spell spell = new Spell {
            Name = "Fireball",
            SpellBook = "Elemental",
            BookLevel = 3,
            SpellBranch = "Fire",
            Description = "A ball of fire that explodes on impact.",
            ManaCost = new ManaCost {
                ManaMin = 5,
                ManaMax = 5,
                ManaRule = ManaRuleEnum.Fixed,
                Base = null
            },
            Range = 30.ToString(),
            DamageProfile = new DamageProfile {
                InitialDiceCount = 6,
                DiceType = DiceType.D6,
                SecondaryDiceCount = null,
                SecondaryDiceType = null,
                SecondaryRule = SecondaryDiceRule.None
            },
        };
        Assert.That(spell.Name, Is.EqualTo("Fireball"));
        Assert.That(spell.SpellBook, Is.EqualTo("Elemental"));
        Assert.That(spell.BookLevel, Is.EqualTo(3));
        Assert.That(spell.SpellBranch, Is.EqualTo("Fire"));
        Assert.That(spell.Description, Is.EqualTo("A ball of fire that explodes on impact."));
        Assert.That(spell.ManaCost.ManaMin, Is.EqualTo(5));
        Assert.That(spell.ManaCost.ManaMax, Is.EqualTo(5));
        Assert.That(spell.ManaCost.ManaRule, Is.EqualTo(ManaRuleEnum.Fixed));
        Assert.That(spell.ManaCost.Base, Is.EqualTo(null));
        Assert.That(spell.Range, Is.EqualTo("30"));
        Assert.That(spell.DamageProfile.InitialDiceCount, Is.EqualTo(6));
        Assert.That(spell.DamageProfile.DiceType, Is.EqualTo(DiceType.D6));
        Assert.That(spell.DamageProfile.SecondaryDiceType, Is.EqualTo(null));
    }

}
