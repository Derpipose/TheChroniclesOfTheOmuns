namespace ChroniclesTest;

using PlayerApp.Models;
using PlayerApp.Models.Enums;
using Xunit;

public class ModifierTypeTests {


    [Fact]
    public void GetMaxHealth_NoModifiers_ReturnsBaseHealth() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(20);

        Assert.Equal(20, character.GetMaxHealth());
    }

    [Fact]
    public void GetMaxHealth_SingleHealthModifier_AddsBonus() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(20);
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 5, ModifierSource.Race));

        Assert.Equal(25, character.GetMaxHealth());
    }

    [Fact]
    public void GetMaxHealth_MultipleHealthModifiers_SumsAllBonuses() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(20);
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 5, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 3, ModifierSource.Class));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 2, ModifierSource.Background));

        Assert.Equal(30, character.GetMaxHealth());
    }

    [Fact]
    public void GetMaxHealth_NegativeHealthModifier_ReducesTotal() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(20);
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, -5, ModifierSource.Subrace));

        Assert.Equal(15, character.GetMaxHealth());
    }

    [Fact]
    public void GetMaxHealth_IgnoresNonHealthModifiers() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(20);
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Strength, 2, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.ArmorClass, 1, ModifierSource.Class));

        Assert.Equal(20, character.GetMaxHealth());
    }

    // -- GetArmorClass --

    [Fact]
    public void GetArmorClass_NoModifiers_ReturnsBaseValue() {
        Character character = new(1, "TestCharacter");

        Assert.Equal(15, character.GetArmorClass(15));
    }

    [Fact]
    public void GetArmorClass_SingleModifier_AddsBonus() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.ArmorClass, 2, ModifierSource.Subrace));

        Assert.Equal(17, character.GetArmorClass(15));
    }

    [Fact]
    public void GetArmorClass_NegativeModifier_ReducesValue() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.ArmorClass, -1, ModifierSource.Background));

        Assert.Equal(14, character.GetArmorClass(15));
    }

    [Fact]
    public void GetArmorClass_IgnoresHealthAndStatModifiers() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 10, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Dexterity, 3, ModifierSource.Class));

        Assert.Equal(15, character.GetArmorClass(15));
    }

    // -- GetStat --

    [Fact]
    public void GetStat_NoMatchingModifiers_ReturnsBaseValue() {
        Character character = new(1, "TestCharacter");

        Assert.Equal(10, character.GetStat(ModifierType.Strength, 10));
    }

    [Fact]
    public void GetStat_SingleMatchingModifier_AddsBonus() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Wisdom, 2, ModifierSource.Race));

        Assert.Equal(14, character.GetStat(ModifierType.Wisdom, 12));
    }

    [Fact]
    public void GetStat_MultipleModifiersSameType_SumsBonuses() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Charisma, 2, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Charisma, 1, ModifierSource.Subrace));

        Assert.Equal(13, character.GetStat(ModifierType.Charisma, 10));
    }

    [Fact]
    public void GetStat_NegativeModifier_ReducesStat() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Intelligence, -2, ModifierSource.Class));

        Assert.Equal(8, character.GetStat(ModifierType.Intelligence, 10));
    }

    [Fact]
    public void GetStat_OnlyAppliesModifiersOfRequestedType() {
        Character character = new(1, "TestCharacter");
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Strength, 5, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Dexterity, 5, ModifierSource.Race));

        Assert.Equal(10, character.GetStat(ModifierType.Dexterity, 5));
        Assert.Equal(0, character.GetStat(ModifierType.Constitution, 0));
    }

    // ---------- Others ----------

    [Fact]
    public void MixedModifiers_EachGetterOnlyPullsItsOwnType() {
        Character character = new(1, "TestCharacter");
        character.SetMaxHealth(30);

        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Health, 10, ModifierSource.Race));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.ArmorClass, 2, ModifierSource.Subrace));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Strength, 1, ModifierSource.Class));
        character.Modifiers.Add(new CharacterModifier(character.Id, ModifierType.Strength, 1, ModifierSource.Background));

        Assert.Equal(40, character.GetMaxHealth());
        Assert.Equal(17, character.GetArmorClass(15));
        Assert.Equal(12, character.GetStat(ModifierType.Strength, 10));
    }
}
