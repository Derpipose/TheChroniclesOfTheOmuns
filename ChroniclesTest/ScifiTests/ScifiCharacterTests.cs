namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class ScifiCharacterTests {
    
    [Test]
    public void TestScifiCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character);

        Assert.That(scifiCharacter, Is.TypeOf<ScifiCharacter>());
        Assert.That(scifiCharacter != null);
    }

    [Test]
    public void AssignScifiCharacterToCharacterBase() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        Character character = new Character(player);
        character.SimulatePersist(1);

        Assert.That(character, Is.TypeOf<Character>());
        Assert.That(character != null);

        ScifiCharacter scifiCharacter = new ScifiCharacter(character);

        Assert.That(scifiCharacter.CharacterId, Is.EqualTo(1));
        Assert.That(scifiCharacter.CharacterId, Is.EqualTo(character.Id));
    }
}