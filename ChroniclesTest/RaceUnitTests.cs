using PlayerApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChroniclesTest;

public class RaceUnitTests {
    private CharacterRaceService raceService;

    [SetUp]
    public void Setup() {
        raceService = new CharacterRaceService();
    }

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

    [Test]
    public void TestRacePullWorks() {
        List<CharacterRace> list = raceService.GetAllRacesAsync().Result;

        Assert.That(list.Count, Is.EqualTo(87));
    }
}
