using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igs_backend.Models
{
    public class News
    {
        [Key]
        public int ID { get; set; }

        [StringLength(250)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        public int? Sort { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }


    }
}
