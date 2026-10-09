using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using PlayerApp.Models;
using PlayerApp.Models.Database;
// using Resend;

namespace ChroniclesAPI.Services;

public class AuthService(AppDbContext db, IConfiguration config)// will need IResend resend added back in if we go back. 
{
    public async Task<bool> SendOtpAsync(string email)
    {
        var player = await db.Players.FirstOrDefaultAsync(p => p.Email == email);
        if (player is null) {
            // Console.WriteLine($"No player found for email: {email}");
            return false;
        }

        // Console.WriteLine($"Player found: {player.Id} - {player.Email}");

        // Invalidate any existing unused codes for this player
        var existing = await db.AuthCodes
            .Where(a => a.PlayerId == player.Id && !a.Used && a.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
        foreach (var old in existing) old.Used = true;

        // Generate a 6-digit OTP
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otp)));

        db.AuthCodes.Add(new AuthCode
        {
            PlayerId = player.Id,
            CodeHash = hash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });

        await db.SaveChangesAsync();
        // Console.WriteLine("OTP saved to DB");

        try {
            await SendEmailAsync(email, otp);
            // Console.WriteLine("Email sent successfully");
        }
        catch (Exception ex) {
            // Console.WriteLine($"Email send failed: {ex.Message}");
            return false;
        }

        return true;
    }

    private async Task SendEmailAsync(string toEmail, string otp)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(config["Smtp:From"]!));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your Chronicles login code";
        message.Body = new TextPart("html")
        {
            Text = $"<p>Your login code is: <strong>{otp}</strong></p><p>It expires in 10 minutes.</p>"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(config["Smtp:Host"]!, int.Parse(config["Smtp:Port"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(config["Smtp:Username"]!, config["Smtp:Password"]!);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    //     public async Task<bool> SendOtpAsync(string email)
    // {
    //     var player = await db.Players.FirstOrDefaultAsync(p => p.Email == email);
    //     if (player is null) return false;

    //     // Invalidate any existing unused codes for this player
    //     var existing = await db.AuthCodes
    //         .Where(a => a.PlayerId == player.Id && !a.Used && a.ExpiresAt > DateTime.UtcNow)
    //         .ToListAsync();
    //     foreach (var old in existing) old.Used = true;

    //     // Generate a 6-digit OTP
    //     var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    //     var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otp)));

    //     db.AuthCodes.Add(new AuthCode
    //     {
    //         PlayerId = player.Id,
    //         CodeHash = hash,
    //         ExpiresAt = DateTime.UtcNow.AddMinutes(10)
    //     });

    //     await db.SaveChangesAsync();

    //     // Send email
    //     var message = new EmailMessage
    //     {
    //         From = "onboarding@resend.dev",
    //         Subject = "Your Chronicles login code"
    //     };
    //     message.To.Add(email);
    //     message.HtmlBody = $"<p>Your login code is: <strong>{otp}</strong></p><p>It expires in 10 minutes.</p>";

    //     await resend.EmailSendAsync(message);
    //     return true;
    // }

    public async Task<string?> VerifyOtpAsync(string email, string otp)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otp)));

        var player = await db.Players.FirstOrDefaultAsync(p => p.Email == email);
        if (player is null) return null;

        var code = await db.AuthCodes.FirstOrDefaultAsync(a =>
            a.PlayerId == player.Id &&
            a.CodeHash == hash &&
            !a.Used &&
            a.ExpiresAt > DateTime.UtcNow);

        if (code is null) return null;

        code.Used = true;
        player.UpdateLastLogin();
        await db.SaveChangesAsync();

        return GenerateJwt(player);
    }

    private string GenerateJwt(Player player)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(double.Parse(config["Jwt:ExpiryHours"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, player.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}