using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerAppAvalonia.Commands;
using PlayerApp.Models;
using PlayerAppAvalonia.Services;

namespace PlayerAppAvalonia.ViewModels;

public class DashboardViewModel : BaseViewModel {
    private readonly AppCharacterService _characterService;
    private readonly NavigationService _navigationService;
    private readonly CharactersViewModel _charactersViewModel;
    private readonly CharacterClassService _classService;
    private readonly CharacterRaceService _raceService;
    private ObservableCollection<Character> _recentCharacters = new();
    private string _statusMessage = "";

    public ObservableCollection<Character> RecentCharacters {
        get => _recentCharacters;
        set => SetProperty(ref _recentCharacters, value);
    }

    public string StatusMessage {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public RelayCommand GoToCharactersCommand { get; }
    public RelayCommand LoadClassesCommand { get; }
    public RelayCommand LoadRacesCommand { get; }

    public DashboardViewModel(AppCharacterService characterService, NavigationService navigationService, CharactersViewModel charactersViewModel) {
        _characterService = characterService;
        _navigationService = navigationService;
        _charactersViewModel = charactersViewModel;
        _classService = new CharacterClassService();
        _raceService = new CharacterRaceService();

        GoToCharactersCommand = new RelayCommand(() => _navigationService.Navigate(_charactersViewModel));
        LoadClassesCommand = new RelayCommand(async _ => await LoadClassesIntoDb());
        LoadRacesCommand = new RelayCommand(async _ => await LoadRacesIntoDb());

        _ = LoadRecentCharacters();
    }

    private async Task LoadRecentCharacters() {
        var characters = await _characterService.GetAllCharactersAsync();
        RecentCharacters = new ObservableCollection<Character>(characters);
    }

    private async Task LoadClassesIntoDb() {
        try {
            var classes = await _classService.GetAllClassesAsync();
            int count = 0;
            foreach (var characterClass in classes) {
                // Clear the navigation properties to avoid inserting DiceTypes
                characterClass.HitDice = null;
                characterClass.ManaDice = null;
                await _characterService.CreateClassAsync(characterClass);
                count++;
            }
            StatusMessage = $"Success! {count} classes have been loaded into the database!";
        } catch (Exception ex) {
            StatusMessage = $"Error loading classes: {ex.Message}";
        }
    }

    private async Task LoadRacesIntoDb() {
        try {
            var races = await _raceService.GetAllRacesAsync();
            int count = 0;
            foreach (var race in races) {
                await _characterService.CreateRaceAsync(race);
                count++;
            }
            StatusMessage = $"Success! {count} races have been loaded into the database!";
        } catch (Exception ex) {
            StatusMessage = $"Error loading races: {ex.Message}";
        }
    }
}
