using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiCharacter {
    
    public int Id { get; private set; } = 0;
    public int CharacterId { get; private set; } = 0;
    public Character? Character { get; private set; } 
    public int? ScifiRaceId { get; private set; } 
    public int? ScifiSubraceId { get; private set; }
    public int? ScifiClassId { get; private set; }
    public int? ScifiBackgroundId { get; private set; }
    


    public ScifiRace? ScifiRace { get; private set; }
    public ScifiSubrace? ScifiSubrace { get; private set; }
    public ScifiClass? ScifiClass { get; private set; }
    public ScifiBackground? ScifiBackground { get; private set; }


    public ScifiCharacter(int characterid) {
        CharacterId = characterid;
    }

    // Parameterless constructor for EF Core
    protected ScifiCharacter() {
        // EF Core needs this to materialize entities
    }
    public void SetScifiRace(int scifiRaceId) {
        if (scifiRaceId == 0)
            throw new InvalidOperationException(
                "ScifiRace must be persisted before being assigned to a ScifiCharacter.");
        ScifiRaceId = scifiRaceId;
    }

    public void SetScifiSubrace(int scifiSubraceId) {
        if (ScifiRaceId == 0)
            throw new InvalidOperationException(
                "ScifiRace must be set before setting a ScifiSubrace.");
        if (scifiSubraceId == 0)
            throw new InvalidOperationException(
                "ScifiSubrace must be persisted before being assigned to a ScifiCharacter.");
        ScifiSubraceId = scifiSubraceId;
    }

    public void SetScifiClass(int scifiClassId) {
        if (scifiClassId == 0)
            throw new InvalidOperationException(
                "ScifiClass must be persisted before being assigned to a ScifiCharacter.");
        ScifiClassId = scifiClassId;
    }

    public void SetScifiBackground(int scifiBackgroundId) {
        if (scifiBackgroundId == 0)
            throw new InvalidOperationException(
                "ScifiBackground must be persisted before being assigned to a ScifiCharacter.");
        ScifiBackgroundId = scifiBackgroundId;
    }

    public void ClearScifiRace() {
        ScifiRaceId = null;
    }

    public void ClearScifiSubrace() {
        ScifiSubraceId = null;
    }

    public void ClearScifiClass() {
        ScifiClassId = null;
    }

    public void ClearScifiBackground() {
        ScifiBackgroundId = null;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

}