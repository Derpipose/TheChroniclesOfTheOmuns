using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace PlayerApp.Models;

public class CharacterRaceService {
    public async Task<List<CharacterRace>> GetAllRacesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/Races.json";
        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);
        var races = JsonSerializer.Deserialize<List<CharacterRace>>(json);

        return races ?? new List<CharacterRace>();
    }
}
