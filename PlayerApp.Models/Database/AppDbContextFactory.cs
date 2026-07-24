using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlayerApp.Models.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=100.80.37.115;Port=5433;Database=chroniclesdb_dev;Username=chronicles_dev;Password=devpassword");

        return new AppDbContext(optionsBuilder.Options);
    }
}