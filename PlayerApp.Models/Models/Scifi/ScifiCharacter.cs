using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiCharacter {
    
    public int Id { get; private set; } = 0;
    public int CharacterId { get; private set; } = 0;
    public Character? Character { get; private set; } 
    public int ScifiRaceId { get; private set; } = 0;
    public int ScifiSubraceId { get; private set; } = 0;
    public int ScifiClassId { get; private set; } = 0;
    public int ScifiBackgroundId { get; private set; } = 0;
    


    public ScifiRace? ScifiRace { get; private set; }
    public ScifiSubrace? ScifiSubrace { get; private set; }
    public ScifiClass? ScifiClass { get; private set; }
    public ScifiBackground? ScifiBackground { get; private set; }


    public ScifiCharacter(Character character) {
        if (character.Id == 0)
            throw new InvalidOperationException(
                "Character must be persisted before creating a ScifiCharacter.");
        CharacterId = character.Id;
    }

    private ScifiCharacter(int characterId) {
        CharacterId = characterId;
    }

    public void SetScifiRace(ScifiRace scifiRace) {
        if (scifiRace.Id == 0)
            throw new InvalidOperationException(
                "ScifiRace must be persisted before being assigned to a ScifiCharacter.");
        ScifiRaceId = scifiRace.Id;
    }

    public void SetScifiSubrace(ScifiSubrace scifiSubrace) {
        if (ScifiRaceId == 0)
            throw new InvalidOperationException(
                "ScifiRace must be set before setting a ScifiSubrace.");
        if (scifiSubrace.Id == 0)
            throw new InvalidOperationException(
                "ScifiSubrace must be persisted before being assigned to a ScifiCharacter.");
        ScifiSubraceId = scifiSubrace.Id;
    }

    public void SetScifiClass(ScifiClass scifiClass) {
        if (scifiClass.Id == 0)
            throw new InvalidOperationException(
                "ScifiClass must be persisted before being assigned to a ScifiCharacter.");
        ScifiClassId = scifiClass.Id;
    }

    public void SetScifiBackground(ScifiBackground scifiBackground) {
        if (scifiBackground.Id == 0)
            throw new InvalidOperationException(
                "ScifiBackground must be persisted before being assigned to a ScifiCharacter.");
        ScifiBackgroundId = scifiBackground.Id;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

}