using ChroniclesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChroniclesAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("request")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDto request)
    {
        var sent = await authService.SendOtpAsync(request.Email);
        
        // We return the same response whether the email exists or not
        // so attackers can't enumerate valid emails
        return Ok(new { message = "If that email is registered, a code has been sent." });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto request)
    {
        var token = await authService.VerifyOtpAsync(request.Email, request.Otp);
        if (token is null) return Unauthorized(new { message = "Invalid or expired code." });

        return Ok(new { token });
    }
}

public record OtpRequestDto(string Email);
public record OtpVerifyDto(string Email, string Otp);