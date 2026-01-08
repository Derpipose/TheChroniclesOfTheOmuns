namespace PlayerApp.Models;

public class RaceStatBonus {
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int BonusValue { get; set; }
    public int? StatId { get; set; }
    public bool IsSelectable { get; set; }
}