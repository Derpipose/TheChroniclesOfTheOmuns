using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlayerApp.Models.Enums;

namespace PlayerApp.Models;

public class CharacterRaceService {
    public async Task<List<CharacterRace>> GetAllRacesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/RacesExpounded.json";
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        try {
            var json = await http.GetStringAsync(url);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var jsonRaces = JsonSerializer.Deserialize<List<CharacterRaceJsonDto>>(json, options);
            
            var races = jsonRaces?.Select(MapToCharacterRace).ToList() 
                ?? new List<CharacterRace>();
            
            return races;
        } catch (HttpRequestException ex) {
            Console.WriteLine($"Failed to fetch races from JSON: {ex.Message}");
            return new List<CharacterRace>();
        }
    }

    private static CharacterRace MapToCharacterRace(CharacterRaceJsonDto dto) {
        var race = new CharacterRace {
            Name = dto.Name,
            Description = dto.Description
        };

        if (!string.IsNullOrEmpty(dto.BonusMana) && int.TryParse(dto.BonusMana, out int value) && value != 0) {
            var type = dto.AddOrMultMana == "Add" 
                ? ModifierType.ManaBonus 
                : ModifierType.ManaMultiplier;
            race.AddModifier(type, value);
        }

        return race;
    }
}

public class CharacterRaceJsonDto {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [JsonConverter(typeof(FlexibleStringConverter))]
    public string BonusMana { get; set; } = string.Empty;
    public string AddOrMultMana { get; set; } = string.Empty;
}

public class FlexibleStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => string.Empty
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
