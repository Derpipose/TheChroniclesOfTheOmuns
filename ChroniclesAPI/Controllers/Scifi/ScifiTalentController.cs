using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/scifi/[controller]")]
[Authorize]
public class ScifiTalentController(AppDbContext db) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var talents = await db.ScifiTalents
            .OrderBy(t => t.Name)
            .Select(t => new ScifiTalentResponseDto(t.Id, t.Name, t.Description, t.Ability))
            .ToListAsync();

        return Ok(talents);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var talent = await db.ScifiTalents
            .Where(t => t.Id == id)
            .Select(t => new ScifiTalentResponseDto(t.Id, t.Name, t.Description, t.Ability))
            .FirstOrDefaultAsync();

        if (talent is null)
            return NotFound(new { message = $"ScifiTalent {id} was not found." });

        return Ok(talent);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScifiTalentDto request) {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Talent name is required." });

        var talent = new ScifiTalent(request.Name, request.Description, request.Ability);
        db.ScifiTalents.Add(talent);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = talent.Id },
            new ScifiTalentResponseDto(talent.Id, talent.Name, talent.Description, talent.Ability));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScifiTalentDto request) {
        var talent = await db.ScifiTalents.FirstOrDefaultAsync(t => t.Id == id);

        if (talent is null)
            return NotFound(new { message = $"ScifiTalent {id} was not found." });

        if (request.Name is not null) talent.SetName(request.Name);
        if (request.Description is not null) talent.SetDescription(request.Description);
        if (request.Ability is not null) talent.SetAbility(request.Ability);

        await db.SaveChangesAsync();
        return Ok(new ScifiTalentResponseDto(talent.Id, talent.Name, talent.Description, talent.Ability));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var talent = await db.ScifiTalents.FirstOrDefaultAsync(t => t.Id == id);

        if (talent is null)
            return NotFound(new { message = $"ScifiTalent {id} was not found." });

        db.ScifiTalents.Remove(talent);
        await db.SaveChangesAsync();
        return NoContent();
    }

    public record CreateScifiTalentDto(string Name, string Description, string Ability);
    public record UpdateScifiTalentDto(string? Name, string? Description, string? Ability);
    public record ScifiTalentResponseDto(int Id, string Name, string Description, string Ability);
}