using Xunit;
using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class StatsUnitTests {

    [Fact]
    public void BlankCreateCharacterStats() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);

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
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        // Arrange & Act
        CharacterStats stats = new CharacterStats(character);
        stats.SetStat(StatType.Strength, 15, character);
        stats.SetStat(StatType.Constitution, 14, character);
        stats.SetStat(StatType.Dexterity, 13, character);
        stats.SetStat(StatType.Wisdom, 12, character);
        stats.SetStat(StatType.Charisma, 10, character);
        stats.SetStat(StatType.Intelligence, 8, character);

        // Assert
        Assert.Equal(15, stats.Strength);
        Assert.Equal(14, stats.Constitution);
        Assert.Equal(13, stats.Dexterity);
        Assert.Equal(12, stats.Wisdom);
        Assert.Equal(10, stats.Charisma);
        Assert.Equal(8, stats.Intelligence);
    }

    [Fact]
    public void InvalidStatValue() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);

        Assert.Throws<ArgumentException>(() => stats.SetStat(StatType.Strength, -5, character));
        Assert.Throws<ArgumentException>(() => stats.SetStat(StatType.Constitution, 0, character));
    }

    [Fact]
    public void InvalidStatType() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.SetStat((StatType)999, 10, character));
    }

    [Fact]
    public void CannotEditActiveCharacterStats() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);
        character.SetActive();

        Assert.Throws<InvalidOperationException>(() => stats.SetStat(StatType.Strength, 15, character));
    }   

    [Fact]
    public void IncreaseAndDecreaseStats() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);
        character.SetActive();

        stats.IncreaseStat(StatType.Strength, 5, character);
        stats.DecreaseStat(StatType.Constitution, 3, character);

        Assert.Equal(15, stats.Strength);
        Assert.Equal(7, stats.Constitution);
    }

    [Fact]
    public void InvalidIncreaseDecreaseAmount() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);
        character.SetActive();

        Assert.Throws<ArgumentException>(() => stats.IncreaseStat(StatType.Strength, -5, character));
        Assert.Throws<ArgumentException>(() => stats.DecreaseStat(StatType.Constitution, 0, character));
    }

    [Fact]
    public void CannotIncreaseDecreaseInactiveCharacter() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);

        Assert.Throws<InvalidOperationException>(() => stats.IncreaseStat(StatType.Strength, 5, character));
        Assert.Throws<InvalidOperationException>(() => stats.DecreaseStat(StatType.Constitution, 3, character));

        Assert.Equal(10, stats.Strength);
        Assert.Equal(10, stats.Constitution);
    }

    [Fact]
    public void InvalidStatTypeForIncreaseDecrease() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);
        character.SetActive();

        Assert.Throws<ArgumentOutOfRangeException>(() => stats.IncreaseStat((StatType)999, 5, character));
        Assert.Throws<ArgumentOutOfRangeException>(() => stats.DecreaseStat((StatType)999, 3, character));
    }

    [Fact]
    public void SetStatAcceptsMinimumValidValue() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        CharacterStats stats = new CharacterStats(character);

        stats.SetStat(StatType.Dexterity, 1, character);

        Assert.Equal(1, stats.Dexterity);
    }

    [Fact]
    public void CharacterStatsLinksToCorrectCharacter() {
        Player player = new Player("TestPlayer", "TestSetting");
        Character character = new Character(player);
        character.SimulatePersist(42);
        CharacterStats stats = new CharacterStats(character);

        Assert.Equal(42, stats.CharacterId);
    }
}