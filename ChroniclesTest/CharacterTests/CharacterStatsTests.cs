using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

[TestFixture]
public class CharacterStatsUnitTests {
    CharacterService characterService = new CharacterService();
    CharacterRace race;
    CharacterRace raceWithBonuses;
    CharacterClass charClass;
    Character character;

    [SetUp]
    public void Setup() {
        race = new CharacterRace {
            Name = "TestRace",
            RaceType = "TestType",
            Description = "A test race"
        };
        raceWithBonuses = new CharacterRace {
            Name = "BonusRace",
            RaceType = "TestType",
            Description = "A test race with bonuses",
            RaceStatBonuses = new List<RaceStatBonus> {
                new() { BonusValue = 2, StatId = (int)PlayerApp.Models.Enums.StatType.Strength, IsSelectable = false },
                new() { BonusValue = 1, StatId = (int)PlayerApp.Models.Enums.StatType.Dexterity, IsSelectable = false }
            }
        };
        charClass = new CharacterClass {
            Name = "TestClass",
            ClassType = PlayerApp.Models.Enums.ClassTypeEnum.Combat,
            Description = "A test class",
            HitDiceId = 5,
            ManaDiceId = 1,
            HitDice = new DiceType { Id = 5, Name = "D12", Sides = 12 },
            ManaDice = new DiceType { Id = 1, Name = "D4", Sides = 4 }
        };
        character = new Character {
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
        Character thisCharacter = character;
        characterService.UpdateCharacterRace(thisCharacter, race);
        characterService.UpdateCharacterClass(thisCharacter, charClass);
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
        Character thisCharacter = character;
        characterService.UpdateCharacterRace(thisCharacter, raceWithBonuses);
        characterService.UpdateCharacterClass(thisCharacter, charClass);
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
