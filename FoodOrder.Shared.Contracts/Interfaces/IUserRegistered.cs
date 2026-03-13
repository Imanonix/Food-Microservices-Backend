using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Shared.Contracts.Interfaces
{
    public interface IUserRegistered
    {
        //Guid UserId { get; }
        string UserEmail { get; }
        //string FullName { get; }
    }
}
