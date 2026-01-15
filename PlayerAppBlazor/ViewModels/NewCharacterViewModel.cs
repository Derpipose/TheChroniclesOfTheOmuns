using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlayerApp.Models;
using PlayerApp.Models.Enums;
using PlayerAppBlazor.Database;
using Microsoft.EntityFrameworkCore;

namespace PlayerAppBlazor.ViewModels;

public class NewCharacterViewModel : INotifyPropertyChanged {
    private readonly AppDbContext _db;
    private readonly CharacterService _characterService;

    public NewCharacterViewModel(AppDbContext db) {
        _db = db;
        _characterService = new CharacterService();
        LoadData();
    }

    private string _characterName = "";
    public string CharacterName {
        get => _characterName;
        set { SetProperty(ref _characterName, value); }
    }

    private int? _selectedRaceId;
    public int? SelectedRaceId {
        get => _selectedRaceId;
        set {
            if (SetProperty(ref _selectedRaceId, value)) {
                UpdateDraftCharacter();
            }
        }
    }

    private int? _selectedClassId;
    public int? SelectedClassId {
        get => _selectedClassId;
        set {
            if (SetProperty(ref _selectedClassId, value)) {
                UpdateDraftCharacter();
            }
        }
    }

    private Character? _draftCharacter;
    public Character? DraftCharacter {
        get => _draftCharacter;
        private set { SetProperty(ref _draftCharacter, value); }
    }

    private List<CharacterStatBonus> _selectableBonuses = new();
    public List<CharacterStatBonus> SelectableBonuses {
        get => _selectableBonuses;
        private set { SetProperty(ref _selectableBonuses, value); }
    }

    private int _strength = 10;
    public int Strength {
        get => _strength;
        set { SetProperty(ref _strength, value); }
    }

    private int _constitution = 10;
    public int Constitution {
        get => _constitution;
        set { SetProperty(ref _constitution, value); }
    }

    private int _dexterity = 10;
    public int Dexterity {
        get => _dexterity;
        set { SetProperty(ref _dexterity, value); }
    }

    private int _wisdom = 10;
    public int Wisdom {
        get => _wisdom;
        set { SetProperty(ref _wisdom, value); }
    }

    private int _charisma = 10;
    public int Charisma {
        get => _charisma;
        set { SetProperty(ref _charisma, value); }
    }

    private int _intelligence = 10;
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

    private int _createdCharacterId;
    public int CreatedCharacterId {
        get => _createdCharacterId;
        set { SetProperty(ref _createdCharacterId, value); }
    }

    private StatType? _selectedPick1;
    public StatType? SelectedPick1 {
        get => _selectedPick1;
        set {
            if (SetProperty(ref _selectedPick1, value)) {
                UpdateAvailableStats();
            }
        }
    }

    private StatType? _selectedPick2;
    public StatType? SelectedPick2 {
        get => _selectedPick2;
        set {
            if (SetProperty(ref _selectedPick2, value)) {
                UpdateAvailableStats();
            }
        }
    }

    private List<StatType> _availableStatsForPick1 = new();
    public List<StatType> AvailableStatsForPick1 {
        get => _availableStatsForPick1;
        private set { SetProperty(ref _availableStatsForPick1, value); }
    }

    private List<StatType> _availableStatsForPick2 = new();
    public List<StatType> AvailableStatsForPick2 {
        get => _availableStatsForPick2;
        private set { SetProperty(ref _availableStatsForPick2, value); }
    }

    public List<CharacterRace> Races { get; private set; } = new();
    public List<CharacterClass> Classes { get; private set; } = new();

    private void LoadData() {
        Races = _db.CharacterRaces.Include(r => r.RaceStatBonuses).ToList();
        Classes = _db.CharacterClasses.Where(c => !c.IsVeteranLocked).ToList();
    }

    public void ResetForm() {
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
        DraftCharacter = null;
        SelectableBonuses = new();
        SelectedPick1 = null;
        SelectedPick2 = null;
        UpdateAvailableStats();
    }

    public async Task<bool> CreateCharacterAsync() {
        if (string.IsNullOrWhiteSpace(CharacterName)) {
            StatusMessage = "Character name is required.";
            return false;
        }

        try {
            IsLoading = true;

            var character = new Character {
                Name = CharacterName,
                Level = 1,
                Health = 0,
                Mana = 0,
                Stats = new CharacterStats {
                    Strength = Strength,
                    Constitution = Constitution,
                    Dexterity = Dexterity,
                    Wisdom = Wisdom,
                    Charisma = Charisma,
                    Intelligence = Intelligence
                }
            };

            if (SelectedRaceId.HasValue) {
                var race = _db.CharacterRaces
                    .Include(r => r.Modifiers)
                    .ThenInclude(m => m.Modifier)
                    .FirstOrDefault(r => r.Id == SelectedRaceId.Value);
                if (race != null)
                    _characterService.UpdateCharacterRace(character, race);
            }

            if (SelectedClassId.HasValue) {
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
        } catch (Exception ex) {
            StatusMessage = $"Error creating character: {ex.Message}";
            return false;
        } finally {
            IsLoading = false;
        }
    }

    public int GetRaceStatBonus(int statId) {
        if (DraftCharacter?.CharacterStatBonuses == null) return 0;
        return DraftCharacter.CharacterStatBonuses
            .Where(b => b.StatId == statId && b.BonusSource == "Race")
            .Sum(b => b.BonusValue);
    }

    public int GetCalculatedStatValue(int baseValue, int statId) {
        return baseValue + GetRaceStatBonus(statId);
    }

    public int GetMaxStatForInput(int statId) {
        const int maxStatValue = 18;
        var bonus = GetRaceStatBonus(statId);
        var maxInput = maxStatValue - bonus;
        return Math.Max(1, maxInput);
    }

    public (bool isValid, int maxAllowed) ValidateStatInput(int value, int statId) {
        var maxAllowed = GetMaxStatForInput(statId);
        return (value <= maxAllowed, maxAllowed);
    }

    private void UpdateDraftCharacter() {
        // Create a fresh draft character
        var character = new Character {
            Name = CharacterName,
            Level = 1,
            Health = 0,
            Mana = 0,
            Stats = new CharacterStats {
                Strength = Strength,
                Constitution = Constitution,
                Dexterity = Dexterity,
                Wisdom = Wisdom,
                Charisma = Charisma,
                Intelligence = Intelligence
            }
        };

        // Apply race if selected
        if (SelectedRaceId.HasValue) {
            var race = _db.CharacterRaces
                .Include(r => r.Modifiers)
                .ThenInclude(m => m.Modifier)
                .FirstOrDefault(r => r.Id == SelectedRaceId.Value);
            if (race != null) {
                _characterService.UpdateCharacterRace(character, race);
            }
        }

        // Apply class if selected
        if (SelectedClassId.HasValue) {
            var characterClass = _db.CharacterClasses
                .Include(c => c.HitDice)
                .Include(c => c.ManaDice)
                .FirstOrDefault(c => c.Id == SelectedClassId.Value);
            if (characterClass != null) {
                _characterService.UpdateCharacterClass(character, characterClass);
            }
        }

        DraftCharacter = character;

        // Update selectable bonuses
        SelectableBonuses = _characterService.GetSelectableRaceBonusesOnCharacter(character);

        // Reset picks
        SelectedPick1 = null;
        SelectedPick2 = null;
        UpdateAvailableStats();
    }

    private void UpdateAvailableStats() {
        // Get all stat types
        var allStats = Enum.GetValues<StatType>().ToList();

        // If Pick1 is selected, Remove it from both lists
        // If Pick2 is selected, Remove it from both lists
        var selectedStats = new List<StatType>();
        if (SelectedPick1.HasValue) {
            selectedStats.Add(SelectedPick1.Value);
        }
        if (SelectedPick2.HasValue) {
            selectedStats.Add(SelectedPick2.Value);
        }

        // Available for Pick1 = all stats except Pick2 (if selected)
        if (SelectedPick2.HasValue) {
            AvailableStatsForPick1 = allStats.Where(s => s != SelectedPick2.Value).ToList();
        } else {
            AvailableStatsForPick1 = allStats;
        }

        // Available for Pick2 = all stats except Pick1 (if selected)
        if (SelectedPick1.HasValue) {
            AvailableStatsForPick2 = allStats.Where(s => s != SelectedPick1.Value).ToList();
        } else {
            AvailableStatsForPick2 = allStats;
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
