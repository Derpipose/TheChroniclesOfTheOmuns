using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PlayerApp.Models;

public class Character
{

    [Required]
    public required string Name { get; set; }
    [Required]
    public int Level { get; set; }
    public CharacterStats? Stats { get; set; }
}
