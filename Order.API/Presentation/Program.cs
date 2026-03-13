using Application.mapper;
using Domain.models;
using Infra.implement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Cmp;
using Presentation;

using Presentation.Implementation;
using System;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;
using DotNetEnv;
using Infra;
using Domain.IRepository;
using Application;
using Application.interfaces;
using Application.implementation;
using static Infra.orderDb.OrderDbContext;
using MassTransit;
using Application.Consumers;


Env.Load();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
try
{
    Console.WriteLine("🚀 Application starting...");

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = environment
    });
    Console.WriteLine($"🧭 Environment: {environment}");

    builder.Configuration.AddConfiguration(configuration);


    builder.Services.AddScoped<IServiceProvider, ServiceProvider>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IAppEnvironment,  AppEnvironment>();


    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();

    builder.Services.AddScoped<SymmetricSecurityKey>(sp =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:JWTKey"))));

    //builder.Services.AddScoped<IAppEnvironment, AppEnvironment>();
    //builder.Services.AddScoped<SeedData>();          //Add the SeedData class to the dependency injection container

    builder.Services.AddMassTransit(x =>
    {
        // ۱. جستجوی خودکار: تمام کانسومرهای موجود در لایه‌ای که کلاس Consumer قرار دارد را پیدا می‌کند
        var assembly = typeof(FoodItemCreatedConsumer).Assembly;
        x.AddConsumers(assembly);

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/");

            // ۲. جادوی اصلی: ساخت خودکار صف‌ها و Bindings بر اساس نام کانسومرها
            cfg.ConfigureEndpoints(context);
        });
    });

    builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;


    // Config MySQL 
    var connectionString = builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"];
    var serverVersion = ServerVersion.AutoDetect(connectionString);
   
    builder.Services.AddDbContext<OrderDb>(options =>
    {
        options.UseMySql(connectionString,serverVersion);
    });

    builder.Services.AddAutoMapper(typeof(Mapper));


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });

        // 🚀 تنظیمات مخصوص سرور اصلی
        options.AddPolicy("ProdCors", policy =>
        {
            policy.WithOrigins(
                    "https://imangilani.com",
                    "https://www.imangilani.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddSwaggerGen();

    var app = builder.Build();


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
        app.UseCors("AllowAll");
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

    // 6. Seed Data (در محیط توسعه یا یک‌بار)
    //using var scope = app.Services.CreateScope();
    //var services = scope.ServiceProvider;
    //var context = services.GetRequiredService<ResumeDb>();

    //Console.WriteLine("🔍 Checking if database exists...");
    //if (await context.Database.EnsureCreatedAsync())
    //{
    //    Console.WriteLine("✅ Database was created because it didn’t exist before.");
    //}
    //else
    //{
    //    Console.WriteLine("ℹ️ Database already exists.");
    //}

    //// حالا SeedData را اجرا کن
    //Console.WriteLine("Seeding data...");
    //await SeedData.Add(services,configuration);

    //Console.WriteLine("Migrations and seeding completed!");

    
  


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