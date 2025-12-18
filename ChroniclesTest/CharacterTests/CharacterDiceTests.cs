namespace ChroniclesTest;

using PlayerApp.Models;

[TestFixture]
public class CharacterDiceTests {
    [Test]
    public void CharacterWithNoClassHasNoHitDie() {
        Character character = new();

        DiceType? hitDie = character.GetHitDice();

        Assert.That(hitDie, Is.Null);
    }

    [Test]
    public void CharacterWithNoClassHasNoManaDie() {
        Character character = new();

        DiceType? manaDie = character.GetManaDice();

        Assert.That(manaDie, Is.Null);
    }
}
