using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public class ScifiWeaponInstanceService(AppDbContext db) {
    public async Task<ScifiWeaponInstance> CreateScifiWeaponInstanceAsync(CreateScifiWeaponInstanceDto request) {
        var weaponExists = await db.ScifiWeapons.AnyAsync(w => w.Id == request.ScifiWeaponId);
        if (!weaponExists) {
            throw new NotFoundException($"ScifiWeapon {request.ScifiWeaponId} was not found.");
        }
        var scifiCharacterExists = await db.ScifiCharacters.AnyAsync(c => c.Id == request.ScifiCharacterId);
        if (!scifiCharacterExists) {
            throw new NotFoundException($"ScifiCharacter {request.ScifiCharacterId} was not found.");
        }
        var blueprint = await db.ScifiWeapons.FirstOrDefaultAsync(w => w.Id == request.ScifiWeaponBlueprintId)
            ?? throw new NotFoundException($"ScifiWeaponBlueprint {request.ScifiWeaponBlueprintId} was not found.");

        var instance = new ScifiWeaponInstance(
            request.ScifiWeaponId,
            request.ScifiCharacterId,
            blueprint
        );

        db.ScifiWeaponInstances.Add(instance);
        await db.SaveChangesAsync();

        return instance;
    }

    public async Task<ScifiWeaponInstance> GetScifiWeaponInstanceAsync(int id) {
        return await GetInstanceAsync(id);
    }

    public async Task<IEnumerable<ScifiWeaponInstance>> GetUsersScifiWeaponInstancesAsync(int userId) {
        return await db.ScifiWeaponInstances.Where(i => i.ScifiCharacterId == userId).ToListAsync();
    }



    public async Task DeleteScifiWeaponInstanceAsync(int id) {
        var instance = await db.ScifiWeaponInstances.FindAsync(id)
            ?? throw new NotFoundException($"ScifiWeaponInstance {id} was not found.");

        db.ScifiWeaponInstances.Remove(instance);
        await db.SaveChangesAsync();
    }

    // Player customization methods
    public async Task<ScifiWeaponInstance> RenameWeaponAsync(int id, string newName) {
        var instance = await GetInstanceAsync(id);
        instance.RenameWeapon(newName);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> ToggleFavoriteAsync(int id) {
        var instance = await GetInstanceAsync(id);
        instance.ToggleFavorite();
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> ChangeOwnerAsync(int id, int newCharacterId) {
        var instance = await GetInstanceAsync(id);
        var characterExists = await db.ScifiCharacters.AnyAsync(c => c.Id == newCharacterId);
        if (!characterExists) {
            throw new NotFoundException($"ScifiCharacter {newCharacterId} was not found.");
        }
        instance.ChangeOwner(newCharacterId);
        await db.SaveChangesAsync();
        return instance;
    }

    // Ammo management
    public async Task<ScifiWeaponInstance> UseAmmoAsync(int id, int count) {
        var instance = await GetInstanceAsync(id);
        instance.UseAmmo(count);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> ReloadAsync(int id) {
        var instance = await GetInstanceAsync(id);
        instance.Reload();
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetAmmoAsync(int id, int amount) {
        var instance = await GetInstanceAsync(id);
        instance.SetAmmo(amount);
        await db.SaveChangesAsync();
        return instance;
    }

    // Damage & combat stats
    public async Task<ScifiWeaponInstance> SetDamageFormulaAsync(int id, string formula) {
        var instance = await GetInstanceAsync(id);
        instance.SetDamageFormula(formula);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetDamageTypeAsync(int id, DamageType type) {
        var instance = await GetInstanceAsync(id);
        instance.SetDamageType(type);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetEnergyTypeAsync(int id, EnergyType type) {
        var instance = await GetInstanceAsync(id);
        instance.SetEnergyType(type);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetModifierStatAsync(int id, StatType stat) {
        var instance = await GetInstanceAsync(id);
        instance.SetModifierStat(stat);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetRateOfFireAsync(int id, string rateOfFire) {
        var instance = await GetInstanceAsync(id);
        instance.SetRateOfFire(rateOfFire);
        await db.SaveChangesAsync();
        return instance;
    }

    // Ammo handling
    public async Task<ScifiWeaponInstance> SetAmmoTypeAsync(int id, ScifiAmmoType ammoType) {
        var instance = await GetInstanceAsync(id);
        instance.SetAmmoType(ammoType);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetAmmoCapacityAsync(int id, int capacity) {
        var instance = await GetInstanceAsync(id);
        instance.SetAmmoCapacity(capacity);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetReloadTimeAsync(int id, int actions) {
        var instance = await GetInstanceAsync(id);
        instance.SetReloadTime(actions);
        await db.SaveChangesAsync();
        return instance;
    }

    // Critical hit stats
    public async Task<ScifiWeaponInstance> SetCriticalRangeAsync(int id, int range) {
        var instance = await GetInstanceAsync(id);
        instance.SetCriticalRange(range);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetCriticalMultiplierAsync(int id, int multiplier) {
        var instance = await GetInstanceAsync(id);
        instance.SetCriticalMultiplier(multiplier);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetCriticalFailRangeAsync(int id, int range) {
        var instance = await GetInstanceAsync(id);
        instance.SetCriticalFailRange(range);
        await db.SaveChangesAsync();
        return instance;
    }

    public async Task<ScifiWeaponInstance> SetRangeCategoryAsync(int id, WeaponRange range) {
        var instance = await GetInstanceAsync(id);
        instance.SetRangeCategory(range);
        await db.SaveChangesAsync();
        return instance;
    }

    // Helper method
    private async Task<ScifiWeaponInstance> GetInstanceAsync(int id) {
        return await db.ScifiWeaponInstances.FindAsync(id)
            ?? throw new NotFoundException($"ScifiWeaponInstance {id} was not found.");
    }
}

public class CreateScifiWeaponInstanceDto {
    public int ScifiWeaponId { get; set; }
    public int ScifiCharacterId { get; set; }
    public int ScifiWeaponBlueprintId { get; set; }
}
