using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PlayerApp.Models;
using PlayerApp.Models.DTOs;

namespace PlayerAppBlazor.ViewModels;

public class EditCharacterViewModel : INotifyPropertyChanged {
    private readonly HttpClient _httpClient;

    public EditCharacterViewModel(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    private FantasyCharacter? _character;
    public FantasyCharacter? character {
        get => _character;
        set { SetProperty(ref _character, value); }
    }

    private string _characterName = "";
    public string CharacterName {
        get => _characterName;
        set { SetProperty(ref _characterName, value); }
    }

    private int? _selectedRaceId;
    public int? SelectedRaceId {
        get => _selectedRaceId;
        set { SetProperty(ref _selectedRaceId, value); }
    }

    private int? _selectedClassId;
    public int? SelectedClassId {
        get => _selectedClassId;
        set { SetProperty(ref _selectedClassId, value); }
    }

    private int _strength;
    public int Strength {
        get => _strength;
        set { SetProperty(ref _strength, value); }
    }

    private int _constitution;
    public int Constitution {
        get => _constitution;
        set { SetProperty(ref _constitution, value); }
    }

    private int _dexterity;
    public int Dexterity {
        get => _dexterity;
        set { SetProperty(ref _dexterity, value); }
    }

    private int _wisdom;
    public int Wisdom {
        get => _wisdom;
        set { SetProperty(ref _wisdom, value); }
    }

    private int _charisma;
    public int Charisma {
        get => _charisma;
        set { SetProperty(ref _charisma, value); }
    }

    private int _intelligence;
    public int Intelligence {
        get => _intelligence;
        set { SetProperty(ref _intelligence, value); }
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

    public List<FantasyCharacterRace> Races { get; private set; } = new();
    public List<FantasyCharacterClass> Classes { get; private set; } = new();

    public async Task LoadCharacterAsync(int characterId) {
        try {
            IsLoading = true;
            StatusMessage = "";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            // Deserialize into DTO which has public properties
            var characterJson = await _httpClient.GetStringAsync($"api/fantasy/characters/{characterId}");
            var characterDto = JsonSerializer.Deserialize<FantasyCharacterDto>(characterJson, options);

            if (characterDto == null) {
                StatusMessage = "FantasyCharacter not found.";
                return;
            }

            var racesJson = await _httpClient.GetStringAsync("api/fantasy/races");
            Races = JsonSerializer.Deserialize<List<FantasyCharacterRace>>(racesJson, options) ?? new List<FantasyCharacterRace>();
            
            var classesJson = await _httpClient.GetStringAsync("api/fantasy/classes/non-veteran");
            Classes = JsonSerializer.Deserialize<List<FantasyCharacterClass>>(classesJson, options) ?? new List<FantasyCharacterClass>();

            // Create a minimal character object for display/editing
            character = new FantasyCharacter {
                Id = characterDto.Id,
                Name = characterDto.Name,
                Level = characterDto.Level,
                Health = characterDto.Health,
                Mana = characterDto.Mana,
                Stats = characterDto.Stats ?? new CharacterStats()
            };
            if (characterDto.FantasyCharacterRace != null) {
                character.AssignFantasyCharacterRace(characterDto.FantasyCharacterRace);
            }
            if (characterDto.FantasyCharacterClass != null) {
                character.AssignFantasyCharacterClass(characterDto.FantasyCharacterClass);
            }

            CharacterName = character.Name;
            SelectedRaceId = characterDto.FantasyCharacterRaceId;
            SelectedClassId = characterDto.FantasyCharacterClassId;
            Strength = character.Stats?.Strength ?? 10;
            Constitution = character.Stats?.Constitution ?? 10;
            Dexterity = character.Stats?.Dexterity ?? 10;
            Wisdom = character.Stats?.Wisdom ?? 10;
            Charisma = character.Stats?.Charisma ?? 10;
            Intelligence = character.Stats?.Intelligence ?? 10;
        } catch (Exception ex) {
            StatusMessage = $"Error loading character: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    public async Task<bool> SaveCharacterAsync() {
        if (string.IsNullOrWhiteSpace(CharacterName) || character == null) {
            StatusMessage = "FantasyCharacter name is required.";
            return false;
        }

        try {
            IsLoading = true;

            // Build DTO for the update
            var updateDto = new CharacterUpdateDto {
                Id = character.Id,
                Name = CharacterName,
                Level = character.Level,
                FantasyCharacterRaceId = SelectedRaceId,
                FantasyCharacterClassId = SelectedClassId,
                Stats = new CharacterStats {
                    Strength = Strength,
                    Constitution = Constitution,
                    Dexterity = Dexterity,
                    Wisdom = Wisdom,
                    Charisma = Charisma,
                    Intelligence = Intelligence
                }
            };

            var response = await _httpClient.PutAsJsonAsync($"api/fantasy/characters/{character.Id}", updateDto);
            
            if (!response.IsSuccessStatusCode) {
                StatusMessage = $"Error saving character: {response.StatusCode}";
                return false;
            }

            StatusMessage = $"FantasyCharacter '{CharacterName}' updated successfully!";
            return true;
        } catch (Exception ex) {
            StatusMessage = $"Error saving character: {ex.Message}";
            return false;
        } finally {
            IsLoading = false;
        }
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
