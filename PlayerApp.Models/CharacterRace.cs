using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace PlayerApp.Models;

public class CharacterRace {
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]

    public required string Description { get; set; }



    public static async Task<List<CharacterRace>> AllRacesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/Races.json";

        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);

        var races = JsonSerializer.Deserialize<List<CharacterRace>>(json);

        return races ?? new List<CharacterRace>();
    }

}
