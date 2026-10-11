namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using Xunit;

public class ModifierTypeTests {



    [Fact]
    public void GetMaxHealth_NoModifiers_ReturnsBaseHealth() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(20);

        Assert.Equal(20, character.GetMaxHealth(new List<ICharacterModifier>()));
    }

    [Fact]
    public void GetMaxHealth_SingleHealthModifier_AddsBonus() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(20);
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier> {
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = 5
            }
        };

        Assert.Equal(25, character.GetMaxHealth(Modifiers));
    }

    [Fact]
    public void GetMaxHealth_MultipleHealthModifiers_SumsAllBonuses() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(20);
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier> {
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = 5
            },
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = 3
            },
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = 2
            }
        };

        Assert.Equal(30, character.GetMaxHealth(Modifiers));
    }

    [Fact]
    public void GetMaxHealth_NegativeHealthModifier_ReducesTotal() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(20);
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier> {
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = -5
            }
        };

        Assert.Equal(15, character.GetMaxHealth(Modifiers));
    }

    [Fact]
    public void GetMaxHealth_IgnoresNonHealthModifiers() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(20);
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier> {
            new TestModifier {
                ModifierType = ModifierType.Strength,
                Amount = 2
            },
            new TestModifier {
                ModifierType = ModifierType.ArmorClass,
                Amount = 1
            }
        };

        Assert.Equal(20, character.GetMaxHealth(Modifiers));
    }

    // -- GetStat --

    [Fact]
    public void GetStat_NoMatchingModifiers_ReturnsBaseValue() {
        Character character = new(1, "TestCharacter");
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>();

        Assert.Equal(10, character.GetStat(Modifiers,ModifierType.Strength, 10));
    }

    [Fact]
    public void GetStat_SingleMatchingModifier_AddsBonus() {
        Character character = new(1, "TestCharacter");
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>{
            new TestModifier {
                ModifierType = ModifierType.Wisdom,
                Amount = 2
            }
        };

        Assert.Equal(14, character.GetStat(Modifiers,ModifierType.Wisdom, 12));
    }

    [Fact]
    public void GetStat_MultipleModifiersSameType_SumsBonuses() {
        Character character = new(1, "TestCharacter");
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>{
            new TestModifier {
                ModifierType = ModifierType.Charisma,
                Amount = 2
            },
            new TestModifier {
                ModifierType = ModifierType.Charisma,
                Amount = 1
            }
        };

        Assert.Equal(13, character.GetStat(Modifiers,ModifierType.Charisma, 10));
    }

    [Fact]
    public void GetStat_NegativeModifier_ReducesStat() {
        Character character = new(1, "TestCharacter");
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>{
            new TestModifier {
                ModifierType = ModifierType.Intelligence,
                Amount = -2
            }
        };
    
        Assert.Equal(8, character.GetStat(Modifiers,ModifierType.Intelligence, 10));
    }

    [Fact]
    public void GetStat_OnlyAppliesModifiersOfRequestedType() {
        Character character = new(1, "TestCharacter");
        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>{
            new TestModifier {
                ModifierType = ModifierType.Strength,
                Amount = 5
            },
            new TestModifier {
                ModifierType = ModifierType.Dexterity,
                Amount = 5
            }
        };

        Assert.Equal(10, character.GetStat(Modifiers,ModifierType.Dexterity, 5));
        Assert.Equal(0, character.GetStat(Modifiers,ModifierType.Constitution, 0));
    }

    // ---------- Others ----------

    [Fact]
    public void MixedModifiers_EachGetterOnlyPullsItsOwnType() {
        Character character = new(1, "TestCharacter");
        character.SetBaseHealth(30);

        IEnumerable<ICharacterModifier> Modifiers = new List<ICharacterModifier>{
            new TestModifier {
                ModifierType = ModifierType.Health,
                Amount = 10
            },
            new TestModifier {
                ModifierType = ModifierType.Strength,
                Amount = 1
            },
            new TestModifier {
                ModifierType = ModifierType.Strength,
                Amount = 1
            }
        };

        Assert.Equal(40, character.GetMaxHealth(Modifiers));
        Assert.Equal(12, character.GetStat(Modifiers,ModifierType.Strength, 10));
    }
}


public class TestModifier : ICharacterModifier {
    public ModifierType ModifierType { get; set; }
    public int Amount { get; set; }
}