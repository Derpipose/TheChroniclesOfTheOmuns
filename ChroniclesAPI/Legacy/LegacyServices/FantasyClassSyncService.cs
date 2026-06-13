// using PlayerApp.Models;

// namespace ChroniclesAPI.Services;

// public class FantasyClassSyncService {
//     private readonly AppDbContext _db;
//     private readonly FantasyCharacterClassService _classService;

//     public FantasyClassSyncService(AppDbContext db) {
//         _db = db;
//         _classService = new FantasyCharacterClassService();
//     }

//     public async Task<SyncResult> SyncClassesAsync() {
//         var result = new SyncResult();

//         try {
//             var jsonClasses = await _classService.GetAllClassesAsync();

//             foreach (var jsonClass in jsonClasses) {
//                 var existingClass = _db.FantasyCharacterClasses
//                     .FirstOrDefault(c => c.Name == jsonClass.Name && c.FantasyClassType == jsonClass.FantasyClassType);

//                 if (existingClass != null) {
//                     existingClass.Description = jsonClass.Description;
//                     existingClass.HitDice = jsonClass.HitDice;
//                     existingClass.ManaDice = jsonClass.ManaDice;
//                     existingClass.IsVeteranLocked = jsonClass.IsVeteranLocked;
//                     result.Updated++;
//                 } else {
//                     _db.FantasyCharacterClasses.Add(new FantasyCharacterClass {
//                         Name = jsonClass.Name,
//                         FantasyClassType = jsonClass.FantasyClassType,
//                         Description = jsonClass.Description,
//                         HitDice = jsonClass.HitDice,
//                         ManaDice = jsonClass.ManaDice,
//                         IsVeteranLocked = jsonClass.IsVeteranLocked
//                     });
//                     result.Inserted++;
//                 }
//             }

//             await _db.SaveChangesAsync();
//             result.Success = true;
//             result.Message = $"Sync complete: {result.Inserted} inserted, {result.Updated} updated.";
//         } catch (Exception ex) {
//             result.Success = false;
//             result.Message = $"Sync failed: {ex.Message}";
//         }

//         return result;
//     }

    
// }
