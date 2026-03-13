using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminDTOs.user
{
    public class AdminAddUserDTO
    {

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string PasswordSalt { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Role { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
