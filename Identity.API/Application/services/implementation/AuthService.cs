using Application.AdminDTOs.user;
using Application.DTOs;
using Application.ExceptionClass;
using Application.services.interfaces;
using AutoMapper;
using Domain.interfaces;
using Domain.models;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.implementation
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        public AuthService(IUserRepository userRepository,
            IConfiguration configuration,
            SymmetricSecurityKey symmetricSecurityKey,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IWebHostEnvironment env,
            ITokenService tokenService,
            ICookieService cookieService
)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _env = env;
            _tokenService = tokenService;
            _cookieService = cookieService;
        }
        public async Task<AuthResponseDTO> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDTO.Email)
                           ?? throw new NotFoundException("The Email or Password is incorrect. Please try again.");

                var valid = VerifyPassword(loginDTO.Password, user.PasswordHash, user.PasswordSalt);

                if (!valid)
                    throw new ExceptionClass.ValidationException("Incorrect email or password. Please try again.");

                if (!user.IsActive)
                {
                    throw new ValidationException("You did not active your account");
                }

                var accessToken = _tokenService.GenerateJWTToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;

                UpdateUser(user);
                await _userRepository.SaveChangesAsync();
                _cookieService.SetAuthCookies(accessToken, refreshToken, user.Email);

                return new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = _mapper.Map<UserDTO>(user)
                };
            }
            catch (NotFoundException ex)
            {
                throw;
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred during the operation.", ex);
            }
        }

        public (string Hash, string Salt) HashPassword(string password)
        {
            // ۱. ساخت Salt
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16); // 16 بایت کافیه
            string salt = Convert.ToBase64String(saltBytes);

            // ۲. ترکیب پسورد و Salt
            using var sha256 = SHA256.Create();
            byte[] combined = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(combined);

            // ۳. خروجی
            string hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using var sha256 = SHA256.Create();
            byte[] combined = Encoding.UTF8.GetBytes(password + storedSalt);
            byte[] hashBytes = sha256.ComputeHash(combined);
            string hash = Convert.ToBase64String(hashBytes);

            return hash == storedHash;
        }

        public void UpdateUser(User user)
        {
            _userRepository.Update(user);
        }

        public async Task RevokeRefreshToken(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new NotFoundException("کاربر یافت نشد");

            user.RefreshToken = null; // یا ""
            user.RefreshTokenExpirationTime = DateTime.UtcNow; // برای امنیت بیشتر انقضا رو هم صفر کن

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public string GenerateConfirmationToken()
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            return token;
        }

        public async void SaveChangesAsync()
        {
            await _userRepository.SaveChangesAsync();
        }
    }
}
