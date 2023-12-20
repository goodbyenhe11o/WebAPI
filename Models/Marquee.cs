using System.ComponentModel.DataAnnotations;

namespace igs_backend.Models
{
    public class Marquee
    {
        [Key]
        public int ID { get; set; }

        [StringLength(200)]
        public string? Content { get; set; }

        public int? Sort { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Boolean? IsPeriod { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
