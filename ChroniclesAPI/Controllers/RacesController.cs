using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using ChroniclesAPI.Database;
using ChroniclesAPI.Services;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RacesController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly Services.CharacterService _characterService;

    public RacesController(AppDbContext db, Services.CharacterService characterService) {
        _db = db;
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CharacterRace>>> GetAllRaces() {
        var races = await _db.CharacterRaces.ToListAsync();
        return Ok(races);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterRace>> GetRaceById(int id) {
        var race = await _db.CharacterRaces
        .Include(r => r.Modifiers)
        .ThenInclude(m => m.Modifier)
        .FirstOrDefaultAsync(r => r.Id == id);
        
        if (race == null) {
            return NotFound();
        }   
        return Ok(race);
    }
}