using EmirsCorrespondence.Models;

namespace EmirsCorrespondence.Data.DTO
{
    public class UserDTO
    {

        public string Username { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }

        public DateTime createdAt { get; set; } = DateTime.Now;

        public UserViewModel user { get; set; }
        public IEnumerable<UserViewModel> userDTO { get; set; }
    }
}
