using System;
using System.Threading.Tasks;
using PlayerApp.Models;
using PlayerAppAvalonia.Services;

namespace PlayerAppAvalonia.ViewModels;

public class CharacterViewModel : BaseViewModel {
    private readonly AppCharacterService _characterService;
    private Character? _character;

    public Character? Character {
        get => _character;
        set => SetProperty(ref _character, value);
    }

    public CharacterStats? CharacterStats {
        get => _character?.Stats;
    }

    public string CharacterClassName {
        get => _character?.CharacterClass?.Name ?? "None";
    }

    public string CharacterRaceName {
        get => _character?.CharacterRace?.Name ?? "None";
    }

    public CharacterViewModel(AppCharacterService characterService, int characterId = 0) {
        _characterService = characterService;
        if (characterId > 0) {
            _ = LoadCharacter(characterId);
        }
    }

    public async Task LoadCharacter(int characterId) {
        Character = await _characterService.GetCharacterByIdAsync(characterId);
        OnPropertyChanged(nameof(CharacterStats));
        OnPropertyChanged(nameof(CharacterClassName));
        OnPropertyChanged(nameof(CharacterRaceName));
    }

    public async Task RefreshCharacterData() {
        if (Character?.Id > 0) {
            await LoadCharacter(Character.Id);
        }
    }
}
