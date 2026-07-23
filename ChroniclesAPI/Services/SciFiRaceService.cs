
using System.Text.Json;
using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;
// using Resend;

namespace ChroniclesAPI.Services;

public class SciFiRaceService(AppDbContext db, IHostEnvironment env) {
    private static readonly JsonSerializerOptions SeedJsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    public async Task SeedScifiRacesFromFileAsync(string fileName = "ScifiRaces.json") {
        var path = Path.Combine(env.ContentRootPath, "JsonSeeds", fileName);

        if (!File.Exists(path)) {
            throw new FileNotFoundException($"Scifi race seed file not found at {path}.");
        }

        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<ScifiRaceSeedRecord>>(json, SeedJsonOptions)
            ?? new List<ScifiRaceSeedRecord>();

        int created = 0, skipped = 0;

        foreach (var record in records) {
            var dto = MapSeedRecordToCreateDto(record);

            try {
                await CreateSciFiRaceAsync(dto);
                created++;
            } catch (ConflictException) {
                // Already seeded -- skip silently, this is expected on re-runs.
                skipped++;
            }
        }
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
            request.AgeMax);

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


    public async Task SeedScifiSubracesFromFileAsync(string fileName = "ScifiSubraces.json") {
        var path = Path.Combine(env.ContentRootPath, "JsonSeeds", fileName);

        if (!File.Exists(path)) {
            throw new FileNotFoundException($"Scifi subrace seed file not found at {path}.");
        }

        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<ScifiSubraceSeedRecord>>(json, SeedJsonOptions)
            ?? new List<ScifiSubraceSeedRecord>();

        int created = 0, skipped = 0;

        foreach (var record in records) {
            var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Name == record.Race);
            if (race is null) {
                throw new InvalidOperationException(
                    $"Cannot seed subrace '{record.SubRaceName}': parent race '{record.Race}' " +
                    "was not found. Run SeedScifiRacesFromFileAsync first.");
            }

            var dto = MapSubraceSeedRecordToCreateDto(record);

            try {
                await CreateScifiSubraceAsync(race.Id, dto);
                created++;
            } catch (ConflictException) {
                skipped++;
            }
        }
    }

    private static CreateScifiSubraceDto MapSubraceSeedRecordToCreateDto(ScifiSubraceSeedRecord record) {
        var modifiers = new List<RaceStatModifierDto>();

        void AddIfPresent(StatType stat, JsonElement element) {
            var parsed = ParseStatValue(element);
            if (parsed is { } value)
                modifiers.Add(new RaceStatModifierDto(stat, value.Modifier, value.IsLocked));
        }

        AddIfPresent(StatType.Strength, record.Str);
        AddIfPresent(StatType.Dexterity, record.Dex);
        AddIfPresent(StatType.Constitution, record.Con);
        AddIfPresent(StatType.Wisdom, record.Wis);
        AddIfPresent(StatType.Intelligence, record.Int);
        AddIfPresent(StatType.Charisma, record.Cha);

        var raceStatsDto = new RaceStatsDto(modifiers, FreeStatPicks: ParseFreeStatPicks(record.Pick));

        return new CreateScifiSubraceDto(
            SubRaceName: record.SubRaceName,
            Description: record.Description,
            RaceStatsId: 0, // ignored by ResolveRaceStatsIdAsync when RaceStats is provided
            RaceStats: raceStatsDto);
    }

    /// <summary>
    /// Parses a stat field that may be: blank (""), a bare number (a plain
    /// modifier), or "L{n}" (a locked absolute value, e.g. "L18" -> 18,
    /// IsLocked: true). Returns null when the field carries no stat.
    /// </summary>
    private static (int Modifier, bool IsLocked)? ParseStatValue(JsonElement element) {
        switch (element.ValueKind) {
            case JsonValueKind.Number:
                return (element.GetInt32(), false);

            case JsonValueKind.String: {
                    var raw = element.GetString();
                    if (string.IsNullOrWhiteSpace(raw))
                        return null;

                    if (raw.StartsWith('L') && int.TryParse(raw.AsSpan(1), out var locked))
                        return (locked, true);

                    if (int.TryParse(raw, out var plain))
                        return (plain, false);

                    return null; // unrecognized format -- treat as no stat rather than guess
                }

            default:
                return null; // null / missing / empty
        }
    }

    /// <summary>
    /// Parses a Pick field that may be: blank (""), a bare number (2),
    /// or a comma list ("1,2", "1,2,2,3") into FreeStatPicks.
    /// </summary>
    private static List<int> ParseFreeStatPicks(JsonElement element) {
        switch (element.ValueKind) {
            case JsonValueKind.Number:
                return new List<int> { element.GetInt32() };

            case JsonValueKind.String: {
                    var raw = element.GetString();
                    if (string.IsNullOrWhiteSpace(raw) || raw!.Equals("Subrace", StringComparison.OrdinalIgnoreCase))
                        return new List<int>();

                    return raw
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(part => int.TryParse(part, out var n) ? n : (int?)null)
                        .Where(n => n.HasValue)
                        .Select(n => n!.Value)
                        .ToList();
                }

            default:
                return new List<int>();
        }
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


