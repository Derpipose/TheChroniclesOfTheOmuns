using System;
using System.Diagnostics;

namespace PlayerApp.Models;

public class Character {
    public string Name { get; private set; } = "Tav";
    public int PlayerId { get; private set; } = 0;
    public int Level { get; private set; } = 1;
    public string Setting { get; private set; } = "Unregistered";
    public int MaxHealth { get; private set; } = 0;
    public int Id { get; private set;}
    public bool IsActive { get; private set; } = false;

    public Player? Player { get; private set; }
    

    public Character(int playerid, string name) {
        PlayerId = playerid;
        Name = name;
    }

    private Character(int playerId) {
        this.PlayerId = playerId;
    }

    // Parameterless constructor for EF Core
    protected Character() {
        // EF Core needs this to materialize the entity
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

    public void SetActive(){
        IsActive = true;
    }
}