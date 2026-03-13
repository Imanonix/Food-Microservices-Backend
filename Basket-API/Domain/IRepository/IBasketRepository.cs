using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface IBasketRepository
    {
        Task<Basket?> GetShoppingBasketAsync(string userId);
        Task<Basket?> UpdateShoppingBasketAsync(string id, Basket basket);
        Task<bool> DeleteBasketAsync(string userId);
    }
}
