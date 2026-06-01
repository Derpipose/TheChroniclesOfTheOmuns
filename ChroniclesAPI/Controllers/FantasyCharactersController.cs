using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using PlayerApp.Models.DTOs;
using ChroniclesAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/fantasy/[controller]")]
public class CharactersController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly Services.FantasyCharacterService _characterService;

    public CharactersController(AppDbContext db, Services.FantasyCharacterService characterService) {
        _db = db;
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FantasyCharacterDto>>> GetAllCharacters() {
        var characters = await _db.FantasyCharacters
            .Include(c => c.FantasyCharacterRace)
            .Include(c => c.FantasyCharacterClass)
            .ToListAsync();

        var result = characters.Select(character => new FantasyCharacterDto {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            Health = character.Health,
            Mana = character.Mana,
            FantasyCharacterRaceId = character.FantasyCharacterRace?.Id,
            FantasyCharacterRace = character.FantasyCharacterRace,
            FantasyCharacterClassId = character.FantasyCharacterClass?.Id,
            FantasyCharacterClass = character.FantasyCharacterClass,
            Stats = character.Stats,
            CharacterStatBonuses = character.CharacterStatBonuses
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FantasyCharacterDto>> GetCharacterById(int id) {
        var character = await _db.FantasyCharacters
            .Include(c => c.Stats)
            .Include(c => c.FantasyCharacterRace)
            .Include(c => c.FantasyCharacterClass)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (character == null) {
            return NotFound();
        }

        var dto = new FantasyCharacterDto {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            Health = character.Health,
            Mana = character.Mana,
            FantasyCharacterRaceId = character.FantasyCharacterRace?.Id,
            FantasyCharacterRace = character.FantasyCharacterRace,
            FantasyCharacterClassId = character.FantasyCharacterClass?.Id,
            FantasyCharacterClass = character.FantasyCharacterClass,
            Stats = character.Stats,
            CharacterStatBonuses = character.CharacterStatBonuses
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<FantasyCharacter>> CreateCharacter([FromBody] FantasyCharacterCreateDto createDto) {
        var character = new FantasyCharacter {
            Name = createDto.Name,
            Level = createDto.Level < 1 ? 1 : createDto.Level,
            Stats = createDto.Stats ?? new CharacterStats()
        };

        if (createDto.FantasyCharacterRaceId.HasValue) {
            var race = await _db.FantasyCharacterRaces.FirstOrDefaultAsync(r => r.Id == createDto.FantasyCharacterRaceId.Value);
            if (race == null) {
                return BadRequest("Invalid RaceId.");
            }

            character.AssignFantasyCharacterRace(race);
        }

        if (createDto.FantasyCharacterClassId.HasValue) {
            var FantasyCharacterClass = await _db.FantasyCharacterClasses.FirstOrDefaultAsync(c => c.Id == createDto.FantasyCharacterClassId.Value);
            if (FantasyCharacterClass == null) {
                return BadRequest("Invalid ClassId.");
            }

            character.AssignFantasyCharacterClass(FantasyCharacterClass);
        }

        if (!_characterService.VerifyCharacterDB(character, out var errorMessage)) {
            return BadRequest(errorMessage);
        }

        if (!string.IsNullOrEmpty(errorMessage)) {
            return BadRequest(errorMessage);
        }

        FantasyCharacter verifiedCharacter = _characterService.BuildVerifiedCharacter(character);
        _db.FantasyCharacters.Add(verifiedCharacter);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCharacterById), new { id = verifiedCharacter.Id }, verifiedCharacter);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] CharacterUpdateDto updateDto) {
        if (updateDto.Id != id) {
            return BadRequest("FantasyCharacter ID mismatch");
        }

        var existingCharacter = await _db.FantasyCharacters
            .Include(c => c.Stats)
            .Include(c => c.FantasyCharacterRace)
            .Include(c => c.FantasyCharacterClass)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (existingCharacter == null) {
            return NotFound();
        }

        var characterForVerification = new FantasyCharacter {
            Name = updateDto.Name,
            Level = updateDto.Level,
            Stats = updateDto.Stats ?? existingCharacter.Stats
        };

        if (updateDto.FantasyCharacterRaceId.HasValue) {
            var race = await _db.FantasyCharacterRaces.FirstOrDefaultAsync(r => r.Id == updateDto.FantasyCharacterRaceId.Value);
            if (race != null) {
                characterForVerification.AssignFantasyCharacterRace(race);
            }
        }

        if (updateDto.FantasyCharacterClassId.HasValue) {
            var FantasyCharacterClass = await _db.FantasyCharacterClasses.FirstOrDefaultAsync(c => c.Id == updateDto.FantasyCharacterClassId.Value);
            if (FantasyCharacterClass != null) {
                characterForVerification.AssignFantasyCharacterClass(FantasyCharacterClass);
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
            FantasyCharacterRaceId = existingCharacter.FantasyCharacterRace?.Id,
            FantasyCharacterClassId = existingCharacter.FantasyCharacterClass?.Id,
            Stats = existingCharacter.Stats
        };

        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacter(int id) {
        var character = await _db.FantasyCharacters.FindAsync(id);
        if (character == null) {
            return NotFound();
        }

        _db.FantasyCharacters.Remove(character);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}