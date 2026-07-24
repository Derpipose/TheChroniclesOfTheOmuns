using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("players")]
public class PlayerController(AppDbContext db) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterPlayerDto request)
    {
        var player = new Player(request.DisplayName, request.Email);

        db.Players.Add(player);

        try {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException) {
            return Conflict(new { message = "An account with that email already exists." });
        }

        return Ok(new { message = "Account created. You can now request a login code." });
    }
}

public record RegisterPlayerDto(string DisplayName, string Email);