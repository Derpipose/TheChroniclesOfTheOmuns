// Enums
using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;


// Blueprint: Shop/Dealer template
public class ScifiWeapon {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;

    public string DamageFormula { get; private set; } = string.Empty;  // "1d6"

    public StatType ModifierStat { get; private set; } = StatType.Dexterity;
    public DamageType DamageType { get; private set; } = DamageType.Ballistic;
    public EnergyType EnergyType { get; private set; } = EnergyType.Physical;

    public int RateOfFire { get; private set; } = 0;
    public int AmmoCapacity { get; private set; } = 0;
    public int ReloadTimeInActions { get; private set; } = 1;

    public string CriticalRange { get; private set; } = string.Empty;  // "20" or "19-20" Maybe Crit Base, since range always goes to 20. 
    public string CriticalMultiplier { get; private set; } = string.Empty;  // "x2" Not string, Int. It's always a whole number and always a multiplier. 

    public string? RangeCategory { get; private set; }
    public string? CriticalFailRange { get; private set; } //Same thing as crit range, but for critical fails. We always go to 1, So we really just need the highest number in the range. 


    public ScifiWeapon(
        string name,
        string damageFormula,
        DamageType damageType,
        EnergyType energyType,
        int rateOfFire,
        int ammoCapacity,
        string criticalRange,
        string criticalMultiplier,
        int reloadTimeInActions = 1,
        string? rangeCategory = null,
        string? criticalFailRange = null) {
        Name = name;
        DamageFormula = damageFormula;
        DamageType = damageType;
        EnergyType = energyType;
        RateOfFire = rateOfFire;
        AmmoCapacity = ammoCapacity;
        CriticalRange = criticalRange;
        CriticalMultiplier = criticalMultiplier;
        ReloadTimeInActions = reloadTimeInActions;
        RangeCategory = rangeCategory;
        CriticalFailRange = criticalFailRange;
    }

    protected ScifiWeapon() { }

    // Setters for blueprint modifications (rarely used)
    public void SetName(string name) => Name = name;
    public void SetDamageFormula(string formula) => DamageFormula = formula;
    public void SetDamageType(DamageType type) => DamageType = type;
    public void SetEnergyType(EnergyType type) => EnergyType = type;
    public void SetRateOfFire(int rof) => RateOfFire = rof;
    public void SetAmmoCapacity(int capacity) => AmmoCapacity = capacity;
    public void SetReloadTime(int actions) => ReloadTimeInActions = actions;
    public void SetCriticalRange(string range) => CriticalRange = range;
    public void SetCriticalMultiplier(string multiplier) => CriticalMultiplier = multiplier;
    public void SetRangeCategory(string? range) => RangeCategory = range;
    public void SetCriticalFailRange(string? range) => CriticalFailRange = range;

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) => Id = id;
}
