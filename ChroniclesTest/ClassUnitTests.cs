using PlayerApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChroniclesTest;

public class ClassUnitTests {
    List<CharacterClass>? ClassList;
    [SetUp]
    public void SetUp() {
        ClassList = new List<CharacterClass> {
            new CharacterClass(){ Name = "Mage", Description = "Magic Casting class for all sorts of fun", HitDiceId = 2 },
            new CharacterClass(){ Name = "Warrior", Description = "Melee Fighting class for all sorts of bashing", HitDiceId = 5 },
            new CharacterClass(){ Name = "Rogue", Description = "Stealth and trickery class for sneaky types", HitDiceId = 3 },
        };
    }

    [Test]
    [Order(1)]
    public void TestCharacterClassCreation() {
        CharacterClass charClass = new CharacterClass() {
            Name = "Warrior",
            Description = "A strong melee fighter",
            HitDiceId = 5
        };
        Assert.Multiple(() => {
            Assert.That(charClass.Name, Is.EqualTo("Warrior"));
            Assert.That(charClass.Description, Is.EqualTo("A strong melee fighter"));
        });
    }

    [Test]
    [Order(2)]
    public void TestClassPullWorks() {
        List<CharacterClass> list = new List<CharacterClass>();
        list = CharacterClass.AllClassesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(32));

        ClassList = list;
    }

    [Test]
    [Order(3)]
    public void TestAssignCharacterClassToCharacter() {
        Character character = new Character();
        CharacterClass charClass = new CharacterClass() {
            Name = "Mage",
            Description = "A master of arcane arts",
            HitDiceId = 2
        };

        character.CharacterClass = charClass;

        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
    }

    [Test]
    [Order(4)]
    public void TestCharacterWithClassMageHasCorrectHitDie() {
        Character character = new Character();
        character.CharacterClass = CharacterClass.AllClassesAsync().Result.Find(c => c.Name == "Mage");
        DiceType? hitDie = character.GetHitDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(hitDie, Is.Not.Null);
        Assert.That(hitDie?.Name, Is.EqualTo("D6"));

    }

    [Test]
    [Order(5)]
    public void TestCharacterWithClassMageHasCorrectManaDie() {
        Character character = new Character();
        character.CharacterClass = CharacterClass.AllClassesAsync().Result.Find(c => c.Name == "Mage");
        DiceType? manaDie = character.GetManaDice();

        Assert.That(character.CharacterClass, Is.Not.Null);
        Assert.That(character.CharacterClass?.Name, Is.EqualTo("Mage"));
        Assert.That(manaDie, Is.Not.Null);
        Assert.That(manaDie?.Name, Is.EqualTo("D12"));
    }
}
