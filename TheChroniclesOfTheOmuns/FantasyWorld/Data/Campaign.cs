using System.ComponentModel.DataAnnotations;

namespace FantasyWorld.Data {
    public class Campaign {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string? Name { get; set; }
    }
}
