using PlayerApp.Models;
using PlayerAppBlazor.Database;

namespace PlayerAppBlazor.Services;

public class ClassSyncService
{
    private readonly AppDbContext _db;
    private readonly CharacterClassService _classService;

    public ClassSyncService(AppDbContext db)
    {
        _db = db;
        _classService = new CharacterClassService();
    }

    public async Task<SyncResult> SyncClassesAsync()
    {
        var result = new SyncResult();

        try
        {
            Console.WriteLine("Starting class sync...");
            var jsonClasses = await _classService.GetAllClassesAsync();
            Console.WriteLine($"Retrieved {jsonClasses.Count} classes from JSON");

            foreach (var jsonClass in jsonClasses)
            {
                var existingClass = _db.CharacterClasses
                    .FirstOrDefault(c => c.Name == jsonClass.Name && c.ClassType == jsonClass.ClassType);

                if (existingClass != null)
                {
                    // Update existing class
                    existingClass.Description = jsonClass.Description;
                    existingClass.HitDiceId = jsonClass.HitDiceId;
                    existingClass.ManaDiceId = jsonClass.ManaDiceId;
                    result.Updated++;
                }
                else
                {
                    // Insert new class - only set IDs, don't attach navigation properties
                    var newClass = new CharacterClass
                    {
                        Name = jsonClass.Name,
                        ClassType = jsonClass.ClassType,
                        Description = jsonClass.Description,
                        HitDiceId = jsonClass.HitDiceId,
                        ManaDiceId = jsonClass.ManaDiceId
                    };
                    _db.CharacterClasses.Add(newClass);
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
