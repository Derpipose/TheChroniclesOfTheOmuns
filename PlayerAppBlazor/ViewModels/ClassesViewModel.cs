using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;

namespace PlayerAppBlazor.ViewModels;

public class ClassesViewModel : INotifyPropertyChanged {
    private readonly HttpClient _httpClient;
    private List<FantasyCharacterClass> _classes = new();
    private bool _isLoading;
    private string _statusMessage = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<FantasyCharacterClass> Classes {
        get => _classes;
        set => SetProperty(ref _classes, value);
    }

    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ClassesViewModel(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task LoadClassesAsync() {
        IsLoading = true;
        StatusMessage = "Loading classes...";

        try {
            Classes = await _httpClient.GetFromJsonAsync<List<FantasyCharacterClass>>("api/classes") ?? new List<FantasyCharacterClass>();
            StatusMessage = $"Loaded {Classes.Count} class(es).";
        } catch (Exception ex) {
            StatusMessage = $"Error loading classes: {ex.Message}";
        } finally {
            IsLoading = false;
        }
    }

    public async Task SyncClassesAsync() {
        IsLoading = true;
        StatusMessage = "Syncing classes from JSON...";

        try {
            var result = await _httpClient.PostAsync("api/classes/sync", null);
            if (!result.IsSuccessStatusCode) {
                StatusMessage = $"Sync failed: {result.ReasonPhrase}";
                return;
            }
            var syncResult = await result.Content.ReadFromJsonAsync<PlayerApp.Models.SyncResult>();
            StatusMessage = syncResult?.Message ?? "Sync completed.";
            await LoadClassesAsync();
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
