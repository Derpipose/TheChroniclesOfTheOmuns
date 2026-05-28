using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;

namespace PlayerAppBlazor.Database;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {
    }

    public DbSet<FantasyCharacter> FantasyCharacters => Set<FantasyCharacter>();
    public DbSet<FantasyCharacterClass> FantasyCharacterClasses => Set<FantasyCharacterClass>();
    public DbSet<FantasyCharacterRace> FantasyCharacterRaces => Set<FantasyCharacterRace>();
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<RacialModifier> RacialModifiers => Set<RacialModifier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}
