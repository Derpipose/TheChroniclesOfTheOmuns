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

    public int GetBonus(Character character, string statName) {
        return _calculationService.GetBonus(character, statName);
    }

    public int GetRaceModifierValue(Character character, ModifierType type) {
        return _calculationService.GetRaceModifierValue(character, type);
    }
}
