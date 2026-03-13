using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IBasketService
{
    public interface IShoppingBasketService
    {
        Task<BasketDTO?> GetShoppingBasketAsync(string userId);
        Task<BasketDTO?> UpdateShoppingBasketAsync(string id, AddItemDTO item);
        Task<bool> DeleteBasketAsync(string userId);
        Task SendBasketToOrder(string id);
    }
}
