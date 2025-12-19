namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterBonusTests {
    private CharacterService characterService;

    [SetUp]
    public void Setup() {
        characterService = new CharacterService();
    }

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
            Assert.That(characterService.GetBonus(character, "Strength"), Is.EqualTo(4));
            Assert.That(characterService.GetBonus(character, "Dexterity"), Is.EqualTo(2));
            Assert.That(characterService.GetBonus(character, "Constitution"), Is.EqualTo(1));
            Assert.That(characterService.GetBonus(character, "Intelligence"), Is.EqualTo(0));
            Assert.That(characterService.GetBonus(character, "Wisdom"), Is.EqualTo(-1));
            Assert.That(characterService.GetBonus(character, "Charisma"), Is.EqualTo(3));
        });
    }
}
