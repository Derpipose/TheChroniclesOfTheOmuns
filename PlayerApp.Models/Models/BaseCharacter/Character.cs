using System;
using System.Diagnostics;

namespace PlayerApp.Models;

public class Character {
    public string Name { get; private set; } = "Tav";
    public int playerId { get; private set; } = 0;
    public int Level { get; private set; } = 1;
    public string Setting { get; private set; } = "Unregistered";
    public int MaxHealth { get; private set; } = 0;
    public int Id { get; private set;} 

    public Character(Player player) {
        playerId = player.Id;
    }

    public void SetName(string name) {
        Name = name;
    }

    public void SetMaxHealth(int maxHealth) {
        MaxHealth = maxHealth;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}