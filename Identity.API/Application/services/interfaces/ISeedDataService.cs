using Application.AdminDTOs.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface ISeedDataService
    {
        Task<AdminUserDTO> GetUserByEmail(string email);
        Task<bool> AddUser(AdminAddUserDTO userDTO);
    }
}
