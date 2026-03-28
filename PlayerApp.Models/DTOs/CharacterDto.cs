namespace PlayerApp.Models.DTOs;


public class CharacterDto {
    public int Id { get; set; }
    public string Name { get; set; } = "Tav";
    public int Level { get; set; } = 1;
    public int Health { get; set; } = 0;
    public int Mana { get; set; } = 0;

    public int? CharacterRaceId { get; set; }
    public CharacterRace? CharacterRace { get; set; }
    
    public int? CharacterClassId { get; set; }
    public CharacterClass? CharacterClass { get; set; }
    
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
    
    public int? CharacterRaceId { get; set; }
    public int? CharacterClassId { get; set; }
    
    public CharacterStats? Stats { get; set; }
}
