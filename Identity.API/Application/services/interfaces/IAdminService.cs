using Application.AdminDTOs.user;
using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface IAdminService
    {
        /// Activates or deactivates a user by their ID.
        Task<bool> AdminUpdateUserAsync(AdminUpdateUserDTO adminUpdateUserDTO);

        /// Deletes a user by their unique ID.
        Task<bool> AdminDeleteUserAsync(Guid userId);

        /// Retrieves all users in the system.
        Task<List<AdminUserDTO>> AdminGetAllUsersAsync();

        /// Retrieves a user by their unique ID.
        Task<AdminUserDTO> AdminGetUserByIdAsync(Guid userId);

        /// Retrieves a user by their email address.
        Task<AdminUserDTO> AdminGetUserByEmailAsync(string email);

        Task<int> AdminGetTotalAmount();


        /// Retrieves the 10 latest Users to display on the admin dashboard.
        Task<List<AdminUserDTO>> AdminGetLatestUsersAsync();

        Task<Guid> AddUser(AdminAddUserDTO userDTO);

        Task<User> GetUserByEmail(string email);

        Task<bool> DeleteUser(Guid userId);

        
    }
}
