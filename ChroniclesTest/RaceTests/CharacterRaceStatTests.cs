using PlayerApp.Models;
using PlayerApp.Models.Enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ChroniclesTest;

[TestFixture]
public class CharacterRaceBonusTests {

    CharacterService characterService = new CharacterService();
    private CharacterRaceService raceService = new CharacterRaceService();
    private CharacterStats defaultStats;

    [SetUp]
    public void Setup() {
        defaultStats = new CharacterStats {
            Strength = 10,
            Dexterity = 10,
            Constitution = 10,
            Intelligence = 10,
            Wisdom = 10,
            Charisma = 10
        };
    }

    private Character CreateCharacter(string name) {
        return new Character {
            Name = name,
            Level = 1,
            Stats = defaultStats,
            CharacterStatBonuses = new List<CharacterStatBonus>()
        };
    }

    private CharacterRace CreateRace(int id, string name, string raceType, string description, List<RaceStatBonus> bonuses) {
        return new CharacterRace {
            Id = id,
            Name = name,
            RaceType = raceType,
            Description = description,
            RaceStatBonuses = bonuses
        };
    }

    private RaceStatBonus CreateStatBonus(int id, int raceId, int bonusValue, int statId, bool isSelectable = false) {
        return new RaceStatBonus {
            Id = id,
            RaceId = raceId,
            BonusValue = bonusValue,
            StatId = statId,
            IsSelectable = isSelectable
        };
    }

    [Test]
    public void ApplyRaceBonuses_WithFixedBonus_AppliesStatBonus() {
        var race = CreateRace(1, "Dwarf", "Stout", "Short and sturdy", 
            new List<RaceStatBonus> { CreateStatBonus(1, 1, 2, (int)StatType.Constitution) });
        var character = CreateCharacter("Gimli");

        characterService.UpdateCharacterRace(character, race);

        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));

        var bonus = character.CharacterStatBonuses.First();

        Assert.Multiple(() => {
            Assert.That(bonus.StatId, Is.EqualTo((int)StatType.Constitution));
            Assert.That(bonus.BonusValue, Is.EqualTo(2));
        });
    }

    [Test]
    public void RemoveRace_RemovesAppliedBonuses() {
        var race = CreateRace(1, "Dwarf", "Stout", "Short and sturdy",
            new List<RaceStatBonus> { CreateStatBonus(1, 1, 2, (int)StatType.Constitution) });
        var character = CreateCharacter("Gimli");

        characterService.UpdateCharacterRace(character, race);
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));

        characterService.RemoveCharacterRace(character);
        Assert.That(character.CharacterStatBonuses, Is.Empty);
    }

    [Test]
    public void ChangeRace_UpdatesAppliedBonuses() {
        var dwarfRace = CreateRace(1, "Dwarf", "Stout", "Short and sturdy",
            new List<RaceStatBonus> { CreateStatBonus(1, 1, 2, (int)StatType.Constitution) });
        var elfRace = CreateRace(2, "Elf", "Graceful", "Tall and agile",
            new List<RaceStatBonus> { CreateStatBonus(2, 2, 2, (int)StatType.Dexterity) });
        var character = CreateCharacter("Legolas");

        characterService.UpdateCharacterRace(character, dwarfRace);
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));
        Assert.That(character.CharacterStatBonuses.First().StatId, Is.EqualTo((int)StatType.Constitution));

        characterService.UpdateCharacterRace(character, elfRace);
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(1));
        var bonus = character.CharacterStatBonuses.First();
        Assert.Multiple(() => {
            Assert.That(bonus.StatId, Is.EqualTo((int)StatType.Dexterity));
            Assert.That(bonus.BonusValue, Is.EqualTo(2));
        });
    }

    // MultiBonus Tests
    [Test]
    public void ApplyRaceBonuses_WithMultipleFixedBonuses_AppliesAllBonuses() {
        var race = CreateRace(1, "Orc", "Brutish", "Strong and fierce",
            new List<RaceStatBonus> {
                CreateStatBonus(1, 1, 2, (int)StatType.Strength),
                CreateStatBonus(2, 1, 1, (int)StatType.Constitution)
            });
        var character = CreateCharacter("Gorg");

        characterService.UpdateCharacterRace(character, race);
        
        Assert.That(character.CharacterStatBonuses, Has.Count.EqualTo(2));
    }


    [Test]
    public void TestThatCharacterCanReturnPicks() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Aarakocra");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        var selectableBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.IsSelectable == true);
        Assert.That(selectableBonus, Is.Not.Null);
        Assert.That(selectableBonus.BonusValue, Is.EqualTo(1));

        characterService.UpdateCharacterRace(character, charRace);

        // Returns list of selectable bonuses
        var selectableBonuses = characterService.GetSelectableRaceBonusesOnCharacter(character);
        Assert.That(selectableBonuses, Is.Not.Null);
        Assert.That(selectableBonuses.Count, Is.EqualTo(1));
        Assert.That(selectableBonuses[0].StatId, Is.EqualTo(selectableBonus.StatId));
    }

    [Test]
    public void TestThatCharacterWithChooseOneModCanChooseOne() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Aarakocra");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        var selectableBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.IsSelectable == true);
        Assert.That(selectableBonus, Is.Not.Null);
        Assert.That(selectableBonus.BonusValue, Is.EqualTo(1));

        characterService.UpdateCharacterRace(character, charRace);
        characterService.AssignSelectableRaceBonus(character, 1, StatType.Strength);
        Assert.That(() => characterService.AssignSelectableRaceBonus(character, 2, StatType.Strength), Throws.TypeOf<System.Exception>());
        
        var selectableBonusAssigned = characterService.GetSelectableRaceBonusesOnCharacter(character);
        Assert.That(selectableBonusAssigned, Is.Not.Null);
        Assert.That(selectableBonusAssigned.Count, Is.EqualTo(1));
        Assert.That(selectableBonusAssigned[0].StatId, Is.EqualTo((int)StatType.Strength));
    }

    [Test]
    public void TestThatCharacterWithChooseTwoModCanChooseTwo() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Kenku");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        var selectableBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.IsSelectable == true);
        Assert.That(selectableBonus, Is.Not.Null);
        Assert.That(selectableBonus.BonusValue, Is.EqualTo(2));

        characterService.UpdateCharacterRace(character, charRace);
        characterService.AssignSelectableRaceBonus(character, 2, StatType.Strength);
        Assert.That(() => characterService.AssignSelectableRaceBonus(character, 1, StatType.Strength), Throws.TypeOf<System.Exception>());
        
        var selectableBonusAssigned = characterService.GetSelectableRaceBonusesOnCharacter(character);
        Assert.That(selectableBonusAssigned, Is.Not.Null);
        Assert.That(selectableBonusAssigned.Count, Is.EqualTo(1));
        Assert.That(selectableBonusAssigned[0].StatId, Is.EqualTo((int)StatType.Strength));
    }

    [Test]
    public void TestThatCharacterWithChooseOneModCanNotAssignToAnAlreadyAssignedStat() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Aarakocra");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        var selectableBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.IsSelectable == true);
        Assert.That(selectableBonus, Is.Not.Null);
        Assert.That(selectableBonus.BonusValue, Is.EqualTo(1));

        characterService.UpdateCharacterRace(character, charRace);
        // Aarakocra have a +2 in dexterity fixed bonus
        // This means that they cannot assign the +1 selectable bonus to dexterity
        Assert.That(() => characterService.AssignSelectableRaceBonus(character, 1, StatType.Dexterity), Throws.TypeOf<System.Exception>());
        
        // This should pass though
        characterService.AssignSelectableRaceBonus(character, 1, StatType.Strength);
        
        var selectableBonusAssigned = characterService.GetSelectableRaceBonusesOnCharacter(character);
        Assert.That(selectableBonusAssigned, Is.Not.Null);
        Assert.That(selectableBonusAssigned.Count, Is.EqualTo(1));
        Assert.That(selectableBonusAssigned[0].StatId, Is.EqualTo((int)StatType.Strength));
    }

    [Test]
    public void TestThatCharacterWithChooseBothModsCanAssignStats() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Human");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        characterService.UpdateCharacterRace(character, charRace);
        characterService.AssignSelectableRaceBonus(character, 1, StatType.Strength);
        characterService.AssignSelectableRaceBonus(character, 2, StatType.Dexterity);
        
        var selectableBonusAssigned = characterService.GetSelectableRaceBonusesOnCharacter(character);
        Assert.That(selectableBonusAssigned, Is.Not.Null);
        Assert.That(selectableBonusAssigned.Count, Is.EqualTo(2));

        var bonusValue1 = selectableBonusAssigned.FirstOrDefault(b => b.BonusValue == 1);
        var bonusValue2 = selectableBonusAssigned.FirstOrDefault(b => b.BonusValue == 2);

        Assert.Multiple(() => {
            Assert.That(bonusValue1, Is.Not.Null);
            Assert.That(bonusValue1?.StatId, Is.EqualTo((int)StatType.Strength));
            Assert.That(bonusValue2, Is.Not.Null);
            Assert.That(bonusValue2?.StatId, Is.EqualTo((int)StatType.Dexterity));
        });
    }

    [Test]
    public void TestThatCharacterWithChooseBothModsCanNotDoubleAssignStats() {
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Human");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        characterService.UpdateCharacterRace(character, charRace);
        characterService.AssignSelectableRaceBonus(character, 1, StatType.Strength);
        Assert.That(() => characterService.AssignSelectableRaceBonus(character, 2, StatType.Strength), Throws.TypeOf<System.Exception>());
    }

    [Test]
    public void TestThatGetCharacterAvailableStatPickWorks(){
        var character = CreateCharacter("TestCharacter");
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Aarakocra");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        characterService.UpdateCharacterRace(character, charRace);
        
        var availablePicks = characterService.GetAvailableStatSelectableBonusesOnCharacter(character);
        Assert.That(availablePicks, Is.Not.Null);
        Assert.That(availablePicks.Count, Is.EqualTo(5));
        Assert.That(availablePicks.Any(b => b != StatType.Dexterity), Is.True);
    }
}
