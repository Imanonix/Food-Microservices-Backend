using Application.DTOs;
using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface ITokenService
    {
        /// <summary> Generates a JWT token. Purely computational (CPU-bound). </summary>
        string GenerateJWTToken(User user);

        /// <summary> Generates a random secure string for refresh token. </summary>
        string GenerateRefreshToken();

        /// <summary> Validates token against Database (I/O-bound). </summary>
        Task<UserDTO> RefreshTokenValidationAsync(string email,string refreshToken, string token);
    }
}
