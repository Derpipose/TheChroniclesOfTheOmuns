using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using ChroniclesAPI.Database;
using ChroniclesAPI.Services;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/fantasy/[controller]")]
public class RacesController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly Services.FantasyCharacterService _characterService;

    public RacesController(AppDbContext db, Services.FantasyCharacterService characterService) {
        _db = db;
        _characterService = characterService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FantasyCharacterRace>>> GetAllRaces() {
        var races = await _db.FantasyCharacterRaces.ToListAsync();
        return Ok(races);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FantasyCharacterRace>> GetRaceById(int id) {
        var race = await _db.FantasyCharacterRaces
        .Include(r => r.Modifiers)
        .ThenInclude(m => m.Modifier)
        .FirstOrDefaultAsync(r => r.Id == id);
        
        if (race == null) {
            return NotFound();
        }   
        return Ok(race);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResult>> SyncRaces() {
        var syncService = new FantasyRaceSyncService(_db);
        var result = await syncService.SyncRacesAsync();
        return Ok(result);
    }
}