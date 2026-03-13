using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{
    public class EmailNotification : INotification
    {
        public Task<bool> SendAsync(Guid id, string email, string userName)
        {
            //here implement email logic
            Console.WriteLine($"id:{id} -  email:{email} - userName:{userName}");
            return Task.FromResult(true);
        }
    }
}
