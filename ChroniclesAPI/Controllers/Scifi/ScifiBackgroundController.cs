using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiBackgroundController(AppDbContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var backgrounds = await db.ScifiBackgrounds
            .OrderBy(b => b.Name)
            .Select(b => new ScifiBackgroundResponseDto(
                b.Id, b.Name, b.Description, b.Restricted, b.HasClassOverlap))
            .ToListAsync();

        return Ok(backgrounds);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var background = await db.ScifiBackgrounds
            .Where(b => b.Id == id)
            .Select(b => new ScifiBackgroundResponseDto(
                b.Id, b.Name, b.Description, b.Restricted, b.HasClassOverlap))
            .FirstOrDefaultAsync();

        if (background is null)
            return NotFound(new { message = $"ScifiBackground {id} was not found." });

        return Ok(background);
    }

    [HttpGet("{id:int}/talents")]
    public async Task<IActionResult> GetTalents(int id) {
        var backgroundExists = await db.ScifiBackgrounds.AnyAsync(b => b.Id == id);
        if (!backgroundExists)
            return NotFound(new { message = $"ScifiBackground {id} was not found." });

        var talents = await db.ScifiBackgroundTalents
            .Where(bt => bt.ScifiBackgroundId == id)
            .Include(bt => bt.ScifiTalent)
            .Select(bt => new ScifiTalentResponseDto(
                bt.ScifiTalent!.Id,
                bt.ScifiTalent.Name,
                bt.ScifiTalent.Description,
                bt.ScifiTalent.Ability))
            .ToListAsync();

        return Ok(talents);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiBackgroundDto request) {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Background name is required." });

        var background = new ScifiBackground(
            request.Name,
            request.Description ?? string.Empty,
            request.Restricted ?? false,
            request.HasClassOverlap ?? false);

        db.ScifiBackgrounds.Add(background);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = background.Id },
            new ScifiBackgroundResponseDto(
                background.Id, background.Name, background.Description,
                background.Restricted, background.HasClassOverlap));
    }

    [HttpPost("{id:int}/talents")]
    public async Task<IActionResult> AddTalent(int id, [FromBody] AddTalentDto request) {
        var backgroundExists = await db.ScifiBackgrounds.AnyAsync(b => b.Id == id);
        if (!backgroundExists)
            return NotFound(new { message = $"ScifiBackground {id} was not found." });

        var talentExists = await db.ScifiTalents.AnyAsync(t => t.Id == request.ScifiTalentId);
        if (!talentExists)
            return NotFound(new { message = $"ScifiTalent {request.ScifiTalentId} was not found." });

        var alreadyLinked = await db.ScifiBackgroundTalents
            .AnyAsync(bt => bt.ScifiBackgroundId == id && bt.ScifiTalentId == request.ScifiTalentId);
        if (alreadyLinked)
            return Conflict(new { message = "This talent is already linked to this background." });

        db.ScifiBackgroundTalents.Add(new ScifiBackgroundTalent(id, request.ScifiTalentId));
        await db.SaveChangesAsync();

        return Ok(new { message = "Talent linked to background." });
    }

    [HttpDelete("{id:int}/talents/{talentId:int}")]
    public async Task<IActionResult> RemoveTalent(int id, int talentId) {
        var link = await db.ScifiBackgroundTalents
            .FirstOrDefaultAsync(bt => bt.ScifiBackgroundId == id && bt.ScifiTalentId == talentId);

        if (link is null)
            return NotFound(new { message = "Talent link not found." });

        db.ScifiBackgroundTalents.Remove(link);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiBackgroundDto request) {
        var background = await db.ScifiBackgrounds.FirstOrDefaultAsync(b => b.Id == id);

        if (background is null)
            return NotFound(new { message = $"ScifiBackground {id} was not found." });

        if (request.Name is not null) background.SetName(request.Name);
        if (request.Description is not null) background.SetDescription(request.Description);
        if (request.Restricted is not null) background.SetRestricted(request.Restricted.Value);
        if (request.HasClassOverlap is not null) background.SetHasClassOverlap(request.HasClassOverlap.Value);

        await db.SaveChangesAsync();
        return Ok(new ScifiBackgroundResponseDto(
            background.Id, background.Name, background.Description,
            background.Restricted, background.HasClassOverlap));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var background = await db.ScifiBackgrounds.FirstOrDefaultAsync(b => b.Id == id);

        if (background is null)
            return NotFound(new { message = $"ScifiBackground {id} was not found." });

        db.ScifiBackgrounds.Remove(background);
        await db.SaveChangesAsync();
        return NoContent();
    }

    public record CreateScifiBackgroundDto(string Name, string? Description, bool? Restricted, bool? HasClassOverlap);
    public record UpdateScifiBackgroundDto(string? Name, string? Description, bool? Restricted, bool? HasClassOverlap);
    public record ScifiBackgroundResponseDto(int Id, string Name, string Description, bool Restricted, bool HasClassOverlap);
    public record ScifiTalentResponseDto(int Id, string Name, string Description, string Ability);
    public record AddTalentDto(int ScifiTalentId);
}