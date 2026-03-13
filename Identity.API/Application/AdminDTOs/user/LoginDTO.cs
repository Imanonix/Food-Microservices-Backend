using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AdminDTOs.user
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage ="Your email is not valid")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
