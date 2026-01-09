using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;
using PlayerAppBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace PlayerAppBlazor.ViewModels;

public class NewCharacterViewModel : INotifyPropertyChanged
{
    private readonly AppDbContext _db;
    private readonly CharacterService _characterService;

    public NewCharacterViewModel(AppDbContext db)
    {
        _db = db;
        _characterService = new CharacterService();
        LoadData();
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

    private int _strength = 10;
    public int Strength
    {
        get => _strength;
        set { SetProperty(ref _strength, value); }
    }

    private int _constitution = 10;
    public int Constitution
    {
        get => _constitution;
        set { SetProperty(ref _constitution, value); }
    }

    private int _dexterity = 10;
    public int Dexterity
    {
        get => _dexterity;
        set { SetProperty(ref _dexterity, value); }
    }

    private int _wisdom = 10;
    public int Wisdom
    {
        get => _wisdom;
        set { SetProperty(ref _wisdom, value); }
    }

    private int _charisma = 10;
    public int Charisma
    {
        get => _charisma;
        set { SetProperty(ref _charisma, value); }
    }

    private int _intelligence = 10;
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

    private int _createdCharacterId;
    public int CreatedCharacterId
    {
        get => _createdCharacterId;
        set { SetProperty(ref _createdCharacterId, value); }
    }

    public List<CharacterRace> Races { get; private set; } = new();
    public List<CharacterClass> Classes { get; private set; } = new();

    private void LoadData()
    {
        Races = _db.CharacterRaces.ToList();
        Classes = _db.CharacterClasses.Where(c => !c.IsVeteranLocked).ToList();
    }

    public void ResetForm()
    {
        CharacterName = "";
        SelectedRaceId = null;
        SelectedClassId = null;
        Strength = 10;
        Constitution = 10;
        Dexterity = 10;
        Wisdom = 10;
        Charisma = 10;
        Intelligence = 10;
        StatusMessage = "";
    }

    public async Task<bool> CreateCharacterAsync()
    {
        if (string.IsNullOrWhiteSpace(CharacterName))
        {
            StatusMessage = "Character name is required.";
            return false;
        }

        try
        {
            IsLoading = true;

            var character = new Character
            {
                Name = CharacterName,
                Level = 1,
                Health = 0,
                Mana = 0,
                Stats = new CharacterStats
                {
                    Strength = Strength,
                    Constitution = Constitution,
                    Dexterity = Dexterity,
                    Wisdom = Wisdom,
                    Charisma = Charisma,
                    Intelligence = Intelligence
                }
            };

            if (SelectedRaceId.HasValue)
            {
                var race = _db.CharacterRaces
                    .Include(r => r.Modifiers)
                    .ThenInclude(m => m.Modifier)
                    .FirstOrDefault(r => r.Id == SelectedRaceId.Value);
                if (race != null)
                    _characterService.UpdateCharacterRace(character, race);
            }

            if (SelectedClassId.HasValue)
            {
                var characterClass = _db.CharacterClasses
                    .Include(c => c.HitDice)
                    .Include(c => c.ManaDice)
                    .FirstOrDefault(c => c.Id == SelectedClassId.Value);
                if (characterClass != null)
                    _characterService.UpdateCharacterClass(character, characterClass);
            }

            _db.Characters.Add(character);
            await _db.SaveChangesAsync();

            CreatedCharacterId = character.Id;
            StatusMessage = $"Character '{CharacterName}' created successfully!";
            return true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error creating character: {ex.Message}";
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
