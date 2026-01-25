using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class ManaCost{
public int Id {get; set;}
public int ManaMin  {get; set;}
public int ManaMax  {get; set;}
public ManaRuleEnum ManaRule {get; set;}
public int? Base {get; set;}
}