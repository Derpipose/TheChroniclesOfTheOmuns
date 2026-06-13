namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;
using Xunit;

public class BaseCharacterTests {

    [Fact]
    public void TestBaseCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);

        Assert.IsType<Character>(character);
        Assert.NotNull(character);
    }

    [Fact]
    public void TestSetCharacterName() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SetName("Cressa");
        Assert.Equal("Cressa", character.Name);
        character.SetName("Kreena");
        Assert.Equal("Kreena", character.Name);
    }

    [Fact]
    public void TestCharacterHealth() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        Assert.Equal(0, character.MaxHealth);
    }

    [Fact]
    public void SetMaxHealthCharacter() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SetMaxHealth(100);
        Assert.Equal(100, character.MaxHealth);
    }

    [Fact]
    public void TestCharacterId() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        Assert.Equal(player.Id, character.Id);
    }

    [Fact]
    public void CharacterBelongsToPlayer() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player);
        Assert.Equal(player.Id, character.playerId);
    }
}