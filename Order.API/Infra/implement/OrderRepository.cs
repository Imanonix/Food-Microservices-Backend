using Domain.IRepository;
using Domain.models;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static Infra.orderDb.OrderDbContext;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
namespace Infra.implement
{
    public class OrderRepository : IOrderRepository
    {
        readonly private OrderDb _context;
        public OrderRepository(OrderDb context)
        {
            _context = context;
        }

        public async Task<Domain.models.Order> AddOrderAsync(Domain.models.Order order)
        {
            await _context.AddAsync(order);
            return order;
        }

        public async Task<Domain.models.Order?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context.Orders.Include(oi => oi.OrderItems).SingleOrDefaultAsync(o => o.Id == orderId);
            return order;
        }

        public async Task<IEnumerable<Domain.models.Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _context.Orders.Include(oi => oi.OrderItems).Where(o => o.UserId == userId).ToListAsync();
            return orders;
        }

        public Task<bool> UpdateOrderAsync(Domain.models.Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddFoodItemAsync(FoodItem foodItem)
        {
            await _context.FoodItems.AddAsync(foodItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
