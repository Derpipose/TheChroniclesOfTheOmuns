using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;
using PlayerAppBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace PlayerAppBlazor.ViewModels;

public class ViewCharacterViewModel : INotifyPropertyChanged {
    private readonly AppDbContext _db;

    public ViewCharacterViewModel(AppDbContext db) {
        _db = db;
    }

    private Character? _character;
    public Character? Character {
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

            Character = await _db.Characters
                .Include(c => c.Stats)
                .Include(c => c.CharacterRace)
                .Include(c => c.CharacterClass)
                .ThenInclude(cc => cc!.HitDice)
                .Include(c => c.CharacterClass)
                .ThenInclude(cc => cc!.ManaDice)
                .FirstOrDefaultAsync(c => c.Id == characterId);

            if (Character == null) {
                StatusMessage = "Character not found.";
            }
        } catch (Exception ex) {
            StatusMessage = $"Error loading character: {ex.Message}";
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
