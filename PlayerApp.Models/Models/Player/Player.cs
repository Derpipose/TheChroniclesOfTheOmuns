using System;
using System.Diagnostics;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class Player {
    public int Id { get; set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public ThemeEnum Theme { get; set; } = ThemeEnum.General;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<AuthCode> AuthCodes { get; set; } = [];
    public ICollection<Character> Characters { get; set; } = [];

    public Player(string displayName, string email) {
        DisplayName = displayName;
        Email = email;
    }

    // Parameterless constructor for EF Core
    protected Player() {
        // EF Core needs this to materialize the entity
    }
    public void UpdateLastLogin() {
        LastLoginAt = DateTime.UtcNow;
    }
    public void SetDisplayName(string displayName) {
        DisplayName = displayName;
    }
    public void SetEmail(string email) {
        Email = email;
    }
    public void SetTheme(ThemeEnum theme) {
        Theme = theme;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }


}

public class AuthCode
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}