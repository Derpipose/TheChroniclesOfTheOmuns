using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/weapon")]
[Authorize]
public class ScifiWeaponController(ScifiWeaponService weaponService) : ControllerBase {

    [HttpPost]
    public async Task<ActionResult<ScifiWeapon>> CreateScifiWeapon(CreateScifiWeaponDto request) {
        try {
            var weapon = await weaponService.CreateScifiWeaponAsync(request);
            return CreatedAtAction(nameof(GetScifiWeapon), new { id = weapon.Id }, weapon);
        } catch (InvalidOperationException ex) {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ScifiWeapon>> UpdateScifiWeapon(int id, UpdateScifiWeaponDto request) {
        try {
            var weapon = await weaponService.UpdateScifiWeaponAsync(id, request);
            return Ok(weapon);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteScifiWeapon(int id) {
        try {
            await weaponService.DeleteScifiWeaponAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScifiWeapon>> GetScifiWeapon(int id) {
        try {
            var weapon = await weaponService.GetScifiWeaponAsync(id);
            return Ok(weapon);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScifiWeapon>>> GetAllScifiWeapons() {
        var weapons = await weaponService.GetAllScifiWeaponsAsync();
        return Ok(weapons);
    }
}
