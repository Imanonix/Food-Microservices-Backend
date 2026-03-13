using Application.AdminDTOs.user;
using Application.DTOs;
using Application.ExceptionClass;
using Application.services.interfaces;
using Domain.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Claims;
using System.Text;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        //private readonly IConfiguration _configuration;
        public UserController(
             IAccountService userService,
             IAccountService accountService,
             IAdminService adminService,
             IAuthService authService,
             ICookieService cookieService,
             ITokenService tokenService,
             IWebHostEnvironment webHostEnvironment,
             IConfiguration configuration)
        {
            _accountService = userService;
            _adminService= adminService;
            _authService= authService;
            _cookieService= cookieService;
            _tokenService= tokenService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }


        [Route("/api/user/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            try
            {
                var result = await _accountService.Register(registerDTO);

                return Ok(new ApiResponse { Data = result, Message = "Registration successful! Please activate your account using the link sent to your email", Status = 200 });
            }

            catch (ValidationException ex)
            {
                return Ok(new ApiResponse { Data = true, Message = ex.Message, Status = 400 });

            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Data = true, Message = ex.Message, Status = 500 });

            }

        }

        [Route("/api/user/email-confirmation")]
        [HttpGet]
        public async Task<IActionResult> RegisterConfirmation([FromQuery] string userEmail, string token)
        {
            var user = await _adminService.GetUserByEmail(userEmail);
            var decodedBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);
            var result = await _accountService.ConfirmEmail(user.Email, token);
            var status = result ? "success" : "failed";
            var baseRedirectUrl = _configuration["EmailConfirmationRedirectUrl"];
            var fullPath = $"{baseRedirectUrl}?status={status}";

            return Redirect(fullPath);
        }


        [Route("/api/user/login")]
        [HttpPost]

        public async Task<IActionResult> Login([FromForm] LoginDTO loginDTO)
        {
            Console.WriteLine($"[LOGIN CONTROLLER] Request received at {DateTime.Now}");
            Console.WriteLine($"[LOGIN CONTROLLER] Email: {loginDTO?.Email}");

            try
            {
                var userDTO = await _authService.Login(loginDTO);
                Console.WriteLine($"[LOGIN CONTROLLER] Login successful for {loginDTO.Email}");

                return Ok(new ApiResponse { Data = userDTO, Message = "شما با موفقیت وارد شدید", Status = 200 });
            }
            catch (NotFoundException)
            {
                Console.WriteLine($"[LOGIN CONTROLLER] NotFoundException for {loginDTO.Email}");
                return NotFound(new ApiResponse { Data = false, Message = "کاربری با این مشخصات پیدا نشد", Status = 404 });
            }
            catch (ValidationException)
            {
                Console.WriteLine($"[LOGIN CONTROLLER] ValidationException for {loginDTO.Email}");
                return Unauthorized(new ApiResponse { Data = false, Message = "نام کاربری یا رمز عبور اشتباه است", Status = 401 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN CONTROLLER] Unexpected error: {ex.Message}");
                return StatusCode(500, new ApiResponse { Data = false, Message = "خطای سرور رخ داد", Status = 500 });
            }

        }

        [Route("/api/user/logout")]
        [HttpGet]

        public async Task<IActionResult> Logout()
        {
            try
            {
                var (email, refreshToken, token) = _cookieService.GetAuthCookies();
                await _authService.RevokeRefreshToken(email);
                _cookieService.DeleteAuthCookies();
                
                return Ok(new ApiResponse { Data = true, Message = "you are loged out", Status = 200 });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse { Data = false, Message = ex.Message, Status = 404 });
            }
            catch (ValidationException ex)
            {
                return Unauthorized(new ApiResponse { Data = false, Message = ex.Message, Status = 401 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }
        }


        [Route("/api/user/refreshAccessToken")]
        [HttpPost]
        public async Task<IActionResult> GetNewAccessToken()
        {
            try
            {
                var (email, refreshToken, token) = _cookieService.GetAuthCookies();
                var user = await _tokenService.RefreshTokenValidationAsync(email, refreshToken, token);

                return Ok(new ApiResponse { Data = user, Message = "Token is issued", Status = 200 });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new ApiResponse { Data = false, Message = ex.Message, Status = 401 });
            }
            catch (UnAuthorizeException ex)
            {
                return Unauthorized(new ApiResponse { Data = false, Message = ex.Message, Status = 401 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }

        }

        //[Authorize(Roles = "owner")]
        //[Route("/api/user")]
        //[HttpGet]
        //public async Task<IActionResult> GetByEmailForMember([FromQuery] string email)
        //{
        //    try
        //    {
        //        var user = await _userService.GetUserIncludeAddressByEmailAsync(email);

        //        return Ok(new ApiResponse { Data = user, Message = "", Status = 200 });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
        //    }

        //}

        //[Authorize(Roles = "owner,admin")]
        [Route("/api/private/user/all")]
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var result = await _adminService.AdminGetAllUsersAsync();

                return Ok(new ApiResponse { Data = result, Message = "", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }

        }

        [Authorize(Roles = "owner,admin")]
        [Route("/api/private/user")]
        [HttpGet]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _adminService.AdminGetUserByEmailAsync(email);

                return Ok(new ApiResponse { Data = user, Message = "", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }

        }

        [Authorize(Roles = "owner,admin")]
        [Route("/api/private/user/update")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromForm] AdminUpdateUserDTO adminUpdateUserDTO)
        {
            try
            {
                var user = await _adminService.AdminUpdateUserAsync(adminUpdateUserDTO);

                return Ok(new ApiResponse { Data = user, Message = "User updated successfully ", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }
        }
        [Authorize(Roles = "owner,admin")]
        [Route("/api/private/user/delete")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)
        {
            try
            {
                var id = new Guid(userId);
                var user = await _adminService.AdminDeleteUserAsync(id);

                return Ok(new ApiResponse { Data = user, Message = "User deleted successfully ", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }
        }

        ////for membership request
        //[Route("/api/user/join-request")]
        //[HttpPost]
        //public async Task<IActionResult> MemberShipRequest([FromForm] MembershipRequestDTO membershipRequestDTO)
        //{
        //    try
        //    {
        //        var result = await _userService.MembershipRequest(membershipRequestDTO);
        //        return Ok(new ApiResponse { Data = true, Message = "User deleted successfully ", Status = 200 });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
        //    }
        //}
    }
}
