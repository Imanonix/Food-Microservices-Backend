using Application.AdminDTOs.user;
using Application.DTOs;
using Application.ExceptionClass;
using Application.services.interfaces;
using AutoMapper;
using Domain.interfaces;
using Domain.models;
using FoodOrder.Shared.Contracts.Interfaces;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly IMapper _mapper;
        private readonly ICookieService _cookieService;
        private readonly IAuthService _authService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<AccountService> _logger;
        public AccountService(IUserRepository userRepository,
            IConfiguration configuration,
            SymmetricSecurityKey symmetricSecurityKey,
            IMapper mapper,
            ICookieService cookieService,
            IAuthService authService,
            IPublishEndpoint publishEndpoint,
            ILogger<AccountService> logger
)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _symmetricSecurityKey = symmetricSecurityKey;
            _mapper = mapper;
            _cookieService = cookieService;
            _authService = authService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<bool> Register(RegisterDTO registerDTO)
        {
            _logger.LogInformation("New registration request for email: {Email}", registerDTO.Email);
            var user = await _userRepository.GetByEmailAsync(registerDTO.Email.ToLower());
            if (user != null)
            {
                throw new Exception(" The email address is already in use. Please use a different email.");
            }
            var newUser = _mapper.Map<User>(registerDTO);

            var (passwordHash, salt) = _authService.HashPassword(registerDTO.Password);
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = salt;

            newUser.ConfirmationToken = _authService.GenerateConfirmationToken();

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User {Email} successfully saved to database with ID: {UserId}", newUser.Email, newUser.Id);

            try
            {
                _logger.LogDebug("Attempting to publish UserRegistered event for {Email}", newUser.Email);

                await _publishEndpoint.Publish<IUserRegistered>(new { UserEmail = newUser.Email });

                _logger.LogInformation("UserRegistered event published successfully for {Email}", newUser.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MassTransit: Failed to publish registration event for {Email}", newUser.Email);
            }
            return true;
        }

        public async Task<bool> ConfirmEmail(string email, string confirmationToken)
        {
            var user = await _userRepository.GetByEmailAsync(email) ?? throw new NotFoundException("user not Found");

            if (user.ConfirmationToken == confirmationToken && DateTime.UtcNow < user.ConfirmationTokenExpirationTime)
            {
                user.IsActive = true;
                user.EmailConfirmed = true;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsEmailConfirmed(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception();
            return user.IsActive ? true : false;
        }

        public async Task<UserDTO> GetUserIncludeAddressByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressesByEmailAsync(email) ?? throw new NotFoundException("user not found");
                return _mapper.Map<UserDTO>(user);
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
        //public async Task<bool> SendForgotPasswordToken(ResetPasswordRequest resetPasswordRequest)
        //{
        //    await _unitOfWork.BeginTransaction();
        //    try
        //    {
        //        var user = await _unitOfWork.Users.GetByEmailAsync(resetPasswordRequest.Email);
        //        if (user == null)
        //        {
        //            throw new NotFoundException("If an account with this email exists, a password reset link has been sent. Please check your inbox.");
        //        }

        //        // Generate Token 
        //        var forgotPasswordToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        //        // Encode Token
        //        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(forgotPasswordToken));
        //        var link = $"{_configuration["ForgotPasswordLink:Link"]}?token={encodedToken}";

        //        // save in database 
        //        user.ForgotPasswordToken = forgotPasswordToken;
        //        user.ForgotPasswordExpirationTime = DateTime.UtcNow.AddHours(1);
        //        user.IsForgotPasswordTokenUsed = false;

        //        await _unitOfWork.Users.Update(user);
        //        await _unitOfWork.Commit();

        //        // Create Email
        //        var email = await _emailService.CreateForgotPasswordEmail(resetPasswordRequest.Email, link);

        //        //´Send Email
        //        await _emailService.SendEmail(email);

        //        return true;
        //    }
        //    catch (NotFoundException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex) {
        //        await _unitOfWork.RollBack();
        //        throw new Exception("An unexpected error occurred during the operation.", ex);
        //    }

        //}

        //public async Task<bool> ResetPasswordConfirmation(ResetPasswordForm resetPasswordForm)
        //{
        //    await _unitOfWork.BeginTransaction();
        //    try
        //    {
        //        // Decode token
        //        var decodedBytes = WebEncoders.Base64UrlDecode(resetPasswordForm.Token);
        //        var decodedToken = Encoding.UTF8.GetString(decodedBytes);

        //        // Retrieve user by forgotPasswordToken
        //        var user = await _unitOfWork.Users.GetByForgotPasswordTokenAsync(decodedToken);
        //        if (user == null)
        //        {
        //            throw new NotFoundException("The password reset link is invalid or has already been used.");
        //        }

        //        // Check token expiration
        //        if (user.ForgotPasswordExpirationTime < DateTime.UtcNow || user.IsForgotPasswordTokenUsed == true)
        //        {
        //            throw new ValidationException("The password reset link has expired. Please request a new one.");

        //        }
        //        // Hash new password
        //        var hashedPassword = await PasswordHasherAsync(resetPasswordForm.Password);

        //        // Update user password and clear token
        //        user.ForgotPasswordToken = "";
        //        user.PasswordHash = hashedPassword;
        //        user.IsForgotPasswordTokenUsed = true;
        //        var result = await _unitOfWork.Users.Update(user);
        //        await _unitOfWork.Commit();
        //        return true;
        //    }
        //    catch (NotFoundException)
        //    {
        //        throw;
        //    }
        //    catch (ValidationException)
        //    {
        //        throw;
        //    }

        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollBack();
        //        throw new Exception("An unexpected error occurred while resetting your password. Please try again later or contact support.", ex);
        //    }
        //}

    }
}
