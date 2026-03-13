using Application.AdminDTOs.user;
using Application.ExceptionClass;
using Application.services.interfaces;
using AutoMapper;
using Domain.interfaces;
using Domain.models;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.implementation
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly IMapper _mapper;
        private readonly ICookieService _cookieService;
        private readonly IAuthService _authService;
        public AdminService(IUserRepository userRepository,
           IConfiguration configuration,
           SymmetricSecurityKey symmetricSecurityKey,
           IHttpContextAccessor httpContextAccessor,
           IMapper mapper,
           IAuthService authService,
           ICookieService cookieService
)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _authService = authService;
            _cookieService = cookieService;
        }
        public async Task<bool> AdminUpdateUserAsync(AdminUpdateUserDTO adminUpdateUserDTO)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(adminUpdateUserDTO.Email);
                if (user == null)
                {
                    throw new NotFoundException("user not found");
                }
                ///////////////////
                if (adminUpdateUserDTO.Role != null)
                {
                    user.Role = adminUpdateUserDTO.Role;
                }
                if (adminUpdateUserDTO.IsActive != null)
                {
                    user.IsActive = (bool)adminUpdateUserDTO.IsActive;
                }
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during the operation.", ex);
            }
        }

        public async Task<bool> AdminDeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");
                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }
                user.IsActive = false;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during the operation.", ex);
            }
        }

        public async Task<List<AdminUserDTO>> AdminGetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null || !users.Any())
                {
                    throw new NotFoundException("No users found.");
                }
                return _mapper.Map<List<AdminUserDTO>>(users);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during the operation.", ex);
            }
        }
        public async Task<Guid> AddUser(AdminAddUserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            await _userRepository.AddAsync(user);
            return user.Id;
        }

        public async Task<AdminUserDTO> AdminGetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found.");
            return _mapper.Map<AdminUserDTO>(user);
        }

        public async Task<AdminUserDTO> AdminGetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email) ?? throw new NotFoundException("User not found.");
            return _mapper.Map<AdminUserDTO>(user);
        }

        public async Task<int> AdminGetTotalAmount()
        {
            return await _userRepository.CountUserAsync();
        }

        public async Task<List<AdminUserDTO>> AdminGetLatestUsersAsync()
        {
            var users = await _userRepository.GetLatestUsersAsync(10);
            return _mapper.Map<List<AdminUserDTO>>(users);
        }

        public async Task<AdminUserDTO> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return _mapper.Map<AdminUserDTO>(user);
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
            return true;  
        }

        Task<User> IAdminService.GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
