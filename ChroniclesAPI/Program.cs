using Microsoft.EntityFrameworkCore;
using PlayerApp.Models;

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
builder.Services.AddScoped<ChroniclesAPI.Services.FantasyCharacterService>();
builder.Services.AddScoped<ChroniclesAPI.Services.FantasyClassSyncService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Auto-create DB (dev-friendly)
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
