using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerApp.Commands;
using PlayerApp.Models;
using PlayerApp.Services;

namespace PlayerApp.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly AppCharacterService _characterService;
    private readonly NavigationService _navigationService;
    private readonly CharactersViewModel _charactersViewModel;
    private ObservableCollection<Character> _recentCharacters = new();

    public ObservableCollection<Character> RecentCharacters
    {
        get => _recentCharacters;
        set => SetProperty(ref _recentCharacters, value);
    }

    public RelayCommand GoToCharactersCommand { get; }

    public DashboardViewModel(AppCharacterService characterService, NavigationService navigationService, CharactersViewModel charactersViewModel)
    {
        _characterService = characterService;
        _navigationService = navigationService;
        _charactersViewModel = charactersViewModel;
        GoToCharactersCommand = new RelayCommand(() => _navigationService.Navigate(_charactersViewModel));
        _ = LoadRecentCharacters();
    }

    private async Task LoadRecentCharacters()
    {
        var characters = await _characterService.GetAllCharactersAsync();
        RecentCharacters = new ObservableCollection<Character>(characters);
    }
}
