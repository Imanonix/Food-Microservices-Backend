using FoodOrder.Shared.Contracts.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Application.Consumers
{
    public class UserRegisterConsumer : IConsumer<IUserRegistered>
    {
        private readonly ILogger<UserRegisterConsumer> _logger;

        public UserRegisterConsumer(ILogger<UserRegisterConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IUserRegistered> context)
        {
            var message = context.Message;

            // در اینجا عملیات واقعی (مثل ارسال ایمیل یا پیامک) را انجام می‌دهید
            _logger.LogInformation($"[Notification Service] User Registered: ({message.UserEmail})");

            await Task.CompletedTask;
        }
    }
}
