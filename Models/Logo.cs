using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igs_backend.Models
{
    public class Logo
    {
        [Key]
        public int ID { get; set; }

        [NotMapped]
        public IFormFile? DesktopImage { get; set; }

        [StringLength(500)]
        public string? DesktopImgPath { get; set; }

        [NotMapped]
        public IFormFile? MobileImage { get; set; }

        [StringLength(500)]
        public string? MobileImgPath { get; set; }

        [NotMapped]
        public IFormFile? PWAImage { get; set; }

        [StringLength(500)]
        public string? PWAImgPath { get; set; }

    }
}
