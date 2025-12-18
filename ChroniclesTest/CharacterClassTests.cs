using PlayerApp.Models;

namespace ChroniclesTest;

[TestFixture]
public class CharacterClassTests {
    [Test]
    public void TestCharacterClassCreation() {
        CharacterClass charClass = new CharacterClass() {
            Name = "Warrior",
            ClassType = "Combat",
            Description = "A strong melee fighter",
            HitDiceId = 5,
            ManaDiceId = 1
        };
        Assert.Multiple(() => {
            Assert.That(charClass.Name, Is.EqualTo("Warrior"));
            Assert.That(charClass.Description, Is.EqualTo("A strong melee fighter"));
        });
    }

    [Test]
    public void TestAssignCharacterClassToCharacter() {
        Character character = new Character();
        CharacterClass charClass = new CharacterClass() {
            Name = "Mage",
            ClassType = "Magic",
            Description = "A master of arcane arts",
            HitDiceId = 2,
            ManaDiceId = 6,
            HitDice = new DiceType { Id = 2, Name = "D6", Sides = 6 }
        };

        character.AssignCharacterClass(charClass);

        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
    }
}
