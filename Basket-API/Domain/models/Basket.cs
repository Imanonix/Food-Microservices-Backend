using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class Basket
    {
        public Basket(Guid userId)
        {
            UserId = userId;
        }
        public string Id { get; set; } 
        public Guid UserId { get; set; } 
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
        public decimal TotalPrice => BasketItems.Sum(item => item.TotalItemPrice); 
    }

    public class BasketItem
        
    {
        public BasketItem()
        {
            // اگر موقع خواندن از دیتابیس خالی بود، یکی بساز
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; } 
        public required string ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalItemPrice => (Price + (SelectedOptions?.Sum(opt => opt.ExtraPrice)?? 0 )) * Quantity;
        public List<SelectedOption>? SelectedOptions { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
    
    public class SelectedOption
    {
        public required string OptionId { get; set; }
        public required string Name { get; set; } 
        public required decimal ExtraPrice { get; set; }
    }
}
