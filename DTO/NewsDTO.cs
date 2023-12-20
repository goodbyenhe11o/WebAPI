

namespace igs_backend.DTO
{
    public class NewsDTO
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Language { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }

        public int? Sort { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
