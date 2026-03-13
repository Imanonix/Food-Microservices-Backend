using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class FoodItem
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Customizations { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    //public class FoodCustomization
    //{
    //    public required string CustomizationId { get; set; } 

    //    public string Title { get; set; } = string.Empty;
    //    public List<CustomizationOption> Options { get; set; } = new();
    //}

    //public class CustomizationOption
    //{
    //    public required string OptionId { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public decimal ExtraPrice { get; set; }
    //}
}
