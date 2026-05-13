using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using PlayerApp.Models;

namespace PlayerAppBlazor.ViewModels;

public class RacesViewModel : INotifyPropertyChanged {
    private readonly HttpClient _httpClient;
    private List<CharacterRace> _races = new();
    private bool _isLoading;
    private string _statusMessage = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<CharacterRace> Races {
        get => _races;
        set => SetProperty(ref _races, value);
    }

    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public RacesViewModel(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task LoadRacesAsync() {
        IsLoading = true;
        StatusMessage = "Loading races...";

        try {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var racesJson = await _httpClient.GetStringAsync("api/races");
            Races = JsonSerializer.Deserialize<List<CharacterRace>>(racesJson, options) ?? new List<CharacterRace>();
            StatusMessage = $"Loaded {Races.Count} race(s).";
        } catch (Exception ex) {
            StatusMessage = $"Error loading races: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    public async Task SyncRacesAsync() {
        IsLoading = true;
        StatusMessage = "Syncing races from JSON...";

        try {
            var result = await _httpClient.PostAsync("api/races/sync", null);
            if (!result.IsSuccessStatusCode) {
                StatusMessage = $"Sync failed: {result.ReasonPhrase}";
                return;
            }
            var syncResult = await result.Content.ReadFromJsonAsync<PlayerApp.Models.SyncResult>();
            StatusMessage = syncResult?.Message ?? "Sync completed.";
            await LoadRacesAsync();
        } catch (Exception ex) {
            StatusMessage = $"Sync error: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
        if (!EqualityComparer<T>.Default.Equals(field, value)) {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
