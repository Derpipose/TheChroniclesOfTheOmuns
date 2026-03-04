using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using ChroniclesAPI.Database;
using ChroniclesAPI.Services;

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
        var characters = await Task.Run(() => _db.Characters.ToList());
        return Ok(characters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Character>> GetCharacterById(int id) {
        var character = await _db.Characters.FindAsync(id);
        if (character == null) {
            return NotFound();
        }
        return Ok(character);
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
            character.Level = 1; // Default to level 1 if invalid level provided
        }

        Character verifiedCharacter = _characterService.BuildVerifiedCharacter(character);
        _db.Characters.Add(verifiedCharacter);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCharacterById), new { id = verifiedCharacter.Id }, verifiedCharacter);
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