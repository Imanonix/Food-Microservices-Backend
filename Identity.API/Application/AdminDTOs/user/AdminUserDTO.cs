using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminDTOs.user
{
    public class AdminUserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; } = false;

        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationTime { get; set; } = DateTime.UtcNow;

        public string ComfirmationToken { get; set; } = string.Empty;
        public DateTime ConfirmationTokenExpirationTime { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } 
    }
}
