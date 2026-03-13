using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Shared.Contracts.Interfaces
{
    public interface IOrderPlaced
    {
        Guid OrderId { get; }
        DateTime CreatedAt { get; }
        string CustomerPhone { get; }
        decimal Amount { get; }
        List<string> Items { get; } // لیست غذاهای سفارش داده شده
    }
}
