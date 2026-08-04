using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public class ScifiWeaponService(AppDbContext db) {

    public async Task<ScifiWeapon> CreateScifiWeaponAsync(CreateScifiWeaponDto request) {
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


}

public class CreateScifiWeaponDto {
    public string Name { get; set; } = string.Empty;
    public string DamageFormula { get; set; } = string.Empty;
    public ScifiAmmoType AmmoType { get; set; }
    public DamageType DamageType { get; set; }
    public EnergyType EnergyType { get; set; }
    public required string RateOfFire { get; set; }
    public int AmmoCapacity { get; set; }
    public int CriticalRange { get; set; } = 20;
    public int CriticalMultiplier { get; set; } = 2;
    public int ReloadTimeInActions { get; set; } = 1;
    public WeaponRange RangeCategory { get; set; }
    public int CriticalFailRange { get; set; }

}
