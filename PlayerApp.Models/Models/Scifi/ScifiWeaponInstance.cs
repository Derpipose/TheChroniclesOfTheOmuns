
// Instance: Player-owned weapon (copied from blueprint, then independent)
// Instance: Player-owned weapon (independent after creation)
using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class ScifiWeaponInstance {
    public int Id { get; private set; } = 0;  // EF Core sets this, never set manually
    public string Name { get; private set; } = string.Empty;
    public string Nickname { get; private set; } = string.Empty;  // Player can rename weapon instance
    public bool Favorite { get; private set; } = false;  // Player can mark weapon instance as favorite

    // Foreign key to blueprint (for reset-to-factory, NOT for normal play)
    public int ScifiWeaponId { get; private set; } = 0;
    // NO navigation property — instance doesn't depend on blueprint after creation
    // And shouldn't

    // Owner reference
    public int ScifiCharacterId { get; private set; } = 0;

    // Instance-owned state
    public int CurrentAmmo { get; private set; } = 0;

    // Snapshot of blueprint stats at creation (independent copy)
    public string DamageFormula { get; private set; } = string.Empty;

    public StatType ModifierStat { get; private set; } = StatType.Dexterity;
    public DamageType DamageType { get; private set; } = DamageType.Ballistic;
    public EnergyType EnergyType { get; private set; } = EnergyType.Physical;
    public ScifiAmmoType AmmoType { get; private set; } = ScifiAmmoType.None;

    public int AmmoCapacity { get; private set; } = 0;
    public string RateOfFire { get; private set; } = "1";
    public int ReloadTimeInActions { get; private set; } = 1;

    public int CriticalMultiplier { get; private set; } = 2;
    public int CriticalRange { get; private set; } = 20;
    public int? CriticalFailRange { get; private set; } = 1;

    public WeaponRange RangeCategory { get; private set; } = WeaponRange.None;


    // Constructor: takes already-prepared data (no blueprint dependency)
    public ScifiWeaponInstance(
                        int scifiWeaponId,
                        int scifiCharacterId,
                        ScifiWeapon blueprint) {
                        ScifiWeaponId = scifiWeaponId;
                        ScifiCharacterId = scifiCharacterId;

        // Copy stats from blueprint at creation time
        DamageFormula = blueprint.DamageFormula;
        DamageType = blueprint.DamageType;
        EnergyType = blueprint.EnergyType;
        RateOfFire = blueprint.RateOfFire;
        AmmoCapacity = blueprint.AmmoCapacity;
        ReloadTimeInActions = blueprint.ReloadTimeInActions;
        CriticalRange = blueprint.CriticalRange;
        CriticalMultiplier = blueprint.CriticalMultiplier;
        CriticalFailRange = blueprint.CriticalFailRange;
        RangeCategory = blueprint.RangeCategory;
        AmmoType = blueprint.AmmoType;
        Name = blueprint.Name;
        CurrentAmmo = blueprint.AmmoCapacity;
        ModifierStat = blueprint.ModifierStat;



        // Ammo defaults to full capacity
        CurrentAmmo = blueprint.AmmoCapacity;
    }

    // Parameterless constructor for EF Core
    protected ScifiWeaponInstance() { }

    // Player actions
    public void RenameWeapon(string newName) => Nickname = newName;
    public void UseAmmo(int count) => CurrentAmmo -= count;

    // So, reloading should take into account current ammo on the
    // character, but we will handle that when we implement the inventory system.
    // For now, we will just reset to full capacity.
    public void Reload() => CurrentAmmo = AmmoCapacity;

    public void ToggleFavorite() => Favorite = !Favorite;

    public void ChangeOwner(int newCharacterId) => ScifiCharacterId = newCharacterId;   //don't expect it to be used much, but here it is

    // Modify instance stats (independent of blueprint)
    public void SetDamageFormula(string formula) => DamageFormula = formula;
    public void SetDamageType(DamageType type) => DamageType = type;
    public void SetEnergyType(EnergyType type) => EnergyType = type;
    public void SetAmmo(int amount) => CurrentAmmo = amount;
    public void SetModifierStat(StatType stat) => ModifierStat = stat;
    public void SetRateOfFire(string rof) => RateOfFire = rof;
    public void SetAmmoType(ScifiAmmoType ammoType) => AmmoType = ammoType;
    public void SetAmmoCapacity(int capacity) => AmmoCapacity = capacity;
    public void SetCriticalRange(int range) => CriticalRange = range;
    public void SetCriticalMultiplier(int multiplier) => CriticalMultiplier = multiplier;
    public void SetReloadTime(int actions) => ReloadTimeInActions = actions;
    public void SetCriticalFailRange(int range) => CriticalFailRange = range;
    public void SetRangeCategory(WeaponRange range) => RangeCategory = range;



    [Conditional("DEBUG")]
    public void SimulatePersist(int id) => Id = id;
}
