namespace PlayerApp.Models;

public class CharacterStatBonus {
    // This is for building a character. It can be used for racial bonuses, class bonuses, or even temporary bonuses from items or buffs. 
    // Not meant for temporary bonuses like from spells or buffs, but more for permanent bonuses that are applied to the character's stats.
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public int BonusValue { get; set; }
    public string BonusSource { get; set; } = string.Empty;
    public int? StatId { get; set; }
    public bool IsSelectable { get; set; }
}
