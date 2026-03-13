using Application.AdminDTOs.user;
using Application.DTOs;
using Domain.models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface IAuthService
    {
        /// [USER] Logs in a user and returns a JWT token.
        Task<AuthResponseDTO> Login(LoginDTO user);

        /// [USER] Hashes the given password using a secure algorithm.
        (string Hash, string Salt) HashPassword(string password);

        bool VerifyPassword(string password, string storedHash, string storedSalt);

        void UpdateUser(User user);

        Task RevokeRefreshToken(string email);

        string GenerateConfirmationToken();

        void SaveChangesAsync();
    }
}
