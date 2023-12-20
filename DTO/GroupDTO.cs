

namespace igs_backend.DTO
{
    public class GroupDTO
    {
        public string? Name { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }

        public string? Language { get; set; }

        public string? grp { get; set; }

        public Boolean DefaultTab { get; set; }

        public int? Sort { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
