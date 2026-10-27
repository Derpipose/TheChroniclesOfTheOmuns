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
public class ScifiTalentController(AppDbContext db, PlayerContext playerContext) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        var talents = await db.ScifiTalents
            .OrderBy(t => t.Name)
            .Select(t => new ScifiTalentResponseDto(t.Id, t.Name, t.Description, t.Ability, t.Perk, t.PerkAmount))
            .ToListAsync();

        return Ok(talents);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var talent = await db.ScifiTalents
            .Where(t => t.Id == id)
            .Select(t => new ScifiTalentResponseDto(t.Id, t.Name, t.Description, t.Ability, t.Perk, t.PerkAmount))
            .FirstOrDefaultAsync();

        if (talent is null)
            return NotFound(new { message = $"ScifiTalent {id} was not found." });

        return Ok(talent);
    }

    [HttpGet("TalentPicks/{characterId:int}")]
    public async Task<IActionResult> GetTalentPicks(int characterId) {
        if (characterId <= 0)
            return BadRequest(new { message = "Invalid character ID." });

        var character = await db.ScifiCharacters
            .Include(sc => sc.ScifiClass)
            .Include(sc => sc.ScifiBackground)
            .Include(sc => sc.Talents)
            .FirstOrDefaultAsync(sc =>
                sc.CharacterId == characterId &&
                sc.Character!.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} was not found." });

        if (character.ScifiClass is null || character.ScifiBackground is null)
            return BadRequest(new { message = "Choose a class and background before selecting talents." });

        var scifiClass = character.ScifiClass;
        var scifiBackground = character.ScifiBackground;

        // The background pick the player already saved, if any
        int? selectedTalentId = character.Talents
            .FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background)
            ?.ScifiTalentId;

        // classes all class talents

        /*
        If (overflow){
        If(class != background){
        pick one talent from class talents }
        else (class == background){
        pick one talent from background talents }
        }
        */

        // Granted: all of the class's talents
        var classTalents = await db.ScifiClassTalents
            .Where(ct => ct.ScifiClassId == scifiClass.Id)
            .Select(ct => new ScifiTalentPicksResponseDto(
                ct.ScifiTalent!.Id, ct.ScifiTalent.Name, ct.ScifiTalent.Description,
                ct.ScifiTalent.Ability, ct.ScifiTalent.Perk, ct.ScifiTalent.PerkAmount,
                false, false))
            .ToListAsync();

        // Pool: depends on the overlap rule
        List<ScifiTalentPicksResponseDto> poolTalents;

        if (scifiBackground.HasClassOverlap && scifiClass.Name != scifiBackground.Name) {
            // Mismatch: pick from the class that shares the background's name
            var matchingClass = await db.ScifiClasses
                .FirstOrDefaultAsync(c => c.Name == scifiBackground.Name);

            if (matchingClass is null)
                return BadRequest(new { message = $"No class matches background '{scifiBackground.Name}'." });

            poolTalents = await db.ScifiClassTalents
                .Where(ct => ct.ScifiClassId == matchingClass.Id)
                .Select(ct => new ScifiTalentPicksResponseDto(
                    ct.ScifiTalent!.Id, ct.ScifiTalent.Name, ct.ScifiTalent.Description,
                    ct.ScifiTalent.Ability, ct.ScifiTalent.Perk, ct.ScifiTalent.PerkAmount,
                    true, ct.ScifiTalentId == selectedTalentId))
                .ToListAsync();
        } else {
            // No overlap, or class matches: pick from the background's own talents
            poolTalents = await db.ScifiBackgroundTalents
                .Where(bt => bt.ScifiBackgroundId == scifiBackground.Id)
                .Select(bt => new ScifiTalentPicksResponseDto(
                    bt.ScifiTalent!.Id, bt.ScifiTalent.Name, bt.ScifiTalent.Description,
                    bt.ScifiTalent.Ability, bt.ScifiTalent.Perk, bt.ScifiTalent.PerkAmount,
                    true, bt.ScifiTalentId == selectedTalentId))
                .ToListAsync();
        }

        // Granted first; skip pool talents the character already gets from the class
        var talentPicks = new List<ScifiTalentPicksResponseDto>(classTalents);
        foreach (var talent in poolTalents) {
            if (!talentPicks.Any(t => t.Id == talent.Id))
                talentPicks.Add(talent);
        }

        return Ok(talentPicks);
    }

    public record ScifiTalentResponseDto(int Id, string Name, string Description, string Ability, string Perk, string PerkAmount);
    public record ScifiTalentPicksResponseDto(int Id, string Name, string Description, string Ability, string Perk, string PerkAmount, bool IsPickable, bool IsSelected);
}