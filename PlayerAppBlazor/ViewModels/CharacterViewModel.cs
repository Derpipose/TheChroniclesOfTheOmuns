using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;
using PlayerAppBlazor.Database;

namespace PlayerAppBlazor.ViewModels;

public class CharacterViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private List<Character> _characters = new();
    private bool _isLoading;
    private string _statusMessage = "";
    private string _newCharacterName = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public List<Character> Characters
    {
        get => _characters;
        set => SetProperty(ref _characters, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string NewCharacterName
    {
        get => _newCharacterName;
        set => SetProperty(ref _newCharacterName, value);
    }

    public CharacterViewModel(AppDbContext db)
    {
        _db = db;
    }

    public async Task LoadCharactersAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading characters...";

        try
        {
            Characters = await Task.Run(() => _db.Characters.ToList());
            StatusMessage = $"Loaded {Characters.Count} character(s).";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading characters: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task CreateCharacterAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Character name cannot be empty.";
            return;
        }

        try
        {
            var character = new Character { Name = name.Trim() };
            _db.Characters.Add(character);
            await _db.SaveChangesAsync();
            StatusMessage = $"Character '{name}' created!";
            await LoadCharactersAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating character: {ex.Message}";
        }
    }

    public async Task DeleteCharacterAsync(int id)
    {
        try
        {
            var character = _db.Characters.Find(id);
            if (character != null)
            {
                _db.Characters.Remove(character);
                await _db.SaveChangesAsync();
                StatusMessage = "Character deleted.";
                await LoadCharactersAsync();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting character: {ex.Message}";
        }
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
