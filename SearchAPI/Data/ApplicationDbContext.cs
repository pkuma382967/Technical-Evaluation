using Microsoft.EntityFrameworkCore;
using SearchAPI.Models.Domain;

namespace SearchAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        // seed some data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Admin User
            var adminUser = new User
            {
                Id = 1, // Ensure the Id is unique and manually assigned.
                Username = "admin",
                PasswordHash = User.HashPassword("password123"), // Hash the password before saving
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");

            // Seeding data for Products
            modelBuilder
                .Entity<Product>()
                .HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "Laptop",
                        Description = "High-performance laptop with 16GB RAM",
                        Price = 1200.00m,
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Smartphone",
                        Description = "5G smartphone with 128GB storage",
                        Price = 800.00m,
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Tablet",
                        Description = "10-inch tablet with stylus support",
                        Price = 500.00m,
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Headphones",
                        Description = "Noise-cancelling wireless headphones",
                        Price = 250.00m,
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Smartwatch",
                        Description = "Fitness tracker with heart rate monitor",
                        Price = 200.00m,
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Camera",
                        Description = "Mirrorless camera with 4K video",
                        Price = 900.00m,
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Gaming Console",
                        Description = "Next-gen gaming console with 1TB storage",
                        Price = 500.00m,
                    },
                    new Product
                    {
                        Id = 8,
                        Name = "Monitor",
                        Description = "27-inch 4K monitor",
                        Price = 400.00m,
                    },
                    new Product
                    {
                        Id = 9,
                        Name = "Keyboard",
                        Description = "Mechanical gaming keyboard",
                        Price = 120.00m,
                    },
                    new Product
                    {
                        Id = 10,
                        Name = "Mouse",
                        Description = "Wireless ergonomic mouse",
                        Price = 80.00m,
                    }
                );
        }
    }
}
