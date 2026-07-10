using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiTalent {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public ScifiTalent(string name, string description) {
        Name = name;
        Description = description;
    }

    // Parameterless constructor for EF Core
    protected ScifiTalent() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}