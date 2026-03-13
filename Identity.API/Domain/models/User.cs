using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required] 
        public string UserName { get; set; } = string.Empty ;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        // Salt اختصاصی برای هر کاربر
        [Required]
        public string PasswordSalt { get; set; } = string.Empty;
        public string ForgotPasswordToken { get; set; } = string.Empty;

        public DateTime ForgotPasswordExpirationTime { get; set; } = DateTime.UtcNow.AddHours(1);

        public bool IsForgotPasswordTokenUsed { get; set; } = false;

        public string Role { get; set; } = "";

        public bool EmailConfirmed { get; set; } = false;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? RefreshTokenExpirationTime { get; set; }

        public string ConfirmationToken { get; set; } = string.Empty;

        public DateTime ConfirmationTokenExpirationTime { get; set; } = DateTime.UtcNow.AddDays(3);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = false;

        public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();

    }
}
