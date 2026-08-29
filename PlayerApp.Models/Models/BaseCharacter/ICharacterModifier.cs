using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public interface ICharacterModifier {
    ModifierType ModifierType { get; }
    int Amount { get; }
}