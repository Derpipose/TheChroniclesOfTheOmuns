using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

namespace ChroniclesTest;

[TestFixture]
public class FantasyCharacterClassTests {
    [Test]
    public void TestFantasyCharacterClassCreation() {
        FantasyCharacterClass charClass = new FantasyCharacterClass() {
            Name = "Warrior",
            FantasyClassType = FantasyClassTypeEnum.Combat,
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
    public void TestAssignFantasyCharacterClassToCharacter() {
        FantasyCharacter character = new FantasyCharacter();
        FantasyCharacterClass charClass = new FantasyCharacterClass() {
            Name = "Mage",
            FantasyClassType = FantasyClassTypeEnum.Magic,
            Description = "A master of arcane arts",
            HitDice = DiceType.D6,
            ManaDice = DiceType.D12
        };

        character.AssignFantasyCharacterClass(charClass);

        Assert.That(character.FantasyCharacterClass?.Name, Is.EqualTo("Mage"));
    }
}
