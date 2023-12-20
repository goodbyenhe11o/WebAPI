using System.ComponentModel.DataAnnotations;

namespace igs_backend.Models
{
    public class UserPermissions
    {
        [Key]

        public int ID { get; set; }

        public int UserID { get; set; }
        public bool isActive { get; set; } = false;

        public int PermissionsID { get; set; }
        public Permissions? Permissions { get; set; }

        public User? user { get; set; }
    }
}
