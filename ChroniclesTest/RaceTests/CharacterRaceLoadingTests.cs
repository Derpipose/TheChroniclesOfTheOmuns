using System.Linq;
using PlayerApp.Models;

namespace ChroniclesTest;

[TestFixture]
public class CharacterRaceLoadingTests {
    private CharacterRaceService raceService;
    private CharacterService characterService;

    [SetUp]
    public void Setup() {
        raceService = new CharacterRaceService();
        characterService = new CharacterService();
    }

    [Test]
    [Order(1)]
    public void TestRacePullWorks() {
        List<CharacterRace> list = raceService.GetAllRacesAsync().Result;

        Assert.That(list.Count, Is.GreaterThan(0));
        Assert.That(list.Count, Is.EqualTo(49));
    }

    [Test]
    [Order(2)]
    public void TestCharacterWithRaceCanBeAssigned() {
        Character character = new Character();
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault();
        Assert.That(charRace, Is.Not.Null);

        character.AssignCharacterRace(charRace);

        Assert.That(character.CharacterRace, Is.Not.Null);
        Assert.That(character.CharacterRace?.Name, Is.EqualTo(charRace?.Name));
    }

    [Test]
    [Order(3)]
    public void TestCharacterRaceHasStatBonuses() {
        Character character = new Character {
            Name = "TestCharacter",
            Level = 1,
            Stats = new CharacterStats {
                Strength = 10,
                Dexterity = 10,
                Constitution = 10,
                Intelligence = 10,
                Wisdom = 10,
                Charisma = 10
            }
        };
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Kobold");
        Assert.That(charRace, Is.Not.Null);

        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        character.AssignCharacterRace(charRace);

        Assert.That(character.CharacterRace, Is.Not.Null);
        Assert.That(character.CharacterRace?.RaceStatBonuses.Count, Is.EqualTo(2));
    }

    [Test]
    [Order(4)]
    public void TestCharacterRaceStatBonusesAreCorrect() {
        Character character = new Character {
            Name = "TestCharacter",
            Level = 1,
            Stats = new CharacterStats {
                Strength = 10,
                Dexterity = 10,
                Constitution = 10,
                Intelligence = 10,
                Wisdom = 10,
                Charisma = 10
            }
        };
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Kobold");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));
        Assert.That(charRace.RaceStatBonuses.FirstOrDefault(b => b.StatId == (int)PlayerApp.Models.Enums.StatType.Wisdom)?.BonusValue, Is.EqualTo(2));
        Assert.That(charRace.RaceStatBonuses.FirstOrDefault(b => b.StatId == (int)PlayerApp.Models.Enums.StatType.Dexterity)?.BonusValue, Is.EqualTo(1));

        characterService.UpdateCharacterRace(character, charRace);

        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));
        var wisBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.StatId == (int)PlayerApp.Models.Enums.StatType.Wisdom);
        var dexBonus = charRace.RaceStatBonuses.FirstOrDefault(b => b.StatId == (int)PlayerApp.Models.Enums.StatType.Dexterity);
        Assert.That(wisBonus, Is.Not.Null);
        Assert.That(dexBonus, Is.Not.Null);
        Assert.That(wisBonus?.BonusValue, Is.EqualTo(2));
        Assert.That(dexBonus?.BonusValue, Is.EqualTo(1));
        Assert.Multiple(() => {
            Assert.That(characterService.GetStat(character, PlayerApp.Models.Enums.StatType.Wisdom), Is.EqualTo(12));
            Assert.That(characterService.GetStat(character, PlayerApp.Models.Enums.StatType.Dexterity), Is.EqualTo(11));
        });
    }

    [Test]
    [Order(5)]
    public void TestThatCharacterRaceIsLoadedWithChooseOneMod() {
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Aarakocra");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        Assert.That(charRace.RaceStatBonuses.Any(b => b.IsSelectable == true), Is.True);
        Assert.That(charRace.RaceStatBonuses.Any(b => b.BonusValue == 1), Is.True);
    }

    [Test]
    [Order(6)]
    public void TestThatCharacterRaceIsLoadedWithChooseTwoMod() {
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Kenku");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        Assert.That(charRace.RaceStatBonuses.Any(b => b.IsSelectable == true), Is.True);
        Assert.That(charRace.RaceStatBonuses.Any(b => b.BonusValue == 2), Is.True);
    }

    [Test]
    [Order(7)]
    public void TestThatCharacterRaceIsLoadedWithChooseBothMods() {
        CharacterRace? charRace = raceService.GetAllRacesAsync().Result.FirstOrDefault(r => r.Name == "Human");
        Assert.That(charRace, Is.Not.Null);
        Assert.That(charRace.RaceStatBonuses.Count, Is.EqualTo(2));

        var selectableBonus1 = charRace.RaceStatBonuses.ElementAtOrDefault(0);
        var selectableBonus2 = charRace.RaceStatBonuses.ElementAtOrDefault(1);


        Assert.That(selectableBonus1?.IsSelectable, Is.EqualTo(true));
        Assert.That(selectableBonus2?.IsSelectable, Is.EqualTo(true));
        Assert.That(charRace.RaceStatBonuses.Any(b => b.BonusValue == 1), Is.True);
        Assert.That(charRace.RaceStatBonuses.Any(b => b.BonusValue == 2), Is.True);
    }
}
