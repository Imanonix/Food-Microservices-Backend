using Application.AdminDTOs.user;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface IAccountService
    {
        /// Registers a new user with basic info and sends a confirmation email.
        Task<bool> Register(RegisterDTO registerDTO);

        /// Confirms the user's email using the token.
        Task<bool> ConfirmEmail(string email, string confirmationToken);

        /// Checks if a user's email has been confirmed.
        Task<bool> IsEmailConfirmed(string email);

        Task<UserDTO> GetUserIncludeAddressByEmailAsync(string email);

        //Task<bool> SendForgotPasswordToken(ResetPasswordRequest resetPasswordRequest);

        //Task<bool> ResetPasswordConfirmation(ResetPasswordForm resetPasswordForm);
    }
}
