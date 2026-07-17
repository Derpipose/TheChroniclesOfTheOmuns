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
public class ScifiClassController(AppDbContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var classes = await db.ScifiClasses
            .OrderBy(c => c.Name)
            .Select(c => new ScifiClassResponseDto(c.Id, c.Name, c.HitDice, c.Description))
            .ToListAsync();

        return Ok(classes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var scifiClass = await db.ScifiClasses
            .Where(c => c.Id == id)
            .Select(c => new ScifiClassResponseDto(c.Id, c.Name, c.HitDice, c.Description))
            .FirstOrDefaultAsync();

        if (scifiClass is null)
            return NotFound(new { message = $"ScifiClass {id} was not found." });

        return Ok(scifiClass);
    }

    [HttpGet("{id:int}/talents")]
    public async Task<IActionResult> GetTalents(int id) {
        var classExists = await db.ScifiClasses.AnyAsync(c => c.Id == id);
        if (!classExists)
            return NotFound(new { message = $"ScifiClass {id} was not found." });

        var talents = await db.ScifiClassTalents
            .Where(ct => ct.ScifiClassId == id)
            .Include(ct => ct.ScifiTalent)
            .Select(ct => new ScifiTalentResponseDto(
                ct.ScifiTalent!.Id,
                ct.ScifiTalent.Name,
                ct.ScifiTalent.Description,
                ct.ScifiTalent.Ability))
            .ToListAsync();

        return Ok(talents);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiClassDto request) {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Class name is required." });

        var scifiClass = new ScifiClass(request.Name, request.HitDice, request.Description ?? "In progress");
        db.ScifiClasses.Add(scifiClass);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = scifiClass.Id },
            new ScifiClassResponseDto(scifiClass.Id, scifiClass.Name, scifiClass.HitDice, scifiClass.Description));
    }

    [HttpPost("{id:int}/talents")]
    public async Task<IActionResult> AddTalent(int id, [FromBody] AddTalentDto request) {
        var classExists = await db.ScifiClasses.AnyAsync(c => c.Id == id);
        if (!classExists)
            return NotFound(new { message = $"ScifiClass {id} was not found." });

        var talentExists = await db.ScifiTalents.AnyAsync(t => t.Id == request.ScifiTalentId);
        if (!talentExists)
            return NotFound(new { message = $"ScifiTalent {request.ScifiTalentId} was not found." });

        var alreadyLinked = await db.ScifiClassTalents
            .AnyAsync(ct => ct.ScifiClassId == id && ct.ScifiTalentId == request.ScifiTalentId);
        if (alreadyLinked)
            return Conflict(new { message = "This talent is already linked to this class." });

        db.ScifiClassTalents.Add(new ScifiClassTalent(id, request.ScifiTalentId));
        await db.SaveChangesAsync();

        return Ok(new { message = "Talent linked to class." });
    }

    [HttpDelete("{id:int}/talents/{talentId:int}")]
    public async Task<IActionResult> RemoveTalent(int id, int talentId) {
        var link = await db.ScifiClassTalents
            .FirstOrDefaultAsync(ct => ct.ScifiClassId == id && ct.ScifiTalentId == talentId);

        if (link is null)
            return NotFound(new { message = "Talent link not found." });

        db.ScifiClassTalents.Remove(link);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiClassDto request) {
        var scifiClass = await db.ScifiClasses.FirstOrDefaultAsync(c => c.Id == id);

        if (scifiClass is null)
            return NotFound(new { message = $"ScifiClass {id} was not found." });

        if (request.Name is not null) scifiClass.SetName(request.Name);
        if (request.Description is not null) scifiClass.SetDescription(request.Description);
        if (request.HitDice is not null) scifiClass.SetHitDice(request.HitDice.Value);

        await db.SaveChangesAsync();
        return Ok(new ScifiClassResponseDto(scifiClass.Id, scifiClass.Name, scifiClass.HitDice, scifiClass.Description));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var scifiClass = await db.ScifiClasses.FirstOrDefaultAsync(c => c.Id == id);

        if (scifiClass is null)
            return NotFound(new { message = $"ScifiClass {id} was not found." });

        db.ScifiClasses.Remove(scifiClass);
        await db.SaveChangesAsync();
        return NoContent();
    }

    public record CreateScifiClassDto(string Name, DiceType HitDice, string? Description);
    public record UpdateScifiClassDto(string? Name, string? Description, DiceType? HitDice);
    public record ScifiClassResponseDto(int Id, string Name, DiceType HitDice, string Description);
    public record ScifiTalentResponseDto(int Id, string Name, string Description, string Ability);
    public record AddTalentDto(int ScifiTalentId);
}