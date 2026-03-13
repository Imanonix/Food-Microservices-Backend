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
using MongoDB.Driver;
using Domain.IRepository;
using Application;
using Application.interfaces;
using Application.implementation;
using MassTransit;


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
    builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
    builder.Services.AddScoped<ICatalogService, CatalogService>();
    builder.Services.AddScoped<IAppEnvironment,  AppEnvironment>();


    builder.Services.AddHttpContextAccessor();
    builder.Host.UseSerilog();

    builder.Services.AddScoped<SymmetricSecurityKey>(sp =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:JWTKey"))));

    //builder.Services.AddScoped<IAppEnvironment, AppEnvironment>();
    //builder.Services.AddScoped<SeedData>();          //Add the SeedData class to the dependency injection container



    builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;


    builder.Services.AddAutoMapper(typeof(Mapper));

    //Add MongoDb
    builder.Services.Configure<CatalogDatabaseSettings>(     // map appsetting MyMongoConfig section to CatalogDatabaseSettings class
        builder.Configuration.GetSection("MyMongoConfig"));

    builder.Services.AddSingleton<IMongoClient>(sp =>
    {
        var settings = builder.Configuration.GetSection("MyMongoConfig").Get<CatalogDatabaseSettings>()?? throw new Exception("error in setting");
        return new MongoClient(settings.ConnectionStrings); 
    });

    // Add Redis 
    builder.Services.AddStackExchangeRedisCache(option =>
    {
        option.Configuration = builder.Configuration.GetSection("MyRedisSetting")["ConnectionString"];
        option.InstanceName = "Catalog_";  // نشان میدهد که این کلید ها برای سرویس کاتالوگ هستند. این جوری کلید های سرویس کاتالوگ با بقیه سرویس ها قاطی نمیشه
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