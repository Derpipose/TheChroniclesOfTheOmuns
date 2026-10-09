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

    [Fact]
    public void UpdateHealth_AppliesNoRacialBonus() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        scifiCharacter.SimulatePersist(1);
        scifiCharacter.SimulateSetCharacter(character);
        
        
        ScifiClass scifiClass = new ScifiClass("TestClass", DiceType.D10, "Test");
        scifiClass.SimulatePersist(1);
        CharacterStats stats = new CharacterStats(character.Id);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(new RaceStats(
            modifiers: new List<RaceStatModifier> { },
            freeStatPicks: null));

        scifiCharacter.SetScifiRace(scifiRace, []);

        scifiCharacter.UpdateHealth(stats, scifiClass);

        int expectedConstitution = 10 ; // base 10 + racial bonus
        int expectedConMod = (expectedConstitution - 10) / 2; //
        int expectedHitDiceMax = scifiClass.HitDice.Sides(); // D10
        int expectedHealth = expectedConstitution * 2 + expectedHitDiceMax + expectedConMod;
        Assert.Equal(expectedHealth, character.MaxHealth);

    }

    [Fact]
    public void UpdateHealth_AppliesConstitutionRacialBonus() {
        // ??? — build a Character, a CharacterStats with a known Constitution (e.g. 10),
        //       a ScifiClass with a known HitDice, and a ScifiCharacter linked to the Character
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        scifiCharacter.SimulatePersist(1);
        scifiCharacter.SimulateSetCharacter(character);
        
        
        ScifiClass scifiClass = new ScifiClass("TestClass", DiceType.D10, "Test");
        scifiClass.SimulatePersist(1);
        CharacterStats stats = new CharacterStats(character.Id);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(new RaceStats(
            modifiers: new List<RaceStatModifier> { new(StatType.Constitution, 4, false) },
            freeStatPicks: null));

        scifiCharacter.SetScifiRace(scifiRace, []);

        scifiCharacter.UpdateHealth(stats, scifiClass);

        int expectedConstitution = 10 + 4; // base 10 + racial bonus
        int expectedConMod = (expectedConstitution - 10) / 2; //
        int expectedHitDiceMax = scifiClass.HitDice.Sides(); // D10
        int expectedHealth = expectedConstitution * 2 + expectedHitDiceMax + expectedConMod;
        Assert.Equal(expectedHealth, character.MaxHealth);

    }

    [Fact]
    public void SetScifiRace_Throws_WhenModifiersPushStatBelowMinimum() {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        scifiCharacter.SimulatePersist(1);
        CharacterStats stats = new CharacterStats(character.Id);

        ScifiRace scifiRace = new ScifiRace("TestSubType", "TestRace", "Test description", true, 1, 30, 0, 0);
        scifiRace.SimulatePersist(1);
        scifiRace.SetRaceStats(new RaceStats(
            modifiers: new List<RaceStatModifier> { new RaceStatModifier(StatType.Constitution, -2) },
            freeStatPicks: null));

        Assert.Throws<InvalidOperationException>(() => scifiCharacter.SetScifiRace(scifiRace, [], null, stats));

        // Raising the base stat first makes the same race valid
        Character other = new Character(1, "Other");
        stats.SetStat(StatType.Constitution, 12, other);
        scifiCharacter.SetScifiRace(scifiRace, [], null, stats);
    }

    [Fact]
    public void SetStat_Throws_WhenModifiersPushStatBelowMinimum() {
        Character character = new Character(1, "TestCharacter");
        CharacterStats stats = new CharacterStats(1);
        var modifiers = new List<ICharacterModifier> {
            new ScifiCharacterModifier(1, ModifierType.Constitution, -3, ModifierSource.Race)
        };

        Assert.Throws<InvalidOperationException>(() => stats.SetStat(StatType.Constitution, 12, character, modifiers));
        stats.SetStat(StatType.Constitution, 13, character, modifiers);
        Assert.Equal(10, stats.GetStat(StatType.Constitution, modifiers));
    }

    // GetArmorClass

    private static (ScifiCharacter scifiCharacter, CharacterStats stats) BuildAcCharacter(bool withRace = true, bool withClass = true, bool withBackground = true) {
        Character character = new Character(1, "TestCharacter");
        character.SimulatePersist(1);
        ScifiCharacter scifiCharacter = new ScifiCharacter(character.Id);
        scifiCharacter.SimulatePersist(1);
        scifiCharacter.SimulateSetCharacter(character);

        ScifiRace? race = withRace ? new ScifiRace("TestSubType", "TestRace", "Test description", true, 1, 30, 0, 0) : null;
        ScifiClass? scifiClass = withClass ? new ScifiClass("TestClass", DiceType.D10, "Test") : null;
        ScifiBackground? background = withBackground ? new ScifiBackground("TestBackground", "Test description") : null;
        scifiCharacter.SimulateSetNavigations(race, scifiClass, background);

        return (scifiCharacter, new CharacterStats(character.Id));
    }

    [Fact]
    public void GetArmorClass_DefaultStats_Is10() {
        var (scifiCharacter, stats) = BuildAcCharacter();

        Assert.Equal(10, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_AddsDexterityModifier() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        stats.SetStat(StatType.Dexterity, 14, scifiCharacter.Character!); // +2

        Assert.Equal(12, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_UsesDexterityAfterModifiers() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.Dexterity, 4, ModifierSource.Race)); // 10 + 4 = 14 -> +2

        Assert.Equal(12, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_AddsArmorClassModifiers() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.ArmorClass, 3, ModifierSource.Talent));
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.Strength, 4, ModifierSource.Race)); // unrelated, ignored

        Assert.Equal(13, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_NegativeArmorClassModifier_ReducesValue() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.ArmorClass, -1, ModifierSource.Talent));

        Assert.Equal(9, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_IgnoresHealthAndOtherStatModifiers() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.Health, 10, ModifierSource.Class));
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.Strength, 3, ModifierSource.Race));

        Assert.Equal(10, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_CombinesDexterityAndArmorClassModifiers() {
        var (scifiCharacter, stats) = BuildAcCharacter();
        stats.SetStat(StatType.Dexterity, 16, scifiCharacter.Character!); // +3
        scifiCharacter.Modifiers.Add(new ScifiCharacterModifier(1, ModifierType.ArmorClass, 2, ModifierSource.Talent));

        Assert.Equal(15, scifiCharacter.GetArmorClass(stats));
    }

    [Fact]
    public void GetArmorClass_Works_WithoutRaceClassBackgroundOrCharacter() {
        // Edit-mode characters may not have these assigned (or loaded) yet
        ScifiCharacter scifiCharacter = new ScifiCharacter(1);
        CharacterStats stats = new CharacterStats(1);

        Assert.Equal(10, scifiCharacter.GetArmorClass(stats));
    }
}
