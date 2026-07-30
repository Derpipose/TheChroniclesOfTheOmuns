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


}

public class CreateScifiWeaponInstanceDto {
    public int ScifiWeaponId { get; set; }
    public int ScifiCharacterId { get; set; }
    public int ScifiWeaponBlueprintId { get; set; }
}
