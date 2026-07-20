namespace ChroniclesTest;

using ChroniclesAPI.Services;







public class SciFiRaceApiTests {
    [Fact]
    public void Constructor_SetsNameAndDescription() {


    }

    [Fact]
    public void Id_DefaultsToZero() {
        var background = new ScifiBackground("Mercenary", "Someone who fights for money.");

        Assert.Equal(0, background.Id);
    }

    
}

