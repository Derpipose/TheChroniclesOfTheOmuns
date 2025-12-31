using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;
using PlayerApp.Models.Enums;

namespace PlayerAppAvalonia.Database;

public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
    }

    public ApplicationDbContext() {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
#if DEBUG
            // Development: SQL Server LocalDB
            optionsBuilder.UseSqlServer("Server=(localdb)\\ChroniclesDB;Database=chronicles_of_omuns;Trusted_Connection=true");
#else
            // Production: SQLite
            optionsBuilder.UseSqlite("Data Source=chronicles_of_omuns.db");
#endif
        }
    }

    public DbSet<Character> Character { get; set; }
    public DbSet<DiceType> DiceTypes { get; set; }
    public DbSet<CharacterStats> CharacterStats { get; set; }
    public DbSet<CharacterClass> CharacterClass { get; set; }
    public DbSet<CharacterRace> CharacterRace { get; set; }
    public DbSet<Modifier> Modifiers { get; set; }
    public DbSet<RacialModifier> RacialModifiers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Configure Character entity
        modelBuilder.Entity<Character>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Character>()
            .HasOne(c => c.Stats)
            .WithOne(s => s.Character)
            .HasForeignKey<CharacterStats>(s => s.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure CharacterStats entity
        modelBuilder.Entity<CharacterStats>()
            .HasKey(s => s.Id);

        // Configure CharacterClass entity
        modelBuilder.Entity<CharacterClass>()
            .HasKey(cc => cc.Id);

        // Configure CharacterRace entity
        modelBuilder.Entity<CharacterRace>()
            .HasKey(cr => cr.Id);

        // Configure DiceType relationships for CharacterClass
        modelBuilder.Entity<CharacterClass>()
            .HasOne(c => c.HitDice)
            .WithMany()
            .HasForeignKey(c => c.HitDiceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CharacterClass>()
            .HasOne(c => c.ManaDice)
            .WithMany()
            .HasForeignKey(c => c.ManaDiceId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure RacialModifier relationship
        modelBuilder.Entity<RacialModifier>()
            .HasOne(rm => rm.Race)
            .WithMany(r => r.Modifiers)
            .HasForeignKey(rm => rm.RaceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RacialModifier>()
            .HasOne(rm => rm.Modifier)
            .WithMany(m => m.RacialModifiers)
            .HasForeignKey(rm => rm.ModifierId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed DiceType data
        modelBuilder.Entity<DiceType>().HasData(
            new DiceType { Id = (int)DiceTypeEnum.D4, Name = "D4", Sides = 4 },
            new DiceType { Id = (int)DiceTypeEnum.D6, Name = "D6", Sides = 6 },
            new DiceType { Id = (int)DiceTypeEnum.D8, Name = "D8", Sides = 8 },
            new DiceType { Id = (int)DiceTypeEnum.D10, Name = "D10", Sides = 10 },
            new DiceType { Id = (int)DiceTypeEnum.D12, Name = "D12", Sides = 12 },
            new DiceType { Id = (int)DiceTypeEnum.D20, Name = "D20", Sides = 20 }
        );
    }
}
