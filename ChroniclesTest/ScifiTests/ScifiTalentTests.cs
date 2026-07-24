namespace ChroniclesTest;

using PlayerApp.Models;

public class ScifiTalentTests {
    [Fact]
    public void Constructor_SetsNameAndDescription() {
        var talent = new ScifiTalent("Resourcefulness", "Ability to make do with what's available.", "An ability");

        Assert.Equal("Resourcefulness", talent.Name);
        Assert.Equal("Ability to make do with what's available.", talent.Description);
    }

    [Fact]
    public void Id_DefaultsToZero() {
        var talent = new ScifiTalent("Resourcefulness", "Ability to make do with what's available.", "An ability");

        Assert.Equal(0, talent.Id);
    }

#if DEBUG
    [Fact]
    public void SimulatePersist_SetsId() {
        var talent = new ScifiTalent("Resourcefulness", "Ability to make do with what's available.", "An ability");
        talent.SimulatePersist(1);

        Assert.Equal(1, talent.Id);
    }
#endif
}