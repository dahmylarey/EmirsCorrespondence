using EmirsCorrespondence.Models;
using System.ComponentModel.DataAnnotations;

namespace JWTRoleBasedAuthorization.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;


        // Navigation Properties
        public ICollection<Users> Users { get; set; }
    }
}
