
namespace igs_backend.DTO
{
    public class BannerDTO
    {
        public int? Sort { get; set; }

        public string? Language { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
