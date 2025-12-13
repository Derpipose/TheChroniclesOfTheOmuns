using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlayerApp.Models;

public class CharacterClass {
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Description { get; set; }

}

