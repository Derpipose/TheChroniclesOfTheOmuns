namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;

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
            Modifiers = new List<RacialModifier> { }
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
            Modifiers = new List<RacialModifier> { }
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

    [Test]
    public void DerpyJrKoboldKnightHealthAndManaAreCorrect() {
        // DerpyJr: Kobold Knight
        // Expected: Health 40, Mana 47
        Character character = new();
        character.Stats = new CharacterStats() {
            Strength = 12,
            Constitution = 14,
            Dexterity = 13,
            Intelligence = 12,
            Wisdom = 14,
            Charisma = 13
        };

        var knightClass = new CharacterClass {
            Name = "Knight",
            ClassType = "Combat",
            Description = "A master of martial combat",
            HitDiceId = 1,
            ManaDiceId = 3,
            HitDice = new DiceType { Id = 1, Name = "D10", Sides = 10 },
            ManaDice = new DiceType { Id = 3, Name = "D8", Sides = 8 }
        };

        var koboldRace = new CharacterRace {
            Name = "Kobold",
            Description = "Small crafty creatures"
        };
        koboldRace.AddModifier(ModifierType.ManaBonus, 15);

        characterService.UpdateCharacterRaceAndCalculateAttributes(character, koboldRace);
        characterService.UpdateCharacterClassAndCalculateAttributes(character, knightClass);

        // Health: 2*CON + HitDie + CON_bonus = 2*14 + 10 + 2 = 40
        Assert.That(character.Health, Is.EqualTo(40), "Health calculation failed");

        // Mana (Combat): mainStat + (2*ManaDie) + stat_bonus + race_bonus
        // = WIS + (2*8) + WIS_bonus + 15 = 14 + 16 + 2 + 15 = 47
        Assert.That(character.Mana, Is.EqualTo(47), "Mana calculation failed");
    }
}
