using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChroniclesAPI.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;
// using Resend;

namespace ChroniclesAPI.Services;

public class SciFiRaceService(AppDbContext db) {

    public async Task<ScifiRace> CreateSciFiRaceAsync(CreateScifiRaceDto request) {
        var testRace = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Name == request.Name);
        if (testRace is not null) {
            throw new ConflictException($"ScifiRace with name '{request.Name}' already exists.");
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

        return race;
    }
    public async Task<ScifiSubrace> CreateScifiSubraceAsync(int raceId, CreateScifiSubraceDto request) {
        var raceExists = await db.ScifiRaces.AnyAsync(r => r.Id == raceId);
        if (!raceExists)
            throw new NotFoundException($"ScifiRace {raceId} was not found.");

        var subraceExists = await db.ScifiSubraces
            .AnyAsync(s => s.ScifiRaceId == raceId && s.SubRaceName == request.SubRaceName);
        if (subraceExists)
            throw new ConflictException($"ScifiSubrace '{request.SubRaceName}' already exists for ScifiRace {raceId}.");

        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        var subrace = new ScifiSubrace(
            raceId,
            request.SubRaceName,
            request.Description,
            raceStatsId);

        db.ScifiSubraces.Add(subrace);
        await db.SaveChangesAsync();

        return subrace;
    }
    public async Task<ScifiRace> UpdateRace(int id, UpdateScifiRaceDto request) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException($"ScifiRace {id} was not found.");
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

        return race;
    }
    public async Task<ScifiSubrace> UpdateSubrace(int id, UpdateScifiSubraceDto request) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");
        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        subrace.Update(
            request.SubRaceName,
            request.Description,
            raceStatsId);

        await db.SaveChangesAsync();

        return subrace;
    }
    public async Task<List<ScifiRace>> GetAllRaces() {
        return await db.ScifiRaces.ToListAsync();
    }
    public async Task<ScifiRace> GetRaceById(int id) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException($"ScifiRace {id} was not found.");
        return race;
    }
    public async Task<ScifiRace> GetRaceByIdWithStats(int id) {
        var race = await db.ScifiRaces
            .Include(r => r.RaceStats)
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException($"ScifiRace {id} was not found.");

        return race;
    }
    public async Task<List<ScifiRace>> GetAllRacesWithStats() {
        return await db.ScifiRaces
            .Include(r => r.RaceStats)
            .ToListAsync();
    }
    public async Task<ScifiSubrace> GetSubraceById(int id) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");
        return subrace;
    }

    public async Task<ScifiSubrace> GetSubraceByRaceId(int raceId) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.ScifiRaceId == raceId) ?? throw new NotFoundException($"ScifiSubrace with RaceId {raceId} was not found.");
        return subrace;
    }
    public async Task<ScifiSubrace> GetSubraceByIdWithStats(int id) {
        var subrace = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");
        return subrace;
    }

    public async Task<ScifiSubrace> GetSubraceByRaceWithStats(int raceId) {
        var subrace = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstOrDefaultAsync(s => s.ScifiRaceId == raceId) ?? throw new NotFoundException($"ScifiSubrace with RaceId {raceId} was not found.");
        return subrace;
    }

    public async Task DeleteRaceAsync(int raceId) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == raceId) ?? throw new NotFoundException($"ScifiRace {raceId} was not found.");
        var subraces = await db.ScifiSubraces
            .Where(s => s.ScifiRaceId == raceId)
            .ToListAsync();

        var subraceIds = subraces.Select(s => s.Id).ToList();

        var affectedCharacters = await db.ScifiCharacters
            .Where(c => c.ScifiRaceId == raceId || (c.ScifiSubraceId != null && subraceIds.Contains(c.ScifiSubraceId.Value)))
            .ToListAsync();

        foreach (var character in affectedCharacters) {
            character.ClearScifiRace();
            character.ClearScifiSubrace();
        }

        db.ScifiSubraces.RemoveRange(subraces);
        db.ScifiRaces.Remove(race);
        await db.SaveChangesAsync();
    }

    public async Task DeleteSubraceAsync(int subraceId) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == subraceId) ?? throw new NotFoundException($"ScifiSubrace {subraceId} was not found.");

        var affectedCharacters = await db.ScifiCharacters
            .Where(c => c.ScifiSubraceId == subraceId)
            .ToListAsync();

        foreach (var character in affectedCharacters) {
            character.ClearScifiSubrace();
        }

        db.ScifiSubraces.Remove(subrace);
        await db.SaveChangesAsync();
    }

    public async Task<int> ResolveRaceStatsIdAsync(RaceStatsDto? statsDto, int fallbackId) {
        // This method resolves the ID of the race stats, creating new ones if necessary
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

public record CreateScifiSubraceDto(
    string SubRaceName,
    string Description,
    int RaceStatsId,
    RaceStatsDto? RaceStats);

public record UpdateScifiSubraceDto(
    string SubRaceName,
    string Description,
    int RaceStatsId,
    RaceStatsDto? RaceStats
);
