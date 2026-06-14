using Xunit;
using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class StatsUnitTests {

    [Fact]
    public void BlankCreateCharacterStats() {
        CharacterStats stats = new();

        Xunit.Assert.Equal(10, stats.Strength);
        Xunit.Assert.Equal(10, stats.Constitution);
        Xunit.Assert.Equal(10, stats.Dexterity);
        Xunit.Assert.Equal(10, stats.Wisdom);
        Xunit.Assert.Equal(10, stats.Charisma);
        Xunit.Assert.Equal(10, stats.Intelligence);
        Xunit.Assert.IsType<CharacterStats>(stats);
    }

    [Fact]
    public void CreateCharacterStats() {
        // Arrange & Act
        CharacterStats stats = new CharacterStats();
        Player player = new Player("TestPlayer", "TestSetting");
        stats.SetStat(StatType.Strength, 15, new Character(player));
        stats.SetStat(StatType.Constitution, 14, new Character(player));
        stats.SetStat(StatType.Dexterity, 13, new Character(player));
        stats.SetStat(StatType.Wisdom, 12, new Character(player));
        stats.SetStat(StatType.Charisma, 10, new Character(player));
        stats.SetStat(StatType.Intelligence, 8, new Character(player));

        // Assert
        Xunit.Assert.Equal(15, stats.Strength);
        Xunit.Assert.Equal(14, stats.Constitution);
        Xunit.Assert.Equal(13, stats.Dexterity);
        Xunit.Assert.Equal(12, stats.Wisdom);
        Xunit.Assert.Equal(10, stats.Charisma);
        Xunit.Assert.Equal(8, stats.Intelligence);
    }
}