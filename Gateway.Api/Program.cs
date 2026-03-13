using Microsoft.AspNetCore.RateLimiting;
using System.Text;
using System;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Gateway.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

Env.Load();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";


var jwtKey = environment == "Development"
      ? builder.Configuration["JWT:JWTKey"]
      : Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["JWT:JWTKey"];

builder.Services.AddScoped<SymmetricSecurityKey>(sp =>
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
);


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                            ?? builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                            ?? builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            //OnMessageReceived = context =>
            //{
            //    // خواندن توکن از کوکی
            //    if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            //    {
            //        context.Token = context.Request.Cookies["X-Access-Token"];
            //    }
            //    return Task.CompletedTask;
            //},

            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Data = false,
                    Message = "Session expired or invalid. Please log in again to continue.",
                    Status = 401
                });
            },

            OnChallenge = context =>
            {
                // وقتی توکن وجود ندارد یا منقضی شده است
                context.HandleResponse(); // جلوگیری از پاسخ پیش‌فرض
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Data = false,
                    Message = "Session expired or invalid. Please log in again to continue.",
                    Status = 401
                });
            },

            OnForbidden = context =>
            {
                // وقتی کاربر لاگین کرده ولی مجوز دسترسی ندارد (403)
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Data = false,
                    Message = "Access denied. You do not have the required permissions for this action.",
                    Status = 403
                });
            }
        };
    });


// Define a Authorization policy for use in YARP.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddRateLimiter(options =>
options.AddFixedWindowLimiter("ratelimiter-policy", opt =>
{
    opt.Window = TimeSpan.FromSeconds(10);
    opt.PermitLimit = 100;
    opt.QueueLimit = 20;
    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

}));
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// active ratelimiter
app.UseRateLimiter();


// ۲. فعال کردن هابِ مسیریابی YARP
app.MapReverseProxy();

app.Run();
