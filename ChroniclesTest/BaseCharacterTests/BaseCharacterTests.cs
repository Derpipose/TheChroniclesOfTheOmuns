namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class BaseCharacterTests {
    
    [Test]
    public void TestBaseCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);

        Assert.That(character, Is.TypeOf<Character>());
        Assert.That(character != null);
    }

    [Test]
    public void TestSetCharacterName() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SetName("Cressa");
        Assert.That(character.Name, Is.EqualTo("Cressa"));
        character.SetName("Kreena");
        Assert.That(character.Name, Is.EqualTo("Kreena"));
    }

    [Test]
    public void TestCharacterHealth() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        Assert.That(character.MaxHealth, Is.EqualTo(0));
    }

    [Test]
    public void SetMaxHealthCharacter() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SetMaxHealth(100);
        Assert.That(character.MaxHealth, Is.EqualTo(100));
    }

    [Test]
    public void TestCharacterId() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        Assert.That(character.Id, Is.EqualTo(player.Id));
    }

    [Test]
    public void CharacterBelongsToPlayer() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player);
        Assert.That(character.playerId, Is.EqualTo(player.Id));
    }

}