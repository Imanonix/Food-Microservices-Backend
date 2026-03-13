using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<Order> AddOrderAsync(Order order);
        // معمولاً سفارش‌ها ویرایش نمی‌شوند، مگر برای تغییر وضعیت (Status)
        Task<bool> UpdateOrderAsync(Order order);

        // Consumer
        Task<bool> AddFoodItemAsync(FoodItem foodItem);
    }
}
