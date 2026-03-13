using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public required string ProductId { get; set; }

        public required string ProductName { get; set; }

        public required decimal UnitPrice { get; set; }

        public required decimal Discount { get; set; }

        public required int Quantity { get; set; }

        public decimal PricePaid { get; set; }

        public List<string> CustomizationOptions { get; set; } = new();

        public Guid OrderId { get; set; }
        public Order Order { get; set; } = new Order();
    }
}
