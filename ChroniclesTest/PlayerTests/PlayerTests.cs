namespace ChroniclesTest;
using PlayerApp.Models;

[TestFixture]
public class PlayerTests {
    
    [Test]
    public void TestPlayerCreation() {
        Player player = new Player("TestPlayer", "testplayer@example.com");

        Assert.That(player, Is.TypeOf<Player>());
        Assert.That(player != null);
    }

    [Test]
    public void TestPlayerProperties() {
        Player player = new Player("TestPlayer", "testplayer@example.com")  ;

        Assert.That(player.DisplayName, Is.EqualTo("TestPlayer"));
        Assert.That(player.Email, Is.EqualTo("testplayer@example.com"));
    }

    [Test]
    public void TestPlayerLastLogin() {
        Player player = new Player("TestPlayer", "testplayer@example.com");

        player.UpdateLastLogin();
        Assert.That(player.LastLoginAt, Is.Not.Null);
    }

    [Test]
    public void TestPlayerSetters() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SetDisplayName("NewName");
        player.SetEmail("newemail@example.com");

        Assert.That(player.DisplayName, Is.EqualTo("NewName"));
        Assert.That(player.Email, Is.EqualTo("newemail@example.com"));
    }

    [Test]
    public void TestPlayerSimulatePersist() {
        Player player = new Player("TestPlayer", "testplayer@example.com");
        player.SimulatePersist(1);

        Assert.That(player.Id, Is.EqualTo(1));
    }

}