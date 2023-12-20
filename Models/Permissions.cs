using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace igs_backend.Models
{
    public class Permissions
    {
        [Key]
        public int ID { get; set; }

        [StringLength(20)]
        public string? Name { get; set; }



        public ICollection<UserPermissions>? UserPermissions { get; set; }

    }
}
