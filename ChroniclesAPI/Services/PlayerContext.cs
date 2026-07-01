using System.Security.Claims;

namespace ChroniclesAPI.Services;

public class PlayerContext(IHttpContextAccessor httpContextAccessor)
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int PlayerId =>
        int.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User?.FindFirstValue("sub")
            ?? throw new InvalidOperationException("No player ID in token."));

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email)
            ?? User?.FindFirstValue("email")
            ?? throw new InvalidOperationException("No email in token.");

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}