using ChroniclesAPI.Exceptions;
using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiSubraceController(SciFiSubraceService subraceService) : ControllerBase {

    [HttpPost("race/{raceId:int}")]
    public async Task<IActionResult> CreateSubrace(int raceId, [FromBody] CreateScifiSubraceDto request) {
        try {
            var subrace = await subraceService.CreateScifiSubraceAsync(raceId, request);
            return Ok(subrace);
        } catch (ConflictException ex) {
            return Conflict(new { message = ex.Message });
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSubrace(int id, [FromBody] UpdateScifiSubraceDto request) {
        try {
            var subrace = await subraceService.UpdateSubrace(id, request);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSubraceById(int id) {
        try {
            var subrace = await subraceService.GetSubraceById(id);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("by-race/{raceId:int}")]
    public async Task<IActionResult> GetSubracesByRace(int raceId) {
        try {
            var subraces = await subraceService.GetSubraceByRaceId(raceId);
            return Ok(subraces);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/stats")]
    public async Task<IActionResult> GetSubraceByIdWithStats(int id) {
        try {
            var subrace = await subraceService.GetSubraceByIdWithStats(id);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("by-race/{raceId:int}/stats")]
    public async Task<IActionResult> GetSubracesByRaceWithStats(int raceId) {
        try {
            var subrace = await subraceService.GetSubraceByRaceWithStats(raceId);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSubrace(int id) {
        try {
            await subraceService.DeleteSubraceAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }
}
