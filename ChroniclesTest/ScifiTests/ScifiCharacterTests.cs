namespace ChroniclesTest;

using Xunit;
using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

public class ScifiCharacterTests {

    [Fact]
    public void TestScifiCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);

        Assert.IsType<ScifiCharacter>(scifiCharacter);
        Assert.NotNull(scifiCharacter);
    }

    [Fact]
    public void AssignScifiCharacterToCharacterBase() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);

        Assert.IsType<Character>(character);
        Assert.NotNull(character);

        ScifiCharacter scifiCharacter = new ScifiCharacter(character);

        Assert.Equal(1, scifiCharacter.CharacterId);
        Assert.Equal(character.Id, scifiCharacter.CharacterId);
    }
}