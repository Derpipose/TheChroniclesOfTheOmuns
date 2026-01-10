namespace PlayerApp.Models;

public class CharacterStatBonus {
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public int BonusValue { get; set; }
    public string BonusSource { get; set; } = string.Empty;
    public int? StatId { get; set; }
    public bool IsSelectable { get; set; }
}
