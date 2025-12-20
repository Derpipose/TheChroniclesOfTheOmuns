using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayerApp.Database;
using PlayerApp.Services;
using PlayerApp.ViewModels;

namespace PlayerApp;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // Register DbContext with conditional database provider
        services.AddDbContext<ApplicationDbContext>(options =>
        {
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
        services.AddSingleton<MainViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Apply migrations if needed
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            context.Database.Migrate();
        }

        // Show MainWindow with MainViewModel
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        MainWindow = new MainWindow { DataContext = mainViewModel };
        MainWindow.Show();
    }
}

