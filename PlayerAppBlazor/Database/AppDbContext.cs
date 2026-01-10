using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;

namespace PlayerAppBlazor.Database;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {
    }

    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();
    public DbSet<CharacterRace> CharacterRaces => Set<CharacterRace>();
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
    public DbSet<DiceType> DiceTypes => Set<DiceType>();
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<RacialModifier> RacialModifiers => Set<RacialModifier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Seed standard dice types
        modelBuilder.Entity<DiceType>().HasData(
            new DiceType { Id = 1, Name = "D4", Sides = 4 },
            new DiceType { Id = 2, Name = "D6", Sides = 6 },
            new DiceType { Id = 3, Name = "D8", Sides = 8 },
            new DiceType { Id = 4, Name = "D10", Sides = 10 },
            new DiceType { Id = 5, Name = "D12", Sides = 12 },
            new DiceType { Id = 6, Name = "D20", Sides = 20 }
        );
    }
}
