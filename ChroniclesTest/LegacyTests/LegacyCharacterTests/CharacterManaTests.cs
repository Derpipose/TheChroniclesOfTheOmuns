namespace LegacyTests;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class CharacterManaTests {
    private FantasyCharacterClass mageClass;
    private FantasyCharacterRace TieflingRace;
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

        // Create modifiers for the Tiefling race
        var manaBonusModifier = new Modifier { Id = 1, Type = ModifierType.ManaBonus, Value = 10 };

        TieflingRace = new FantasyCharacterRace() {
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
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Intelligence = 16,
            Wisdom = 18
        };

        characterService.UpdateFantasyCharacterClass(character, mageClass);
        characterService.UpdateFantasyCharacterRace(character, TieflingRace);

        int expectedMP = 60;
        int actualMP = character.Mana;

        Assert.That(character.Stats.Intelligence, Is.EqualTo(16));
        Assert.That(character.Stats.Wisdom, Is.EqualTo(18));
        Assert.That(character.FantasyCharacterClass, Is.Not.Null);
        Assert.That(character.FantasyCharacterRace, Is.Not.Null);
        Assert.That(characterService.GetRaceModifierValue(character, ModifierType.ManaBonus), Is.EqualTo(10));

        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }

    [Test]
    public void CharacterHasCorrectHitPointsAtLevelOneForCombatClass() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Intelligence = 16,
            Wisdom = 18
        };

        FantasyCharacterClass fighterClass = new() {
            Name = "Barbarian",
            FantasyClassType = FantasyClassTypeEnum.Combat,
            Description = "A strong melee fighter",
            ManaDice = DiceType.D4,
            HitDice = DiceType.D12
        };

        characterService.UpdateFantasyCharacterClass(character, fighterClass);
        characterService.UpdateFantasyCharacterRace(character, TieflingRace);

        int expectedMP = 40;
        int actualMP = character.Mana;

        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }

    [Test]
    public void CharacterManaPointsNotCalculatedIfNoStatsClassOrRace() {
        FantasyCharacter character = new();

        int expectedMP = 0;
        int actualMP = character.Mana;
        Assert.That(actualMP, Is.EqualTo(expectedMP));
    }
}
