namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;
using Xunit;

public class BaseCharacterTests {

    [Fact]
    public void TestBaseCharacterCreation() {
        Character character = new Character(1, "TestCharacter");

        Assert.IsType<Character>(character);
        Assert.NotNull(character);
    }

    [Fact]
    public void TestSetCharacterName() {
        Character character = new Character(1, "TestCharacter");
        character.SetName("Cressa");
        Assert.Equal("Cressa", character.Name);
        character.SetName("Kreena");
        Assert.Equal("Kreena", character.Name);
    }

    [Fact]
    public void TestCharacterHealth() {
        Character character = new Character(1, "TestCharacter");
        Assert.Equal(0, character.BaseHealth);
    }

    [Fact]
    public void SetMaxHealthCharacter() {
        Character character = new Character(1, "TestCharacter");
        character.SetBaseHealth(100);
        Assert.Equal(100, character.BaseHealth);
    }

    [Fact]
    public void TestCharacterId() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        Assert.Equal(1, character.Id);
    }

    [Fact]
    public void CharacterBelongsToPlayer() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        Assert.Equal(player.Id, character.PlayerId);
    }
}