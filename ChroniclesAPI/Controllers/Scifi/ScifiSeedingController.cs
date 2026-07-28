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
            var result = await seedingService.SeedTalentsAsync(sourceUrl);
            return Ok(new { created = result.created, updated = result.updated, total = result.total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("classes")]
    public async Task<IActionResult> SeedClasses([FromQuery] string? sourceUrl) {
        try {
            var result = await seedingService.SeedClassesAsync(sourceUrl);
            return Ok(new { created = result.created, updated = result.updated, total = result.total, skippedClasses = result.skippedClasses, skippedTalents = result.skippedTalents });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("backgrounds")]
    public async Task<IActionResult> SeedBackgrounds([FromQuery] string? sourceUrl) {
        try {
            var result = await seedingService.SeedBackgroundsAsync(sourceUrl);
            return Ok(new { created = result.created, updated = result.updated, total = result.total, skippedTalents = result.skippedTalents });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("races")]
    public async Task<IActionResult> SeedRaces([FromQuery] string? sourceUrl) {
        try {
            var result = await seedingService.SeedRacesAsync(sourceUrl);
            return Ok(new { created = result.created, skipped = result.skipped, total = result.total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("subraces")]
    public async Task<IActionResult> SeedSubraces([FromQuery] string? sourceUrl) {
        try {
            var result = await seedingService.SeedSubracesAsync(sourceUrl);
            return Ok(new { created = result.created, skipped = result.skipped, total = result.total });
        } catch (InvalidOperationException ex) {
            return BadRequest(new { message = ex.Message });
        }
    }
}