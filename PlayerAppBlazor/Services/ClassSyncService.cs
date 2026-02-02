using PlayerApp.Models;
using PlayerAppBlazor.Database;

namespace PlayerAppBlazor.Services;

public class ClassSyncService {
    private readonly AppDbContext _db;
    private readonly CharacterClassService _classService;

    public ClassSyncService(AppDbContext db) {
        _db = db;
        _classService = new CharacterClassService();
    }

    public async Task<SyncResult> SyncClassesAsync() {
        var result = new SyncResult();

        try {
            var jsonClasses = await _classService.GetAllClassesAsync();

            foreach (var jsonClass in jsonClasses) {
                var existingClass = _db.CharacterClasses
                    .FirstOrDefault(c => c.Name == jsonClass.Name && c.ClassType == jsonClass.ClassType);

                if (existingClass != null) {
                    existingClass.Description = jsonClass.Description;
                    existingClass.HitDice = jsonClass.HitDice;
                    existingClass.ManaDice = jsonClass.ManaDice;
                    existingClass.IsVeteranLocked = jsonClass.IsVeteranLocked;
                    result.Updated++;
                } else {
                    _db.CharacterClasses.Add(new CharacterClass {
                        Name = jsonClass.Name,
                        ClassType = jsonClass.ClassType,
                        Description = jsonClass.Description,
                        HitDice = jsonClass.HitDice,
                        ManaDice = jsonClass.ManaDice,
                        IsVeteranLocked = jsonClass.IsVeteranLocked
                    });
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
