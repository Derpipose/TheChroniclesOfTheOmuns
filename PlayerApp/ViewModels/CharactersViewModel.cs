using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using PlayerApp.Commands;
using PlayerApp.Models;
using PlayerApp.Services;
using PlayerApp.Views;

namespace PlayerApp.ViewModels;

public class CharactersViewModel : BaseViewModel {
    private readonly AppCharacterService _characterService;
    private readonly NavigationService _navigationService;
    private ObservableCollection<Character> _characters = new();
    private Character? _selectedCharacter;

    public ObservableCollection<Character> Characters {
        get => _characters;
        set => SetProperty(ref _characters, value);
    }

    public Character? SelectedCharacter {
        get => _selectedCharacter;
        set => SetProperty(ref _selectedCharacter, value);
    }

    public RelayCommand LoadCharactersCommand { get; }
    public RelayCommand NewCharacterCommand { get; }
    public RelayCommand EditCharacterCommand { get; }
    public RelayCommand DeleteCharacterCommand { get; }

    public CharactersViewModel(AppCharacterService characterService, NavigationService navigationService) {
        _characterService = characterService;
        _navigationService = navigationService;
        LoadCharactersCommand = new RelayCommand(async _ => await LoadCharacters());
        NewCharacterCommand = new RelayCommand(_ => CreateNewCharacter());
        EditCharacterCommand = new RelayCommand(_ => EditSelectedCharacter(), _ => SelectedCharacter != null);
        DeleteCharacterCommand = new RelayCommand(async _ => await DeleteSelectedCharacter(), _ => SelectedCharacter != null);

        _ = LoadCharacters();
    }

    public async Task LoadCharacters() {
        var characters = await _characterService.GetAllCharactersAsync();
        Characters = new ObservableCollection<Character>(characters);
    }

    private void CreateNewCharacter() {
        var newCharViewModel = new NewCharacterViewModel(_characterService, _navigationService, this);
        _navigationService.Navigate(newCharViewModel);
    }

    private void EditSelectedCharacter() {
        if (SelectedCharacter == null)
            return;

        // TODO: Open edit character dialog/window
        // This is a placeholder - implement the actual edit UI
    }

    private async Task DeleteSelectedCharacter() {
        if (SelectedCharacter?.Id > 0) {
            await _characterService.DeleteCharacterAsync(SelectedCharacter.Id);
            await LoadCharacters();
        }
    }
}
