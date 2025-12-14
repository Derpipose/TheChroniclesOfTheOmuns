using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace PlayerApp.Models;

public class CharacterClass {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }


    public static async Task<List<CharacterClass>> AllClassesAsync() {
        const string url = "https://derpipose.github.io/JsonFiles/Classes.json";

        using var http = new HttpClient();

        var json = await http.GetStringAsync(url);

        var dtoList = JsonSerializer.Deserialize<List<CharacterClassDto>>(json);

        if (dtoList == null)
            return new List<CharacterClass>();

        return dtoList.Select(dto => new CharacterClass {
            Name = dto.ClassName,
            Description = dto.Description
        }).ToList();
    }

}


public class CharacterClassDto {
    public string ClassName { get; set; } = "";
    public string Description { get; set; } = "";
}
