using Domain.IRepository;
using Domain.models;
using FoodOrder.Shared.Contracts.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Consumers
{
    public class FoodItemCreatedConsumer : IConsumer<IFoodItemCreated>
    {
        private readonly IOrderRepository _orderRepository;
        public FoodItemCreatedConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task Consume(ConsumeContext<IFoodItemCreated> context)
        {
            var data = context.Message;
            var foodItem = new FoodItem { Id = data.Id, Name = data.Name, Price = data.Price, Customizations = JsonSerializer.Serialize(data.Customizations) };
            await _orderRepository.AddFoodItemAsync(foodItem);
        }
    }
}
