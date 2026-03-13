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
        public List<FoodCustomization>? Customizations { get; set; }
        //public required IFormFile File { get; set; } 
    }

    public class FoodCustomization
    {
        public required string CustomizationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public List<CustomizationOption> Options { get; set; } = new();
    }

    public class CustomizationOption
    {
        public required string OptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
    }

}
