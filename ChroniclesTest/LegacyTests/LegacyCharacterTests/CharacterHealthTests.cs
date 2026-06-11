namespace LegacyTests;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class CharacterHealthTests {
    private FantasyCharacterClass mageClass;
    private CharacterService characterService;

    [SetUp]
    public void Setup() {
        characterService = new CharacterService();
        mageClass = new FantasyCharacterClass() {
            Name = "Mage",
            FantasyClassType = FantasyClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
        };
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOne() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };
        FantasyCharacterRace humanRace = new() {
            Name = "Human",
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };
        characterService.UpdateFantasyCharacterRace(character, humanRace);

        characterService.UpdateFantasyCharacterClass(character, mageClass);

        int expectedHP = 31; // (2 * 6) + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 16
        };

        FantasyCharacterClass fighterClass = new() {
            Name = "Barbarian",
            FantasyClassType = FantasyClassTypeEnum.Combat,
            Description = "A strong melee fighter",
            HitDice = DiceType.D12,
            ManaDice = DiceType.D4
        };

        FantasyCharacterRace humanRace = new() {
            Name = "Human",
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };

        characterService.UpdateFantasyCharacterClass(character, fighterClass);
        characterService.UpdateFantasyCharacterRace(character, humanRace);

        int expectedHP = 47; // 2 * 16 + 12 + 3
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void CharacterHitPointsNotCalculatedIfNoStatsOrClass() {
        FantasyCharacter character = new();

        int expectedHP = 0;
        int actualHP = character.Health;

        Assert.That(actualHP, Is.EqualTo(expectedHP));
    }

    [Test]
    public void DerpyJrKoboldKnightHealthAndManaAreCorrect() {
        // DerpyJr: Kobold Knight
        // Expected: Health 40, Mana 47
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Strength = 12,
            Constitution = 14,
            Dexterity = 13,
            Intelligence = 12,
            Wisdom = 14,
            Charisma = 13
        };

        var knightClass = new FantasyCharacterClass {
            Name = "Knight",
            FantasyClassType = FantasyClassTypeEnum.Combat,
            Description = "A master of martial combat",
            HitDice = DiceType.D10,
            ManaDice = DiceType.D8
        };

        var koboldRace = new FantasyCharacterRace {
            Name = "Kobold",
            RaceType = "Reptilian",
            Description = "Small crafty creatures"
        };
        koboldRace.AddModifier(ModifierType.ManaBonus, 15);

        characterService.UpdateFantasyCharacterRace(character, koboldRace);
        characterService.UpdateFantasyCharacterClass(character, knightClass);

        // Health: 2*CON + HitDie + CON_bonus = 2*14 + 10 + 2 = 40
        Assert.That(character.Health, Is.EqualTo(40), "Health calculation failed");

        // Mana (Combat): mainStat + (2*ManaDie) + stat_bonus + race_bonus
        // = WIS + (2*8) + WIS_bonus + 15 = 14 + 16 + 2 + 15 = 47
        Assert.That(character.Mana, Is.EqualTo(47), "Mana calculation failed");
    }
}
