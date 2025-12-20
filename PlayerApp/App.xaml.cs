using System.Windows;
using Microsoft.EntityFrameworkCore;
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
        var services = new ServiceCollection();

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=chronicle.db"));

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

        // Create DB if it doesn't exist
        using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
        {
            context.Database.EnsureCreated();
        }

        // Show MainWindow with MainViewModel
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        MainWindow = new MainWindow { DataContext = mainViewModel };
        MainWindow.Show();
    }
}

