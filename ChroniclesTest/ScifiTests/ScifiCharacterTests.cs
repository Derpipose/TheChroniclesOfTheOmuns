namespace ChroniclesTest;

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

    [Fact]
    public void SetScifiRace() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);

        scifiCharacter.SetScifiRace(scifiRace);

        Assert.Equal(scifiRace.Id, scifiCharacter.ScifiRaceId);
    }

    [Fact]
    public void SetScifiSubrace() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);
        ScifiSubrace scifiSubrace = new ScifiSubrace(1,"TestSubrace", "Test description", 1);
        scifiSubrace.SimulatePersist(1);

        scifiCharacter.SetScifiRace(scifiRace);
        scifiCharacter.SetScifiSubrace(scifiSubrace);

        Assert.Equal(scifiSubrace.Id, scifiCharacter.ScifiSubraceId);
    }

    [Fact]
    public void SetScifiClass() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);
        ScifiClass scifiClass = new ScifiClass("TestClass", DiceType.D10, "Test");
        scifiClass.SimulatePersist(1);

        scifiCharacter.SetScifiClass(scifiClass);

        Assert.Equal(scifiClass.Id, scifiCharacter.ScifiClassId);
    }

    [Fact]
    public void SetScifiBackground() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);
        ScifiBackground scifiBackground = new ScifiBackground("TestBackground", "Test description");
        scifiBackground.SimulatePersist(1);

        scifiCharacter.SetScifiBackground(scifiBackground);

        Assert.Equal(scifiBackground.Id, scifiCharacter.ScifiBackgroundId);
    }
}