namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

public class ScifiCharacterTests {

    [Fact]
    public void TestScifiCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        Assert.IsType<ScifiCharacter>(scifiCharacter);
        Assert.NotNull(scifiCharacter);
    }

    [Fact]
    public void AssignScifiCharacterToCharacterBase() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);

        Assert.IsType<Character>(character);
        Assert.NotNull(character);

        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        Assert.Equal(1, scifiCharacter.CharacterId);
        Assert.Equal(character.Id, scifiCharacter.CharacterId);
    }

    [Fact]
    public void SetScifiRace() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);

        scifiCharacter.SetScifiRace(scifiRace.Id);

        Assert.Equal(scifiRace.Id, scifiCharacter.ScifiRaceId);
    }

    [Fact]
    public void SetScifiSubrace() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);
        ScifiSubrace scifiSubrace = new ScifiSubrace(1,"TestSubrace", "Test description", 1);
        scifiSubrace.SimulatePersist(1);

        scifiCharacter.SetScifiRace(scifiRace.Id);
        scifiCharacter.SetScifiSubrace(scifiSubrace.Id);

        Assert.Equal(scifiSubrace.Id, scifiCharacter.ScifiSubraceId);
    }

    [Fact]
    public void SetScifiClass() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiClass scifiClass = new ScifiClass("TestClass", DiceType.D10, "Test");
        scifiClass.SimulatePersist(1);

        scifiCharacter.SetScifiClass(scifiClass.Id);

        Assert.Equal(scifiClass.Id, scifiCharacter.ScifiClassId);
    }

    [Fact]
    public void SetScifiBackground() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiBackground scifiBackground = new ScifiBackground("TestBackground", "Test description");
        scifiBackground.SimulatePersist(1);

        scifiCharacter.SetScifiBackground(scifiBackground.Id);

        Assert.Equal(scifiBackground.Id, scifiCharacter.ScifiBackgroundId);
    }
}