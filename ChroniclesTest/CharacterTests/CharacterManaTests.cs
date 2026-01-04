namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;

[TestFixture]
public class CharacterManaTests {
    private CharacterClass mageClass;
    private CharacterRace TieflingRace;
    private CharacterService characterService;

    [SetUp]
    public void Setup() {
        characterService = new CharacterService();
        mageClass = new CharacterClass() {
            Name = "Mage",
            ClassType = ClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDiceId = 2,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 },
            ManaDiceId = 6,
            ManaDice = new DiceType { Id = 6, Name = "D12", Sides = 12 }
        };

        // Create modifiers for the Tiefling race
        var manaBonusModifier = new Modifier { Id = 1, Type = ModifierType.ManaBonus, Value = 10 };

        TieflingRace = new CharacterRace() {
            Name = "Tiefling",
            RaceType = "Demonic",
            Description = "A demonic race spawned from the Hells themselves. " +
            "According to popular play styles, their skin can be any shade of red, " +
            "purple, or normal skin tones. Some even are blue",
            Modifiers = new List<RacialModifier> {
                new RacialModifier {Modifier = manaBonusModifier,
                                    Race = TieflingRace }
            }
        };
    }

    [Test]
    public void CharacterHasCorrectManaPointsAtLevelOne() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Intelligence = 16,
            Wisdom = 18
        };

        characterService.UpdateCharacterClassAndCalculateAttributes(character, mageClass);
        characterService.UpdateCharacterRaceAndCalculateAttributes(character, TieflingRace);

        int expectedMP = 60;
        int actualMP = character.Mana;

        Assert.That(character.Stats.Intelligence, Is.EqualTo(16));
        Assert.That(character.Stats.Wisdom, Is.EqualTo(18));
        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterRace, Is.Not.Null);
        Assert.That(characterService.GetRaceModifierValue(character, ModifierType.ManaBonus), Is.EqualTo(10));
        Assert.That(character.CharacterClass.ManaDice, Is.Not.Null);

        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Intelligence = 16,
            Wisdom = 18
        };

        CharacterClass fighterClass = new() {
            Name = "Barbarian",
            ClassType = ClassTypeEnum.Combat,
            Description = "A strong melee fighter",
            HitDiceId = 5,
            ManaDiceId = 1,
            ManaDice = new DiceType { Id = 1, Name = "D4", Sides = 4 },
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 }
        };

        characterService.UpdateCharacterClassAndCalculateAttributes(character, fighterClass);
        characterService.UpdateCharacterRaceAndCalculateAttributes(character, TieflingRace);

        int expectedMP = 40;
        int actualMP = character.Mana;

        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }

    [Test]
    public void CharacterManaPointsNotCalculatedIfNoStatsClassOrRace() {
        Character character = new();

        int expectedMP = 0;
        int actualMP = character.Mana;
        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }
}
