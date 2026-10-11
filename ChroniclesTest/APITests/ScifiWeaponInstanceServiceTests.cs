using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken

public class ScifiWeaponInstanceServiceTests {

    // [Fact]
    // public async Task CreateScifiWeaponInstanceAsync_CreatesInstance_WhenWeaponAndBlueprintExist() {
    //     await using var db = TestDb.Create();
    //     var weaponService = CreateWeaponService(db);
    //     var instanceService = CreateInstanceService(db);

    //     var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
    //     var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());

    //     var characterBase = new Character(1, "TestCharacter");
    //     characterBase.SetMaxHealth(10);
    //     characterBase.SetLevel(1);
    //     db.Characters.Add(characterBase);
    //     await db.SaveChangesAsync();


    //     var result = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
    //         ScifiWeaponId = weapon.Id,
    //         ScifiCharacterId = characterBase.Id,
    //         ScifiWeaponBlueprintId = blueprint.Id
    //     });

    //     Assert.NotNull(result);
    //     Assert.Equal(weapon.Id, result.ScifiWeaponId);
    //     Assert.Equal(characterBase.Id, result.ScifiCharacterId);
    //     Assert.Single(db.ScifiWeaponInstances);
    // }

    [Fact]
    public async Task CreateScifiWeaponInstanceAsync_Throws_WhenWeaponDoesNotExist() {
        await using var db = TestDb.Create();
        var instanceService = CreateInstanceService(db);

        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        await Assert.ThrowsAsync<NotFoundException>(() => instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = 999,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = 999
        }));
    }

    [Fact]
    public async Task CreateScifiWeaponInstanceAsync_Throws_WhenBlueprintDoesNotExist() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        await Assert.ThrowsAsync<NotFoundException>(() => instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = 999
        }));
    }

    [Fact]
    public async Task GetScifiWeaponInstanceAsync_ReturnsInstance_WhenIdExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var created = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.GetScifiWeaponInstanceAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(weapon.Id, result.ScifiWeaponId);
    }

    [Fact]
    public async Task GetScifiWeaponInstanceAsync_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var instanceService = CreateInstanceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => instanceService.GetScifiWeaponInstanceAsync(999));
    }

    [Fact]
    public async Task GetUsersScifiWeaponInstancesAsync_ReturnsCharacterWeapons() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon1 = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var weapon2 = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(new CreateScifiWeaponDto {
            Name = "Knife",
            DamageFormula = "1d6",
            AmmoType = ScifiAmmoType.None,
            DamageType = DamageType.Melee,
            EnergyType = EnergyType.Physical,
            RateOfFire = "N/A",
            AmmoCapacity = 0,
            CriticalRange = 19,
            CriticalMultiplier = 2,
            ReloadTimeInActions = 0,
            RangeCategory = WeaponRange.Melee,
            CriticalFailRange = 1
        });

        var scifiCharacter1 = await CreateScifiCharacterAsync(db, 1, "Character1");
        var scifiCharacter2 = await CreateScifiCharacterAsync(db, 2, "Character2");

        await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon1.Id,
            ScifiCharacterId = scifiCharacter1.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon2.Id,
            ScifiCharacterId = scifiCharacter1.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon1.Id,
            ScifiCharacterId = scifiCharacter2.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.GetUsersScifiWeaponInstancesAsync(scifiCharacter1.Id);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task DeleteScifiWeaponInstanceAsync_RemovesInstance_WhenIdExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        await instanceService.DeleteScifiWeaponInstanceAsync(instance.Id);

        Assert.False(await db.ScifiWeaponInstances.AnyAsync(i => i.Id == instance.Id));
    }

    [Fact]
    public async Task DeleteScifiWeaponInstanceAsync_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var instanceService = CreateInstanceService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => instanceService.DeleteScifiWeaponInstanceAsync(999));
    }

    // Player Customization Tests
    [Fact]
    public async Task RenameWeaponAsync_UpdatesName_WhenInstanceExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.RenameWeaponAsync(instance.Id, "Inferno Rifle");

        Assert.Equal("Inferno Rifle", result.Nickname);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_TogglesFavoriteStatus() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        Assert.False(instance.Favorite);

        var toggled = await instanceService.ToggleFavoriteAsync(instance.Id);
        Assert.True(toggled.Favorite);

        var toggledAgain = await instanceService.ToggleFavoriteAsync(instance.Id);
        Assert.False(toggledAgain.Favorite);
    }

    [Fact]
    public async Task ChangeOwnerAsync_ChangesCharacter_WhenNewCharacterExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());

        // Create base characters
        var characterBase1 = new Character(1, "Character1");
        characterBase1.SetBaseHealth(10);
        characterBase1.SetLevel(1);
        db.Characters.Add(characterBase1);

        var characterBase2 = new Character(2, "Character2");
        characterBase2.SetBaseHealth(10);
        characterBase2.SetLevel(1);
        db.Characters.Add(characterBase2);
        await db.SaveChangesAsync();

        // Create ScifiCharacters with base character IDs
        var scifiCharacter1 = new ScifiCharacter(characterBase1.Id);
        db.ScifiCharacters.Add(scifiCharacter1);

        var scifiCharacter2 = new ScifiCharacter(characterBase2.Id);
        db.ScifiCharacters.Add(scifiCharacter2);
        await db.SaveChangesAsync();

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter1.Id,  // Use the ScifiCharacter's Id
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.ChangeOwnerAsync(instance.Id, scifiCharacter2.Id);

        Assert.Equal(scifiCharacter2.Id, result.ScifiCharacterId);
    }

    [Fact]
    public async Task ChangeOwnerAsync_Throws_WhenNewCharacterDoesNotExist() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        await Assert.ThrowsAsync<NotFoundException>(() => instanceService.ChangeOwnerAsync(instance.Id, 999));
    }

    // Ammo Management Tests
    [Fact]
    public async Task UseAmmoAsync_ReducesAmmo_WhenAmmoAvailable() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var initialAmmo = instance.CurrentAmmo;
        var result = await instanceService.UseAmmoAsync(instance.Id, 5);

        Assert.Equal(initialAmmo - 5, result.CurrentAmmo);
    }

    [Fact]
    public async Task ReloadAsync_ResetsAmmo_ToMaxCapacity() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        await instanceService.UseAmmoAsync(instance.Id, 10);
        var result = await instanceService.ReloadAsync(instance.Id);

        Assert.Equal(blueprint.AmmoCapacity, result.CurrentAmmo);
    }

    [Fact]
    public async Task SetAmmoAsync_SetsAmmoToSpecificAmount() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.SetAmmoAsync(instance.Id, 20);

        Assert.Equal(20, result.CurrentAmmo);
    }

    // Damage & Combat Stats Tests
    [Fact]
    public async Task SetDamageFormulaAsync_UpdatesDamageFormula() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.SetDamageFormulaAsync(instance.Id, "5d8+4");

        Assert.Equal("5d8+4", result.DamageFormula);
    }

    [Fact]
    public async Task SetCriticalRangeAsync_UpdatesCriticalRange() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);
        var instanceService = CreateInstanceService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(CreatePlasmaRifleDto());
        var blueprint = await weaponService.CreateScifiWeaponAsync(CreateLaserPistolDto());
        var scifiCharacter = await CreateScifiCharacterAsync(db, 1, "TestCharacter");

        var instance = await instanceService.CreateScifiWeaponInstanceAsync(new CreateScifiWeaponInstanceDto {
            ScifiWeaponId = weapon.Id,
            ScifiCharacterId = scifiCharacter.Id,
            ScifiWeaponBlueprintId = blueprint.Id
        });

        var result = await instanceService.SetCriticalRangeAsync(instance.Id, 17);

        Assert.Equal(17, result.CriticalRange);
    }

    // Helper methods
    private static async Task<ScifiCharacter> CreateScifiCharacterAsync(AppDbContext db, int characterId, string name) {
        var character = new Character(characterId, name);
        character.SetBaseHealth(10);
        character.SetLevel(1);
        db.Characters.Add(character);

        var scifiCharacter = new ScifiCharacter(character.Id);
        db.ScifiCharacters.Add(scifiCharacter);

        await db.SaveChangesAsync();
        return scifiCharacter;
    }

    private static ScifiWeaponService CreateWeaponService(AppDbContext db) =>
        new(db);

    private static ScifiWeaponInstanceService CreateInstanceService(AppDbContext db) =>
        new(db);

    private static CreateScifiWeaponDto CreatePlasmaRifleDto() => new() {
        Name = "Plasma Rifle",
        DamageFormula = "3d6+2",
        AmmoType = ScifiAmmoType.Generic,
        DamageType = DamageType.Energy,
        EnergyType = EnergyType.Blaze,
        RateOfFire = "Semi-Auto",
        AmmoCapacity = 30,
        CriticalRange = 19,
        CriticalMultiplier = 3,
        ReloadTimeInActions = 1,
        RangeCategory = WeaponRange.Long,
        CriticalFailRange = 1
    };

    private static CreateScifiWeaponDto CreateLaserPistolDto() => new() {
        Name = "Laser Pistol",
        DamageFormula = "2d4+1",
        AmmoType = ScifiAmmoType.Generic,
        DamageType = DamageType.Energy,
        EnergyType = EnergyType.Cryo,
        RateOfFire = "Semi-Auto",
        AmmoCapacity = 15,
        CriticalRange = 20,
        CriticalMultiplier = 2,
        ReloadTimeInActions = 1,
        RangeCategory = WeaponRange.Medium,
        CriticalFailRange = 1
    };
}
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
