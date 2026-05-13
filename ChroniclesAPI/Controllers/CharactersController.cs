using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using PlayerApp.Models.DTOs;
using ChroniclesAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly Services.CharacterService _characterService;

    public CharactersController(AppDbContext db, Services.CharacterService characterService) {
        _db = db;
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Character>>> GetAllCharacters() {
        var characters = await _db.Characters
            .Include(c => c.CharacterRace)
            .Include(c => c.CharacterClass)
            .ToListAsync();
        return Ok(characters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterDto>> GetCharacterById(int id) {
        var character = await _db.Characters
            .Include(c => c.Stats)
            .Include(c => c.CharacterRace)
            .Include(c => c.CharacterClass)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (character == null) {
            return NotFound();
        }

        var dto = new CharacterDto {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            Health = character.Health,
            Mana = character.Mana,
            CharacterRaceId = character.CharacterRace?.Id,
            CharacterRace = character.CharacterRace,
            CharacterClassId = character.CharacterClass?.Id,
            CharacterClass = character.CharacterClass,
            Stats = character.Stats,
            CharacterStatBonuses = character.CharacterStatBonuses
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<Character>> CreateCharacter([FromBody] Character character) {
        if (!_characterService.VerifyCharacterDB(character, out var errorMessage)) {
            return BadRequest(errorMessage);
        }
        if (!string.IsNullOrEmpty(errorMessage)) {
            return BadRequest(errorMessage);
        }
        if (character.Level < 1) {
            character.Level = 1; 
        }

        Character verifiedCharacter = _characterService.BuildVerifiedCharacter(character);
        _db.Characters.Add(verifiedCharacter);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCharacterById), new { id = verifiedCharacter.Id }, verifiedCharacter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] CharacterUpdateDto updateDto) {
        if (updateDto.Id != id) {
            return BadRequest("Character ID mismatch");
        }

        var existingCharacter = await _db.Characters
            .Include(c => c.Stats)
            .Include(c => c.CharacterRace)
            .Include(c => c.CharacterClass)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCharacter == null) {
            return NotFound();
        }

        var characterForVerification = new Character {
            Name = updateDto.Name,
            Level = updateDto.Level,
            Stats = updateDto.Stats ?? existingCharacter.Stats
        };

        if (updateDto.CharacterRaceId.HasValue) {
            var race = await _db.CharacterRaces.FirstOrDefaultAsync(r => r.Id == updateDto.CharacterRaceId.Value);
            if (race != null) {
                characterForVerification.AssignCharacterRace(race);
            }
        }

        if (updateDto.CharacterClassId.HasValue) {
            var characterClass = await _db.CharacterClasses.FirstOrDefaultAsync(c => c.Id == updateDto.CharacterClassId.Value);
            if (characterClass != null) {
                characterForVerification.AssignCharacterClass(characterClass);
            }
        }

        if (!_characterService.VerifyCharacterDB(characterForVerification, out var errorMessage)) {
            return BadRequest(errorMessage);
        }

        _characterService.UpdateCharacter(existingCharacter, characterForVerification);
        await _db.SaveChangesAsync();

        var resultDto = new CharacterUpdateDto {
            Id = existingCharacter.Id,
            Name = existingCharacter.Name,
            Level = existingCharacter.Level,
            CharacterRaceId = existingCharacter.CharacterRace?.Id,
            CharacterClassId = existingCharacter.CharacterClass?.Id,
            Stats = existingCharacter.Stats
        };

        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacter(int id) {
        var character = await _db.Characters.FindAsync(id);
        if (character == null) {
            return NotFound();
        }

        _db.Characters.Remove(character);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}