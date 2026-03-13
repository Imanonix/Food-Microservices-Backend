using Application.AdminDTOs.user;
using Application.services.interfaces;
using Domain.models;
using Microsoft.AspNetCore.Identity;

namespace Presentation
{
    public class SeedData
    {
        public static async Task Add(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Console.WriteLine("🌱 SeedData.Add started...");
            using var scope = serviceProvider.CreateScope();
            var seedService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            string admin1Email, admin1Password;
            var userList = new List<AdminAddUserDTO>();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            if (env == "Production")
            {
                admin1Email = Environment.GetEnvironmentVariable("ADMIN1_EMAIL");
                admin1Password = Environment.GetEnvironmentVariable("ADMIN1_PASSWORD");
                var (password1Hash, salt1) =  authService.HashPassword(admin1Password);

                userList.Add(new AdminAddUserDTO { Email = admin1Email, UserName = "owner", PasswordHash = password1Hash, PasswordSalt = salt1, EmailConfirmed = true, Role = "owner", IsActive = true });
                authService.SaveChangesAsync();
            }
            else
            {
                admin1Email = configuration["SeedUsers:Admin1:Email"];
                admin1Password = configuration["SeedUsers:Admin1:Password"];
                var (password1Hash, salt1) = authService.HashPassword(admin1Password);
                

                userList.Add(new AdminAddUserDTO { Email = admin1Email, UserName = "owner", PasswordHash = password1Hash, PasswordSalt = salt1, EmailConfirmed = true, Role = "owner", IsActive = true });
                authService.SaveChangesAsync();
            }
            // Seed admin user
            foreach (var item in userList)
            {
                Console.WriteLine($"👤 Checking if '{item.Email}' exists...");

                var user = await seedService.GetUserByEmail(item.Email);
                if (user == null)
                {
                    Console.WriteLine("⚙️ Creating admin user...");
                    await seedService.AddUser(item);
                    Console.WriteLine("✅ Admin user created successfully!");
                }
                else
                {
                    Console.WriteLine($"✅ {item.Email} user already exists, skipping seed.");
                }

            }

        }
    }
}
