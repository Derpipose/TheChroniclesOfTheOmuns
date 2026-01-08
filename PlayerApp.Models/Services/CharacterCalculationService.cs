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
            character.Health = 2 * character.Stats.Constitution + character.CharacterClass.HitDice.Sides + GetBonus(character, "Constitution");
        } else {
            character.Health = (2 * character.CharacterClass.HitDice.Sides) + character.Stats.Constitution + GetBonus(character, "Constitution");
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

    public int GetBonus(Character character, string statName) {
        if (character.Stats == null)
            return 0;

        return statName.ToLower() switch {
            "strength" => (character.Stats.Strength - 10) / 2,
            "constitution" => (character.Stats.Constitution - 10) / 2,
            "dexterity" => (character.Stats.Dexterity - 10) / 2,
            "wisdom" => (character.Stats.Wisdom - 10) / 2,
            "charisma" => (character.Stats.Charisma - 10) / 2,
            "intelligence" => (character.Stats.Intelligence - 10) / 2,
            _ => 0
        };
    }

    public int GetRaceModifierValue(Character character, ModifierType type) {
        if (character.CharacterRace?.Modifiers == null) return 0;
        var modifier = character.CharacterRace.Modifiers
            .FirstOrDefault(m => m.Modifier.Type == type);
        return modifier?.Modifier.Value ?? 0;
    }

    private (int mainStat, int secondaryStat, string statName) GetManaStatInfo(Character character) {
        if (character.Stats.Intelligence <= character.Stats.Wisdom) {
            return (character.Stats.Wisdom, character.Stats.Intelligence, "Wisdom");
        }
        return (character.Stats.Intelligence, character.Stats.Wisdom, "Intelligence");
    }

    private int ApplyManaFormula(Character character, int mainStat, int secondaryStat, string statName, int diceSides, int diceMultiplier, bool isMagic) {
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
}
