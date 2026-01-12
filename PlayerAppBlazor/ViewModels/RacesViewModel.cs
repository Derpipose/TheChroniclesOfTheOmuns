using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerAppBlazor.Database;
using PlayerAppBlazor.Services;

namespace PlayerAppBlazor.ViewModels;

public class RacesViewModel : INotifyPropertyChanged {
    private readonly AppDbContext _db;
    private readonly RaceSyncService _syncService;
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

    public RacesViewModel(AppDbContext db) {
        _db = db;
        _syncService = new RaceSyncService(db);
    }

    public async Task LoadRacesAsync() {
        IsLoading = true;
        StatusMessage = "Loading races...";

        try {
            Races = await Task.Run(() => _db.CharacterRaces.Include(r => r.Modifiers).ThenInclude(m => m.Modifier).ToList());
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
            var result = await _syncService.SyncRacesAsync();
            StatusMessage = result.Message;
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
