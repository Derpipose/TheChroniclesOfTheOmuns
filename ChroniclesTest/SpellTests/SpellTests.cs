using PlayerApp.Models;

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
            ManaRule = PlayerApp.Models.Enums.ManaRuleEnum.Fixed,
            Base = null
        };
        Assert.That(manaCost.ManaMin, Is.EqualTo(1));
        Assert.That(manaCost.ManaMax, Is.EqualTo(1));
        Assert.That(manaCost.ManaRule, Is.EqualTo(PlayerApp.Models.Enums.ManaRuleEnum.Fixed));
        Assert.That(manaCost.Base, Is.EqualTo(null));
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
                ManaRule = PlayerApp.Models.Enums.ManaRuleEnum.Fixed,
                Base = null
            },
            Range = 30.ToString(),
            HitDiceId = 2
        };
        Assert.That(spell.Name, Is.EqualTo("Fireball"));
        Assert.That(spell.SpellBook, Is.EqualTo("Elemental"));
        Assert.That(spell.BookLevel, Is.EqualTo(3));
        Assert.That(spell.SpellBranch, Is.EqualTo("Fire"));
        Assert.That(spell.Description, Is.EqualTo("A ball of fire that explodes on impact."));
        Assert.That(spell.ManaCost.ManaMin, Is.EqualTo(5));
        Assert.That(spell.ManaCost.ManaMax, Is.EqualTo(5));
        Assert.That(spell.ManaCost.ManaRule, Is.EqualTo(PlayerApp.Models.Enums.ManaRuleEnum.Fixed));
        Assert.That(spell.ManaCost.Base, Is.EqualTo(null));
        Assert.That(spell.Range, Is.EqualTo("30"));
        Assert.That(spell.HitDiceId, Is.EqualTo(2));

    }

}
