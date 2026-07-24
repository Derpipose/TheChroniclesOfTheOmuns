using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiTalent {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Ability { get; private set; } = string.Empty;

    public ScifiTalent(string name, string description, string ability) {
        Name = name;
        Description = description;
        Ability = ability;
    }

    // Parameterless constructor for EF Core
    protected ScifiTalent() {
        // EF Core needs this to materialize the entity
    }
    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }

    public void SetName(string name) => Name = name;
    public void SetDescription(string description) => Description = description;
    public void SetAbility(string ability) => Ability = ability;
}