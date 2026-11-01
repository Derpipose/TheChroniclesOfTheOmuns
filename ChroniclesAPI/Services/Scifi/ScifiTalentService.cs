using PlayerApp.Models.Database;
using PlayerApp.Models;
using Microsoft.EntityFrameworkCore;
using PlayerApp.Models.Enums;

namespace ChroniclesAPI.Services;

public record TalentPool(List<ScifiTalent> Granted, List<ScifiTalent> Pickable);

public class ScifiTalentService(AppDbContext db) {

    // Caller has already loaded the character (class, background) and checked both are set
    public async Task<TalentPool> GetTalentPoolAsync(ScifiCharacter scifiCharacter) {
        // 1. granted = the class's talents
        var classTalents = await db.ScifiClassTalents
            .Where(ct => ct.ScifiClassId == scifiCharacter.ScifiClassId)
            .Select(ct => ct.ScifiTalent!)
            .ToListAsync();

        // Pool: depends on the overlap rule
        List<ScifiTalent> poolTalents;

        if (scifiCharacter.ScifiBackground!.HasClassOverlap && scifiCharacter.ScifiClass!.Name != scifiCharacter.ScifiBackground.Name) {
            // Mismatch: pick from the class that shares the background's name
            var matchingClass = await db.ScifiClasses
                .FirstOrDefaultAsync(c => c.Name == scifiCharacter.ScifiBackground.Name);

            if (matchingClass is null)
                return new TalentPool(classTalents, new List<ScifiTalent>());

            poolTalents = await db.ScifiClassTalents
                .Where(ct => ct.ScifiClassId == matchingClass.Id)
                .Select(ct => ct.ScifiTalent!)
                .ToListAsync();
        } else {
            // No overlap, or class matches: pick from the background's own talents
            poolTalents = await db.ScifiBackgroundTalents
                .Where(bt => bt.ScifiBackgroundId == scifiCharacter.ScifiBackgroundId)
                .Select(bt => bt.ScifiTalent!)
                .ToListAsync();
        }
        // 2. pickable = namesake class talents (overlap + mismatch) or background talents
        //  remove from pickable anything already in granted
        var pickable = poolTalents.Where(p => !classTalents.Any(g => g.Id == p.Id)).ToList();
        return new TalentPool(classTalents, pickable);
    }

    // Bring the character's stored talents in line with their current class and background.
    // Call after any class or background change, and at creation. Safe to run any time.
    // Caller must have loaded Character, Talents, ScifiClass, and ScifiBackground. Caller saves.
    public async Task SyncTalentsAsync(ScifiCharacter scifiCharacter) {
        // No talents are stored until both class and background are set
        if (scifiCharacter.ScifiClass is null || scifiCharacter.ScifiBackground is null) {
            scifiCharacter.ClearTalentsBySource(TalentSourceEnum.Class);
            scifiCharacter.ClearTalentsBySource(TalentSourceEnum.Background);
            return;
        }

        var pool = await GetTalentPoolAsync(scifiCharacter);

        // 1. Clear the background pick if it's no longer valid
        var currentPick = scifiCharacter.Talents
            .FirstOrDefault(t => t.SourceType == TalentSourceEnum.Background);

        if (currentPick is not null && !pool.Pickable.Any(t => t.Id == currentPick.ScifiTalentId))
            scifiCharacter.ClearTalentsBySource(TalentSourceEnum.Background);

        // 2. Remove class talents the current class doesn't grant
        var grantedIds = pool.Granted.Select(t => t.Id).ToHashSet();

        var staleClassTalentIds = scifiCharacter.Talents
            .Where(t => t.SourceType == TalentSourceEnum.Class && !grantedIds.Contains(t.ScifiTalentId))
            .Select(t => t.ScifiTalentId)
            .ToList();

        foreach (var id in staleClassTalentIds)
            scifiCharacter.RemoveTalent(id);

        // 3. Add granted talents the character doesn't have yet
        foreach (var talent in pool.Granted) {
            if (!scifiCharacter.Talents.Any(t => t.ScifiTalentId == talent.Id))
                scifiCharacter.AddTalent(talent, TalentSourceEnum.Class);
        }
    }
}
