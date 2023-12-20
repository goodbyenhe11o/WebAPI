using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igs_backend.Models
{
    public class Group
    {
        [Key]
        public int ID { get; set; }

        [StringLength(250)]
        public string? Name { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        [StringLength(20)]
        public string? grp { get; set; }

        public Boolean? DefaultTab { get; set; }

        public int? Sort { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

    }

}
