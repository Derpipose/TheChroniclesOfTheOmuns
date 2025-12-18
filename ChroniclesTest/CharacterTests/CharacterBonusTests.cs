namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterBonusTests {
    [Test]
    public void CharacterGetsCorrectBonusFromStats() {
        Character character = new();
        character.Stats = new CharacterStats() {
            Strength = 18,
            Dexterity = 14,
            Constitution = 12,
            Intelligence = 10,
            Wisdom = 8,
            Charisma = 16
        };

        Assert.Multiple(() => {
            Assert.That(character.GetBonus("Strength"), Is.EqualTo(4));
            Assert.That(character.GetBonus("Dexterity"), Is.EqualTo(2));
            Assert.That(character.GetBonus("Constitution"), Is.EqualTo(1));
            Assert.That(character.GetBonus("Intelligence"), Is.EqualTo(0));
            Assert.That(character.GetBonus("Wisdom"), Is.EqualTo(-1));
            Assert.That(character.GetBonus("Charisma"), Is.EqualTo(3));
        });
    }
}
