using Application.AdminDTOs.user;
using Application.services.interfaces;
using AutoMapper;
using Domain.interfaces;
using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.implementation
{
    public class SeedDataService:ISeedDataService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public SeedDataService(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;   
        }

        public async Task<bool> AddUser(AdminAddUserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<AdminUserDTO> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email.ToLower());
            return _mapper.Map<AdminUserDTO>(user);
        }

        //public async Task<(string Hash, string Salt)> HashPassword(string password)
        //{
        //    // ۱. ساخت Salt
        //    byte[] saltBytes = RandomNumberGenerator.GetBytes(16); // 16 بایت کافیه
        //    string salt = Convert.ToBase64String(saltBytes);

        //    // ۲. ترکیب پسورد و Salt
        //    using var sha256 = SHA256.Create();
        //    byte[] combined = Encoding.UTF8.GetBytes(password + salt);
        //    byte[] hashBytes = sha256.ComputeHash(combined);

        //    // ۳. خروجی
        //    string hash = Convert.ToBase64String(hashBytes);

        //    return (hash, salt);
        //}
    }
}
