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
public class ScifiTalentController(AppDbContext db, PlayerContext playerContext, ScifiTalentService scifiTalentService) : ControllerBase {

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

    [HttpGet("PreviewTalentPool/{classId:int}/{backgroundId:int}")]
    public async Task<IActionResult> PreviewTalentPool(int classId, int backgroundId) {
        if (classId <= 0 || backgroundId <= 0)
            return BadRequest(new { message = "Invalid class or background ID." });

        var scifiClass = await db.ScifiClasses
            .FirstOrDefaultAsync(c => c.Id == classId);

        if (scifiClass is null)
            return NotFound(new { message = $"ScifiClass {classId} was not found." });

        var scifiBackground = await db.ScifiBackgrounds
            .FirstOrDefaultAsync(b => b.Id == backgroundId);

        if (scifiBackground is null)
            return NotFound(new { message = $"ScifiBackground {backgroundId} was not found." });

        var (classTalents, poolTalents) = await scifiTalentService.GetTalentPoolAsync(scifiClass, scifiBackground);

        var granted = classTalents.Select(t => 
                    new ScifiTalentPickPreviewDto(
                        t.Id, 
                        t.Name, 
                        t.Description, 
                        t.Ability, 
                        t.Perk, 
                        t.PerkAmount,
                        false
                    )).ToList();

        var pickable = poolTalents.Select(t => 
                    new ScifiTalentPickPreviewDto(
                        t.Id, 
                        t.Name, 
                        t.Description, 
                        t.Ability, 
                        t.Perk, 
                        t.PerkAmount,
                        true
                    )).ToList();

        // Put the two together. Yay!
        var talentPicks = granted.Concat(pickable).ToList();

        return Ok(talentPicks);
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

        var (classTalents, poolTalents) = await scifiTalentService.GetTalentPoolAsync(character);

        var granted = classTalents.Select(t => 
                    new ScifiTalentPicksResponseDto(
                        t.Id, 
                        t.Name, 
                        t.Description, 
                        t.Ability, 
                        t.Perk, 
                        t.PerkAmount,
                        false, 
                        false
                    )).ToList();

        int selectedTalentId = character.Talents.FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background)?.ScifiTalentId ?? 0;

        var pickable = poolTalents.Select(t => 
                    new ScifiTalentPicksResponseDto(
                        t.Id, 
                        t.Name, 
                        t.Description, 
                        t.Ability, 
                        t.Perk, 
                        t.PerkAmount,
                        true, 
                        t.Id == selectedTalentId
                    )).ToList();

        // Put the two together. Yay!
        var talentPicks = granted.Concat(pickable).ToList();

        return Ok(talentPicks);
    }

    [HttpPost("TalentPicks/{characterId:int}/{talentId:int}")]
    public async Task<IActionResult> SelectTalentPick(int characterId, int talentId) {
        if (characterId <= 0 || talentId <= 0)
            return BadRequest(new { message = "Invalid character ID or talent ID." });

        var character = await db.ScifiCharacters
            .Include(sc => sc.ScifiClass)
            .Include(sc => sc.ScifiBackground)
            .Include(sc => sc.Talents)
            .Include(sc => sc.Character)
            .FirstOrDefaultAsync(sc =>
                sc.CharacterId == characterId &&
                sc.Character!.PlayerId == playerContext.PlayerId);

        if (character is null)
            return NotFound(new { message = $"Character {characterId} was not found." });

        if(character.Character!.IsActive)
            return Conflict(new { message = "Cannot select talents for an active character." });

        if (character.ScifiClass is null || character.ScifiBackground is null)
            return BadRequest(new { message = "Choose a class and background before selecting talents." });
        
        var characterTalent = character.Talents.FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background);
        // Player picked the same thing they had before. Or a save was triggered. Either way, we are fine to just return. 
        if (talentId == characterTalent?.ScifiTalentId)
            return Ok();

        var pool = await scifiTalentService.GetTalentPoolAsync(character);

        var talent = pool.Pickable.FirstOrDefault(t => t.Id == talentId);
        if (talent is null)
            return BadRequest(new { message = "That talent isn't one of this character's background options." });
        
        // Clear any existing background talent
        character.ClearTalentsBySource(TalentSourceEnum.Background);

        // Add the new talent
        character.AddTalent(talent, TalentSourceEnum.Background);
        await db.SaveChangesAsync();
        return Ok();
    }

    

    public record ScifiTalentResponseDto(int Id, string Name, string Description, string Ability, string Perk, string PerkAmount);
    public record ScifiTalentPicksResponseDto(int Id, string Name, string Description, string Ability, string Perk, string PerkAmount, bool IsPickable, bool IsSelected);
    public record ScifiTalentPickPreviewDto(int Id, string Name, string Description, string Ability, string Perk, string PerkAmount, bool IsPickable);
}