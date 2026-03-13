using Domain;
using Domain.models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Infra.identityDb
{
    public class IdentityDb : DbContext
    {
        public IdentityDb(DbContextOptions<IdentityDb> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserAddress>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(u => u.UserId);
        }
    }
   

    public class BloggingContextFactory : IDesignTimeDbContextFactory<IdentityDb>
    {
        public IdentityDb CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            string connectionString;
            Env.Load();
            Console.WriteLine(environment);
            if (environment == "Development")
            {
                // 🔥 هاردکد مخصوص محیط توسعه
                connectionString = "Server=localhost;Port=3306;Database=food_identity;User=root;Password=iman44433.7aban;CharSet=utf8mb4";
            }
            else
            {
                // 🔥 از Environment Variable در محیط Production
                connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

                Console.WriteLine("Using ENVIRONMENT VARIABLE connection string:");
                Console.WriteLine(connectionString);
            }
            //var connectionString = "Server=localhost;Port=3306;Database=food_Identity;User=root;Password=iman44433.7aban;CharSet=utf8mb4";
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDb>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new IdentityDb(optionsBuilder.Options);
        }
    }
}

