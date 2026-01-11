using System;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterService {
    private readonly CharacterCalculationService _calculationService;

    public CharacterService() {
        _calculationService = new CharacterCalculationService();
    }

    public void UpdateCharacterClass(Character character, CharacterClass characterClass) {
        if (characterClass == null) {
            character.RemoveCharacterClass();
            return;
        }
        character.AssignCharacterClass(characterClass);
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void RemoveCharacterClass(Character character) {
        character.RemoveCharacterClass();
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void UpdateCharacterRace(Character character, CharacterRace characterRace) {
        if (characterRace == null) {
            character.RemoveCharacterRace();
            return;
        }
        if (character.CharacterRace != null)
            character.CharacterStatBonuses.RemoveAll(b => b.BonusSource == "Race");


        character.AssignCharacterRace(characterRace);
        foreach (var bonus in characterRace.RaceStatBonuses) {
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

    public void RemoveCharacterRace(Character character) {
        character.RemoveCharacterRace();
        character.CharacterStatBonuses.RemoveAll(b => b.BonusSource == "Race");
        _calculationService.CalculateManaPoints(character);
        _calculationService.CalculateHitPoints(character);
    }

    public void UpdateCharacterStats(Character character, CharacterStats newStats) {
        character.Stats = newStats;
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public int GetStat(Character character, StatType statName) {
        return _calculationService.GetStat(character, statName);
    }

    public int GetBonus(Character character, StatType statName) {
        return _calculationService.GetBonus(character, statName);
    }

    public int GetRaceModifierValue(Character character, ModifierType type) {
        return _calculationService.GetRaceModifierValue(character, type);
    }

    public List<CharacterStatBonus> GetSelectableRaceBonusesOnCharacter(Character character) {
        return character.CharacterStatBonuses
            .Where(b => b.BonusSource == "Race" && b.IsSelectable)
            .ToList();
    }

    public void AssignSelectableRaceBonus(Character character, int v, StatType type) {
        var alreadyAssigned = character.CharacterStatBonuses
            .FirstOrDefault(b => b.BonusSource == "Race" && b.StatId == (int)type);
        if (alreadyAssigned != null)
            throw new Exception("Character already has such bonus assigned. Bonuses cannot be stacked.");

        var selectableBonus = character.CharacterStatBonuses
            .FirstOrDefault(b => b.BonusSource == "Race" && b.IsSelectable && b.BonusValue == v);
        
        if(selectableBonus == null) {
            throw new Exception("Character does not have such selectable bonus available.");
        }
        if (selectableBonus != null) {
            selectableBonus.StatId = (int)type;
        }
    }
}
