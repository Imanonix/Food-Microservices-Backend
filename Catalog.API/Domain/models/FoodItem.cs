using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class FoodItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<FoodCustomization>? Customizations { get; set; }
        //public string ImageUrl { get; set; } = string.Empty;
    }

    public class FoodCustomization
    {
        public string CustomizationId { get; set; } = Guid.NewGuid().ToString();
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
