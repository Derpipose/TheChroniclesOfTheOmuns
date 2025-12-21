using System.Windows;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayerApp.Database;
using PlayerApp.Services;
using PlayerApp.ViewModels;

namespace PlayerApp;

public partial class App : Application {
    private readonly ServiceProvider _serviceProvider;

    public App() {
        // Build configuration - look in project root first, then output directory
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        // If not found in output, try project root (for development)
        if (!File.Exists(configPath)) {
            var projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
            configPath = Path.Combine(projectRoot, "appsettings.json");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(configPath) ?? AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(Path.GetFileName(configPath), optional: true, reloadOnChange: true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=(localdb)\\ChroniclesDB;Database=chronicles_of_omuns;Trusted_Connection=true;MultipleActiveResultSets=true" }
            })
            .Build();

        var services = new ServiceCollection();

        // Register DbContext with conditional database provider
        services.AddDbContext<ApplicationDbContext>(options => {
#if DEBUG
            // Development/Test: SQL Server LocalDB
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
#else
            // Production: SQLite
            options.UseSqlite("Data Source=chronicles_of_omuns.db");
#endif
        });

        // Register Services
        services.AddScoped<AppCharacterService>();
        services.AddSingleton<NavigationService>();

        // Register ViewModels
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<CharactersViewModel>();
        services.AddTransient<NewCharacterViewModel>();
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        // Apply migrations if needed
        using (var scope = _serviceProvider.CreateScope()) {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        // Show MainWindow with MainViewModel
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        MainWindow = new MainWindow { DataContext = mainViewModel };
        MainWindow.Show();
    }
}

