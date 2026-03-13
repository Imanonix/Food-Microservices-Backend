using Application.DTOs;
using Application.ExceptionClass;
using Application.services.interfaces;
using AutoMapper;
using Domain.interfaces;
using Domain.models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Application.services.implementation
{
    public class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly IMapper _mapper;

        private readonly ICookieService _cookieService;
        public TokenService(IUserRepository userRepository, IConfiguration configuration, SymmetricSecurityKey symmetricSecurityKey,IMapper mapper, ICookieService cookieService
)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _symmetricSecurityKey = symmetricSecurityKey;
            _mapper = mapper;
            _cookieService = cookieService;
        }

        public string GenerateJWTToken(User user)
        {
            var role = user.Role;
            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role)
                };
            var Credential = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = Credential,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddHours(2)
            };
            var TokenHandler = new JwtSecurityTokenHandler();
            var JWTToken = TokenHandler.CreateToken(TokenDescriptor);
            return TokenHandler.WriteToken(JWTToken);
        }

        public string GenerateRefreshToken()
        {
            var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            return refreshToken;
        }

        public async Task<UserDTO> RefreshTokenValidationAsync(string email, string refreshToken, string token)
        {

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new UnAuthorizeException("please login again");

            if (user.RefreshToken != refreshToken || DateTime.UtcNow > user.RefreshTokenExpirationTime)
            {
                throw new UnAuthorizeException("please login again");
            }

            var newAccessToken = GenerateJWTToken(user);
            _cookieService.SetAuthCookies(newAccessToken, user.RefreshToken, user.Email);
            return _mapper.Map<UserDTO>(user);

        }

    }
}
