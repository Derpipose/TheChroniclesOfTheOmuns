using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterClassService {
    public async Task<List<CharacterClass>> GetAllClassesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/ClassesExpounded.json";

        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);

        var dtoList = JsonSerializer.Deserialize<List<CharacterClassDto>>(json);

        if (dtoList == null)
            return new List<CharacterClass>();

        var diceTypes = DiceType.GetStandardDice();

        return dtoList
            .Where(dto => dto.Classification != "Sci fi" && dto.Classification != "Eastern")
            .Select(dto => {
                var isVeteran = dto.Classification == "Veteran";
                var classificationToUse = isVeteran ? dto.VeteranTag : dto.Classification;

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
                    ClassType = ParseClassType(classificationToUse),
                    Description = dto.Description,
                    HitDiceId = hitDiceId,
                    ManaDiceId = manaDiceId,
                    IsVeteranLocked = isVeteran,
                    HitDice = diceTypes.ContainsKey(hitDiceId) ? diceTypes[hitDiceId] : null,
                    ManaDice = diceTypes.ContainsKey(manaDiceId) ? diceTypes[manaDiceId] : null
                };
            }).ToList();
    }

    private ClassTypeEnum ParseClassType(string classType) {
        // Console.WriteLine($"Parsing ClassType: '{classType}'");
        var result = classType.ToLower() switch {
            "magic" => ClassTypeEnum.Magic,
            "combat" => ClassTypeEnum.Combat,
            "utility" => ClassTypeEnum.Utility,
            _ => ClassTypeEnum.Other
        };
        // Console.WriteLine($"  -> Mapped to: {result}");
        return result;
    }
}

public class CharacterClassDto {
    public string ClassName { get; set; } = "";
    public string ClassType { get; set; } = "";
    public string Description { get; set; } = "";
    public string IsVeteranLocked { get; set; } = "False";      
    public object HitDie { get; set; } = "";
    public object ManaDie { get; set; } = "";
    public string Classification { get; set; } = "";
    public string VeteranTag { get; set; } = "";
}
