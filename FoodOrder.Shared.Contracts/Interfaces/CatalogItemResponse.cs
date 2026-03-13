using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Shared.Contracts.Interfaces
{
    public class CatalogItemResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<FoodCustomization>? Customizations { get; set; }
        //public string ImageUrl { get; set; } = string.Empty;
    }

   
}
