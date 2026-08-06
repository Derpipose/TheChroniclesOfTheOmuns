using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiSeedingController(ScifiSeedingService seedingService) : ControllerBase {

    [HttpPost("talents")]
    public async Task<IActionResult> SeedTalents([FromQuery] string? sourceUrl) {
        try {
            var (created, updated, total) = await seedingService.SeedTalentsAsync(sourceUrl);
            return Ok(new { created, updated, total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("classes")]
    public async Task<IActionResult> SeedClasses([FromQuery] string? sourceUrl) {
        try {
            var (created, updated, total, skippedClasses, skippedTalents) = await seedingService.SeedClassesAsync(sourceUrl);
            return Ok(new { created, updated, total, skippedClasses, skippedTalents });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("backgrounds")]
    public async Task<IActionResult> SeedBackgrounds([FromQuery] string? sourceUrl) {
        try {
            var (created, updated, total, skippedTalents) = await seedingService.SeedBackgroundsAsync(sourceUrl);
            return Ok(new { created, updated, total, skippedTalents });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("races")]
    public async Task<IActionResult> SeedRaces([FromQuery] string? sourceUrl) {
        try {
            var (created, skipped, total) = await seedingService.SeedRacesAsync(sourceUrl);
            return Ok(new { created, skipped, total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("subraces")]
    public async Task<IActionResult> SeedSubraces([FromQuery] string? sourceUrl) {
        try {
            var (created, skipped, total) = await seedingService.SeedSubracesAsync(sourceUrl);
            return Ok(new { created, skipped, total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("weapons")]
    public async Task<IActionResult> SeedWeapons([FromQuery] string? sourceUrl) {
        try {
            var (created, skipped, total) = await seedingService.SeedWeaponsAsync(sourceUrl);
            return Ok(new { created, skipped, total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }
}
