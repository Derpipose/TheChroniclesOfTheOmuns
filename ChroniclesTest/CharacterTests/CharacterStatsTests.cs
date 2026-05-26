using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

namespace ChroniclesTest;

[TestFixture]
public class CharacterStatsUnitTests {
    CharacterService characterService = new CharacterService();
    FantasyCharacterRace race;
    FantasyCharacterRace raceWithBonuses;
    FantasyCharacterClass charClass;
    FantasyCharacter character;

    [SetUp]
    public void Setup() {
        race = new FantasyCharacterRace {
            Name = "TestRace",
            RaceType = "TestType",
            Description = "A test race"
        };
        raceWithBonuses = new FantasyCharacterRace {
            Name = "BonusRace",
            RaceType = "TestType",
            Description = "A test race with bonuses",
            RaceStatBonuses = new List<FantasyRaceBonus> {
                new() { BonusValue = 2, StatId = (int)PlayerApp.Models.Enums.StatType.Strength, IsSelectable = false },
                new() { BonusValue = 1, StatId = (int)PlayerApp.Models.Enums.StatType.Dexterity, IsSelectable = false }
            }
        };
        charClass = new FantasyCharacterClass {
            Name = "TestClass",
            FantasyClassType = PlayerApp.Models.Enums.FantasyClassTypeEnum.Combat,
            Description = "A test class",
            HitDice = DiceType.D12,
            ManaDice = DiceType.D4
        };
        character = new FantasyCharacter {
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
    }


    [Test]
    public void UpdateCharacterStats() {
        FantasyCharacter thisCharacter = character;
        characterService.UpdateFantasyCharacterRace(thisCharacter, race);
        characterService.UpdateFantasyCharacterClass(thisCharacter, charClass);
        Assert.Multiple(() => {
            Assert.That(thisCharacter.Stats.Strength, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Dexterity, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Constitution, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Wisdom, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Charisma, Is.EqualTo(10));
        });

        characterService.UpdateCharacterStats(thisCharacter, new CharacterStats {
            Strength = 15,
            Dexterity = 14,
            Constitution = 13,
            Intelligence = 12,
            Wisdom = 11,
            Charisma = 10
        });

        Assert.Multiple(() => {
            Assert.That(thisCharacter.Stats.Strength, Is.EqualTo(15));
            Assert.That(thisCharacter.Stats.Dexterity, Is.EqualTo(14));
            Assert.That(thisCharacter.Stats.Constitution, Is.EqualTo(13));
            Assert.That(thisCharacter.Stats.Intelligence, Is.EqualTo(12));
            Assert.That(thisCharacter.Stats.Wisdom, Is.EqualTo(11));
            Assert.That(thisCharacter.Stats.Charisma, Is.EqualTo(10));
        });
    }

    [Test]
    public void UpdateCharacterStatsWithRaceBonuses() {
        FantasyCharacter thisCharacter = character;
        characterService.UpdateFantasyCharacterRace(thisCharacter, raceWithBonuses);
        characterService.UpdateFantasyCharacterClass(thisCharacter, charClass);
        Assert.Multiple(() => {
            Assert.That(thisCharacter.Stats.Strength, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Dexterity, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Constitution, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Intelligence, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Wisdom, Is.EqualTo(10));
            Assert.That(thisCharacter.Stats.Charisma, Is.EqualTo(10));
        });
        int StrengthWithBonus = characterService.GetStat(thisCharacter, StatType.Strength);
        Assert.That(StrengthWithBonus, Is.EqualTo(12)); // 10 + 2
        int DexterityWithBonus = characterService.GetStat(thisCharacter, StatType.Dexterity);
        Assert.That(DexterityWithBonus, Is.EqualTo(11)); // 10 + 1

    }
}
