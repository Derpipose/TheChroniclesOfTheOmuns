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
            ClassType = ClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
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
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };
        characterService.UpdateCharacterRace(character, humanRace);

        characterService.UpdateCharacterClass(character, mageClass);

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
            ClassType = ClassTypeEnum.Combat,
            Description = "A strong melee fighter",
            HitDice = DiceType.D12,
            ManaDice = DiceType.D4
        };

        CharacterRace humanRace = new() {
            Name = "Human",
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };

        characterService.UpdateCharacterClass(character, fighterClass);
        characterService.UpdateCharacterRace(character, humanRace);

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
            ClassType = ClassTypeEnum.Combat,
            Description = "A master of martial combat",
            HitDice = DiceType.D10,
            ManaDice = DiceType.D8
        };

        var koboldRace = new CharacterRace {
            Name = "Kobold",
            RaceType = "Reptilian",
            Description = "Small crafty creatures"
        };
        koboldRace.AddModifier(ModifierType.ManaBonus, 15);

        characterService.UpdateCharacterRace(character, koboldRace);
        characterService.UpdateCharacterClass(character, knightClass);

        // Health: 2*CON + HitDie + CON_bonus = 2*14 + 10 + 2 = 40
        Assert.That(character.Health, Is.EqualTo(40), "Health calculation failed");

        // Mana (Combat): mainStat + (2*ManaDie) + stat_bonus + race_bonus
        // = WIS + (2*8) + WIS_bonus + 15 = 14 + 16 + 2 + 15 = 47
        Assert.That(character.Mana, Is.EqualTo(47), "Mana calculation failed");
    }
}
