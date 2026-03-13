using Application.services.interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.implementation
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _env;
        public CookieService(IHttpContextAccessor contextAccessor, IWebHostEnvironment env)
        {
            _contextAccessor = contextAccessor;
            _env = env;
        }

        public (string? email, string? refreshToken, string? token) GetAuthCookies()
        {
            var email = _contextAccessor.HttpContext?.Request.Cookies["X-Email"];
            var refreshToken = _contextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            var token = _contextAccessor.HttpContext?.Request.Cookies["X-Access-Token"];
            return (email, refreshToken, token);
        }

        public void SetAuthCookies(string token, string refreshToken, string email)
        {
            var cookieDomain = _env.IsDevelopment() ? "admin.localhost" : "imanonix-001-site1.qtempurl.com";

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(1),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Domain = cookieDomain,
            };

            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(365),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Domain = cookieDomain,
            };

            var emailCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(365),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Domain = cookieDomain,
            };

            _contextAccessor.HttpContext?.Response.Cookies.Append("X-Access-Token", token, accessCookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append("X-Refresh-Token", refreshToken, refreshTokenCookieOptions);
            _contextAccessor.HttpContext?.Response.Cookies.Append("X-Email", email, emailCookieOptions);
        }

        public void DeleteAuthCookies()
        {
            var response = _contextAccessor.HttpContext?.Response;

            var cookieDomain = _env.IsDevelopment() ? ".admin.localhost" : ".shahrivar.org"; // ⚠️ این دقیقاً باید همون مقداری باشه که توی login ست کردی

            var options = new CookieOptions
            {
                Domain = cookieDomain,
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            };

            response?.Cookies.Delete("X-Access-Token", options);
            response?.Cookies.Delete("X-Refresh-Token", options);
            response?.Cookies.Delete("X-Email", options);
            response?.Cookies.Delete(".AspNetCore.Session", options);

            _contextAccessor.HttpContext?.Session.Clear();
        }
    }
}
