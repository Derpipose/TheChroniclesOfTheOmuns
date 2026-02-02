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
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
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

        characterService.UpdateCharacterClass(character, mageClass);
        characterService.UpdateCharacterRace(character, TieflingRace);

        int expectedMP = 60;
        int actualMP = character.Mana;

        Assert.That(character.Stats.Intelligence, Is.EqualTo(16));
        Assert.That(character.Stats.Wisdom, Is.EqualTo(18));
        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterRace, Is.Not.Null);
        Assert.That(characterService.GetRaceModifierValue(character, ModifierType.ManaBonus), Is.EqualTo(10));

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
            ManaDice = DiceType.D4,
            HitDice = DiceType.D12
        };

        characterService.UpdateCharacterClass(character, fighterClass);
        characterService.UpdateCharacterRace(character, TieflingRace);

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
