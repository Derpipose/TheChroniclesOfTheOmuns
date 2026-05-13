using ChroniclesAPI.Database;
using PlayerApp.Models;

namespace ChroniclesAPI.Services;

public class RaceSyncService {
    private readonly AppDbContext _db;
    private readonly CharacterRaceService _raceService;

    public RaceSyncService(AppDbContext db) {
        _db = db;
        _raceService = new CharacterRaceService();
    }

    public async Task<SyncResult> SyncRacesAsync() {
        var result = new SyncResult();

        try {
            var jsonRaces = await _raceService.GetAllRacesAsync();

            foreach (var jsonRace in jsonRaces) {
                var existingRace = _db.CharacterRaces
                    .FirstOrDefault(r => r.Name == jsonRace.Name && r.RaceType == jsonRace.RaceType);

                if (existingRace != null) {
                    existingRace.Description = jsonRace.Description;
                    existingRace.Modifiers.Clear();
                    foreach (var modifier in jsonRace.Modifiers) {
                        existingRace.Modifiers.Add(modifier);
                    }
                    existingRace.RaceStatBonuses.Clear();
                    foreach (var bonus in jsonRace.RaceStatBonuses) {
                        existingRace.RaceStatBonuses.Add(bonus);
                    }
                    result.Updated++;
                } else {
                    _db.CharacterRaces.Add(jsonRace);
                    result.Inserted++;
                }
            }

            await _db.SaveChangesAsync();
            result.Success = true;
            result.Message = $"Sync complete: {result.Inserted} inserted, {result.Updated} updated.";
        } catch (Exception ex) {
            result.Success = false;
            result.Message = $"Sync failed: {ex.Message}";
        }

        return result;
    }
}
