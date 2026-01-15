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

            var races = jsonRaces?
                .Where(dto => dto.Campaign != "Scifi" && dto.Campaign != "Oriental")
                .Select(MapToCharacterRace).ToList()
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
            Description = dto.Description,
            RaceType = dto.RaceType ?? "Unknown"
        };

        if (!string.IsNullOrEmpty(dto.BonusMana) && int.TryParse(dto.BonusMana, out int value) && value != 0) {
            var type = dto.AddOrMultMana == "Add"
                ? ModifierType.ManaBonus
                : ModifierType.ManaMultiplier;
            race.AddModifier(type, value);
        }

        AddStatBonus(race, "Strength", dto.Str);
        AddStatBonus(race, "Dexterity", dto.Dex);
        AddStatBonus(race, "Constitution", dto.Con);
        AddStatBonus(race, "Intelligence", dto.Int);
        AddStatBonus(race, "Wisdom", dto.Wis);
        AddStatBonus(race, "Charisma", dto.Cha);

        // TODO: Handle Pick field for selectable bonuses
        if (!string.IsNullOrEmpty(dto.Pick)) {
            // Will be handled separately
            if (dto.Pick == 1.ToString()) {
                var bonus = new RaceStatBonus {
                    BonusValue = 1,
                    IsSelectable = true
                };
                race.RaceStatBonuses.Add(bonus);
            } else if (dto.Pick == 2.ToString()) {
                var bonus = new RaceStatBonus {
                    BonusValue = 2,
                    IsSelectable = true
                };
                race.RaceStatBonuses.Add(bonus);
            } else if (dto.Pick == "Both" || dto.Pick == "Race") {
                var bonus1 = new RaceStatBonus {
                    BonusValue = 1,
                    IsSelectable = true
                };
                var bonus2 = new RaceStatBonus {
                    BonusValue = 2,
                    IsSelectable = true
                };
                race.RaceStatBonuses.Add(bonus1);
                race.RaceStatBonuses.Add(bonus2);
            } else {
                // Handle other cases if necessary
                throw new NotImplementedException($"Unhandled Pick value: {dto.Pick}");
            }
        }
        return race;
    }

    private static void AddStatBonus(CharacterRace race, string statName, int bonusValue) {
        if (bonusValue != 0) {
            var bonus = new RaceStatBonus {
                BonusValue = bonusValue,
                StatId = (int)Enum.Parse<StatType>(statName),
                IsSelectable = false
            };
            race.RaceStatBonuses.Add(bonus);
        }
    }
}

public class CharacterRaceJsonDto {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("SubType")]
    public string RaceType { get; set; } = string.Empty;

    public string Campaign { get; set; } = string.Empty;

    [JsonConverter(typeof(FlexibleStringConverter))]
    public string BonusMana { get; set; } = string.Empty;
    public string AddOrMultMana { get; set; } = string.Empty;

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Str { get; set; }

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Dex { get; set; }

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Con { get; set; }

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Int { get; set; }

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Wis { get; set; }

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Cha { get; set; }

    [JsonConverter(typeof(FlexibleStringConverter))]
    public string Pick { get; set; } = string.Empty;
}

public class FlexibleStringConverter : JsonConverter<string> {
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return reader.TokenType switch {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Number => reader.GetInt32().ToString(),
            _ => string.Empty
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
        writer.WriteStringValue(value);
    }
}

public class FlexibleIntConverter : JsonConverter<int> {
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return reader.TokenType switch {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => int.TryParse(reader.GetString(), out int value) ? value : 0,
            _ => 0
        };
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
        writer.WriteNumberValue(value);
    }
}
