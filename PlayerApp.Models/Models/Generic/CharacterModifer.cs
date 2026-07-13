using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterModifier {
    public int Id { get; private set; }

    public int CharacterId { get; private set; }
    public Character Character { get; private set; } = null!;

    public RaceModifierType ModifierType { get; private set; }
    public int Amount { get; private set; }
    public RaceModifierSource Source { get; private set; }

    public CharacterModifier(int characterId, RaceModifierType modifierType, int amount, RaceModifierSource source, string? sourceDetail = null) {
        CharacterId = characterId;
        ModifierType = modifierType;
        Amount = amount;
        Source = source;
    }

    // EF Core needs this to materialize the entity
    protected CharacterModifier() { }
}
