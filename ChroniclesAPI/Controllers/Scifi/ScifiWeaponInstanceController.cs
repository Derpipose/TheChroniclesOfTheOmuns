using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/weaponinstance")]
[Authorize]
public class ScifiWeaponInstanceController(ScifiWeaponInstanceService weaponService) : ControllerBase {

    // CRUD Operations
    [HttpPost]
    public async Task<ActionResult<ScifiWeaponInstance>> CreateScifiWeaponInstance(CreateScifiWeaponInstanceDto request) {
        try {
            var instance = await weaponService.CreateScifiWeaponInstanceAsync(request);
            return CreatedAtAction(nameof(GetScifiWeaponInstance), new { id = instance.Id }, instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScifiWeaponInstance>> GetScifiWeaponInstance(int id) {
        try {
            var instance = await weaponService.GetScifiWeaponInstanceAsync(id);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("character/{characterId}")]
    public async Task<ActionResult<IEnumerable<ScifiWeaponInstance>>> GetUsersScifiWeaponInstances(int characterId) {
        var instances = await weaponService.GetUsersScifiWeaponInstancesAsync(characterId);
        return Ok(instances);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScifiWeaponInstance(int id) {
        try {
            await weaponService.DeleteScifiWeaponInstanceAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // Player Customization Methods
    [HttpPatch("{id}/rename")]
    public async Task<ActionResult<ScifiWeaponInstance>> RenameWeapon(int id, [FromBody] StringUpdateDto request) {
        try {
            var instance = await weaponService.RenameWeaponAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/favorite")]
    public async Task<ActionResult<ScifiWeaponInstance>> ToggleFavorite(int id) {
        try {
            var instance = await weaponService.ToggleFavoriteAsync(id);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/owner")]
    public async Task<ActionResult<ScifiWeaponInstance>> ChangeOwner(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.ChangeOwnerAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // Ammo Management
    [HttpPatch("{id}/use-ammo")]
    public async Task<ActionResult<ScifiWeaponInstance>> UseAmmo(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.UseAmmoAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/reload")]
    public async Task<ActionResult<ScifiWeaponInstance>> Reload(int id) {
        try {
            var instance = await weaponService.ReloadAsync(id);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/ammo")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetAmmo(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetAmmoAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // Damage & Combat Stats
    [HttpPatch("{id}/damage-formula")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetDamageFormula(int id, [FromBody] StringUpdateDto request) {
        try {
            var instance = await weaponService.SetDamageFormulaAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/damage-type")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetDamageType(int id, [FromBody] UpdateWeaponDamageTypeDto request) {
        try {
            var instance = await weaponService.SetDamageTypeAsync(id, request.DamageType);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/energy-type")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetEnergyType(int id, [FromBody] UpdateWeaponEnergyTypeDto request) {
        try {
            var instance = await weaponService.SetEnergyTypeAsync(id, request.EnergyType);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/modifier-stat")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetModifierStat(int id, [FromBody] UpdateWeaponModifierStatDto request) {
        try {
            var instance = await weaponService.SetModifierStatAsync(id, request.Stat);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/rate-of-fire")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetRateOfFire(int id, [FromBody] StringUpdateDto request) {
        try {
            var instance = await weaponService.SetRateOfFireAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // Ammo Handling
    [HttpPatch("{id}/ammo-type")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetAmmoType(int id, [FromBody] UpdateWeaponAmmoTypeDto request) {
        try {
            var instance = await weaponService.SetAmmoTypeAsync(id, request.AmmoType);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/ammo-capacity")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetAmmoCapacity(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetAmmoCapacityAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/reload-time")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetReloadTime(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetReloadTimeAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // Critical Hit Stats
    [HttpPatch("{id}/critical-range")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetCriticalRange(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetCriticalRangeAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/critical-multiplier")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetCriticalMultiplier(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetCriticalMultiplierAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/critical-fail-range")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetCriticalFailRange(int id, [FromBody] IntUpdateDto request) {
        try {
            var instance = await weaponService.SetCriticalFailRangeAsync(id, request.NewValue);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/range-category")]
    public async Task<ActionResult<ScifiWeaponInstance>> SetRangeCategory(int id, [FromBody] UpdateWeaponRangeCategoryDto request) {
        try {
            var instance = await weaponService.SetRangeCategoryAsync(id, request.Range);
            return Ok(instance);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }
}

// DTOs for updates
public class StringUpdateDto {
    public string NewValue { get; set; } = string.Empty;
}

public class IntUpdateDto {
    public int NewValue { get; set; }
}

public class UpdateWeaponDamageTypeDto {
    public DamageType DamageType { get; set; }
}

public class UpdateWeaponEnergyTypeDto {
    public EnergyType EnergyType { get; set; }
}

public class UpdateWeaponModifierStatDto {
    public StatType Stat { get; set; }
}

public class UpdateWeaponAmmoTypeDto {
    public ScifiAmmoType AmmoType { get; set; }
}
public class UpdateWeaponRangeCategoryDto {
    public WeaponRange Range { get; set; }
}
