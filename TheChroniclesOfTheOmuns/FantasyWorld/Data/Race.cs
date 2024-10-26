using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FantasyWorld.Data {
    public class Race {

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string? Name { get; set; }

        [Required]
        [StringLength(20)]
        public string? Subtype { get; set; }

        [Required]
        [StringLength(20)]
        public int? CampaignId { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        bool? Starter { get; set; }

        public string? Special {  get; set; }

        [Required]
        public int? Str { get; set; }
        [Required]
        public int? Dex { get; set; }
        [Required]
        public int? Con { get; set; }
        [Required]
        public int? Wis { get; set; }
        [Required]
        public int? Int { get; set; }
        [Required]
        public int? Cha { get; set; }

        public string? Pick {  get; set; }  
        public int? BonusMana { get; set; }
        public string? AddOrMultMana { get; set; }
        [Required]
        public int? Speed { get; set; }
        public string? Language { get; set; }
        [Required]
        public int? AdventuringAge { get; set; }
        [Required]
        public int? MaxAge { get; set;}
        [Required]
        public bool? HHCompatable { get; set; }
        [Required]
        public bool? HECompatable { get; set; }
        [Required]
        public bool? HOCompatable { get; set; }



    }
}
