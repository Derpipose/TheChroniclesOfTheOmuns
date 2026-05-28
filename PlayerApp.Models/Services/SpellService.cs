// namespace PlayerApp.Models;

// using System.Text.Json;
// using System.Text.Json.Serialization;
// using PlayerApp.Models.Enums;

// public class SpellService {
//     public async Task<List<Spell>> GetAllSpellsAsync() {
//         const string url = "https://derpipose.github.io/JsonFiles/Spells.json";

//         using var http = new HttpClient();

//         var json = await http.GetStringAsync(url);

//         var dtoList = JsonSerializer.Deserialize<List<SpellDto>>(json);

//         if (dtoList == null)
//             return new List<Spell>();

//         var diceTypes = DiceType.GetStandardDice();

//         return dtoList.Select(dto => {
//             var hitDiceId = dto.HitDie?.ToString() switch {
//                 "4" => (int)DiceTypeEnum.D4,
//                 "6" => (int)DiceTypeEnum.D6,
//                 "8" => (int)DiceTypeEnum.D8,
//                 "10" => (int)DiceTypeEnum.D10,
//                 "12" => (int)DiceTypeEnum.D12,
//                 "20" => (int)DiceTypeEnum.D20,
//                 _ => 0
//             };

//             // return new Spell {
//             //     Name = dto.SpellName,
//             //     SpellBook = dto.SpellBook,
//             //     BookLevel = ParseBookLevel(dto.BookLevel),
//             //     SpellBranch = dto.SpellBranch,
//             //     Description = dto.Description,
//             //     // ManaCost = dto.ManaCost == "-" ? "0" : dto.ManaCost,
//             //     Range = string.IsNullOrEmpty(dto.Range) ? null : dto.Range,
//             //     HitDiceId = hitDiceId == 0 ? null : hitDiceId,
//             //     HitDie = hitDiceId == 0 || !diceTypes.ContainsKey(hitDiceId) ? null : diceTypes[hitDiceId]
//             // };
//         }).ToList();
//     }

//     private static int ParseBookLevel(string bookLevel) {
//         if (string.IsNullOrWhiteSpace(bookLevel))
//             return 1;

//         if (bookLevel.Equals("Cantrip", StringComparison.OrdinalIgnoreCase))
//             return 0;

//         if (bookLevel.Contains("Book", StringComparison.OrdinalIgnoreCase)) {
//             var parts = bookLevel.Split(' ');
//             if (parts.Length > 1 && int.TryParse(parts[^1], out int level)) {
//                 return level;
//             }
//         }

//         return 1;
//     }
// }

// public class SpellDto {
//     public string SpellName { get; set; } = string.Empty;
//     public string SpellBook { get; set; } = string.Empty;
//     public string BookLevel { get; set; } = string.Empty;
//     public string SpellBranch { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;

//     // [JsonConverter(typeof(FlexibleStringConverter))]
//     // public string ManaCost { get; set; } = string.Empty;

//     [JsonConverter(typeof(FlexibleStringConverter))]
//     public string Range { get; set; } = string.Empty;

//     [JsonConverter(typeof(FlexibleIntConverter))]
//     public int? HitDie { get; set; }
// }
