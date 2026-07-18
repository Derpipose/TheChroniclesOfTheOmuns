using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public DbSet<CharacterModifier> CharacterModifiers => Set<CharacterModifier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {

        modelBuilder.Entity<Player>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<AuthCode>()
            .HasOne(a => a.Player)
            .WithMany(p => p.AuthCodes)
            .HasForeignKey(a => a.PlayerId);

        modelBuilder.Entity<CharacterStats>()
            .HasOne<Character>()
            .WithOne()
            .HasForeignKey<CharacterStats>(cs => cs.CharacterId);

        modelBuilder.Entity<ScifiCharacter>()
            .HasOne(sc => sc.Character)
            .WithOne()
            .HasForeignKey<ScifiCharacter>(sc => sc.CharacterId);

        modelBuilder.Entity<ScifiBackgroundTalent>()
            .HasKey(bt => new { bt.ScifiBackgroundId, bt.ScifiTalentId });

        modelBuilder.Entity<ScifiClassTalent>()
            .HasKey(ct => new { ct.ScifiClassId, ct.ScifiTalentId });

        modelBuilder.Entity<ScifiRace>()
            .HasOne(r => r.RaceStats)
            .WithMany()
            .HasForeignKey(r => r.RaceStatsId);

        modelBuilder.Entity<ScifiSubrace>()
            .HasOne(s => s.RaceStats)
            .WithMany()
            .HasForeignKey(s => s.RaceStatsId);

        modelBuilder.Entity<RaceStats>()
            .Property(r => r.Modifiers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => (IReadOnlyList<RaceStatModifier>)(JsonSerializer.Deserialize<List<RaceStatModifier>>(v, (JsonSerializerOptions?)null) ?? new List<RaceStatModifier>())
            )
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<RaceStatModifier>>(
                (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                c => JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                c => JsonSerializer.Deserialize<List<RaceStatModifier>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
            ));

        modelBuilder.Entity<RaceStats>()
            .Property(r => r.FreeStatPicks)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => (IReadOnlyList<int>)(JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions?)null) ?? new List<int>())
            )
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<int>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

        modelBuilder.Entity<CharacterModifier>()
            .HasOne(cm => cm.Character)
            .WithMany(c => c.Modifiers)
            .HasForeignKey(cm => cm.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CharacterModifier>()
            .Property(cm => cm.ModifierType)
            .HasConversion<string>();

        modelBuilder.Entity<CharacterModifier>()
            .Property(cm => cm.Source)
            .HasConversion<string>();


    }
}
