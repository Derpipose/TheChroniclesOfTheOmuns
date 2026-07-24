using PlayerApp.Models;
using PlayerApp.Models.Dice;

namespace ChroniclesTest;

public class ScifiClassTests {
    [Fact]
    public void Constructor_SetsNameAndHitDice() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);

        Assert.Equal("Soldier", scifiClass.Name);
        Assert.Equal(DiceType.D12, scifiClass.HitDice);
    }

    [Fact]
    public void Constructor_Description_DefaultsToInProgress() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);

        Assert.Equal("In progress", scifiClass.Description);
    }

    [Fact]
    public void Constructor_SetsDescription_WhenProvided() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12, "A hardened combatant.");

        Assert.Equal("A hardened combatant.", scifiClass.Description);
    }

    [Fact]
    public void Id_DefaultsToZero() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);

        Assert.Equal(0, scifiClass.Id);
    }

    [Fact]
    public void ClassTalents_DefaultsToEmpty() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);

        Assert.Empty(scifiClass.ClassTalents);
    }

    [Fact]
    public void SimulatePersist_SetsId() {
        var scifiClass = new ScifiClass("Soldier", DiceType.D12);
        scifiClass.SimulatePersist(1);

        Assert.Equal(1, scifiClass.Id);
    }
}