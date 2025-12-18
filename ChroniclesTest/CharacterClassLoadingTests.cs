using PlayerApp.Models;

namespace ChroniclesTest;

[TestFixture]
public class CharacterClassLoadingTests {
    [Test]
    [Order(1)]
    public void TestClassPullWorks() {
        List<CharacterClass> list = new List<CharacterClass>();
        list = CharacterClass.AllClassesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(32));
    }

    [Test]
    [Order(2)]
    public void TestCharacterWithClassMageHasCorrectHitDie() {
        Character character = new Character();
        CharacterClass? charClass = CharacterClass.AllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignCharacterClass(charClass);
        DiceType? hitDie = character.GetHitDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(hitDie, Is.Not.Null);
        Assert.That(hitDie?.Name, Is.EqualTo("D6"));
    }

    [Test]
    [Order(3)]
    public void TestCharacterWithClassMageHasCorrectManaDie() {
        Character character = new Character();
        CharacterClass? charClass = CharacterClass.AllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignCharacterClass(charClass);
        DiceType? manaDie = character.GetManaDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(manaDie, Is.Not.Null);
        Assert.That(manaDie?.Name, Is.EqualTo("D12"));
    }
}
