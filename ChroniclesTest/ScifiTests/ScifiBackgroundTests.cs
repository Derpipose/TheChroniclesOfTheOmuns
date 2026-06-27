namespace ChroniclesTest;

using PlayerApp.Models;

public class ScifiBackgroundTests {
    [Fact]
    public void Constructor_SetsNameAndDescription() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.Equal("Mercenary", background.Name);
        Assert.Equal("Someone who fights for money.", background.Description);
    }

    [Fact]
    public void Id_DefaultsToZero() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.Equal(0, background.Id);
    }

    [Fact]
    public void Restricted_DefaultsToFalse() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.False(background.Restricted);
    }

    [Fact]
    public void HasClassOverlap_DefaultsToFalse() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.False(background.HasClassOverlap);
    }

    [Fact]
    public void Constructor_SetsRestrictedAndHasClassOverlap() {
        var background = new ScifiBackground("Castaway", "Forgotten and marooned.", restricted: true, hasClassOverlap: false);

        Assert.True(background.Restricted);
        Assert.False(background.HasClassOverlap);
    }

    [Fact]
    public void BackgroundTalents_DefaultsToEmpty() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.Empty(background.BackgroundTalents);
    }

    [Fact]
    public void SimulatePersist_SetsId() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");
        background.SimulatePersist(1);

        Assert.Equal(1, background.Id);
    }
}