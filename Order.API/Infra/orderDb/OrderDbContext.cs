using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.models;
using DotNetEnv;

namespace Infra.orderDb
{
    public class OrderDbContext
    {
        public class OrderDb : DbContext
        {
            public OrderDb(DbContextOptions<OrderDb> options)
            : base(options)
            {
            }

            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderItem> OrderItems { get; set; }
            public DbSet<FoodItem> FoodItems { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<OrderItem>()
                    .HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(u => u.OrderId)
                    .OnDelete(DeleteBehavior.Cascade); 
            }
        }


        public class BloggingContextFactory : IDesignTimeDbContextFactory<OrderDb>
        {
            public OrderDb CreateDbContext(string[] args)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                string connectionString;
                Env.Load();
                Console.WriteLine(environment);
                if (environment == "Development")
                {
                    // 🔥 هاردکد مخصوص محیط توسعه
                    connectionString = "Server=localhost;Port=3306;Database=food_order;User=root;Password=iman44433.7aban;CharSet=utf8mb4";
                }
                else
                {
                    // 🔥 از Environment Variable در محیط Production
                    connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");

                    Console.WriteLine("Using ENVIRONMENT VARIABLE connection string:");
                    Console.WriteLine(connectionString);
                }
                //var connectionString = "Server=localhost;Port=3306;Database=food_order;User=root;Password=iman44433.7aban;CharSet=utf8mb4";
                var optionsBuilder = new DbContextOptionsBuilder<OrderDb>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                return new OrderDb(optionsBuilder.Options);
            }
        }
    }
}
