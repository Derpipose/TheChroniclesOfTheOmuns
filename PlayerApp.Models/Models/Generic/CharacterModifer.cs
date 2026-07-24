using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterModifier {
    public int Id { get; private set; }

    public int CharacterId { get; private set; }
    public Character Character { get; private set; } = null!;

    public ModifierType ModifierType { get; private set; }
    public int Amount { get; private set; }
    public ModifierSource Source { get; private set; }

    public CharacterModifier(int characterId, ModifierType modifierType, int amount, ModifierSource source, string? sourceDetail = null) {
        CharacterId = characterId;
        ModifierType = modifierType;
        Amount = amount;
        Source = source;
    }

    // EF Core needs this to materialize the entity
    protected CharacterModifier() { }
}
