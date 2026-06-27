using System.Diagnostics;

namespace PlayerApp.Models;

// pick one talent

// classes all class talents

/*
If (overflow){
If(class != background){
pick one talent from class talents }
else (class == background){
pick one talent from background talents }
}
*/
public class ScifiBackground {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool Restricted { get; private set; } = false;
    public bool HasClassOverlap { get; private set; } = false;

    public IReadOnlyList<ScifiBackgroundTalent> BackgroundTalents { get; private set; } = new List<ScifiBackgroundTalent>();

    public ScifiBackground(string name, string description, bool restricted = false, bool hasClassOverlap = false) {
        Name = name;
        Description = description;
        Restricted = restricted;
        HasClassOverlap = hasClassOverlap;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}


public class ScifiBackgroundTalent {
    public int ScifiBackgroundId { get; private set; }
    public int ScifiTalentId { get; private set; }

    public ScifiBackground? Background { get; private set; }
    public ScifiTalent? Talent { get; private set; }

    public ScifiBackgroundTalent(int backgroundId, int talentId) {
        ScifiBackgroundId = backgroundId;
        ScifiTalentId = talentId;
    }
}