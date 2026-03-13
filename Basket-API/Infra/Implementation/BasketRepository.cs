using Domain.IRepository;
using Domain.models;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infra.Implementation
{
    public class BasketRepository : IBasketRepository
    {
        readonly private IDistributedCache _redisCache;
        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<Basket?> GetShoppingBasketAsync(string userId)
        {
            var cachedBasket = await _redisCache.GetStringAsync(userId);
            if (string.IsNullOrEmpty(cachedBasket))
            {
                return default;
            }
            return JsonSerializer.Deserialize<Basket>(cachedBasket);
        }

        public async Task<Basket?> UpdateShoppingBasketAsync(string id, Basket basket)
        {
            var stringBasket = JsonSerializer.Serialize(basket);
            await _redisCache.SetStringAsync(id, stringBasket);
            return await GetShoppingBasketAsync(id);
        }
        public async Task<bool> DeleteBasketAsync(string userId)
        {
            await _redisCache.RemoveAsync(userId);
            return await Task.FromResult(true);
        }
    }
}
