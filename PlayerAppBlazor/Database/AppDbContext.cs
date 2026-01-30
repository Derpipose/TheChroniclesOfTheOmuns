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
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<RacialModifier> RacialModifiers => Set<RacialModifier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}
