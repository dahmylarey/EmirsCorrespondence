using DreyCorrespondence.Models;
using JWTRoleBasedAuthorization.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmirsCorrespondence.Models
{
    public class Users : IdentityUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime createdAt { get; set; } = DateTime.Now;

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        //public virtual Role Role { get; set; }

        //Navigation Properties
        public Role Role { get; set; }

        public IEnumerable<Users> User { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Log> Logs { get; set; }

    }
}
