using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PlayerApp.Models;
using PlayerApp.Models.DTOs;

namespace PlayerAppBlazor.ViewModels;

public class CharactersViewModel : INotifyPropertyChanged {
    private List<FantasyCharacterDto> _characters = new();
    private bool _isLoading;
    private string _statusMessage = "";
    private string _newCharacterName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<FantasyCharacterDto> Characters {
        get => _characters;
        set => SetProperty(ref _characters, value);
    }

    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string NewCharacterName {
        get => _newCharacterName;
        set => SetProperty(ref _newCharacterName, value);
    }

    private readonly HttpClient _httpClient;

    public CharactersViewModel(HttpClient httpClient) {
        _httpClient = httpClient;
    }
    
    public async Task LoadCharactersAsync() {
        IsLoading = true;
        StatusMessage = "Loading characters...";

        try {
            var response = await _httpClient.GetAsync("api/characters");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Characters = JsonSerializer.Deserialize<List<FantasyCharacterDto>>(json, options) ?? new();
            StatusMessage = $"Loaded {Characters.Count} character(s).";
        } catch (Exception ex) {
            StatusMessage = $"Error loading characters: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    public async Task CreateCharacterAsync(string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            StatusMessage = "FantasyCharacter name cannot be empty.";
            return;
        }

        try {
            var character = new FantasyCharacter { Name = name.Trim() };
            await _httpClient.PostAsJsonAsync("api/characters", character);
            StatusMessage = $"FantasyCharacter '{name}' created!";
            await LoadCharactersAsync();
        } catch (Exception ex) {
            StatusMessage = $"Error creating character: {ex.Message}";
        }
    }

    public async Task DeleteCharacterAsync(int id) {
        try {
            await _httpClient.DeleteAsync($"api/characters/{id}");
            StatusMessage = "FantasyCharacter deleted.";
            await LoadCharactersAsync();
        } catch (Exception ex) {
            StatusMessage = $"Error deleting character: {ex.Message}";
        }
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
        if (!EqualityComparer<T>.Default.Equals(field, value)) {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
