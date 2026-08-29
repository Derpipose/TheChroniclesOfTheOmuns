using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesTest;

public class ScifiWeaponServiceTests {
    [Fact]
    public async Task CreateScifiWeaponAsync_CreatesWeapon_WhenNameIsUnique() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var result = await weaponService.CreateScifiWeaponAsync(new CreateScifiWeaponDto {
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
        });

        Assert.Equal("Plasma Rifle", result.Name);
        Assert.Single(db.ScifiWeapons);
    }

    [Fact]
    public async Task CreateScifiWeaponAsync_Throws_WhenNameAlreadyExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var request = new CreateScifiWeaponDto {
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

        await weaponService.CreateScifiWeaponAsync(request);

        await Assert.ThrowsAsync<InvalidOperationException>(() => weaponService.CreateScifiWeaponAsync(request));
    }

    [Fact]
    public async Task UpdateScifiWeaponAsync_UpdatesWeapon_WhenIdExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(new CreateScifiWeaponDto {
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
        });

        var result = await weaponService.UpdateScifiWeaponAsync(weapon.Id, new UpdateScifiWeaponDto {
            Name = "Plasma Rifle MK2",
            DamageFormula = "4d6+3",
            AmmoType = ScifiAmmoType.Generic,
            DamageType = DamageType.Energy,
            EnergyType = EnergyType.Blaze,
            RateOfFire = "Full-Auto",
            AmmoCapacity = 45,
            CriticalRange = 18,
            CriticalMultiplier = 4,
            ReloadTimeInActions = 2,
            RangeCategory = WeaponRange.Long,
            CriticalFailRange = 1
        });

        Assert.Equal("Plasma Rifle MK2", result.Name);
        Assert.Equal("4d6+3", result.DamageFormula);
        Assert.Equal(45, result.AmmoCapacity);
    }

    [Fact]
    public async Task UpdateScifiWeaponAsync_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => weaponService.UpdateScifiWeaponAsync(999, new UpdateScifiWeaponDto {
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
        }));
    }

    [Fact]
    public async Task GetScifiWeaponAsync_ReturnsWeapon_WhenIdExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var created = await weaponService.CreateScifiWeaponAsync(new CreateScifiWeaponDto {
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
        });

        var result = await weaponService.GetScifiWeaponAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Plasma Rifle", result.Name);
    }

    [Fact]
    public async Task GetScifiWeaponAsync_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => weaponService.GetScifiWeaponAsync(999));
    }

    [Fact]
    public async Task GetAllScifiWeaponsAsync_ReturnsAllWeapons() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var request1 = new CreateScifiWeaponDto {
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

        var request2 = new CreateScifiWeaponDto {
            Name = "Laser Pistol",
            DamageFormula = "2d4+1",
            AmmoType = ScifiAmmoType.Generic,
            DamageType = DamageType.Energy,
            EnergyType = EnergyType.Volt,
            RateOfFire = "Semi-Auto",
            AmmoCapacity = 15,
            CriticalRange = 20,
            CriticalMultiplier = 2,
            ReloadTimeInActions = 1,
            RangeCategory = WeaponRange.Medium,
            CriticalFailRange = 1
        };

        await weaponService.CreateScifiWeaponAsync(request1);
        await weaponService.CreateScifiWeaponAsync(request2);

        var result = await weaponService.GetAllScifiWeaponsAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, w => w.Name == "Plasma Rifle");
        Assert.Contains(result, w => w.Name == "Laser Pistol");
    }

    [Fact]
    public async Task DeleteScifiWeaponAsync_RemovesWeapon_WhenIdExists() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        var weapon = await weaponService.CreateScifiWeaponAsync(new CreateScifiWeaponDto {
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
        });

        await weaponService.DeleteScifiWeaponAsync(weapon.Id);

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        Assert.False(await db.ScifiWeapons.AnyAsync(w => w.Id == weapon.Id));
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
    }

    [Fact]
    public async Task DeleteScifiWeaponAsync_Throws_WhenIdDoesNotExist() {
        await using var db = TestDb.Create();
        var weaponService = CreateWeaponService(db);

        await Assert.ThrowsAsync<NotFoundException>(() => weaponService.DeleteScifiWeaponAsync(999));
    }

    private static ScifiWeaponService CreateWeaponService(AppDbContext db) =>
        new(db);
}
