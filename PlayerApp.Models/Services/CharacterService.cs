using System;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterService {
    private readonly CharacterCalculationService _calculationService;

    public CharacterService() {
        _calculationService = new CharacterCalculationService();
    }

    public void UpdateCharacterClassAndCalculateAttributes(Character character, CharacterClass characterClass) {
        character.AssignCharacterClass(characterClass);
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void RemoveCharacterClassAndCalculateAttributes(Character character) {
        character.RemoveCharacterClass();
        _calculationService.CalculateHitPoints(character);
        _calculationService.CalculateManaPoints(character);
    }

    public void UpdateCharacterRaceAndCalculateAttributes(Character character, CharacterRace characterRace) {
        character.AssignCharacterRace(characterRace);
        _calculationService.CalculateManaPoints(character);
        _calculationService.CalculateHitPoints(character);
    }

    public void RemoveCharacterRaceAndCalculateAttributes(Character character) {
        character.RemoveCharacterRace();
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
