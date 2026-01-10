using PlayerApp.Models;
using PlayerApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChroniclesTest;

[TestFixture]
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
            RaceType = "Versatile",
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

        Assert.That(list.Count, Is.EqualTo(49));
    }

    [Test]
    public void CanAssignModifiersToRace() {
        var race = new CharacterRace { Name = "TestRace", RaceType = "TestType", Description = "A test race" };
        race.AddModifier(ModifierType.ManaBonus, 10);

        var modifier = race.GetModifier(ModifierType.ManaBonus);
        Assert.That(modifier, Is.Not.Null);
        Assert.That(modifier!.Modifier.Value, Is.EqualTo(10));
    }

    [Test]
    public void KoboldHasManaBonusModifierOf15() {
        List<CharacterRace> races = raceService.GetAllRacesAsync().Result;
        CharacterRace? kobold = races.FirstOrDefault(r => r.Name == "Kobold");

        Assert.That(kobold, Is.Not.Null);

        var manaBonus = kobold.GetModifier(ModifierType.ManaBonus);

        Assert.That(manaBonus, Is.Not.Null);
        Assert.That(manaBonus.Modifier.Value, Is.EqualTo(15));
    }

    [Test]
    public void HumanHasNoManaBonus() {
        List<CharacterRace> races = raceService.GetAllRacesAsync().Result;
        CharacterRace? human = races.FirstOrDefault(r => r.Name == "Human");

        Assert.That(human, Is.Not.Null);

        var manaBonus = human!.Modifiers?
            .FirstOrDefault(m => m.Modifier.Type == PlayerApp.Models.Enums.ModifierType.ManaBonus);

        Assert.That(manaBonus, Is.Null);
    }
}
