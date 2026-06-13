namespace ChroniclesTest;

using Xunit;
using PlayerApp.Models;

public class PlayerTests {

    [Fact]
    public void TestPlayerCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");

        Assert.IsType<Player>(player);
        Assert.NotNull(player);
    }

    [Fact]
    public void TestPlayerProperties() {
        Player player = new Player("TestPlayer", "testplayer@example.com");

        Assert.Equal("TestPlayer", player.DisplayName);
        Assert.Equal("testplayer@example.com", player.Email);
    }

    [Fact]
    public void TestPlayerLastLogin() {
        Player player = new Player("TestPlayer", "testplayer@example.com");

        player.UpdateLastLogin();
        Assert.NotNull(player.LastLoginAt);
    }

    [Fact]
    public void TestPlayerSetters() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SetDisplayName("NewName");
        player.SetEmail("newemail@example.com");

        Assert.Equal("NewName", player.DisplayName);
        Assert.Equal("newemail@example.com", player.Email);
    }

    [Fact]
    public void TestPlayerSimulatePersist() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);

        Assert.Equal(1, player.Id);
    }
}