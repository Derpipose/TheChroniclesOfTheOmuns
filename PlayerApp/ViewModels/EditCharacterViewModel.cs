using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerApp.Commands;
using PlayerApp.Models;
using PlayerApp.Services;

namespace PlayerApp.ViewModels;

public class EditCharacterViewModel : BaseViewModel {
    private readonly AppCharacterService _characterService;
    private readonly NavigationService _navigationService;
    private readonly CharactersViewModel _charactersViewModel;
    private int _characterId;
    private string _name = "";
    private CharacterClass? _selectedClass;
    private CharacterRace? _selectedRace;
    private int _strength = 10;
    private int _constitution = 10;
    private int _dexterity = 10;
    private int _wisdom = 10;
    private int _charisma = 10;
    private int _intelligence = 10;

    public ObservableCollection<CharacterClass> Classes { get; } = new();
    public ObservableCollection<CharacterRace> Races { get; } = new();

    public string Name {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public CharacterClass? SelectedClass {
        get => _selectedClass;
        set => SetProperty(ref _selectedClass, value);
    }

    public CharacterRace? SelectedRace {
        get => _selectedRace;
        set => SetProperty(ref _selectedRace, value);
    }

    public int Strength {
        get => _strength;
        set => SetProperty(ref _strength, Math.Clamp(value, 10, 20));
    }

    public int Constitution {
        get => _constitution;
        set => SetProperty(ref _constitution, Math.Clamp(value, 10, 20));
    }

    public int Dexterity {
        get => _dexterity;
        set => SetProperty(ref _dexterity, Math.Clamp(value, 10, 20));
    }

    public int Wisdom {
        get => _wisdom;
        set => SetProperty(ref _wisdom, Math.Clamp(value, 10, 20));
    }

    public int Charisma {
        get => _charisma;
        set => SetProperty(ref _charisma, Math.Clamp(value, 10, 20));
    }

    public int Intelligence {
        get => _intelligence;
        set => SetProperty(ref _intelligence, Math.Clamp(value, 10, 20));
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    public EditCharacterViewModel(int characterId, AppCharacterService characterService, NavigationService navigationService, CharactersViewModel charactersViewModel) {
        _characterId = characterId;
        _characterService = characterService;
        _navigationService = navigationService;
        _charactersViewModel = charactersViewModel;
        SaveCommand = new RelayCommand(async _ => await Save(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => Cancel());

        _ = LoadCharacterAndDropdowns();
    }

    private async Task LoadCharacterAndDropdowns() {
        // Load dropdowns
        var classes = await _characterService.GetAllClassesAsync();
        var races = await _characterService.GetAllRacesAsync();

        Classes.Clear();
        foreach (var c in classes) {
            Classes.Add(c);
        }

        Races.Clear();
        foreach (var r in races) {
            Races.Add(r);
        }

        // Load the character data
        // Note: You may need to add a GetCharacterByIdAsync method to AppCharacterService
        // For now, we'll load all and find the one we need
        var allCharacters = await _characterService.GetAllCharactersAsync();
        var character = allCharacters.FirstOrDefault(c => c.Id == _characterId);

        if (character != null) {
            Name = character.Name;
            SelectedClass = Classes.FirstOrDefault(c => c.Id == character.CharacterClass?.Id);
            SelectedRace = Races.FirstOrDefault(r => r.Id == character.CharacterRace?.Id);
            Strength = character.Stats.Strength;
            Constitution = character.Stats.Constitution;
            Dexterity = character.Stats.Dexterity;
            Wisdom = character.Stats.Wisdom;
            Charisma = character.Stats.Charisma;
            Intelligence = character.Stats.Intelligence;
        }
    }

    private bool CanSave() {
        return !string.IsNullOrWhiteSpace(Name) && SelectedClass != null && SelectedRace != null;
    }

    private async Task Save() {
        // Fetch the full character with related data
        var allCharacters = await _characterService.GetAllCharactersAsync();
        var character = allCharacters.FirstOrDefault(c => c.Id == _characterId);

        if (character != null) {
            character.Name = Name;
            character.Stats.Strength = Strength;
            character.Stats.Constitution = Constitution;
            character.Stats.Dexterity = Dexterity;
            character.Stats.Wisdom = Wisdom;
            character.Stats.Charisma = Charisma;
            character.Stats.Intelligence = Intelligence;
            character.AssignCharacterClass(SelectedClass!);
            character.AssignCharacterRace(SelectedRace!);

            await _characterService.UpdateCharacterAsync(character);

            // Reload characters in the CharactersViewModel before going back
            await _charactersViewModel.LoadCharacters();

            _navigationService.GoBack();
        }
    }

    private void Cancel() {
        _navigationService.GoBack();
    }
}
