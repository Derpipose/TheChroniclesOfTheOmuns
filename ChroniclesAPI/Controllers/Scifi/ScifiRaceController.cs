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

        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        var race = new ScifiRace(
            request.SubType,
            request.Name,
            request.Description,
            request.IsStarterRace,
            raceStatsId,
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

        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        race.Update(
            request.SubType,
            request.Name,
            request.Description,
            request.IsStarterRace,
            raceStatsId,
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

    [HttpGet("{id:int}/stats")]
    public async Task<IActionResult> GetByIdWithStats(int id) {
        var race = await db.ScifiRaces
            .Include(r => r.RaceStats)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (race is null)
            return NotFound(new { message = $"ScifiRace {id} was not found." });

        return Ok(race);
    }

    [HttpGet("with-stats")]
    public async Task<IActionResult> GetAllWithStats() {
        var races = await db.ScifiRaces
            .Include(r => r.RaceStats)
            .ToListAsync();

        return Ok(races);
    }

    public record UpdateScifiRaceDto(
        string SubType,
        string Name,
        string Description,
        bool IsStarterRace,
        int RaceStatsId,          // used if RaceStats is null
        RaceStatsDto? RaceStats,  // used if provided
        int Speed,
        int AdventuringAgeStart,
        int AgeMax,
        bool IsPlayable);

    public record RaceStatModifierDto(StatType Stat, int Modifier, bool IsLocked);

    public record RaceStatsDto(
        List<RaceStatModifierDto>? Modifiers,
        List<int>? FreeStatPicks);

    public record CreateScifiRaceDto(
        string SubType,
        string Name,
        string Description,
        bool IsStarterRace,
        int RaceStatsId,          // used if RaceStats is null
        RaceStatsDto? RaceStats,  // used if provided
        int Speed,
        int AdventuringAgeStart,
        int AgeMax);


    private async Task<int> ResolveRaceStatsIdAsync(RaceStatsDto? statsDto, int fallbackId) {
        if (statsDto is null)
            return fallbackId;

        var newModifiers = (statsDto.Modifiers ?? new List<RaceStatModifierDto>())
            .Select(m => (m.Stat, m.Modifier, m.IsLocked))
            .OrderBy(m => m.Stat)
            .ToList();

        var newFreePicks = (statsDto.FreeStatPicks ?? new List<int>())
            .OrderBy(x => x)
            .ToList();

        var existingStats = await db.RaceStats.ToListAsync();

        var match = existingStats.FirstOrDefault(rs => {
            var existingModifiers = (rs.Modifiers ?? new List<RaceStatModifier>())
                .Select(m => (m.Stat, m.Modifier, m.IsLocked))
                .OrderBy(m => m.Stat)
                .ToList();

            var existingFreePicks = (rs.FreeStatPicks ?? new List<int>())
                .OrderBy(x => x)
                .ToList();

            return existingModifiers.SequenceEqual(newModifiers)
                && existingFreePicks.SequenceEqual(newFreePicks);
        });

        if (match is not null)
            return match.Id;

        var modifiers = (statsDto.Modifiers ?? new List<RaceStatModifierDto>())
            .Select(m => new RaceStatModifier(m.Stat, m.Modifier, m.IsLocked))
            .ToList();

        var newStats = new RaceStats(modifiers, statsDto.FreeStatPicks);
        db.RaceStats.Add(newStats);
        await db.SaveChangesAsync();

        return newStats.Id;
    }



}

