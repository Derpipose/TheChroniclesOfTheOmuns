using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerApp.Models {
    public class CharacterStats
    {
        [Required]
        public int Strength { get; set; }
        [Required]
        public int Constitution { get; set; }
        [Required]
        public int Dexterity { get; set; }
        [Required]
        public int Wisdom { get; set; }
        [Required]
        public int Charisma { get; set; }
        [Required]
        public int Intelligence { get; set; }
    }
}
