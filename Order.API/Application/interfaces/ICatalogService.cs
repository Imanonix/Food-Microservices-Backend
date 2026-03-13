using Application.AdminDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.interfaces
{
    public interface IOrderService
    {
        Task<bool> AddAsync(AdminAddDTO addDTO);
        Task<List<AdminFoodItemDTO>> GetAllAsync();
    }
}
