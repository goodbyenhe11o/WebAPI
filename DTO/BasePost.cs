using System.ComponentModel.DataAnnotations;

namespace igs_backend.DTO
{
    public class BasePost
    {

        public string? Name { get; set; }

        public string? grp { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Language { get; set; }
        public int? Sort { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Boolean? DefaultTab { get; set; }
        public Boolean? IsPeriod { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt { get; set; }

    }
}
