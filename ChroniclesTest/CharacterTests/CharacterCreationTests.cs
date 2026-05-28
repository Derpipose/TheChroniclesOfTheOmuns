namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class CharacterCreationTests {
    private FantasyCharacterClass mageClass;

    [SetUp]
    public void Setup() {
        mageClass = new FantasyCharacterClass() {
            Name = "Mage",
            FantasyClassType = FantasyClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
        };
    }

    [Test]
    public void CreateNewCharacter() {
        FantasyCharacter character = new();

        Assert.That(character, Is.TypeOf<FantasyCharacter>());
        Assert.Multiple(() => {
            Assert.That(character.Name, Is.EqualTo("Tav"));
            Assert.That(character.Level, Is.EqualTo(1));
        });
    }

    [Test]
    public void AddRaceToCharacter() {
        FantasyCharacter character = new();

        Assert.That(character.FantasyCharacterRace, Is.Null);

        FantasyCharacterRace tiefling = new() {
            Name = "Tiefling",
            RaceType = "Demonic",
            Description = "A demonic race spawned from the Hells themselves. " +
            "According to popular playstyles, their skin can be any shade of red, " +
            "purple, or normal skin tones. Some even are blue"
        };

        character.AssignFantasyCharacterRace(tiefling);

        Assert.That(character.FantasyCharacterRace, Is.Not.Null);

        Assert.Multiple(() => {
            Assert.That(character.FantasyCharacterRace.Name, Is.EqualTo("Tiefling"));
            Assert.That(character.FantasyCharacterRace.Description, Is.EqualTo(tiefling.Description));
        });
    }

    [Test]
    public void AddClassToCharacter() {
        FantasyCharacter character = new();

        Assert.That(character.FantasyCharacterClass, Is.Null);

        FantasyCharacterClass fighter = new() {
            Name = "Fighter",
            FantasyClassType = FantasyClassTypeEnum.Combat,
            Description = "They pick up sword and go swingy swingy fast. Eventually fast enough to hit someone twice in a row :O",
            HitDice = DiceType.D12,
            ManaDice = DiceType.D6
        };

        character.AssignFantasyCharacterClass(fighter);

        Assert.That(character.FantasyCharacterClass, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(character.FantasyCharacterClass.Name, Is.EqualTo("Fighter"));
            Assert.That(character.FantasyCharacterClass.Description, Is.EqualTo(fighter.Description));
        });
    }

    [Test]
    public void AddStatsToCharacter() {
        FantasyCharacter character = new();

        Assert.That(character.Stats, Is.Not.Null);

        CharacterStats stats = new() {
            Strength = 15,
            Dexterity = 12,
            Constitution = 14,
            Intelligence = 10,
            Wisdom = 8,
            Charisma = 13
        };

        character.Stats = stats;

        Assert.Multiple(() => {
            Assert.That(character.Stats.Strength, Is.EqualTo(15));
            Assert.That(character.Stats.Dexterity, Is.EqualTo(12));
            Assert.That(character.Stats.Constitution, Is.EqualTo(14));
            Assert.That(character.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(character.Stats.Wisdom, Is.EqualTo(8));
            Assert.That(character.Stats.Charisma, Is.EqualTo(13));
        });
    }

    [Test]
    public void CharacterStartsWithDefaultStatsIfNoneProvided() {
        FantasyCharacter character = new();

        Assert.That(character.Stats, Is.Not.Null);

        Assert.Multiple(() => {
            Assert.That(character.Stats.Strength, Is.EqualTo(10));
            Assert.That(character.Stats.Dexterity, Is.EqualTo(10));
            Assert.That(character.Stats.Constitution, Is.EqualTo(10));
            Assert.That(character.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(character.Stats.Wisdom, Is.EqualTo(10));
            Assert.That(character.Stats.Charisma, Is.EqualTo(10));
        });
    }
}
