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
    public ScifiAmmoType AmmoType { get; private set; } = ScifiAmmoType.None;

    public int AmmoCapacity { get; private set; } = 0;
    public string RateOfFire { get; private set; } = "1";
    public int ReloadTimeInActions { get; private set; } = 1;

    public int CriticalMultiplier { get; private set; } = 2; 
    public int CriticalRange { get; private set; } = 20;  // holds lowest critical success roll, ie 18, 19, 20
    public int? CriticalFailRange { get; private set; } = 1; //Same thing as crit range, but for critical fails. We always go to 1, So we really just need the highest number in the range.

    public WeaponRange RangeCategory { get; private set; } = WeaponRange.None; // "Short", "Medium", "Long" - optional, can be null if not applicable


    public ScifiWeapon(
                        string name,
                        string damageFormula,
                        ScifiAmmoType ammoType,
                        DamageType damageType,
                        EnergyType energyType,
                        string rateOfFire,
                        int ammoCapacity,
                        int criticalRange,
                        int criticalMultiplier,
                        int reloadTimeInActions = 1,
                        WeaponRange rangeCategory = WeaponRange.None,
                        int criticalFailRange = 0) {
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
        AmmoType = ammoType;
    }

    protected ScifiWeapon() { }

    // Setters for blueprint modifications (rarely used)
    public void SetName(string name) => Name = name;
    public void SetDamageFormula(string formula) => DamageFormula = formula;
    public void SetDamageType(DamageType type) => DamageType = type;
    public void SetEnergyType(EnergyType type) => EnergyType = type;
    public void SetRateOfFire(string rof) => RateOfFire = rof;
    public void SetAmmoCapacity(int capacity) => AmmoCapacity = capacity;
    public void SetAmmoType(ScifiAmmoType ammoType) => AmmoType = ammoType;
    public void SetReloadTime(int actions) => ReloadTimeInActions = actions;
    public void SetCriticalRange(int range) => CriticalRange = range;
    public void SetCriticalMultiplier(int multiplier) => CriticalMultiplier = multiplier;
    public void SetRangeCategory(WeaponRange range) => RangeCategory = range;
    public void SetCriticalFailRange(int? range) => CriticalFailRange = range;


    [Conditional("DEBUG")]
    public void SimulatePersist(int id) => Id = id;
}
