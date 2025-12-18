namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterManaTests {
    private CharacterClass mageClass;
    private CharacterRace TieflingRace;
    
    [SetUp]
    public void Setup() {
        mageClass = new CharacterClass() {
            Name = "Mage",
            ClassType = "Magic",
            Description = "A master of arcane arts",
            HitDiceId = 2,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 },
            ManaDiceId = 6,
            ManaDice = new DiceType { Id = 6, Name = "D12", Sides = 12 }
        };
        TieflingRace = new CharacterRace() {
            Name = "Tiefling",
            Description = "A demonic race spawned from the Hells themselves. " +
            "According to popular playstyles, their skin can be any shade of red, " +
            "purple, or normal skin tones. Some even are blue",
            BonusMana = 10
        };
    }

    [Test]
    public void CharacterHasCorrectManaPointsAtLevelOne() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Intelligence = 16,
            Wisdom = 18
        };

        character.AssignCharacterClass(mageClass);
        character.AssignCharacterRace(TieflingRace);

        int expectedMP = 60; // 16 + 18 + 4 + 10 + 12
        int actualMP = character.Mana;

        Assert.That(character.Stats.Intelligence, Is.EqualTo(16));
        Assert.That(character.Stats.Wisdom, Is.EqualTo(18));
        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterRace, Is.Not.Null);
        Assert.That(character.CharacterRace.BonusMana, Is.EqualTo(10));
        Assert.That(character.CharacterClass.ManaDice, Is.Not.Null);

        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }

    // [Test]
    // public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
    //     Character character = new();
    //     character.Stats = new CharacterStats() {
    //         Constitution = 16
    //     };

    //     CharacterClass fighterClass = new() {
    //         Name = "Barbarian",
    //         ClassType = "Combat",
    //         Description = "A strong melee fighter",
    //         HitDiceId = 5,
    //         HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 }
    //     };

    //     character.AssignCharacterClass(fighterClass);

    //     int expectedHP = 47; // 2 * 16 + 12 + 3
    //     int actualHP = character.Health;

    //     Assert.That(actualHP, Is.EqualTo(expectedHP));
    // }

    // [Test]
    // public void CharacterManaPointsNotCalculatedIfNoStatsOrClass() {
    //     Character character = new();

    //     int expectedMP = 0;
    //     int actualMP = character.Mana;
    //     Assert.That(actualMP, Is.EqualTo(expectedMP));
    // }
}
