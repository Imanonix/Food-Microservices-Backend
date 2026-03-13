using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INotification
    {
        Task<bool> SendAsync(Guid id, string email, string userName);
    }
}
