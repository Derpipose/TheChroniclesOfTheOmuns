using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlayerAppAvalonia.Database;
using PlayerAppAvalonia.Services;
using PlayerAppAvalonia.ViewModels;
using System;
using System.Collections.Generic;

namespace PlayerAppAvalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Load configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=(localdb)\\ChroniclesDB;Database=chronicles_of_omuns;Trusted_Connection=true" }
            })
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<AppCharacterService>();
        services.AddSingleton<NavigationService>();

        services.AddScoped<DashboardViewModel>();
        services.AddScoped<CharactersViewModel>();
        services.AddScoped<NewCharacterViewModel>();
        services.AddScoped<EditCharacterViewModel>();

        var provider = services.BuildServiceProvider();

        // Apply migrations
        using (var scope = provider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        // Create main window
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var navigationService = provider.GetRequiredService<NavigationService>();
            var dashboardVm = provider.GetRequiredService<DashboardViewModel>();
            var charactersVm = provider.GetRequiredService<CharactersViewModel>();
            navigationService.CurrentViewModel = dashboardVm;

            var mainWindow = new MainWindow
            {
                DataContext = navigationService
            };
            mainWindow.SetDependencies(navigationService, dashboardVm, charactersVm);

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
