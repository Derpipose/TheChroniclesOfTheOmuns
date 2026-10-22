using System.Text.Json;
using System.Text.Json.Serialization;
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
    public DbSet<ScifiCharacterModifier> ScifiCharacterModifiers => Set<ScifiCharacterModifier>();
    public DbSet<ScifiWeapon> ScifiWeapons { get; set; }
    public DbSet<ScifiWeaponInstance> ScifiWeaponInstances { get; set; }
    public DbSet<ScifiCharacterTalent> ScifiCharacterTalents { get; set; }

    private static readonly JsonSerializerOptions ModifierJsonOptions = new() {
        Converters = { new JsonStringEnumConverter() }
    };

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
                v => JsonSerializer.Serialize(v, ModifierJsonOptions),
                v => (IReadOnlyList<RaceStatModifier>)(JsonSerializer.Deserialize<List<RaceStatModifier>>(v, ModifierJsonOptions) ?? new List<RaceStatModifier>())
            )
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyList<RaceStatModifier>>(
                (c1, c2) => JsonSerializer.Serialize(c1, ModifierJsonOptions) == JsonSerializer.Serialize(c2, ModifierJsonOptions),
                c => JsonSerializer.Serialize(c, ModifierJsonOptions).GetHashCode(),
                c => JsonSerializer.Deserialize<List<RaceStatModifier>>(JsonSerializer.Serialize(c, ModifierJsonOptions), ModifierJsonOptions)!
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

        modelBuilder.Entity<ScifiCharacterModifier>()
            .HasOne(cm => cm.ScifiCharacter)
            .WithMany(c => c.Modifiers)
            .HasForeignKey(cm => cm.ScifiCharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScifiCharacterModifier>()
            .Property(cm => cm.ModifierType)
            .HasConversion<string>();

        modelBuilder.Entity<ScifiCharacterModifier>()
            .Property(cm => cm.Source)
            .HasConversion<string>();

        modelBuilder.Entity<ScifiWeaponInstance>(entity => {
            entity.HasKey(e => e.Id);

            // Database generates the ID (IDENTITY)
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            // ScifiWeaponId is just a plain int property — NO FK constraint
            // It's informational only, for "reset to factory" lookups
            entity.Property(e => e.ScifiWeaponId).IsRequired();

            // FK to character only
            entity.Property(e => e.ScifiCharacterId).IsRequired();
            entity.HasOne<ScifiCharacter>()
                .WithMany()
                .HasForeignKey(e => e.ScifiCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stat fields
            entity.Property(e => e.DamageFormula).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CriticalRange).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CriticalMultiplier).IsRequired().HasMaxLength(10);
        });

        // ScifiWeapon needs no special configuration — it's completely standalone
        modelBuilder.Entity<ScifiWeapon>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DamageFormula).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CriticalRange).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CriticalMultiplier).IsRequired().HasMaxLength(10);
            // No HasMany(Instances) — blueprint doesn't know about instances
        });

        modelBuilder.Entity<ScifiCharacterTalent>(entity => {
            entity.HasKey(ct => new { ct.ScifiCharacterId, ct.ScifiTalentId });

            entity.HasOne(ct => ct.ScifiCharacter)
                .WithMany(sc => sc.Talents)
                .HasForeignKey(ct => ct.ScifiCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ct => ct.ScifiTalent)
                .WithMany()
                .HasForeignKey(ct => ct.ScifiTalentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(ct => ct.SourceType)
                .HasConversion<string>();
        });


    }
}