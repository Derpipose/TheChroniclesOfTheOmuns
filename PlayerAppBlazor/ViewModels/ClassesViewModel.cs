using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerAppBlazor.Database;
using PlayerAppBlazor.Services;

namespace PlayerAppBlazor.ViewModels;

public class ClassesViewModel : INotifyPropertyChanged {
    private readonly AppDbContext _db;
    private readonly ClassSyncService _syncService;
    private List<CharacterClass> _classes = new();
    private bool _isLoading;
    private string _statusMessage = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<CharacterClass> Classes {
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

    public ClassesViewModel(AppDbContext db) {
        _db = db;
        _syncService = new ClassSyncService(db);
    }

    public async Task LoadClassesAsync() {
        IsLoading = true;
        StatusMessage = "Loading classes...";

        try {
            Classes = await Task.Run(() => _db.CharacterClasses.Include(c => c.HitDice).Include(c => c.ManaDice).ToList());
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
            var result = await _syncService.SyncClassesAsync();
            StatusMessage = result.Message;
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
