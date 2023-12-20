using System.ComponentModel.DataAnnotations;

namespace igs_backend.DTO
{
    public class LoginUserDTO
    {
        [StringLength(250)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
