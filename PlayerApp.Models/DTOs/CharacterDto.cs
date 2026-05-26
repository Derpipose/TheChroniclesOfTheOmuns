namespace PlayerApp.Models.DTOs;


public class FantasyCharacterDto {
    public int Id { get; set; }
    public string Name { get; set; } = "Tav";
    public int Level { get; set; } = 1;
    public int Health { get; set; } = 0;
    public int Mana { get; set; } = 0;

    public int? FantasyCharacterRaceId { get; set; }
    public FantasyCharacterRace? FantasyCharacterRace { get; set; }
    
    public int? FantasyCharacterClassId { get; set; }
    public FantasyCharacterClass? FantasyCharacterClass { get; set; }
    
    public CharacterStats? Stats { get; set; }
    public List<CharacterStatBonus> CharacterStatBonuses { get; set; } = new List<CharacterStatBonus>();
}

/// <summary>
/// DTO for character update requests from the client.
/// </summary>
public class CharacterUpdateDto {
    public int Id { get; set; }
    public string Name { get; set; } = "Tav";
    public int Level { get; set; } = 1;
    
    public int? FantasyCharacterRaceId { get; set; }
    public int? FantasyCharacterClassId { get; set; }
    
    public CharacterStats? Stats { get; set; }
}
