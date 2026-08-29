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
public class ScifiRaceController(SciFiRaceService raceService, SciFiSubraceService subraceService) : ControllerBase {

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiRaceDto request) {
        try {
            var race = await raceService.CreateSciFiRaceAsync(request);
            return Ok(race);
        } catch (ConflictException ex) {
            return Conflict(new { message = ex.Message });
        }
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiRaceDto request) {
        try {
            var race = await raceService.UpdateRace(id, request);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // ---------- Getters ----------

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var races = await raceService.GetAllRaces();
        return Ok(races);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        try {
            var race = await raceService.GetRaceById(id);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("with-stats")]
    public async Task<IActionResult> GetAllWithStats() {
        var races = await raceService.GetAllRacesWithStats();
        return Ok(races);
    }

    [HttpGet("{id:int}/stats")]
    public async Task<IActionResult> GetByIdWithStats(int id) {
        try {
            var race = await raceService.GetRaceByIdWithStats(id);
            return Ok(race);
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    // ---------- Others ----------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        try {
            await subraceService.DeleteRaceAsync(id);
            return NoContent();
        } catch (NotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

}
