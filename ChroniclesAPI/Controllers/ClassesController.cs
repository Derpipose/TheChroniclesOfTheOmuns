using Microsoft.AspNetCore.Mvc;
using PlayerApp.Models;
using PlayerApp.Models.Verification;
using ChroniclesAPI.Database;
using ChroniclesAPI.Services;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ClassesController : ControllerBase {
    private readonly AppDbContext _db;

    public ClassesController(AppDbContext db) {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<CharacterClass>>> GetAllClasses() {
        var classes = await Task.Run(() => _db.CharacterClasses.ToList());
        return Ok(classes);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResult>> SyncClasses() {
        var syncService = new ClassSyncService(_db);
        var result = await syncService.SyncClassesAsync();
        return Ok(result);
    }
}