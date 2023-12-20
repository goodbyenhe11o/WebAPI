using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace igs_backend.DTO
{
    public class WithdrawPolicyDTO
    {
        public string? Content { get; set; }

        public string? Language { get; set; }


        public DateTime? UpdatedAt { get; set; }
    }
}
