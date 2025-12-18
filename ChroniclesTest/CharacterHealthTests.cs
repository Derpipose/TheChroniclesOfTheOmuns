namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterHealthTests {
    private CharacterClass mageClass;
    
    [SetUp]
    public void Setup() {
        mageClass = new CharacterClass() {
            Name = "Mage",
            ClassType = "Magic",
            Description = "A master of arcane arts",
            HitDiceId = 2,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 }
        };
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOne() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };

        character.AssignCharacterClass(mageClass);

        int expectedHP = 31; // (2 * 6) + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };

        CharacterClass fighterClass = new() {
            Name = "Barbarian",
            ClassType = "Combat",
            Description = "A strong melee fighter",
            HitDiceId = 5,
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 }
        };

        character.AssignCharacterClass(fighterClass);

        int expectedHP = 47; // 2 * 16 + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void CharacterHitPointsNotCalculatedIfNoStatsOrClass() {
        Character character = new();

        int expectedHP = 0;
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }
}
