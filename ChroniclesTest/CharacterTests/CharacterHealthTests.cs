namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterHealthTests {
    private CharacterClass mageClass;
    private CharacterService characterService;

    [SetUp]
    public void Setup() {
        characterService = new CharacterService();
        mageClass = new CharacterClass() {
            Name = "Mage",
            ClassType = "Magic",
            Description = "A master of arcane arts",
            HitDiceId = 2,
            ManaDiceId = 6,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 },
            ManaDice = new DiceType { Id = 6, Name = "D12", Sides = 12 }
        };
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOne() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };
        CharacterRace humanRace = new() {
            Name = "Human",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> {}
        };
        characterService.UpdateCharacterRaceAndCalculateAttributes(character, humanRace);

        characterService.UpdateCharacterClassAndCalculateAttributes(character, mageClass);

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
            ManaDiceId = 1,
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 },
            ManaDice = new DiceType { Id = 1, Name = "D4", Sides = 4 }
        };

        CharacterRace humanRace = new() {
            Name = "Human",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> {}
        };

        characterService.UpdateCharacterClassAndCalculateAttributes(character, fighterClass);
        characterService.UpdateCharacterRaceAndCalculateAttributes(character, humanRace);

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
