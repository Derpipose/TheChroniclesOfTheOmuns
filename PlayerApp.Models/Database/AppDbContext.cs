using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace PlayerApp.Models.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<Player> Players => Set<Player>();
    public DbSet<AuthCode> AuthCodes => Set<AuthCode>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
    public DbSet<RaceStats> RaceStats => Set<RaceStats>();
    public DbSet<ScifiTalent> ScifiTalents => Set<ScifiTalent>();
    public DbSet<ScifiBackground> ScifiBackgrounds => Set<ScifiBackground>();
    public DbSet<ScifiBackgroundTalent> ScifiBackgroundTalents => Set<ScifiBackgroundTalent>();
    public DbSet<ScifiClass> ScifiClasses => Set<ScifiClass>();
    public DbSet<ScifiClassTalent> ScifiClassTalents => Set<ScifiClassTalent>();
    public DbSet<ScifiRace> ScifiRaces => Set<ScifiRace>();
    public DbSet<ScifiSubrace> ScifiSubraces => Set<ScifiSubrace>();
    public DbSet<ScifiCharacter> ScifiCharacters => Set<ScifiCharacter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<AuthCode>()
            .HasOne(a => a.Player)
            .WithMany(p => p.AuthCodes)
            .HasForeignKey(a => a.PlayerId);

        modelBuilder.Entity<ScifiBackgroundTalent>()
            .HasKey(bt => new { bt.ScifiBackgroundId, bt.ScifiTalentId });

        modelBuilder.Entity<ScifiClassTalent>()
            .HasKey(ct => new { ct.ScifiClassId, ct.ScifiTalentId });

        modelBuilder.Entity<RaceStats>()
            .Property(r => r.Modifiers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<RaceStatModifier>>(v, (JsonSerializerOptions?)null) ?? new List<RaceStatModifier>()
            );

        modelBuilder.Entity<RaceStats>()
            .Property(r => r.FreeStatPicks)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>()
            );
    }
}