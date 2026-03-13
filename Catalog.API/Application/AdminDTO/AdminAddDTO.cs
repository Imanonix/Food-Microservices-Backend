using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.AdminDTO
{
    public class AdminAddDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<AdminAddFoodCustomization>? Customizations { get; set; }
        //public required IFormFile File { get; set; } 
    }

    public class AdminAddFoodCustomization
    {
        public string Title { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public List<AdminAddCustomizationOption> Options { get; set; } = new();
    }

    public class AdminAddCustomizationOption
    {
        public required string OptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
    }

}
