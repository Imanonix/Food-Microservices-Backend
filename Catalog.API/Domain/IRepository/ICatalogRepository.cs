using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IRepository
{
    public interface ICatalogRepository
    {
        Task<FoodItem?> GetByIdAsync(string id);
        Task<List<FoodItem>> GetAllAsync();
        Task<bool> CreateAsync(FoodItem item);
        Task<bool> UpdateAsync(FoodItem item);
        Task<bool> DeleteAsync(string id);
    }
}
