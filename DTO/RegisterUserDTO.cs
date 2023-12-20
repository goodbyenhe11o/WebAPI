using igs_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace igs_backend.DTO
{
    public class RegisterUserDTO
    {

        [StringLength(250)]
        public string Email { get; set; }

        public string Password { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string? Remark { get; set; }

        public string? Role { get; set; }


    }
}
