using PlayerApp.Models;
using PlayerApp.Models.Dice;

namespace LegacyTests;

[TestFixture]
public class FantasyCharacterClassLoadingTests {
    private FantasyCharacterClassService classService;

    [SetUp]
    public void Setup() {
        classService = new FantasyCharacterClassService();
    }
    [Test]
    [Order(1)]
    public void TestClassPullWorks() {
        List<FantasyCharacterClass> list = classService.GetAllClassesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(33));
    }

    [Test]
    [Order(2)]
    public void TestCharacterWithClassMageHasCorrectHitDie() {
        FantasyCharacter character = new FantasyCharacter();
        FantasyCharacterClass? charClass = classService.GetAllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignFantasyCharacterClass(charClass);
        DiceType? hitDie = character.GetHitDice();

        Assert.That(character.FantasyCharacterClass, Is.Not.Null);
        Assert.That(character.FantasyCharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(hitDie, Is.Not.Null);
        Assert.That(hitDie?.DisplayName(), Is.EqualTo("D6"));
    }

    [Test]
    [Order(3)]
    public void TestCharacterWithClassMageHasCorrectManaDie() {
        FantasyCharacter character = new FantasyCharacter();
        FantasyCharacterClass? charClass = classService.GetAllClassesAsync().Result.Find(c => c.Name == "Mage");
        Assert.That(charClass, Is.Not.Null);
        character.AssignFantasyCharacterClass(charClass);
        DiceType? manaDie = character.GetManaDice();

        Assert.That(character.FantasyCharacterClass, Is.Not.Null);
        Assert.That(character.FantasyCharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(manaDie, Is.Not.Null);
        Assert.That(manaDie?.DisplayName(), Is.EqualTo("D12"));
    }
}
