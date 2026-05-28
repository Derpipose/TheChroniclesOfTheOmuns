using System;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterService {
    private readonly CharacterCalculationService _calculationService;

    public CharacterService() {
        _calculationService = new CharacterCalculationService();
    }

    public void UpdateFantasyCharacterClass(FantasyCharacter character, FantasyCharacterClass FantasyCharacterClass) {
        if (FantasyCharacterClass == null) {
            character.RemoveFantasyCharacterClass();
            return;
        }
        character.AssignFantasyCharacterClass(FantasyCharacterClass);
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void RemoveFantasyCharacterClass(FantasyCharacter character) {
        character.RemoveFantasyCharacterClass();
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void UpdateFantasyCharacterRace(FantasyCharacter character, FantasyCharacterRace FantasyCharacterRace) {
        if (FantasyCharacterRace == null) {
            character.RemoveFantasyCharacterRace();
            return;
        }
        if (character.FantasyCharacterRace != null)
            character.CharacterStatBonuses.RemoveAll(b => b.BonusSource == "Race");


        character.AssignFantasyCharacterRace(FantasyCharacterRace);
        foreach (var bonus in FantasyCharacterRace.RaceStatBonuses) {
            character.CharacterStatBonuses.Add(new CharacterStatBonus {
                BonusValue = bonus.BonusValue,
                BonusSource = "Race",
                StatId = bonus.StatId,
                IsSelectable = bonus.IsSelectable
            });
        }
        _calculationService.CalculateManaPoints(character);
        _calculationService.CalculateHitPoints(character);
    }

    public void RemoveFantasyCharacterRace(FantasyCharacter character) {
        character.RemoveFantasyCharacterRace();
        character.CharacterStatBonuses.RemoveAll(b => b.BonusSource == "Race");
        _calculationService.CalculateManaPoints(character);
        _calculationService.CalculateHitPoints(character);
    }

    public void UpdateCharacterStats(FantasyCharacter character, CharacterStats newStats) {
        character.Stats = newStats;
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public int GetStat(FantasyCharacter character, StatType statName) {
        return _calculationService.GetStat(character, statName);
    }

    public int GetBonus(FantasyCharacter character, StatType statName) {
        return _calculationService.GetBonus(character, statName);
    }

    public int GetRaceModifierValue(FantasyCharacter character, ModifierType type) {
        return _calculationService.GetRaceModifierValue(character, type);
    }

    public List<CharacterStatBonus> GetSelectableRaceBonusesOnCharacter(FantasyCharacter character) {
        return character.CharacterStatBonuses
            .Where(b => b.BonusSource == "Race" && b.IsSelectable)
            .ToList();
    }

    public void AssignSelectableRaceBonus(FantasyCharacter character, int v, StatType type) {
        var alreadyAssigned = character.CharacterStatBonuses
            .FirstOrDefault(b => b.BonusSource == "Race" && b.StatId == (int)type);
        if (alreadyAssigned != null)
            throw new Exception("FantasyCharacter already has such bonus assigned. Bonuses cannot be stacked.");

        var selectableBonus = character.CharacterStatBonuses
            .FirstOrDefault(b => b.BonusSource == "Race" && b.IsSelectable && b.BonusValue == v);

        if (selectableBonus == null) {
            throw new Exception("FantasyCharacter does not have such selectable bonus available.");
        }
        if (selectableBonus != null) {
            selectableBonus.StatId = (int)type;
        }
    }

    public List<StatType> GetAvailableStatSelectableBonusesOnCharacter(FantasyCharacter character) {
        // Get all race stat bonuses that have been assigned to a stat
        var assignedStatIds = character.CharacterStatBonuses
            .Where(b => b.BonusSource == "Race" && b.StatId.HasValue)
            .Select(b => (StatType)b.StatId!.Value)
            .Distinct()
            .ToList();

        // Get all possible stat types
        var allStatTypes = Enum.GetValues<StatType>().ToList();

        // Return stats that are NOT yet assigned
        return allStatTypes.Except(assignedStatIds).ToList();
    }
}
