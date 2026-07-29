
// Instance: Player-owned weapon (copied from blueprint, then independent)
// Instance: Player-owned weapon (independent after creation)
using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiWeaponInstance {
    public int Id { get; private set; } = 0;  // EF Core sets this, never set manually

    // Foreign key to blueprint (for reset-to-factory, NOT for normal play)
    public int ScifiWeaponId { get; private set; } = 0;
    // NO navigation property — instance doesn't depend on blueprint after creation

    // Owner reference
    public int ScifiCharacterId { get; private set; } = 0;

    // Instance-owned state
    public int CurrentAmmo { get; private set; } = 0;

    // Snapshot of blueprint stats at creation (independent copy)
    public string DamageFormula { get; private set; } = string.Empty;
    public DamageType DamageType { get; private set; } = DamageType.Ballistic;
    public EnergyType EnergyType { get; private set; } = EnergyType.Physical;
    public int RateOfFire { get; private set; } = 0;
    public int AmmoCapacity { get; private set; } = 0;
    public int ReloadTimeInActions { get; private set; } = 1;
    public string CriticalRange { get; private set; } = string.Empty;
    public string CriticalMultiplier { get; private set; } = string.Empty;

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

        // Ammo defaults to full capacity
        CurrentAmmo = blueprint.AmmoCapacity;
    }

    // Parameterless constructor for EF Core
    protected ScifiWeaponInstance() { }

    // Player actions
    public void UseAmmo(int count) => CurrentAmmo = Math.Max(0, CurrentAmmo - count);
    public void Reload() => CurrentAmmo = AmmoCapacity;

    // Modify instance stats (independent of blueprint)
    public void SetDamageFormula(string formula) => DamageFormula = formula;
    public void SetDamageType(DamageType type) => DamageType = type;
    public void SetEnergyType(EnergyType type) => EnergyType = type;
    public void SetRateOfFire(int rof) => RateOfFire = rof;
    public void SetCriticalRange(string range) => CriticalRange = range;
    public void SetCriticalMultiplier(string multiplier) => CriticalMultiplier = multiplier;

    public ScifiWeaponInstance UpdateWeaponInstance(ScifiWeaponInstance updated) {
        ScifiWeaponId = updated.ScifiWeaponId;
        ScifiCharacterId = updated.ScifiCharacterId;

        DamageFormula = updated.DamageFormula;
        DamageType = updated.DamageType;
        EnergyType = updated.EnergyType;
        RateOfFire = updated.RateOfFire;
        AmmoCapacity = updated.AmmoCapacity;
        ReloadTimeInActions = updated.ReloadTimeInActions;
        CriticalRange = updated.CriticalRange;
        CriticalMultiplier = updated.CriticalMultiplier;

        // Ammo defaults to full capacity
        CurrentAmmo = updated.AmmoCapacity;
        return this;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) => Id = id;
}
