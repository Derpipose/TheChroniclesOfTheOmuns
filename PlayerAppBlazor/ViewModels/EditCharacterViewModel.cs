using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;
using PlayerAppBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace PlayerAppBlazor.ViewModels;

public class EditCharacterViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private readonly CharacterService _characterService;

    public EditCharacterViewModel(AppDbContext db)
    {
        _db = db;
        _characterService = new CharacterService();
    }

    private Character? _character;
    public Character? Character
    {
        get => _character;
        set { SetProperty(ref _character, value); }
    }

    private string _characterName = "";
    public string CharacterName
    {
        get => _characterName;
        set { SetProperty(ref _characterName, value); }
    }

    private int? _selectedRaceId;
    public int? SelectedRaceId
    {
        get => _selectedRaceId;
        set { SetProperty(ref _selectedRaceId, value); }
    }

    private int? _selectedClassId;
    public int? SelectedClassId
    {
        get => _selectedClassId;
        set { SetProperty(ref _selectedClassId, value); }
    }

    private int _strength;
    public int Strength
    {
        get => _strength;
        set { SetProperty(ref _strength, value); }
    }

    private int _constitution;
    public int Constitution
    {
        get => _constitution;
        set { SetProperty(ref _constitution, value); }
    }

    private int _dexterity;
    public int Dexterity
    {
        get => _dexterity;
        set { SetProperty(ref _dexterity, value); }
    }

    private int _wisdom;
    public int Wisdom
    {
        get => _wisdom;
        set { SetProperty(ref _wisdom, value); }
    }

    private int _charisma;
    public int Charisma
    {
        get => _charisma;
        set { SetProperty(ref _charisma, value); }
    }

    private int _intelligence;
    public int Intelligence
    {
        get => _intelligence;
        set { SetProperty(ref _intelligence, value); }
    }

    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set { SetProperty(ref _statusMessage, value); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { SetProperty(ref _isLoading, value); }
    }

    public List<CharacterRace> Races { get; private set; } = new();
    public List<CharacterClass> Classes { get; private set; } = new();

    public async Task LoadCharacterAsync(int characterId)
    {
        try
        {
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

            if (Character == null)
            {
                StatusMessage = "Character not found.";
                return;
            }

            // Load dropdown options
            Races = _db.CharacterRaces.ToList();
            Classes = _db.CharacterClasses.Where(c => !c.IsVeteranLocked).ToList();

            // Populate form fields
            CharacterName = Character.Name;
            SelectedRaceId = Character.CharacterRace?.Id;
            SelectedClassId = Character.CharacterClass?.Id;
            Strength = Character.Stats?.Strength ?? 10;
            Constitution = Character.Stats?.Constitution ?? 10;
            Dexterity = Character.Stats?.Dexterity ?? 10;
            Wisdom = Character.Stats?.Wisdom ?? 10;
            Charisma = Character.Stats?.Charisma ?? 10;
            Intelligence = Character.Stats?.Intelligence ?? 10;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading character: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> SaveCharacterAsync()
    {
        if (string.IsNullOrWhiteSpace(CharacterName) || Character == null)
        {
            StatusMessage = "Character name is required.";
            return false;
        }

        try
        {
            IsLoading = true;

            // Update basic info
            Character.Name = CharacterName;

            // Update stats
            if (Character.Stats != null)
            {
                Character.Stats.Strength = Strength;
                Character.Stats.Constitution = Constitution;
                Character.Stats.Dexterity = Dexterity;
                Character.Stats.Wisdom = Wisdom;
                Character.Stats.Charisma = Charisma;
                Character.Stats.Intelligence = Intelligence;
            }

            // Update race if changed
            if (SelectedRaceId.HasValue && (Character.CharacterRace == null || Character.CharacterRace.Id != SelectedRaceId.Value))
            {
                var race = _db.CharacterRaces
                    .Include(r => r.Modifiers)
                    .ThenInclude(m => m.Modifier)
                    .FirstOrDefault(r => r.Id == SelectedRaceId.Value);
                if (race != null)
                    _characterService.UpdateCharacterRace(Character, race);
            }
            else if (!SelectedRaceId.HasValue && Character.CharacterRace != null)
            {
                _characterService.RemoveCharacterRace(Character);
            }

            // Update class if changed
            if (SelectedClassId.HasValue && (Character.CharacterClass == null || Character.CharacterClass.Id != SelectedClassId.Value))
            {
                var characterClass = _db.CharacterClasses
                    .Include(c => c.HitDice)
                    .Include(c => c.ManaDice)
                    .FirstOrDefault(c => c.Id == SelectedClassId.Value);
                if (characterClass != null)
                    _characterService.UpdateCharacterClass(Character, characterClass);
            }
            else if (!SelectedClassId.HasValue && Character.CharacterClass != null)
            {
                _characterService.RemoveCharacterClass(Character);
            }

            _db.Characters.Update(Character);
            await _db.SaveChangesAsync();

            StatusMessage = $"Character '{CharacterName}' updated successfully!";
            return true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving character: {ex.Message}";
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
