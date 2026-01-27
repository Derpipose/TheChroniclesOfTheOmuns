using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

[TestFixture]
public class CharacterClassTests {
    [Test]
    public void TestCharacterClassCreation() {
        CharacterClass charClass = new CharacterClass() {
            Name = "Warrior",
            ClassType = ClassTypeEnum.Combat,
            Description = "A strong melee fighter",
            HitDice = DiceType.D12,
            ManaDice = DiceType.D4
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
            ClassType = ClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
        };

        character.AssignCharacterClass(charClass);

        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
    }
}
