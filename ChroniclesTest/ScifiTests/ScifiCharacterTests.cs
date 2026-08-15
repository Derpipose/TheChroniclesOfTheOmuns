namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

public class ScifiCharacterTests {

    [Fact]
    public void TestScifiCharacterCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        Assert.IsType<ScifiCharacter>(scifiCharacter);
        Assert.NotNull(scifiCharacter);
    }

    [Fact]
    public void AssignScifiCharacterToCharacterBase() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);

        Assert.IsType<Character>(character);
        Assert.NotNull(character);

        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        Assert.Equal(1, scifiCharacter.CharacterId);
        Assert.Equal(character.Id, scifiCharacter.CharacterId);
    }

    [Fact]
    public void SetScifiRace() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);


        scifiCharacter.SetScifiRace(scifiRace, []);

        Assert.Equal(scifiRace.Id, scifiCharacter.ScifiRaceId);
    }

    [Fact]
    public void SetScifiSubrace() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1,1,0,0);
        scifiRace.SimulatePersist(1);
        ScifiSubrace scifiSubrace = new ScifiSubrace(1,"TestSubrace", "Test description", 1);
        scifiSubrace.SimulatePersist(1);

        scifiCharacter.SetScifiRace(scifiRace, [], scifiSubrace);

        Assert.Equal(scifiSubrace.Id, scifiCharacter.ScifiSubraceId);
    }

    [Fact]
    public void SetScifiClass() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiClass scifiClass = new ScifiClass("TestClass", DiceType.D10, "Test");
        scifiClass.SimulatePersist(1);

        scifiCharacter.SetScifiClass(scifiClass.Id);

        Assert.Equal(scifiClass.Id, scifiCharacter.ScifiClassId);
    }

    [Fact]
    public void SetScifiBackground() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);
        Character character = new Character(player.Id, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        ScifiBackground scifiBackground = new ScifiBackground("TestBackground", "Test description");
        scifiBackground.SimulatePersist(1);

        scifiCharacter.SetScifiBackground(scifiBackground.Id);

        Assert.Equal(scifiBackground.Id, scifiCharacter.ScifiBackgroundId);
    }


    // Tests that will showcase the new scifi race assignments after having the race modifiers added in. 
    [Fact]
    public void SetScifiRace_AppliesFixedModifiers() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Strength, 2, false) },
            freeStatPicks: null);
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        scifiCharacter.SetScifiRace(scifiRace, []);

        Assert.Equal(scifiRace.Id, scifiCharacter.ScifiRaceId);
        Assert.Single(scifiCharacter.Modifiers);
        Assert.Equal(ModifierType.Strength, scifiCharacter.Modifiers.First().ModifierType);
        Assert.Equal(ModifierSource.Race, scifiCharacter.Modifiers.First().Source);
        Assert.Equal(2, scifiCharacter.Modifiers.First().Amount);
    }

    [Fact]
    public void SetScifiRace_DeferredPicks_CreatesUnassignedPlaceholders() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: null,
            freeStatPicks: new List<int> { 1, 2 });
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        scifiCharacter.SetScifiRace(scifiRace, []);

        Assert.Equal(2, scifiCharacter.Modifiers.Count);
        Assert.All(scifiCharacter.Modifiers, m => Assert.Equal(ModifierType.Unassigned, m.ModifierType));
        Assert.All(scifiCharacter.Modifiers, m => Assert.Equal(ModifierSource.RacePick, m.Source));
        Assert.Equal(new List<int> { 1, 2 }, scifiCharacter.Modifiers.Select(m => m.Amount).OrderBy(x => x).ToList());
    }

    [Fact]
    public void SetScifiRace_FullySpecifiedPicks_AppliesRealAssignments() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: null,
            freeStatPicks: new List<int> { 1, 2 });
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        scifiCharacter.SetScifiRace(scifiRace, new List<(StatType, int)> {
            (StatType.Wisdom, 2),
            (StatType.Charisma, 1)
        });

        Assert.Equal(2, scifiCharacter.Modifiers.Count);
        Assert.DoesNotContain(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Unassigned);
        Assert.Contains(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Wisdom && m.Amount == 2 && m.Source == ModifierSource.RacePick);
        Assert.Contains(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Charisma && m.Amount == 1 && m.Source == ModifierSource.RacePick);
    }

    [Fact]
    public void SetScifiRace_PartialPickCount_Throws() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: null,
            freeStatPicks: new List<int> { 1, 2 });
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        scifiCharacter.SetScifiRace(scifiRace, new List<(StatType, int)> { (StatType.Wisdom, 2) });
        Assert.Contains(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Unassigned);
        Assert.Contains(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Wisdom && m.Amount == 2 && m.Source == ModifierSource.RacePick);
        Assert.Contains(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Unassigned && m.Amount == 1 && m.Source == ModifierSource.RacePick);
    }

    [Fact]
    public void SetScifiRace_DuplicateStatPicks_Throws() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: null,
            freeStatPicks: new List<int> { 1, 2 });
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        Assert.Throws<InvalidOperationException>(() =>
            scifiCharacter.SetScifiRace(scifiRace, new List<(StatType, int)> {
                (StatType.Wisdom, 1),
                (StatType.Wisdom, 2)
                /* ??? — two picks, SAME StatType, satisfying the count (2) so only the duplicate-stat check fires */
            }));
    }

    [Fact]
    public void SetScifiRace_PickOnAlreadyModifiedStat_Throws() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Strength, 2, false) },
            freeStatPicks: new List<int> { 1 });
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        Assert.Throws<InvalidOperationException>(() =>
            scifiCharacter.SetScifiRace(scifiRace, new List<(StatType, int)> { (StatType.Strength, 1) }));
    }

    [Fact]
    public void SetScifiRace_SubraceWithStats_OverridesRaceEntirely() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var raceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Strength, 2, false) },
            freeStatPicks: null);
        raceStats.SimulatePersist(1);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, raceStats.Id, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(raceStats);

        var subraceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Dexterity, 3, false) },
            freeStatPicks: null);
        subraceStats.SimulatePersist(2);

        ScifiSubrace scifiSubrace = new ScifiSubrace(scifiRace.Id, "TestSubrace", "Test description", subraceStats.Id);
        scifiSubrace.SimulatePersist(1);
        scifiSubrace.SetSubraceStats(subraceStats);

        scifiCharacter.SetScifiRace(scifiRace, [], scifiSubrace);

        Assert.Single(scifiCharacter.Modifiers);
        Assert.Equal(ModifierType.Dexterity, scifiCharacter.Modifiers.First().ModifierType);
        Assert.Equal(ModifierSource.Subrace, scifiCharacter.Modifiers.First().Source);
        Assert.DoesNotContain(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Strength);
    }

    [Fact]
    public void ClearScifiRace_WipesAllFourModifierSources_OnReassignment() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);

        var firstRaceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Strength, 2, false) },
            freeStatPicks: null);
        firstRaceStats.SimulatePersist(1);

        ScifiRace firstRace = new ScifiRace("TestSubType", "FirstRace", "Test description", true, firstRaceStats.Id, 30, 0, 0);
        firstRace.SimulatePersist(1);
        firstRace.SetRaceStats(firstRaceStats);

        scifiCharacter.SetScifiRace(firstRace, []);
        Assert.Single(scifiCharacter.Modifiers); // sanity check before reassignment

        var secondRaceStats = new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Intelligence, 1) },
            freeStatPicks: null);
        secondRaceStats.SimulatePersist(2);

        ScifiRace secondRace = new ScifiRace("TestSubType", "SecondRace", "Test description", true, secondRaceStats.Id, 30, 0, 0);
        secondRace.SimulatePersist(2);
        secondRace.SetRaceStats(secondRaceStats);

        /* ??? — call SetScifiRace again with secondRace */
        scifiCharacter.SetScifiRace(secondRace, []);

        Assert.Single(scifiCharacter.Modifiers);
        Assert.Equal(ModifierType.Intelligence, scifiCharacter.Modifiers.First().ModifierType);
        Assert.DoesNotContain(scifiCharacter.Modifiers, m => m.ModifierType == ModifierType.Strength);
    }
}