using PlayerApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChroniclesTest;

public class RaceUnitTests {

    [Test]
    public void TestCharacterRace() {
        CharacterRace race = new CharacterRace() {
            Name = "Human",
            Description = "Your standard, vanilla humanoid"
        };

        Assert.Multiple(() => {
            Assert.That(race.Name, Is.EqualTo("Human"));
            Assert.That(race.Description, Is.EqualTo("Your standard, vanilla humanoid"));
        });
    }
}
