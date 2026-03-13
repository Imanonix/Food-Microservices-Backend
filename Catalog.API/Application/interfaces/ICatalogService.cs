using Application.AdminDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.interfaces
{
    public interface ICatalogService
    {
        Task<bool> AddAsync(AdminAddDTO addDTO);
        Task<List<AdminFoodItemDTO>> GetAllAsync();
        Task<AdminFoodItemDTO> GetByIdAsync(string id);
    }
}
