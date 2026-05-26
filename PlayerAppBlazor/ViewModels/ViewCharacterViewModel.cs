using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PlayerApp.Models;
using PlayerApp.Models.DTOs;
using PlayerApp.Models.Enums;

namespace PlayerAppBlazor.ViewModels;

public class ViewCharacterViewModel : INotifyPropertyChanged {
    private readonly HttpClient _httpClient;
    private readonly CharacterService _characterService = new CharacterService();

    public ViewCharacterViewModel(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    private FantasyCharacter? _character;
    public FantasyCharacter? FantasyCharacter {
        get => _character;
        set { SetProperty(ref _character, value); }
    }

    private string _statusMessage = "";
    public string StatusMessage {
        get => _statusMessage;
        set { SetProperty(ref _statusMessage, value); }
    }

    private bool _isLoading;
    public bool IsLoading {
        get => _isLoading;
        set { SetProperty(ref _isLoading, value); }
    }

    public async Task LoadCharacterAsync(int characterId) {
        try {
            IsLoading = true;
            StatusMessage = "";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var characterJson = await _httpClient.GetStringAsync($"api/fantasy/characters/{characterId}");
            var FantasyCharacterDto = JsonSerializer.Deserialize<FantasyFantasyCharacterDto>(characterJson, options);

            if (FantasyCharacterDto == null) {
                StatusMessage = "FantasyCharacter not found.";
                return;
            }

            // Create a minimal character object for display
            FantasyCharacter = new FantasyCharacter {
                Id = FantasyCharacterDto.Id,
                Name = FantasyCharacterDto.Name,
                Level = FantasyCharacterDto.Level,
                Health = FantasyCharacterDto.Health,
                Mana = FantasyCharacterDto.Mana,
                Stats = FantasyCharacterDto.Stats ?? new CharacterStats()
            };
            if (FantasyCharacterDto.FantasyCharacterRace != null) {
                FantasyCharacter.AssignFantasyCharacterRace(FantasyCharacterDto.FantasyCharacterRace);
            }
            if (FantasyCharacterDto.FantasyCharacterClass != null) {
                FantasyCharacter.AssignFantasyCharacterClass(FantasyCharacterDto.FantasyCharacterClass);
            }
        } catch (Exception ex) {
            StatusMessage = $"Error loading character: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    public int GetStat(FantasyCharacter character, StatType statType) {
        return _characterService.GetStat(character, statType);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null) {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
