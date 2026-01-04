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
            var jsonClasses = await _classService.GetAllClassesAsync();
            var allDiceTypes = _db.DiceTypes.ToList();
            
            var diceIdToName = new Dictionary<int, string>
            {
                { 1, "D4" }, { 2, "D6" }, { 3, "D8" }, { 4, "D10" }, { 5, "D12" }, { 6, "D20" }
            };

            foreach (var jsonClass in jsonClasses)
            {
                var existingClass = _db.CharacterClasses
                    .FirstOrDefault(c => c.Name == jsonClass.Name && c.ClassType == jsonClass.ClassType);

                var hitDiceName = diceIdToName.ContainsKey(jsonClass.HitDiceId) ? diceIdToName[jsonClass.HitDiceId] : null;
                var manaDiceName = diceIdToName.ContainsKey(jsonClass.ManaDiceId) ? diceIdToName[jsonClass.ManaDiceId] : null;
                
                var hitDice = hitDiceName != null ? allDiceTypes.FirstOrDefault(d => d.Name == hitDiceName) : null;
                var manaDice = manaDiceName != null ? allDiceTypes.FirstOrDefault(d => d.Name == manaDiceName) : null;

                if (hitDice == null || manaDice == null)
                    continue;

                if (existingClass != null)
                {
                    existingClass.Description = jsonClass.Description;
                    existingClass.HitDiceId = hitDice.Id;
                    existingClass.ManaDiceId = manaDice.Id;
                    existingClass.IsVeteranLocked = jsonClass.IsVeteranLocked;
                    result.Updated++;
                }
                else
                {
                    _db.CharacterClasses.Add(new CharacterClass
                    {
                        Name = jsonClass.Name,
                        ClassType = jsonClass.ClassType,
                        Description = jsonClass.Description,
                        HitDiceId = hitDice.Id,
                        ManaDiceId = manaDice.Id,
                        IsVeteranLocked = jsonClass.IsVeteranLocked
                    });
                    result.Inserted++;
                }
            }

            await _db.SaveChangesAsync();
            result.Success = true;
            result.Message = $"Sync complete: {result.Inserted} inserted, {result.Updated} updated.";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Sync failed: {ex.Message}";
        }

        return result;
    }
}
