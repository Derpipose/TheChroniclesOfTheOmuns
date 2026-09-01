using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Database;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerController(AppDbContext db, PlayerContext playerContext) : ControllerBase
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

    [Authorize]
    [HttpPost("theme")]
    public async Task<IActionResult> SetTheme([FromBody] SetThemeDto request)
    {
        var player = await db.Players.FindAsync(playerContext.PlayerId);
        if (player == null) {
            return NotFound(new { message = "Player not found." });
        }

        player.SetTheme(request.Theme);

        try {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException) {
            return BadRequest(new { message = "Failed to update player theme." });
        }

        return Ok(new { message = "Player theme updated." });
    }

    [Authorize]
    [HttpGet("details")]
    public async Task<ActionResult<PlayerDetailsDto>> GetPlayerDetails()
    {
        var player = await db.Players.FindAsync(playerContext.PlayerId);
        if (player == null) {
            return NotFound(new { message = "Player not found." });
        }

        return Ok(new PlayerDetailsDto(player.Id, player.DisplayName, player.Email, player.Theme));
    }
}

public record RegisterPlayerDto(string DisplayName, string Email);
public record SetThemeDto(ThemeEnum Theme);
public record PlayerDetailsDto(int Id, string DisplayName, string Email, ThemeEnum Theme);