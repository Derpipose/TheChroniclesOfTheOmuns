using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public class ScifiWeaponService(AppDbContext db) {

    public async Task<ScifiWeapon> CreateScifiWeaponAsync(CreateScifiWeaponDto request) {
        var shouldbeNull = await db.ScifiWeapons.FirstOrDefaultAsync(w => w.Name == request.Name);
        if (shouldbeNull != null) {
            throw new InvalidOperationException($"ScifiWeapon with name {request.Name} already exists.");
        }
        var weapon = new ScifiWeapon(
            request.Name,
            request.DamageFormula,
            request.AmmoType,
            request.DamageType,
            request.EnergyType,
            request.RateOfFire,
            request.AmmoCapacity,
            request.CriticalRange,
            request.CriticalMultiplier,
            request.ReloadTimeInActions,
            request.RangeCategory,
            request.CriticalFailRange
        );

        db.ScifiWeapons.Add(weapon);
        await db.SaveChangesAsync();

        return weapon;
    }

    public async Task<ScifiWeapon> UpdateScifiWeaponAsync(int id, UpdateScifiWeaponDto request) {
        var weapon = await db.ScifiWeapons.FindAsync(id) ??
        throw new NotFoundException($"ScifiWeapon {id} was not found.");

        weapon.SetName(request.Name);
        weapon.SetDamageFormula(request.DamageFormula);
        weapon.SetAmmoType(request.AmmoType);
        weapon.SetDamageType(request.DamageType);
        weapon.SetEnergyType(request.EnergyType);
        weapon.SetRateOfFire(request.RateOfFire);
        weapon.SetAmmoCapacity(request.AmmoCapacity);
        weapon.SetCriticalRange(request.CriticalRange);
        weapon.SetCriticalMultiplier(request.CriticalMultiplier);
        weapon.SetReloadTime(request.ReloadTimeInActions);
        weapon.SetRangeCategory(request.RangeCategory);
        weapon.SetCriticalFailRange(request.CriticalFailRange);

        await db.SaveChangesAsync();

        return weapon;
    }

    public async Task DeleteScifiWeaponAsync(int id) {
        var weapon = await db.ScifiWeapons.FindAsync(id) ??
        throw new NotFoundException($"ScifiWeapon {id} was not found.");

        db.ScifiWeapons.Remove(weapon);
        await db.SaveChangesAsync();
    }


}

public class UpdateScifiWeaponDto {
    public string Name { get; internal set; } = string.Empty;
    public string DamageFormula { get; internal set; } = string.Empty;
    public ScifiAmmoType AmmoType { get; internal set; }
    public DamageType DamageType { get; internal set; }
    public EnergyType EnergyType { get; internal set; }
    public string RateOfFire { get; internal set; } = string.Empty;
    public int AmmoCapacity { get; internal set; }
    public int CriticalRange { get; internal set; }
    public int CriticalMultiplier { get; internal set; }
    public int ReloadTimeInActions { get; internal set; }
    public WeaponRange RangeCategory { get; internal set; }
    public int? CriticalFailRange { get; internal set; }
}

public class CreateScifiWeaponDto {
    public string Name { get; set; } = string.Empty;
    public string DamageFormula { get; set; } = string.Empty;
    public ScifiAmmoType AmmoType { get; set; }
    public DamageType DamageType { get; set; }
    public EnergyType EnergyType { get; set; }
    public string RateOfFire { get; set; } = string.Empty;
    public int AmmoCapacity { get; set; }
    public int CriticalRange { get; set; } = 20;
    public int CriticalMultiplier { get; set; } = 2;
    public int ReloadTimeInActions { get; set; } = 1;
    public WeaponRange RangeCategory { get; set; }
    public int CriticalFailRange { get; set; }

}
