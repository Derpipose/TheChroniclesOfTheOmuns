using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using PlayerApp.Models.Enums;
using PlayerApp.Models.Dice;

namespace PlayerApp.Models;

public class FantasyCharacterClassService {
    public async Task<List<FantasyCharacterClass>> GetAllClassesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/ClassesExpounded.json";

        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);

        var dtoList = JsonSerializer.Deserialize<List<FantasyCharacterFantasyClassDto>>(json);

        if (dtoList == null)
            return new List<FantasyCharacterClass>();

        return dtoList
            .Where(dto => dto.Classification != "Sci fi" && dto.Classification != "Eastern")
            .Select(dto => {
                var isVeteran = dto.Classification == "Veteran";
                var classificationToUse = isVeteran ? dto.VeteranTag : dto.Classification;

                var hitDice = dto.HitDie.ToString() switch {
                    "4" => DiceType.D4,
                    "6" => DiceType.D6,
                    "8" => DiceType.D8,
                    "10" => DiceType.D10,
                    "12" => DiceType.D12,
                    "20" => DiceType.D20,
                    _ => DiceType.D6
                };

                var manaDice = dto.ManaDie.ToString() switch {
                    "4" => DiceType.D4,
                    "6" => DiceType.D6,
                    "8" => DiceType.D8,
                    "10" => DiceType.D10,
                    "12" => DiceType.D12,
                    "20" => DiceType.D20,
                    _ => DiceType.D6
                };

                return new FantasyCharacterClass {
                    Name = dto.ClassName,
                    FantasyClassType = ParseFantasyClassType(classificationToUse),
                    Description = dto.Description,
                    IsVeteranLocked = isVeteran,
                    HitDice = hitDice,
                    ManaDice = manaDice
                };
            }).ToList();
    }

    private FantasyClassTypeEnum ParseFantasyClassType(string FantasyClassType) {
        // Console.WriteLine($"Parsing FantasyClassType: '{FantasyClassType}'");
        var result = FantasyClassType.ToLower() switch {
            "magic" => FantasyClassTypeEnum.Magic,
            "combat" => FantasyClassTypeEnum.Combat,
            "utility" => FantasyClassTypeEnum.Utility,
            _ => FantasyClassTypeEnum.Other
        };
        // Console.WriteLine($"  -> Mapped to: {result}");
        return result;
    }
}

public class FantasyCharacterFantasyClassDto {
    public string ClassName { get; set; } = "";
    public string FantasyClassType { get; set; } = "";
    public string Description { get; set; } = "";
    public string IsVeteranLocked { get; set; } = "False";
    public object HitDie { get; set; } = "";
    public object ManaDie { get; set; } = "";
    public string Classification { get; set; } = "";
    public string VeteranTag { get; set; } = "";
}
