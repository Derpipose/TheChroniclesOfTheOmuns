using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterClass {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    public required int HitDiceId { get; set; }
    public DiceType? HitDice { get; set; }
    public DiceType? ManaDice { get; set; }



    public static async Task<List<CharacterClass>> AllClassesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/Classes.json";

        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);

        var dtoList = JsonSerializer.Deserialize<List<CharacterClassDto>>(json);

        if (dtoList == null)
            return new List<CharacterClass>();

        var diceTypes = DiceType.GetStandardDice();

        return dtoList
            .Where(dto => dto.Classification != "Sci fi" && dto.Classification != "Eastern")
            .Select(dto => {
                var hitDiceId = dto.HitDie.ToString() switch {
                    "4" => (int)DiceTypeEnum.D4,
                    "6" => (int)DiceTypeEnum.D6,
                    "8" => (int)DiceTypeEnum.D8,
                    "10" => (int)DiceTypeEnum.D10,
                    "12" => (int)DiceTypeEnum.D12,
                    "20" => (int)DiceTypeEnum.D20,
                    _ => 0
                };
                var manaDiceId = dto.ManaDie.ToString() switch {
                    "4" => (int)DiceTypeEnum.D4,
                    "6" => (int)DiceTypeEnum.D6,
                    "8" => (int)DiceTypeEnum.D8,
                    "10" => (int)DiceTypeEnum.D10,
                    "12" => (int)DiceTypeEnum.D12,
                    "20" => (int)DiceTypeEnum.D20,
                    _ => 0
                };
                return new CharacterClass {
                    Name = dto.ClassName,
                    Description = dto.Description,
                    HitDiceId = hitDiceId,
                    HitDice = diceTypes.ContainsKey(hitDiceId) ? diceTypes[hitDiceId] : null, 
                    ManaDice = diceTypes.ContainsKey(manaDiceId) ? diceTypes[manaDiceId] : null
                };
            }).ToList();
    }

}


public class CharacterClassDto {
    public string ClassName { get; set; } = "";
    public string Description { get; set; } = "";
    public object HitDie { get; set; } = "";
    public object ManaDie { get; set; } = "";
    public string Classification { get; set; } = "";
}
