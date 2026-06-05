namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class CharacterRemovalTests {
    private CharacterService characterService;
    private FantasyCharacterClass mageClass;
    private FantasyCharacterClass knightClass;
    private FantasyCharacterRace humanRace;
    private FantasyCharacterRace koboldRace;

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

        knightClass = new FantasyCharacterClass() {
            Name = "Knight",
            FantasyClassType = FantasyClassTypeEnum.Combat,
            Description = "A master of combat",
            HitDice = DiceType.D10,
            ManaDice = DiceType.D6
        };

        humanRace = new FantasyCharacterRace() {
            Name = "Human",
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };

        koboldRace = new FantasyCharacterRace() {
            Name = "Kobold",
            RaceType = "Bestial",
            Description = "Small reptilian race",
            Modifiers = new List<RacialModifier> { }
        };
    }

    [Test]
    public void RemoveFantasyCharacterClassResets() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateFantasyCharacterRace(character, humanRace);
        characterService.UpdateFantasyCharacterClass(character, mageClass);

        int healthBeforeRemoval = character.Health;
        int manaBeforeRemoval = character.Mana;

        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));
        Assert.That(manaBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveFantasyCharacterClass(character);

        Assert.That(character.FantasyCharacterClass, Is.Null);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));
    }

    [Test]
    public void RemoveFantasyCharacterRaceResets() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateFantasyCharacterRace(character, koboldRace);
        characterService.UpdateFantasyCharacterClass(character, mageClass);

        int manaBeforeRemoval = character.Mana;
        int healthBeforeRemoval = character.Health;
        Assert.That(manaBeforeRemoval, Is.GreaterThan(0));
        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveFantasyCharacterRace(character);

        Assert.That(character.FantasyCharacterRace, Is.Null);
        Assert.That(character.Mana, Is.EqualTo(0));
        Assert.That(character.Health, Is.EqualTo(0));
    }

    [Test]
    public void RemoveFantasyCharacterClassAndRaceResets() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateFantasyCharacterRace(character, humanRace);
        characterService.UpdateFantasyCharacterClass(character, knightClass);

        int healthBeforeRemoval = character.Health;
        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveFantasyCharacterClass(character);
        characterService.RemoveFantasyCharacterRace(character);

        Assert.That(character.FantasyCharacterClass, Is.Null);
        Assert.That(character.FantasyCharacterRace, Is.Null);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));
    }

    [Test]
    public void RemoveClassThenReassignRecalculates() {
        FantasyCharacter character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateFantasyCharacterRace(character, humanRace);
        characterService.UpdateFantasyCharacterClass(character, mageClass);

        int originalHealth = character.Health;
        int originalMana = character.Mana;

        characterService.RemoveFantasyCharacterClass(character);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));

        // Reassign with different class
        characterService.UpdateFantasyCharacterClass(character, knightClass);

        Assert.That(character.Health, Is.GreaterThan(0));
        Assert.That(character.Health, Is.Not.EqualTo(originalHealth)); // Combat class has different calculation
    }
}
