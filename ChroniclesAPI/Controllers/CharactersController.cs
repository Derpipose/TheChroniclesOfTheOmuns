using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using ChroniclesAPI.Database;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharactersController : ControllerBase {
    private readonly AppDbContext _db;

    public CharactersController(AppDbContext db) {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Character>>> GetAllCharacters() {
        var characters = await Task.Run(() => _db.Characters.ToList());
        return Ok(characters);
    }

    [HttpPost]
    public async Task<ActionResult<Character>> CreateCharacter([FromBody] Character character) {
        _db.Characters.Add(character);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllCharacters), new { id = character.Id }, character);
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