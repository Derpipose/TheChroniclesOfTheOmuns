using PlayerApp.Models;
using PlayerAppBlazor.Database;

namespace PlayerAppBlazor.Services;

public class RaceSyncService
{
    private readonly AppDbContext _db;
    private readonly CharacterRaceService _raceService;

    public RaceSyncService(AppDbContext db)
    {
        _db = db;
        _raceService = new CharacterRaceService();
    }

    public async Task<SyncResult> SyncRacesAsync()
    {
        var result = new SyncResult();

        try
        {
            Console.WriteLine("Starting race sync...");
            var jsonRaces = await _raceService.GetAllRacesAsync();
            Console.WriteLine($"Retrieved {jsonRaces.Count} races from JSON");

            foreach (var jsonRace in jsonRaces)
            {
                var existingRace = _db.CharacterRaces
                    .FirstOrDefault(r => r.Name == jsonRace.Name && r.RaceType == jsonRace.RaceType);

                if (existingRace != null)
                {
                    // Update existing race
                    existingRace.Description = jsonRace.Description;
                    // Clear and re-add modifiers
                    existingRace.Modifiers.Clear();
                    foreach (var modifier in jsonRace.Modifiers)
                    {
                        existingRace.Modifiers.Add(modifier);
                    }
                    result.Updated++;
                }
                else
                {
                    // Insert new race
                    _db.CharacterRaces.Add(jsonRace);
                    result.Inserted++;
                }
            }

            await _db.SaveChangesAsync();
            result.Success = true;
            result.Message = $"Sync complete: {result.Inserted} inserted, {result.Updated} updated.";
            Console.WriteLine(result.Message);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Sync failed: {ex.Message}";
            Console.WriteLine($"Sync error: {ex}");
        }

        return result;
    }
}

public class SyncResult
{
    public bool Success { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public string Message { get; set; } = "";
}
