using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiRaceController(AppDbContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var races = await db.ScifiRaces
            .ToListAsync();

        return Ok(races);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var race = await db.ScifiRaces
            .FirstOrDefaultAsync(r => r.Id == id);

        if (race is null)
            return NotFound(new { message = $"ScifiRace {id} was not found." });

        return Ok(race);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiRaceDto request) {
        var testRace = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Name == request.Name);
        if (testRace is not null) {
            return Conflict(new { message = $"ScifiRace with name '{request.Name}' already exists." });
        }
        var race = new ScifiRace(
            request.SubType,
            request.Name,
            request.Description,
            request.IsStarterRace,
            request.RaceStatsId,
            request.Speed,
            request.AdventuringAgeStart,
            request.AgeMax);

        db.ScifiRaces.Add(race);
        await db.SaveChangesAsync();

        return Ok(race);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiRaceDto request) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == id);

        if (race is null)
            return NotFound(new { message = $"ScifiRace {id} was not found." });

        race.Update(
            request.SubType,
            request.Name,
            request.Description,
            request.IsStarterRace,
            request.RaceStatsId,
            request.Speed,
            request.AdventuringAgeStart,
            request.AgeMax,
            request.IsPlayable);

        await db.SaveChangesAsync();

        return Ok(race);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == id);

        if (race is null)
            return NotFound(new { message = $"ScifiRace {id} was not found." });

        db.ScifiRaces.Remove(race);
        await db.SaveChangesAsync();

        return NoContent();
    }

    public record UpdateScifiRaceDto(
        string SubType,
        string Name,
        string Description,
        bool IsStarterRace,
        int RaceStatsId,
        int Speed,
        int AdventuringAgeStart,
        int AgeMax,
        bool IsPlayable);

    public record CreateScifiRaceDto(
        string SubType,
        string Name,
        string Description,
        bool IsStarterRace,
        int RaceStatsId,
        int Speed,
        int AdventuringAgeStart,
        int AgeMax);
}
