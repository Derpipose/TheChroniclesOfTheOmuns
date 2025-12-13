using PlayerApp.Models;

namespace ChroniclesTest;

public class StatsUnitTests {
    [SetUp]
    public void Setup() {

    }

    [Test]
    public void CreateCharacterStats() {
        // Arrange & Act
        CharacterStats stats = new() {
            Strength = 15,
            Constitution = 14,
            Dexterity = 13,
            Wisdom = 12,
            Charisma = 10,
            Intelligence = 8
        };
        Assert.Multiple(() => {
            // Assert
            Assert.That(stats.Strength, Is.EqualTo(15));
            Assert.That(stats.Constitution, Is.EqualTo(14));
            Assert.That(stats.Dexterity, Is.EqualTo(13));
            Assert.That(stats.Wisdom, Is.EqualTo(12));
            Assert.That(stats.Charisma, Is.EqualTo(10));
            Assert.That(stats.Intelligence, Is.EqualTo(8));
        });
    }

    [Test]
    public void BlankCreateCharacterStats() {
        CharacterStats stats = new();
        Assert.Multiple(() => {
            // Assert
            Assert.That(stats.Strength, Is.EqualTo(10));
            Assert.That(stats.Constitution, Is.EqualTo(10));
            Assert.That(stats.Dexterity, Is.EqualTo(10));
            Assert.That(stats.Wisdom, Is.EqualTo(10));
            Assert.That(stats.Charisma, Is.EqualTo(10));
            Assert.That(stats.Intelligence, Is.EqualTo(10));

            Assert.That(stats, Is.TypeOf<CharacterStats>());
        });
    }
}
