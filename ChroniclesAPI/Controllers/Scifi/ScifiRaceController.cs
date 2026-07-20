using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;
using ChroniclesAPI.Exceptions;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiRaceController(AppDbContext db, SciFiRaceService sfrc) : ControllerBase {

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiRaceDto request) {
        try {
            var race = await sfrc.CreateSciFiRaceAsync(request);
            return Ok(race);
        } catch (ConflictException ex) {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{raceId:int}/subrace")]
    public async Task<IActionResult> CreateSubrace(int raceId, [FromBody] CreateScifiSubraceDto request) {
        try {
            var subrace = await sfrc.CreateScifiSubraceAsync(raceId, request);
            return Ok(subrace);
        } catch (ConflictException ex) {
            return Conflict(new { message = ex.Message });
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiRaceDto request) {
        try {
            var race = await sfrc.UpdateRace(id, request);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("subrace/{id:int}")]
    public async Task<IActionResult> UpdateSubrace(int id, [FromBody] UpdateScifiSubraceDto request) {
        try {
            var subrace = await sfrc.UpdateSubrace(id, request);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // ---------- Getters ----------

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var races = await sfrc.GetAllRaces();
        return Ok(races);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        try {
            var race = await sfrc.GetRaceById(id);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("with-stats")]
    public async Task<IActionResult> GetAllWithStats() {
        var races = await sfrc.GetAllRacesWithStats();
        return Ok(races);
    }

    [HttpGet("subrace/{id:int}")]
    public async Task<IActionResult> GetSubraceById(int id) {
        try {
            var subrace = await sfrc.GetSubraceById(id);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("subrace/race/{raceId:int}")]
    public async Task<IActionResult> GetSubracesByRace(int raceId) {
        try {
            var subrace = await sfrc.GetSubraceByRaceId(raceId);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/stats")]
    public async Task<IActionResult> GetByIdWithStats(int id) {
        try {
            var race = await sfrc.GetRaceByIdWithStats(id);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("subrace/{id:int}/stats")]
    public async Task<IActionResult> GetSubraceByIdWithStats(int id) {
        try {
            var subrace = await sfrc.GetSubraceByIdWithStats(id);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("subrace/race/{raceId:int}/stats")]
    public async Task<IActionResult> GetSubracesByRaceWithStats(int raceId) {
        try {
            var subrace = await sfrc.GetSubraceByRaceWithStats(raceId);
            return Ok(subrace);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // ---------- Others ----------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        try {
            await sfrc.DeleteRaceAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("subrace/{id:int}")]
    public async Task<IActionResult> DeleteSubrace(int id) {
        try {
            await sfrc.DeleteSubraceAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }
}
