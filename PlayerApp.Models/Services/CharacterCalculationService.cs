using System;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterCalculationService {
    public void CalculateHitPoints(Character character) {
        if (character.Stats.Constitution == 0 || character.CharacterClass == null || character.CharacterClass.HitDice == null
            || character.CharacterRace == null) {
            character.Health = 0;
            return;
        }

        if (character.CharacterClass.ClassType == ClassTypeEnum.Combat) {
            character.Health = 2 * GetStat(character, StatType.Constitution) + character.CharacterClass.HitDice.Sides + GetBonus(character, StatType.Constitution);
        } else {
            character.Health = (2 * character.CharacterClass.HitDice.Sides) + GetStat(character, StatType.Constitution) + GetBonus(character, StatType.Constitution);
        }
    }

    public void CalculateManaPoints(Character character) {
        if (character.Stats.Intelligence == 0 || character.Stats.Wisdom == 0 || character.CharacterClass == null ||
            character.CharacterClass.ManaDice == null || character.CharacterRace == null) {
            character.Mana = 0;
            return;
        }

        var (mainStat, secondaryStat, statName) = GetManaStatInfo(character);
        bool isMagic = character.CharacterClass.ClassType == ClassTypeEnum.Magic;
        int diceMultiplier = isMagic ? 1 : 2;
        character.Mana = ApplyManaFormula(character, mainStat, secondaryStat, statName, character.CharacterClass.ManaDice.Sides, diceMultiplier, isMagic);
    }

    public int GetBonus(Character character, StatType statName) {
        if (character.Stats == null)
            return 0;

        return statName switch {
            StatType.Strength => (GetStat(character, StatType.Strength) - 10) / 2,
            StatType.Constitution => (GetStat(character, StatType.Constitution) - 10) / 2,
            StatType.Dexterity => (GetStat(character, StatType.Dexterity) - 10) / 2,
            StatType.Wisdom => (GetStat(character, StatType.Wisdom) - 10) / 2,
            StatType.Charisma => (GetStat(character, StatType.Charisma) - 10) / 2,
            StatType.Intelligence => (GetStat(character, StatType.Intelligence) - 10) / 2,
            _ => 0
        };
    }

    public int GetRaceModifierValue(Character character, ModifierType type) {
        if (character.CharacterRace?.Modifiers == null) return 0;
        var modifier = character.CharacterRace.Modifiers
            .FirstOrDefault(m => m.Modifier.Type == type);
        return modifier?.Modifier.Value ?? 0;
    }

    private (int mainStat, int secondaryStat, StatType statName) GetManaStatInfo(Character character) {
        if (GetStat(character, StatType.Intelligence) <= GetStat(character, StatType.Wisdom)) {
            return (GetStat(character, StatType.Wisdom), GetStat(character, StatType.Intelligence), StatType.Wisdom);
        }
        return (GetStat(character, StatType.Intelligence), GetStat(character, StatType.Wisdom), StatType.Intelligence);
    }

    private int ApplyManaFormula(Character character, int mainStat, int secondaryStat, StatType statName, int diceSides, int diceMultiplier, bool isMagic) {
        int diceValue = diceMultiplier * diceSides;
        int statBonus = GetBonus(character, statName);
        int addBonus = GetRaceModifierValue(character, ModifierType.ManaBonus);
        int multValue = GetRaceModifierValue(character, ModifierType.ManaMultiplier);

        if (multValue > 0) {
            return (mainStat * multValue) + secondaryStat + diceValue + statBonus;
        }

        if (isMagic) {
            return mainStat + secondaryStat + diceValue + statBonus + addBonus;
        } else {
            return mainStat + diceValue + statBonus + addBonus;
        }
    }

    internal int GetStat(Character character, StatType statName) {
        int baseValue = statName switch {
            StatType.Strength => character.Stats.Strength,
            StatType.Constitution => character.Stats.Constitution,
            StatType.Dexterity => character.Stats.Dexterity,
            StatType.Wisdom => character.Stats.Wisdom,
            StatType.Charisma => character.Stats.Charisma,
            StatType.Intelligence => character.Stats.Intelligence,
            _ => 0
        };

        int statTypeId = statName switch {
            StatType.Strength => (int)StatType.Strength,
            StatType.Constitution => (int)StatType.Constitution,
            StatType.Dexterity => (int)StatType.Dexterity,
            StatType.Wisdom => (int)StatType.Wisdom,
            StatType.Charisma => (int)StatType.Charisma,
            StatType.Intelligence => (int)StatType.Intelligence,
            _ => 0
        };

        int bonusValue = character.CharacterStatBonuses
            .Where(b => b.StatId == statTypeId)
            .Sum(b => b.BonusValue);
        return baseValue + bonusValue;
    }
}
