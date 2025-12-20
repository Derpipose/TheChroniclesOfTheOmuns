using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PlayerApp.Commands;
using PlayerApp.Models;
using PlayerApp.Services;

namespace PlayerApp.ViewModels;

public class CharactersViewModel : BaseViewModel
{
    private readonly AppCharacterService _characterService;
    private ObservableCollection<Character> _characters = new();
    private Character? _selectedCharacter;

    public ObservableCollection<Character> Characters
    {
        get => _characters;
        set => SetProperty(ref _characters, value);
    }

    public Character? SelectedCharacter
    {
        get => _selectedCharacter;
        set => SetProperty(ref _selectedCharacter, value);
    }

    public RelayCommand LoadCharactersCommand { get; }
    public RelayCommand DeleteCharacterCommand { get; }

    public CharactersViewModel(AppCharacterService characterService)
    {
        _characterService = characterService;
        LoadCharactersCommand = new RelayCommand(async _ => await LoadCharacters());
        DeleteCharacterCommand = new RelayCommand(async _ => await DeleteSelectedCharacter());

        _ = LoadCharacters();
    }

    private async Task LoadCharacters()
    {
        var characters = await _characterService.GetAllCharactersAsync();
        Characters = new ObservableCollection<Character>(characters);
    }

    private async Task DeleteSelectedCharacter()
    {
        if (SelectedCharacter?.Id > 0)
        {
            await _characterService.DeleteCharacterAsync(SelectedCharacter.Id);
            await LoadCharacters();
        }
    }
}
