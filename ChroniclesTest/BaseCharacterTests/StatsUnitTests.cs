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
}