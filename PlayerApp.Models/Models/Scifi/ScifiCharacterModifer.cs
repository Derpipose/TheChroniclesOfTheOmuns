using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class ScifiCharacterModifier : ICharacterModifier {
    public int Id { get; private set; }

    public int ScifiCharacterId { get; private set; }
    public ScifiCharacter ScifiCharacter { get; private set; } = null!;

    public ModifierType ModifierType { get; private set; }
    public int Amount { get; private set; }
    public ModifierSource Source { get; private set; }

    public ScifiCharacterModifier(int scifiCharacterId, ModifierType modifierType, int amount, ModifierSource source) {
        ScifiCharacterId = scifiCharacterId;
        ModifierType = modifierType;
        Amount = amount;
        Source = source;
    }

    // EF Core needs this to materialize the entity
    protected ScifiCharacterModifier() { }
}
