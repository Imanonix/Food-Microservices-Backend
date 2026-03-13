using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.models
{
    public class UserAddress
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = string.Empty; // مثل: خانه، محل کار

        [Required]
        public string FullAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

        // Foreign Key به کاربر
        public Guid UserId { get; set; }

        [JsonIgnore] // برای جلوگیری از آن حلقه بی‌نهایت موقع سریالایز کردن
        public User User { get; set; } = null!;
    }
}
