using PlayerApp.Models;
using PlayerApp.Models.Dice;

namespace LegacyTests;

[TestFixture]
public class CharacterDiceTests {
    [Test]
    public void CharacterWithNoClassHasNoHitDie() {
        FantasyCharacter character = new();

        DiceType? hitDie = character.GetHitDice();

        Assert.That(hitDie, Is.Null);
    }

    [Test]
    public void CharacterWithNoClassHasNoManaDie() {
        FantasyCharacter character = new();

        DiceType? manaDie = character.GetManaDice();

        Assert.That(manaDie, Is.Null);
    }
}
