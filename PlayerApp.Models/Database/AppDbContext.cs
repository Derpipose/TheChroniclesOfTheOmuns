using Microsoft.EntityFrameworkCore;
namespace PlayerApp.Models.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
}
