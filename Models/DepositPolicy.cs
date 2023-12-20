using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igs_backend.Models
{
    public class DepositPolicy
    {
        [Key]
        public int ID { get; set; }

        public string? Content { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }


        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

    }
}
