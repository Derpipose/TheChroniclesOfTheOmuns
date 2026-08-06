using System.Text.Json;
using ChroniclesAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Database;
using PlayerApp.Models.Dice;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public class ScifiSeedingService(
    AppDbContext db,
    IHttpClientFactory httpClientFactory,
    SciFiRaceService raceService,
    SciFiSubraceService subraceService,
    ScifiWeaponService weaponService) {
    private const string DefaultTalentsUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiTalents.json";
    private const string DefaultClassesUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiClasses.json";
    private const string DefaultBackgroundsUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiBackgrounds.json";
    private const string DefaultRacesUrl = "http://derpipose.github.io/JsonFiles/ScifiFiles/ScifiRaces.json";
    private const string DefaultSubracesUrl = "http://derpipose.github.io/JsonFiles/ScifiFiles/ScifiSubRaces.json";
    private const string DefaultWeaponsUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiWeapons.json";

    public async Task<(int created, int updated, int total)> SeedTalentsAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultTalentsUrl;
        var seedData = await FetchSeedDataAsync<List<ScifiTalentSeedDto>>(url, "talent");

        if (seedData is null || seedData.Count == 0) {
            throw new InvalidOperationException("No talent data found at source.");
        }

        var existingTalents = await db.ScifiTalents.ToListAsync();
        int created = 0;
        int updated = 0;

        foreach (var entry in seedData) {
            if (string.IsNullOrWhiteSpace(entry.Talent))
                continue;

            var existing = existingTalents.FirstOrDefault(t => t.Name == entry.Talent);

            if (existing is not null) {
                existing.SetDescription(entry.Description);
                existing.SetAbility(entry.Ability);
                updated++;
            } else {
                db.ScifiTalents.Add(new ScifiTalent(entry.Talent, entry.Description, entry.Ability));
                created++;
            }
        }

        await db.SaveChangesAsync();
        return (created, updated, seedData.Count);
    }

    public async Task<(int created, int updated, int total, List<string> skippedClasses, List<string> skippedTalents)> SeedClassesAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultClassesUrl;
        var seedData = await FetchSeedDataAsync<List<ScifiClassSeedDto>>(url, "class");

        if (seedData is null || seedData.Count == 0) {
            throw new InvalidOperationException("No class data found at source.");
        }

        var existingClasses = await db.ScifiClasses.Include(c => c.ClassTalents).ToListAsync();
        var existingTalents = await db.ScifiTalents.ToListAsync();

        int created = 0;
        int updated = 0;
        var skippedClasses = new List<string>();
        var skippedTalents = new List<string>();
        var pendingLinks = new List<(ScifiClass Class, List<string> TalentNames)>();

        foreach (var entry in seedData) {
            if (string.IsNullOrWhiteSpace(entry.Class))
                continue;

            var diceType = ParseDiceType(entry.HitDice);
            if (diceType is null) {
                skippedClasses.Add($"{entry.Class} (unparseable hit dice '{entry.HitDice}')");
                continue;
            }

            var talentNames = new[] { entry.ClassTalent1, entry.ClassTalent2, entry.ClassTalent3 }
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .ToList();

            var existing = existingClasses.FirstOrDefault(c => c.Name == entry.Class);

            if (existing is not null) {
                existing.SetHitDice(diceType.Value);
                existing.SetDescription(entry.Description);

                db.RemoveRange(existing.ClassTalents);
                pendingLinks.Add((existing, talentNames));
                updated++;
            } else {
                var newClass = new ScifiClass(entry.Class, diceType.Value, entry.Description);
                db.ScifiClasses.Add(newClass);
                pendingLinks.Add((newClass, talentNames));
                created++;
            }
        }

        await db.SaveChangesAsync();

        foreach (var (scifiClass, talentNames) in pendingLinks) {
            foreach (var talentName in talentNames) {
                var talent = existingTalents.FirstOrDefault(t => t.Name == talentName);
                if (talent is null) {
                    skippedTalents.Add($"{scifiClass.Name} -> {talentName}");
                    continue;
                }

                db.Add(new ScifiClassTalent(scifiClass.Id, talent.Id));
            }
        }

        await db.SaveChangesAsync();
        return (created, updated, seedData.Count, skippedClasses, skippedTalents);
    }

    public async Task<(int created, int updated, int total, List<string> skippedTalents)> SeedBackgroundsAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultBackgroundsUrl;
        var seedData = await FetchSeedDataAsync<List<ScifiBackgroundSeedDto>>(url, "background");

        if (seedData is null || seedData.Count == 0) {
            throw new InvalidOperationException("No background data found at source.");
        }

        var existingBackgrounds = await db.ScifiBackgrounds.Include(b => b.BackgroundTalents).ToListAsync();
        var existingTalents = await db.ScifiTalents.ToListAsync();

        int created = 0;
        int updated = 0;
        var skippedTalents = new List<string>();
        var pendingLinks = new List<(ScifiBackground Background, List<string> TalentNames)>();

        foreach (var entry in seedData) {
            if (string.IsNullOrWhiteSpace(entry.Background))
                continue;

            var restricted = ParseYesNo(entry.Restricted);
            var hasOverlap = ParseYesNo(entry.ClassOverflow);
            var ownNames = new[] { entry.Talent1, entry.Talent2 }
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .ToList();

            var existing = existingBackgrounds.FirstOrDefault(b => b.Name == entry.Background);

            if (existing is not null) {
                existing.SetDescription(entry.Description);
                existing.SetRestricted(restricted);
                existing.SetHasClassOverlap(hasOverlap);

                db.RemoveRange(existing.BackgroundTalents);
                pendingLinks.Add((existing, ownNames));
                updated++;
            } else {
                var newBackground = new ScifiBackground(entry.Background, entry.Description, restricted, hasOverlap);
                db.ScifiBackgrounds.Add(newBackground);
                pendingLinks.Add((newBackground, ownNames));
                created++;
            }
        }

        await db.SaveChangesAsync();

        foreach (var (background, talentNames) in pendingLinks) {
            foreach (var talentName in talentNames) {
                var talent = existingTalents.FirstOrDefault(t => t.Name == talentName);
                if (talent is null) {
                    skippedTalents.Add($"{background.Name} -> {talentName}");
                    continue;
                }

                db.Add(new ScifiBackgroundTalent(background.Id, talent.Id));
            }
        }

        await db.SaveChangesAsync();
        return (created, updated, seedData.Count, skippedTalents);
    }

    public async Task<(int created, int skipped, int total)> SeedRacesAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultRacesUrl;
        var records = await FetchSeedDataAsync<List<ScifiRaceSeedRecord>>(url, "race");

        if (records is null || records.Count == 0) {
            throw new InvalidOperationException("No race data found at source.");
        }

        int created = 0;
        int skipped = 0;

        foreach (var record in records) {
            var dto = MapRaceSeedRecordToCreateDto(record);

            try {
                await raceService.CreateSciFiRaceAsync(dto);
                created++;
            } catch (ConflictException) {
                skipped++;
            }
        }

        return (created, skipped, records.Count);
    }

    public async Task<(int created, int skipped, int total)> SeedSubracesAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultSubracesUrl;
        var records = await FetchSeedDataAsync<List<ScifiSubraceSeedRecord>>(url, "subrace");

        if (records is null || records.Count == 0) {
            throw new InvalidOperationException("No subrace data found at source.");
        }

        int created = 0;
        int skipped = 0;

        foreach (var record in records) {
            var race = await db.ScifiRaces.FirstOrDefaultAsync(r => r.Name == record.Race);
            if (race is null) {
                throw new InvalidOperationException(
                    $"Cannot seed subrace '{record.SubRaceName}': parent race '{record.Race}' was not found.");
            }

            var dto = MapSubraceSeedRecordToCreateDto(record);

            try {
                await subraceService.CreateScifiSubraceAsync(race.Id, dto);
                created++;
            } catch (ConflictException) {
                skipped++;
            }
        }

        return (created, skipped, records.Count);
    }

    private async Task<T?> FetchSeedDataAsync<T>(string url, string kind) where T : class {
        try {
            var client = httpClientFactory.CreateClient();
            var data = await client.GetFromJsonAsync<T>(url);
            return data;
        } catch (Exception ex) {
            throw new InvalidOperationException($"Failed to fetch or parse {kind} seed data: {ex.Message}", ex);
        }
    }

    private static DiceType? ParseDiceType(string hitDiceRaw) {
        if (string.IsNullOrWhiteSpace(hitDiceRaw))
            return null;

        var parts = hitDiceRaw.Split('D', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[1], out var sides))
            return null;

        return Enum.IsDefined(typeof(DiceType), sides) ? (DiceType)sides : null;
    }

    private static bool ParseYesNo(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase);

    private static CreateScifiRaceDto MapRaceSeedRecordToCreateDto(ScifiRaceSeedRecord record) {
        var modifiers = new List<RaceStatModifierDto>();

        void AddIfNonZero(StatType stat, int value) {
            if (value != 0) {
                modifiers.Add(new RaceStatModifierDto(stat, value, IsLocked: false));
            }
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
            RaceStatsId: 0,
            RaceStats: raceStatsDto,
            Speed: record.Speed,
            AdventuringAgeStart: ParseIntOrDefault(record.AdventuringAgeStart),
            AgeMax: ParseIntOrDefault(record.AgeMax));
    }

    private static CreateScifiSubraceDto MapSubraceSeedRecordToCreateDto(ScifiSubraceSeedRecord record) {
        var modifiers = new List<RaceStatModifierDto>();

        void AddIfPresent(StatType stat, JsonElement element) {
            var parsed = ParseStatValue(element);
            if (parsed is { } value) {
                modifiers.Add(new RaceStatModifierDto(stat, value.Modifier, value.IsLocked));
            }
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
            RaceStatsId: 0,
            RaceStats: raceStatsDto);
    }

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

                    return null;
                }

            default:
                return null;
        }
    }

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

    // Add this method after SeedSubracesAsync in ScifiSeedingService

    public async Task<(int created, int skipped, int total)> SeedWeaponsAsync(string? sourceUrl = null) {
        var url = sourceUrl ?? DefaultWeaponsUrl;
        var records = await FetchSeedDataAsync<List<ScifiWeaponSeedRecord>>(url, "weapon");

        if (records is null || records.Count == 0) {
            throw new InvalidOperationException("No weapon data found at source.");
        }

        int created = 0;
        int skipped = 0;

        foreach (var record in records) {
            var dto = MapWeaponSeedRecordToCreateDto(record);

            try {
                await weaponService.CreateScifiWeaponAsync(dto);
                created++;
            } catch (InvalidOperationException) {
                // Weapon already exists
                skipped++;
            }
        }

        return (created, skipped, records.Count);
    }

    private static CreateScifiWeaponDto MapWeaponSeedRecordToCreateDto(ScifiWeaponSeedRecord record) {
        var ammoType = ParseAmmoType(record.AmmoType);
        var damageType = ParseDamageTypeFromString(record.DamageType);
        var rangeCategory = ParseWeaponRange(record.Range);

        return new CreateScifiWeaponDto {
            Name = record.Classification,
            DamageFormula = record.Damage,
            AmmoType = ammoType,
            DamageType = damageType,
            EnergyType = EnergyType.Physical,  // Default; can be enhanced with special traits parsing
            RateOfFire = record.RateOfFire.ToString() ?? string.Empty,
            AmmoCapacity = record.Capacity,
            CriticalRange = record.CriticalRange,
            CriticalMultiplier = record.CriticalMultiplier,
            ReloadTimeInActions = 1,  // Default; could be inferred from RateOfFire if needed
            RangeCategory = rangeCategory,
            CriticalFailRange = 1  // Default lower bound
        };
    }

    private static ScifiAmmoType ParseAmmoType(string ammoTypeRaw) {
        if (string.IsNullOrWhiteSpace(ammoTypeRaw))
            return ScifiAmmoType.None;

        // Normalize: "Greater Generic" → "GreaterGeneric", "Lesser Common" → "LesserCommon"
        var normalized = ammoTypeRaw
            .Replace(" ", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        // Try direct enum match
        if (Enum.TryParse<ScifiAmmoType>(normalized, ignoreCase: true, out var result)) {
            return result;
        }

        // Fallback to Generic
        return ScifiAmmoType.Generic;
    }

    private static DamageType ParseDamageTypeFromString(string damageTypeRaw) {
        if (string.IsNullOrWhiteSpace(damageTypeRaw))
            return DamageType.Ballistic;

        return damageTypeRaw.Trim() switch {
            "Kinetic" => DamageType.Ballistic,
            "Energy" => DamageType.Energy,
            "Melee" => DamageType.Melee,
            _ => DamageType.Ballistic  // Default for unknown
        };
    }

    private static WeaponRange ParseWeaponRange(string rangeRaw) {
        if (string.IsNullOrWhiteSpace(rangeRaw))
            return WeaponRange.None;

        return rangeRaw.Trim() switch {
            "Melee" => WeaponRange.Melee,
            "Short" => WeaponRange.Short,
            "Medium" => WeaponRange.Medium,
            "Long" => WeaponRange.Long,
            _ => WeaponRange.None
        };
    }

    // Add this record class at the end of the service



    private static int ParseIntOrDefault(string value, int fallback = 0) =>
        int.TryParse(value, out var result) ? result : fallback;

    public record ScifiBackgroundSeedDto(
        string Background,
        string Description,
        [property: System.Text.Json.Serialization.JsonPropertyName("Talent 1")] string? Talent1,
        [property: System.Text.Json.Serialization.JsonPropertyName("Talent 2")] string? Talent2,
        string ClassOverflow,
        string Restricted);

    public record ScifiTalentSeedDto(string Talent, string Description, string Ability);

    public record ScifiClassSeedDto(
        string Class,
        [property: System.Text.Json.Serialization.JsonPropertyName("Hit Dice")] string HitDice,
        string Description,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 1")] string? ClassTalent1,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 2")] string? ClassTalent2,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 3")] string? ClassTalent3);

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
        public string Special { get; set; } = string.Empty;
        public JsonElement Str { get; set; }
        public JsonElement Dex { get; set; }
        public JsonElement Con { get; set; }
        public JsonElement Wis { get; set; }
        public JsonElement Int { get; set; }
        public JsonElement Cha { get; set; }
        public JsonElement Pick { get; set; }
        public JsonElement Speed { get; set; }
        public string AdventuringAgeStart { get; set; } = string.Empty;
        public string AgeMax { get; set; } = string.Empty;
        public string Pinterest_Inspo_Board { get; set; } = string.Empty;
    }

    public class ScifiWeaponSeedRecord {
        public string Classification { get; set; } = string.Empty;
        public object RateOfFire { get; set; } = string.Empty;  // Can be "1D8" or 2
        public string Damage { get; set; } = string.Empty;
        public string DamageType { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public int CriticalRange { get; set; } = 20;
        public int CriticalMultiplier { get; set; } = 2;
        public string AmmoType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string SpecialTraits { get; set; } = string.Empty;
    }
}
