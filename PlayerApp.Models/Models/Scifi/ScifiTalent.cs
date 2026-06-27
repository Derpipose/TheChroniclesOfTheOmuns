using System.Diagnostics;

namespace PlayerApp.Models;

public class ScifiTalent {
    public int Id { get; private set; } = 0;
    public string Name { get; private set; }
    public string Description { get; private set; }

    public ScifiTalent(string name, string description) {
        Name = name;
        Description = description;
    }

    [Conditional("DEBUG")]
    public void SimulatePersist(int id) {
        Id = id;
    }
}