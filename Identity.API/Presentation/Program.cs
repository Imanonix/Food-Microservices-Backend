using Application.ExceptionClass;
using Application.mapper;
using Application.services;
using Application.services.implementation;
using Application.services.interfaces;
using Domain.interfaces;
using Domain.models;
using Infra.implement;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Cmp;
using Presentation;
using Presentation.Controllers;
using Presentation.Implementation;
using System;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using DotNetEnv;
using Infra.identityDb;
using MassTransit;


Env.Load();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();


try
{
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = environment
    });

    builder.Configuration.AddConfiguration(configuration);

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.AddScoped<ICookieService, CookieService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IServiceProvider, ServiceProvider>();
    builder.Services.AddScoped<ISeedDataService, SeedDataService>();

    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();

    builder.Services.AddScoped<SymmetricSecurityKey>(sp =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:JWTKey"))));

    builder.Services.AddScoped<IAppEnvironment, AppEnvironment>();
    //builder.Services.AddScoped<SeedData>();          //Add the SeedData class to the dependency injection container



    builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;

    builder.Services.AddAutoMapper(typeof(Mapper));

    // Connection string
    var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");

    // DbContext با MySQL
    builder.Services.AddDbContext<IdentityDb>(options =>
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,          // حداکثر ۵ بار تلاش مجدد
                    maxRetryDelay: TimeSpan.FromSeconds(10), // حداکثر ۱۰ ثانیه بین تلاش‌ها
                    errorNumbersToAdd: null    // اگر می‌خوای می‌تونی خطاهای خاص رو هم اضافه کنی
                );
            });
    });

    //RabbitMQ
    builder.Services.AddMassTransit(x =>
    {
        // چون این پروژه فقط پیام "می‌فرستد"، نیازی به AddConsumer نیست

        x.UsingRabbitMq((context, cfg) =>
        {
            // آدرس سرور رابیت (اگر روی سیستمت هست localhost بماند)
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            // این تنظیم باعث می‌شود مس‌تنزیت تمام تنظیمات لازم را به صورت خودکار انجام دهد
            cfg.ConfigureEndpoints(context);
        });
    });
    // Add Serilog
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.WithProperty("ApplicationName", "Identity.API") // برچسب اختصاصی برای این سرویس
        .CreateLogger();

    builder.Host.UseSerilog(); // ۳. این خط حیاتی است


    // Configure Identity services with custom password requirements and Entity Framework integration.
    // Password options:
    // - Require at least one digit.
    // - Require at least one lowercase letter.
    // - Require at least one uppercase letter.
    // - Require at least one non-alphanumeric character.
    // - Set the minimum password length to 8 characters.
    // Identity services are linked to the MupikFurnitureDbContext and default token providers are added for account management features.


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
                OnMessageReceived = context =>
                {
                    // خواندن توکن از کوکی
                    if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                    {
                        context.Token = context.Request.Cookies["X-Access-Token"];
                    }
                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    return context.Response.WriteAsJsonAsync(new ApiResponse
                    {
                        Data = false,
                        Message = "توکن نامعتبر است. لطفاً دوباره وارد شوید.",
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
                        Message = "شما دسترسی لازم را ندارید. لطفاً وارد حساب خود شوید.",
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
                        Message = "شما مجاز به انجام این عملیات نیستید.",
                        Status = 403
                    });
                }
            };
        });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevCors", policy =>
        {
            policy.WithOrigins(
                "https://localhost:5173",
                "http://localhost:5173",
                "https://localhost:5174",
                "http://localhost:5174",
                "http://localhost:3000",
                "http://localhost:3001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });

        // 🚀 Production Policy
        options.AddPolicy("ProdCors", policy =>
        {
            policy.WithOrigins(
                "https://imangilani.com",
                "https://www.imangilani.com",
                "https://resumeadmin.imangilani.com",
                "https://www.resumeadmin.imangilani.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });

    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    Console.WriteLine("✅ App built successfully.");

    // 1. میدلور پررندرینگ — باید اول از همه باشد!
    app.Use(async (context, next) =>
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        if (path.StartsWith("/publication-details"))
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString().ToLower();
            var isBot = userAgent.Contains("whatsapp") ||
                        userAgent.Contains("facebookexternalhit") ||
                        userAgent.Contains("twitter") ||
                        userAgent.Contains("bot") ||
                        userAgent.Contains("crawler") ||
                        userAgent.Contains("slurp") ||
                        userAgent.Contains("googlebot");

            if (isBot)
            {
                var fullUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
                var previewUrl = $"/post/preview-sharing?url={Uri.EscapeDataString(fullUrl)}";
                context.Response.Redirect(previewUrl, permanent: false);
                return;
            }
        }
        await next();
    });

    // 2. استاتیک فایل‌ها — بعد از پررندرینگ
    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            if (ctx.File.Name == "index.html")
            {
                ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers["Pragma"] = "no-cache";
                ctx.Context.Response.Headers["Expires"] = "0";
            }
        }
    });

    // 3. روتینگ و CORS
    app.UseRouting();
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("DevCors");
    }
    else
    {
        app.UseCors("ProdCors");
    }

    // 4. میدلورهای احراز هویت
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    // 5. SPA fallback routing for admin panel
    app.MapFallbackToFile("/imanonix/{*path}", "imanonix/index.html");

    //SPA fallback routing for the main website
    app.MapFallbackToFile("index.html");

    //6.Seed Data(در محیط توسعه یا یک‌بار)
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<IdentityDb>();

    Console.WriteLine("🔍 Checking if database exists...");
    if (await context.Database.EnsureCreatedAsync())
    {
        Console.WriteLine("✅ Database was created because it didn’t exist before.");
    }
    else
    {
        Console.WriteLine("ℹ️ Database already exists.");
    }

    // حالا SeedData را اجرا کن
    Console.WriteLine("Seeding data...");
    await SeedData.Add(services, configuration);

    Console.WriteLine("Migrations and seeding completed!");


    // 9. Swagger فقط در توسعه
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.Run();  // اجرای برنامه.
}
catch (Exception ex)
{
    Console.WriteLine("❌ Application start-up failed!");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}
finally
{
    Console.WriteLine("🧹 Application shutting down...");
}