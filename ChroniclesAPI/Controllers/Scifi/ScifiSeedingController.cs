using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Dice;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiSeedingController(AppDbContext db, IHttpClientFactory httpClientFactory) : ControllerBase {

    private const string DefaultTalentsUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiTalents.json";
    private const string DefaultClassesUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiClasses.json";
    private const string DefaultBackgroundsUrl = "https://derpipose.github.io/JsonFiles/ScifiFiles/ScifiBackgrounds.json";

    [HttpPost("talents")]
    public async Task<IActionResult> SeedTalents([FromQuery] string? sourceUrl) {
        var url = sourceUrl ?? DefaultTalentsUrl;

        List<ScifiTalentSeedDto>? seedData;
        try {
            var client = httpClientFactory.CreateClient();
            seedData = await client.GetFromJsonAsync<List<ScifiTalentSeedDto>>(url);
        } catch (Exception ex) {
            return BadRequest(new { message = $"Failed to fetch or parse talent seed data: {ex.Message}" });
        }

        if (seedData is null || seedData.Count == 0)
            return BadRequest(new { message = "No talent data found at source." });

        // Pull all existing talents once to avoid a query per entry.
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

        return Ok(new { created, updated, total = seedData.Count });
    }

    [HttpPost("classes")]
    public async Task<IActionResult> SeedClasses([FromQuery] string? sourceUrl) {
        var url = sourceUrl ?? DefaultClassesUrl;

        List<ScifiClassSeedDto>? seedData;
        try {
            var client = httpClientFactory.CreateClient();
            seedData = await client.GetFromJsonAsync<List<ScifiClassSeedDto>>(url);
        } catch (Exception ex) {
            return BadRequest(new { message = $"Failed to fetch or parse class seed data: {ex.Message}" });
        }

        if (seedData is null || seedData.Count == 0)
            return BadRequest(new { message = "No class data found at source." });

        var existingClasses = await db.ScifiClasses.Include(c => c.ClassTalents).ToListAsync();
        var existingTalents = await db.ScifiTalents.ToListAsync();

        int created = 0;
        int updated = 0;
        var skippedClasses = new List<string>();
        var skippedTalents = new List<string>();

        // Classes needing their talent links (re)built, resolved after IDs exist.
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

                db.RemoveRange(existing.ClassTalents); // rebuild links fresh below
                pendingLinks.Add((existing, talentNames));
                updated++;
            } else {
                var newClass = new ScifiClass(entry.Class, diceType.Value, entry.Description);
                db.ScifiClasses.Add(newClass);
                pendingLinks.Add((newClass, talentNames));
                created++;
            }
        }

        // Save now so new classes get real IDs and removed links are gone
        // before we try to insert new ScifiClassTalent rows referencing them.
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

        return Ok(new { created, updated, total = seedData.Count, skippedClasses, skippedTalents });
    }

    private static DiceType? ParseDiceType(string hitDiceRaw) {
        if (string.IsNullOrWhiteSpace(hitDiceRaw))
            return null;

        // "1D12" -> ["1", "12"]
        var parts = hitDiceRaw.Split('D', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[1], out var sides))
            return null;

        return Enum.IsDefined(typeof(DiceType), sides) ? (DiceType)sides : null;
    }

    [HttpPost("backgrounds")]
    public async Task<IActionResult> SeedBackgrounds([FromQuery] string? sourceUrl) {
        var url = sourceUrl ?? DefaultBackgroundsUrl;

        List<ScifiBackgroundSeedDto>? seedData;
        try {
            var client = httpClientFactory.CreateClient();
            seedData = await client.GetFromJsonAsync<List<ScifiBackgroundSeedDto>>(url);
        } catch (Exception ex) {
            return BadRequest(new { message = $"Failed to fetch or parse background seed data: {ex.Message}" });
        }

        if (seedData is null || seedData.Count == 0)
            return BadRequest(new { message = "No background data found at source." });

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

                db.RemoveRange(existing.BackgroundTalents); // rebuild links fresh below
                pendingLinks.Add((existing, ownNames));
                updated++;
            } else {
                var newBackground = new ScifiBackground(entry.Background, entry.Description, restricted, hasOverlap);
                db.ScifiBackgrounds.Add(newBackground);
                pendingLinks.Add((newBackground, ownNames));
                created++;
            }
        }

        // Save now so new backgrounds get real IDs and removed links are gone
        // before inserting new ScifiBackgroundTalent rows referencing them.
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

        return Ok(new { created, updated, total = seedData.Count, skippedTalents });
    }

    private static bool ParseYesNo(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase);

    public record ScifiBackgroundSeedDto(
        string Background,
        string Description,
        [property: System.Text.Json.Serialization.JsonPropertyName("Talent 1")] string? Talent1,
        [property: System.Text.Json.Serialization.JsonPropertyName("Talent 2")] string? Talent2,
        string ClassOverflow,
        string Restricted
    );


    public record ScifiTalentSeedDto(string Talent, string Description, string Ability);
    public record ScifiClassSeedDto(
        string Class,
        [property: System.Text.Json.Serialization.JsonPropertyName("Hit Dice")] string HitDice,
        string Description,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 1")] string? ClassTalent1,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 2")] string? ClassTalent2,
        [property: System.Text.Json.Serialization.JsonPropertyName("Class Talent 3")] string? ClassTalent3
    );
}