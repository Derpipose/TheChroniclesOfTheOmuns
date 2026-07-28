
using System.Text.Json;
using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;
// using Resend;

namespace ChroniclesAPI.Services;

public class SciFiRaceService(AppDbContext db) {
    private static readonly JsonSerializerOptions SeedJsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };
    private async Task<bool> RaceHasSubracesAsync(int raceId) {
        return await db.ScifiSubraces.AnyAsync(s => s.ScifiRaceId == raceId);
    }



    private static CreateScifiRaceDto MapSeedRecordToCreateDto(ScifiRaceSeedRecord record) {
        var modifiers = new List<RaceStatModifierDto>();

        void AddIfNonZero(StatType stat, int value) {
            if (value != 0)
                modifiers.Add(new RaceStatModifierDto(stat, value, IsLocked: false));
        }

        AddIfNonZero(StatType.Strength, record.Str);
        AddIfNonZero(StatType.Dexterity, record.Dex);
        AddIfNonZero(StatType.Constitution, record.Con);
        AddIfNonZero(StatType.Wisdom, record.Wis);
        AddIfNonZero(StatType.Intelligence, record.Int);
        AddIfNonZero(StatType.Charisma, record.Cha);

        var raceStatsDto = new RaceStatsDto(modifiers, FreeStatPicks: ParseFreeStatPicks(record.Pick));

        return new CreateScifiRaceDto(
            SubType: record.SubType,
            Name: record.Name,
            Description: record.Description,
            IsStarterRace: string.Equals(record.Starter, "Yes", StringComparison.OrdinalIgnoreCase),
            RaceStatsId: 0, // ignored by ResolveRaceStatsIdAsync when RaceStats is provided
            RaceStats: raceStatsDto,
            Speed: record.Speed,
            AdventuringAgeStart: ParseIntOrDefault(record.AdventuringAgeStart),
            AgeMax: ParseIntOrDefault(record.AgeMax));
    }

    private static int ParseIntOrDefault(string value, int fallback = 0) =>
        int.TryParse(value, out var result) ? result : fallback;

    /// <summary>
    /// Parses the JSON "Pick" field into FreeStatPicks. Observed values:
    ///   ""         -> no free picks
    ///   "Subrace"  -> not a stat pick at all (subrace stats come from the
    ///                 subrace file instead); no free picks here
    ///   "1,2"      -> free stat picks at those indices, e.g. [1, 2]
    /// </summary>
    private static List<int> ParseFreeStatPicks(string pick) {
        if (string.IsNullOrWhiteSpace(pick) || pick.Equals("Subrace", StringComparison.OrdinalIgnoreCase))
            return new List<int>();

        return pick
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => int.TryParse(part, out var n) ? n : (int?)null)
            .Where(n => n.HasValue)
            .Select(n => n!.Value)
            .ToList();
    }

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
            request.AgeMax);

        await db.SaveChangesAsync();

        return race;
    }
    public async Task<List<ScifiRace>> GetAllRaces() {
        var races = await db.ScifiRaces.ToListAsync();
        foreach (var race in races) {
            race.SetHasSubraces(await RaceHasSubracesAsync(race.Id));
        }
        return races;
    }
    public async Task<ScifiRace> GetRaceById(int id) {
        var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException($"ScifiRace {id} was not found.");
        race.SetHasSubraces(await RaceHasSubracesAsync(id));
        return race;
    }
    public async Task<ScifiRace> GetRaceByIdWithStats(int id) {
        var race = await db.ScifiRaces
            .Include(r => r.RaceStats)
            .FirstOrDefaultAsync(r => r.Id == id) ?? throw new NotFoundException($"ScifiRace {id} was not found.");
        race.SetHasSubraces(await RaceHasSubracesAsync(id));

        return race;
    }
    public async Task<List<ScifiRace>> GetAllRacesWithStats() {
        var races = await db.ScifiRaces
            .Include(r => r.RaceStats)
            .ToListAsync();

        foreach (var race in races) {
            race.SetHasSubraces(await RaceHasSubracesAsync(race.Id));
        }
        return races;
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

public class ScifiRaceSeedRecord {
    public string SubType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Starter { get; set; } = string.Empty;
    public string Special { get; set; } = string.Empty;
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Con { get; set; }
    public int Wis { get; set; }
    public int Int { get; set; }
    public int Cha { get; set; }

    // "" -> no free stat picks. "Subrace" -> stats come from the subrace file
    // instead, not a pick here. "1,2" (etc.) -> parsed into FreeStatPicks by
    // the seeder as a List<int>.
    public string Pick { get; set; } = string.Empty;

    public int Speed { get; set; }
    public string AdventuringAgeStart { get; set; } = string.Empty;
    public string AgeMax { get; set; } = string.Empty;
    public string Pinterest_Inspo_Board { get; set; } = string.Empty;
}


public class ScifiSubraceSeedRecord {
    public string Race { get; set; } = string.Empty;
    public string SubRaceName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Special { get; set; } = string.Empty; // not currently mapped -- CreateScifiSubraceDto has no field for it
    public JsonElement Str { get; set; }
    public JsonElement Dex { get; set; }
    public JsonElement Con { get; set; }
    public JsonElement Wis { get; set; }
    public JsonElement Int { get; set; }
    public JsonElement Cha { get; set; }
    public JsonElement Pick { get; set; }

    // Present in the source file but unused: ScifiSubrace has no Speed
    // property and CreateScifiSubraceDto doesn't accept one (subraces
    // inherit Speed from their parent ScifiRace).
    public JsonElement Speed { get; set; }
    public string AdventuringAgeStart { get; set; } = string.Empty;
    public string AgeMax { get; set; } = string.Empty;
    public string Pinterest_Inspo_Board { get; set; } = string.Empty;
}


