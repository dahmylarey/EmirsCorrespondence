using EmirsCorrespondence.Models;
using System.ComponentModel.DataAnnotations;

namespace EmirsCorrespondence.Data.DTO
{
    public class UserViewModel
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;


        public Users User { get; set; }
        public UserDTO uSerDto { get; set; }
        public IEnumerable<UserDTO> userDto { get; set; }
        public List<Users> user { get; set; }


    }



}
