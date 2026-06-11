using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using ChroniclesAPI.Services;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/fantasy/[controller]")]

public class ClassesController : ControllerBase {
    private readonly AppDbContext _db;

    public ClassesController(AppDbContext db) {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<FantasyCharacterClass>>> GetAllClasses() {
        var classes = await Task.Run(() => _db.FantasyCharacterClasses.ToList());
        return Ok(classes);
    }

    [HttpGet("non-veteran")]
    public async Task<ActionResult<List<FantasyCharacterClass>>> GetNonVeteranClasses() {
        var classes = await Task.Run(() => _db.FantasyCharacterClasses.Where(c => !c.IsVeteranLocked).ToList());
        return Ok(classes);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResult>> SyncClasses() {
        var syncService = new FantasyClassSyncService(_db);
        var result = await syncService.SyncClassesAsync();
        return Ok(result);
    }
}