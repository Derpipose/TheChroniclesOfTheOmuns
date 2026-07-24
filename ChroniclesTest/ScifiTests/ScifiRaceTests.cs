using Xunit;
using PlayerApp.Models;

namespace ChroniclesTest;

public class ScifiRaceTests {

    [Fact]
    public void CreateScifiRace() {
        ScifiRace race = new ScifiRace(
            subType: "Alien",
            name: "Divaren",
            description: "A short squat bearded race similar to humans.",
            isStarterRace: true,
            raceStatsId: 1,
            speed: 30,
            adventuringAgeStart: 18,
            ageMax: 90
        );

        Assert.Equal("Alien", race.SubType);
        Assert.Equal("Divaren", race.Name);
        Assert.Equal("A short squat bearded race similar to humans.", race.Description);
        Assert.True(race.IsStarterRace);
        Assert.Equal(1, race.RaceStatsId);
        Assert.Equal(30, race.Speed);
        Assert.Equal(18, race.AdventuringAgeStart);
        Assert.Equal(90, race.AgeMax);
    }

    [Fact]
    public void NonStarterRaceIsAllowed() {
        ScifiRace race = new ScifiRace(
            subType: "Alien",
            name: "Eel'a-Siek-Cha'za",
            description: "Squid-like race that values science above all else.",
            isStarterRace: false,
            raceStatsId: 2,
            speed: 30,
            adventuringAgeStart: 20,
            ageMax: 200
        );

        Assert.False(race.IsStarterRace);
    }

    [Fact]
    public void IdDefaultsToZeroBeforePersist() {
        ScifiRace race = new ScifiRace(
            subType: "Alien", name: "Araka", description: "The Araka are very war-like.",
            isStarterRace: true, raceStatsId: 3, speed: 30,
            adventuringAgeStart: 16, ageMax: 80
        );

        Assert.Equal(0, race.Id);
    }

    [Fact]
    public void SimulatePersistSetsId() {
        ScifiRace race = new ScifiRace(
            subType: "Alien", name: "Araka", description: "The Araka are very war-like.",
            isStarterRace: true, raceStatsId: 3, speed: 30,
            adventuringAgeStart: 16, ageMax: 80
        );

        race.SimulatePersist(7);

        Assert.Equal(7, race.Id);
    }

    [Fact]
    public void RaceStatsIdLinksToCorrectRaceStats() {
        ScifiRace race = new ScifiRace(
            subType: "Alien", name: "Araka", description: "The Araka are very war-like.",
            isStarterRace: true, raceStatsId: 99, speed: 30,
            adventuringAgeStart: 16, ageMax: 80
        );

        Assert.Equal(99, race.RaceStatsId);
    }

    [Fact]
    public void DifferentRacesCanHaveDifferentSpeeds() {
        ScifiRace fastRace = new ScifiRace(
            subType: "Alien", name: "Quin-Val", description: "A fast-moving race.",
            isStarterRace: true, raceStatsId: 4, speed: 40,
            adventuringAgeStart: 18, ageMax: 100
        );

        ScifiRace slowRace = new ScifiRace(
            subType: "Alien", name: "Divaren", description: "A short squat bearded race.",
            isStarterRace: true, raceStatsId: 1, speed: 25,
            adventuringAgeStart: 18, ageMax: 90
        );

        Assert.Equal(40, fastRace.Speed);
        Assert.Equal(25, slowRace.Speed);
    }
}