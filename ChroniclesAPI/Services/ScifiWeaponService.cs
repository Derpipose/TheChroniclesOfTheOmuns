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
    public DamageType DamageType { get; set; }
    public EnergyType EnergyType { get; set; }
    public int RateOfFire { get; set; }
    public int AmmoCapacity { get; set; }
    public string CriticalRange { get; set; } = string.Empty;
    public string CriticalMultiplier { get; set; } = string.Empty;
    public int ReloadTimeInActions { get; set; } = 1;
    public string? RangeCategory { get; set; }
    public string? CriticalFailRange { get; set; }
    
}
