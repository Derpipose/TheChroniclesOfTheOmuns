namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;

[TestFixture]
public class CharacterRemovalTests {
    private CharacterService characterService;
    private CharacterClass mageClass;
    private CharacterClass knightClass;
    private CharacterRace humanRace;
    private CharacterRace koboldRace;

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

        knightClass = new CharacterClass() {
            Name = "Knight",
            ClassType = ClassTypeEnum.Combat,
            Description = "A master of combat",
            HitDice = DiceType.D10,
            ManaDice = DiceType.D6
        };

        humanRace = new CharacterRace() {
            Name = "Human",
            RaceType = "Versatile",
            Description = "Versatile and ambitious race",
            Modifiers = new List<RacialModifier> { }
        };

        koboldRace = new CharacterRace() {
            Name = "Kobold",
            RaceType = "Bestial",
            Description = "Small reptilian race",
            Modifiers = new List<RacialModifier> { }
        };
    }

    [Test]
    public void RemoveCharacterClassResets() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateCharacterRace(character, humanRace);
        characterService.UpdateCharacterClass(character, mageClass);

        int healthBeforeRemoval = character.Health;
        int manaBeforeRemoval = character.Mana;

        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));
        Assert.That(manaBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveCharacterClass(character);

        Assert.That(character.CharacterClass, Is.Null);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));
    }

    [Test]
    public void RemoveCharacterRaceResets() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateCharacterRace(character, koboldRace);
        characterService.UpdateCharacterClass(character, mageClass);

        int manaBeforeRemoval = character.Mana;
        int healthBeforeRemoval = character.Health;
        Assert.That(manaBeforeRemoval, Is.GreaterThan(0));
        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveCharacterRace(character);

        Assert.That(character.CharacterRace, Is.Null);
        Assert.That(character.Mana, Is.EqualTo(0));
        Assert.That(character.Health, Is.EqualTo(0));
    }

    [Test]
    public void RemoveCharacterClassAndRaceResets() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateCharacterRace(character, humanRace);
        characterService.UpdateCharacterClass(character, knightClass);

        int healthBeforeRemoval = character.Health;
        Assert.That(healthBeforeRemoval, Is.GreaterThan(0));

        characterService.RemoveCharacterClass(character);
        characterService.RemoveCharacterRace(character);

        Assert.That(character.CharacterClass, Is.Null);
        Assert.That(character.CharacterRace, Is.Null);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));
    }

    [Test]
    public void RemoveClassThenReassignRecalculates() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Constitution = 14,
            Intelligence = 16,
            Wisdom = 12
        };

        characterService.UpdateCharacterRace(character, humanRace);
        characterService.UpdateCharacterClass(character, mageClass);

        int originalHealth = character.Health;
        int originalMana = character.Mana;

        characterService.RemoveCharacterClass(character);
        Assert.That(character.Health, Is.EqualTo(0));
        Assert.That(character.Mana, Is.EqualTo(0));

        // Reassign with different class
        characterService.UpdateCharacterClass(character, knightClass);

        Assert.That(character.Health, Is.GreaterThan(0));
        Assert.That(character.Health, Is.Not.EqualTo(originalHealth)); // Combat class has different calculation
    }
}
