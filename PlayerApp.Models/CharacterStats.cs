using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Models;
    public class CharacterStats
    {
        [Required]
        public int Strength { get; set; } = 10;
        [Required]
        public int Constitution { get; set; } = 10;
        [Required]
        public int Dexterity { get; set; } = 10;
        [Required]
        public int Wisdom { get; set; } = 10;
        [Required]
        public int Charisma { get; set; } = 10;
        [Required]
        public int Intelligence { get; set; } = 10;
    }
