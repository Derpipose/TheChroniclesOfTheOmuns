using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;

namespace PlayerApp.Database;

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
    public DbSet<CharacterStats> CharacterStats { get; set; }
    public DbSet<CharacterClass> CharacterClass { get; set; }
    public DbSet<CharacterRace> CharacterRace { get; set; }

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
    }
}
