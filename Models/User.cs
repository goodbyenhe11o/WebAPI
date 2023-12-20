using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace igs_backend.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [StringLength(250)]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        [StringLength (50)]
        public string Phone { get; set; }

        [StringLength (50)]
        public string?  Role { get; set; }

        [StringLength(50)]
        public string? Remark { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserPermissions>? UserPermissions { get; set; }   
    }
}
