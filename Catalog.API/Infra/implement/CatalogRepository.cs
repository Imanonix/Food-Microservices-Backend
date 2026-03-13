using Domain.IRepository;
using Domain.models;
using MongoDB.Driver;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace Infra.implement
{
    public class CatalogRepository : ICatalogRepository
    {
        readonly private IMongoCollection<FoodItem> _foods;  //اتصال به سرور مونوگو 
        readonly private IDistributedCache _redisCache;
        public CatalogRepository(IMongoClient mongoClient, IOptions<CatalogDatabaseSettings> setting, IDistributedCache redisCache)
        {
            var database= mongoClient.GetDatabase(setting.Value.DatabaseName);
            _foods = database.GetCollection<FoodItem>(setting.Value.CollectionName);
            _redisCache = redisCache;
        }
        public async Task<bool> CreateAsync(FoodItem item)
        {
            await _foods.InsertOneAsync(item);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var deleteResult = await _foods.DeleteOneAsync(f => f.Id == id);
            await _redisCache.RemoveAsync(id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<List<FoodItem>> GetAllAsync()
        {
            var foods = await _foods.Find(f => true).ToListAsync();
            return foods;
        }

        public async Task<FoodItem?> GetByIdAsync(string id)
        { 
            // Try to retrieve from Redis
            var cachedFood = await _redisCache.GetStringAsync(id);
            if (string.IsNullOrEmpty(cachedFood))
            {
                // Retrieve from MongoDB
                var foodFromDb = await _foods.Find(f => f.Id == id).SingleOrDefaultAsync();

                // Update Redis cache with the data fetched from MongoDB
                if (foodFromDb != null)
                {
                    var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                    var serializedFood = JsonSerializer.Serialize(foodFromDb);
                    await _redisCache.SetStringAsync(id, serializedFood, options);
                }
                return foodFromDb;
            }
            return JsonSerializer.Deserialize<FoodItem>(cachedFood);
        }

        public async Task<bool> UpdateAsync(FoodItem item)
        {
            var updateResult = await _foods.ReplaceOneAsync(filter: f => f.Id == item.Id, replacement: item);
            //Update cache with the updated item 
            if(updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            {
                await _redisCache.RemoveAsync(item.Id);
                return true;
            }
            return false;
        }
    }
}
