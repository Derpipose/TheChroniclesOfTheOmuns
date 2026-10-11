using Xunit;
using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class StatsUnitTests {

    [Fact]
    public void BlankCreateCharacterStats() {
        CharacterStats stats = new CharacterStats(1);

        Assert.Equal(10, stats.Strength);
        Assert.Equal(10, stats.Constitution);
        Assert.Equal(10, stats.Dexterity);
        Assert.Equal(10, stats.Wisdom);
        Assert.Equal(10, stats.Charisma);
        Assert.Equal(10, stats.Intelligence);
        Assert.IsType<CharacterStats>(stats);
    }

    [Fact]
    public void CreateCharacterStats() {
        Character character = new Character(1, "TestCharacter");

        // Arrange & Act
        CharacterStats stats = new CharacterStats(1);
        stats.SetStat(StatType.Strength, 15, character);
        stats.SetStat(StatType.Constitution, 14, character);
        stats.SetStat(StatType.Dexterity, 13, character);
        stats.SetStat(StatType.Wisdom, 12, character);
        stats.SetStat(StatType.Charisma, 10, character);
        stats.SetStat(StatType.Intelligence, 11, character);

        // Assert
        Assert.Equal(15, stats.Strength);
        Assert.Equal(14, stats.Constitution);
        Assert.Equal(13, stats.Dexterity);
        Assert.Equal(12, stats.Wisdom);
        Assert.Equal(10, stats.Charisma);
        Assert.Equal(11, stats.Intelligence);
    }

    [Fact]
    public void InvalidStatValue() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);

        Assert.Throws<ArgumentException>(() => stats.SetStat(StatType.Strength, -5, character));
        Assert.Throws<ArgumentException>(() => stats.SetStat(StatType.Constitution, 0, character));
    }

    [Fact]
    public void InvalidStatType() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.SetStat((StatType)999, 10, character));
    }

    [Fact]
    public void CannotEditActiveCharacterStats() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);
        character.SetActive();

        Assert.Throws<InvalidOperationException>(() => stats.SetStat(StatType.Strength, 15, character));
    }   

    [Fact]
    public void IncreaseAndDecreaseStats() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);
        character.SetActive();

        stats.IncreaseStat(StatType.Strength, 5, character);
        stats.IncreaseStat(StatType.Constitution, 5, character);
        stats.DecreaseStat(StatType.Constitution, 3, character);

        Assert.Equal(15, stats.Strength);
        Assert.Equal(12, stats.Constitution);
    }

    [Fact]
    public void DecreaseStatBelowMinimumThrows() {
        Character character = new Character(1, "TestCharacter");
        CharacterStats stats = new CharacterStats(1);
        character.SetActive();

        Assert.Throws<InvalidOperationException>(() => stats.DecreaseStat(StatType.Constitution, 1, character));
        Assert.Equal(CharacterStats.MinStat, stats.Constitution);
    }

    [Fact]
    public void InvalidIncreaseDecreaseAmount() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);
        character.SetActive();

        Assert.Throws<ArgumentException>(() => stats.IncreaseStat(StatType.Strength, -5, character));
        Assert.Throws<ArgumentException>(() => stats.DecreaseStat(StatType.Constitution, 0, character));
    }

    [Fact]
    public void CannotIncreaseDecreaseInactiveCharacter() {
        Character character = new Character(1, "TestCharacter");

        CharacterStats stats = new CharacterStats(1);

        Assert.Throws<InvalidOperationException>(() => stats.IncreaseStat(StatType.Strength, 5, character));
        Assert.Throws<InvalidOperationException>(() => stats.DecreaseStat(StatType.Constitution, 3, character));

        Assert.Equal(10, stats.Strength);
        Assert.Equal(10, stats.Constitution);
    }

    [Fact]
    public void InvalidStatTypeForIncreaseDecrease() {
        Character character = new Character(1, "TestCharacter");
        CharacterStats stats = new CharacterStats(1);
        character.SetActive();

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.IncreaseStat((StatType)999, 5, character));
        Assert.Throws<ArgumentOutOfRangeException>(() => stats.DecreaseStat((StatType)999, 3, character));
    }

    [Fact]
    public void SetStatAcceptsMinimumValidValue() {
        Character character = new Character(1, "TestCharacter");
        CharacterStats stats = new CharacterStats(1);

        stats.SetStat(StatType.Dexterity, CharacterStats.MinStat, character);

        Assert.Equal(CharacterStats.MinStat, stats.Dexterity);
    }

    [Fact]
    public void CharacterStatsLinksToCorrectCharacter() {
        CharacterStats stats = new CharacterStats(42);

        Assert.Equal(42, stats.CharacterId);
    }
}