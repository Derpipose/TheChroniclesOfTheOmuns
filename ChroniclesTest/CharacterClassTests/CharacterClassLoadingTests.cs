using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

[TestFixture]
public class CharacterClassLoadingTests {
    private CharacterClassService classService;

    [SetUp]
    public void Setup() {
        classService = new CharacterClassService();
    }
    [Test]
    [Order(1)]
    public void TestClassPullWorks() {
        List<CharacterClass> list = classService.GetAllClassesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(33));
    }

    [Test]
    [Order(2)]
    public void TestCharacterWithClassMageHasCorrectHitDie() {
        Character character = new Character();
        CharacterClass? charClass = classService.GetAllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignCharacterClass(charClass);
        DiceType? hitDie = character.GetHitDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(hitDie, Is.Not.Null);
        Assert.That(hitDie?.DisplayName(), Is.EqualTo("D6"));
    }

    [Test]
    [Order(3)]
    public void TestCharacterWithClassMageHasCorrectManaDie() {
        Character character = new Character();
        CharacterClass? charClass = classService.GetAllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignCharacterClass(charClass);
        DiceType? manaDie = character.GetManaDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(manaDie, Is.Not.Null);
        Assert.That(manaDie?.DisplayName(), Is.EqualTo("D12"));
    }
}
