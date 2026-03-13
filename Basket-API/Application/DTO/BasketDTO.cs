using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class BasketDTO
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public List<BasketItemDTO> BasketItems { get; set; } = new List<BasketItemDTO>();
        public decimal TotalPrice { get; set; }
    }

    public class BasketItemDTO
    {
        public string Id { get; set; }
        public required string ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public required int Quantity { get; set; }
        public decimal TotalItemPrice { get; set; }
        public List<SelectedOptionDTO>? SelectedOptionDTOs { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }


    public class SelectedOptionDTO
    {
        public required string OptionId { get; set; }
        public string? Name { get; set; }
        public decimal ExtraPrice { get; set; }
    }
}
