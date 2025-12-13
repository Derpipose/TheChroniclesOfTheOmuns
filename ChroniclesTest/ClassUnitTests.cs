using PlayerApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChroniclesTest;

public class ClassUnitTests {
    [SetUp]
    public void SetUp() {

    }

    [Test]
    public void TestCharacterClassCreation() {
        CharacterClass charClass = new CharacterClass() {
            Name = "Warrior",
            Description = "A strong melee fighter",
        };
        Assert.Multiple(() => {
            Assert.That(charClass.Name, Is.EqualTo("Warrior"));
            Assert.That(charClass.Description, Is.EqualTo("A strong melee fighter"));
        });
    }

    [Test]
    public void TestClassPullWorks() {
        List<CharacterClass> list = new List<CharacterClass>();
        list = CharacterClass.AllClassesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(53));
    }
}
