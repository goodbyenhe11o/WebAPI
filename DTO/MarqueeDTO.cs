
namespace igs_backend.DTO
{
    public class MarqueeDTO
    {

        public string? Content { get; set; }

        public int? Sort { get; set; }

        public string? Language { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Boolean? IsPeriod { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
