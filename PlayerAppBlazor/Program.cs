using PlayerAppBlazor.Components;
using PlayerAppBlazor.Database;
using PlayerAppBlazor.ViewModels;
using PlayerAppBlazor.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cross-platform app data directory
var appDataPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "TheChroniclesOfTheOmuns");

Directory.CreateDirectory(appDataPath);

var dbPath = Path.Combine(appDataPath, "chronicles.db");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
);

// Register ViewModels
builder.Services.AddScoped<CharactersViewModel>();
builder.Services.AddScoped<RacesViewModel>();
builder.Services.AddScoped<ClassesViewModel>();
builder.Services.AddScoped<NewCharacterViewModel>();
builder.Services.AddScoped<ViewCharacterViewModel>();

// Register Services
builder.Services.AddScoped<RaceSyncService>();
builder.Services.AddScoped<ClassSyncService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Auto-create DB (dev-friendly)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
