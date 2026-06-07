namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

[TestFixture]
public class BaseCharacterTests {
    
    [Test]
    public void TestBaseCharacterCreation() {
        Character character = new Character();

        Assert.That(character, Is.TypeOf<Character>());
        Assert.That(character != null);
    }

    [Test]
    public void TestSetCharacterName() {
        Character character = new Character();
        character.SetName("Cressa");
        Assert.That(character.Name, Is.EqualTo("Cressa"));
        character.SetName("Kreena");
        Assert.That(character.Name, Is.EqualTo("Kreena"));
    }

    [Test]
    public void TestCharacterHealth() {
        Character character = new Character();
        Assert.That(character.MaxHealth, Is.EqualTo(0));
    }

    [Test]
    public void SetMaxHealthCharacter() {
        Character character = new Character();
        character.SetMaxHealth(100);
        Assert.That(character.MaxHealth, Is.EqualTo(100));
    }

    [Test]
    public void TestCharacterId() {
        Character character = new Character();
        Assert.That(character.Id, Is.EqualTo(0));
    }

}