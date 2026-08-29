using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public class SciFiSubraceService(AppDbContext db) {
    public async Task<ScifiSubrace> CreateScifiSubraceAsync(int raceId, CreateScifiSubraceDto request) {
        var raceExists = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == raceId);
        if (raceExists == null) {
            throw new NotFoundException($"ScifiRace {raceId} was not found.");
        }

        var subraceExists = await db.ScifiSubraces
            .AnyAsync(s => s.ScifiRaceId == raceId && s.SubRaceName == request.SubRaceName);
        if (subraceExists) {
            throw new ConflictException($"ScifiSubrace '{request.SubRaceName}' already exists for ScifiRace {raceExists.Name}.");
        }

        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        var subrace = new ScifiSubrace(
            raceId,
            request.SubRaceName,
            request.Description,
            raceStatsId);

        db.ScifiSubraces.Add(subrace);
        if (raceExists.HasSubraces == false) {
            raceExists.SetHasSubraces(true);
            var raceStats = await db.RaceStats
                .Where(rs => !rs.Modifiers.Any() && !rs.FreeStatPicks.Any())
                .FirstOrDefaultAsync();

            raceExists.SetRaceStats(raceStats ?? new RaceStats(new List<RaceStatModifier>(), new List<int>())); 

        }
        await db.SaveChangesAsync();

        return subrace;
    }

    public async Task<ScifiSubrace> UpdateSubrace(int id, UpdateScifiSubraceDto request) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");

        var raceStatsId = await ResolveRaceStatsIdAsync(request.RaceStats, request.RaceStatsId);

        subrace.Update(
            request.SubRaceName,
            request.Description,
            raceStatsId);

        await db.SaveChangesAsync();

        return subrace;
    }

    public async Task<ScifiSubrace> GetSubraceById(int id) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");

        return subrace;
    }

    public async Task<List<ScifiSubrace>> GetSubraceByRaceId(int raceId) {
        var subraces = await db.ScifiSubraces
            .Where(s => s.ScifiRaceId == raceId)
            .ToListAsync();

        if (subraces.Count == 0) {
            throw new NotFoundException($"ScifiSubrace with RaceId {raceId} was not found.");
        }

        return subraces;
    }

    public async Task<ScifiSubrace> GetSubraceByIdWithStats(int id) {
        var subrace = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"ScifiSubrace {id} was not found.");

        return subrace;
    }

    public async Task<List<ScifiSubrace>> GetSubraceByRaceWithStats(int raceId) {
        var subrace = await db.ScifiSubraces
            .Include(s => s.RaceStats)
            .Where(s => s.ScifiRaceId == raceId)
            .ToListAsync();

        return subrace;
    }

    public async Task DeleteSubraceAsync(int subraceId) {
        var subrace = await db.ScifiSubraces.FirstOrDefaultAsync(s => s.Id == subraceId)
            ?? throw new NotFoundException($"ScifiSubrace {subraceId} was not found.");

        var affectedCharacters = await db.ScifiCharacters
            .Where(c => c.ScifiSubraceId == subraceId)
            .ToListAsync();

        foreach (var character in affectedCharacters) {
            character.ClearScifiSubrace();
        }

        db.ScifiSubraces.Remove(subrace);
        await db.SaveChangesAsync();
    }

    public async Task DeleteRaceAsync(int raceId) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == raceId)
            ?? throw new NotFoundException($"ScifiRace {raceId} was not found.");

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

    private async Task<int> ResolveRaceStatsIdAsync(RaceStatsDto? statsDto, int fallbackId) {
        if (statsDto is null) {
            return fallbackId;
        }

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

        if (match is not null) {
            return match.Id;
        }

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
    int RaceStatsId,
    RaceStatsDto? RaceStats,
    int Speed,
    int AdventuringAgeStart,
    int AgeMax);

public record RaceStatModifierDto(StatType Stat, int Modifier, bool IsLocked);

public record RaceStatsDto(
    List<RaceStatModifierDto>? Modifiers,
    List<int>? FreeStatPicks);

public record CreateScifiRaceDto(
    string SubType,
    string Name,
    string Description,
    bool IsStarterRace,
    int RaceStatsId,
    RaceStatsDto? RaceStats,
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
    RaceStatsDto? RaceStats);
