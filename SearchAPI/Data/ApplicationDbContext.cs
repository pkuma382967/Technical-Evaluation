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
                        Date = new DateTime(2024, 5, 10),
                        Popularity = 95,
                        Relevance = 0.92f,
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Smartphone",
                        Description = "5G smartphone with 128GB storage",
                        Price = 800.00m,
                        Date = new DateTime(2024, 7, 3),
                        Popularity = 120,
                        Relevance = 0.95f,
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Tablet",
                        Description = "10-inch tablet with stylus support",
                        Price = 500.00m,
                        Date = new DateTime(2024, 6, 15),
                        Popularity = 80,
                        Relevance = 0.85f,
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Headphones",
                        Description = "Noise-cancelling wireless headphones",
                        Price = 250.00m,
                        Date = new DateTime(2024, 8, 1),
                        Popularity = 110,
                        Relevance = 0.88f,
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Smartwatch",
                        Description = "Fitness tracker with heart rate monitor",
                        Price = 200.00m,
                        Date = new DateTime(2024, 4, 20),
                        Popularity = 70,
                        Relevance = 0.82f,
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Camera",
                        Description = "Mirrorless camera with 4K video",
                        Price = 900.00m,
                        Date = new DateTime(2024, 9, 5),
                        Popularity = 65,
                        Relevance = 0.80f,
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Gaming Console",
                        Description = "Next-gen gaming console with 1TB storage",
                        Price = 500.00m,
                        Date = new DateTime(2024, 11, 12),
                        Popularity = 130,
                        Relevance = 0.96f,
                    },
                    new Product
                    {
                        Id = 8,
                        Name = "Monitor",
                        Description = "27-inch 4K monitor",
                        Price = 400.00m,
                        Date = new DateTime(2024, 3, 18),
                        Popularity = 60,
                        Relevance = 0.78f,
                    },
                    new Product
                    {
                        Id = 9,
                        Name = "Keyboard",
                        Description = "Mechanical gaming keyboard",
                        Price = 120.00m,
                        Date = new DateTime(2024, 10, 2),
                        Popularity = 85,
                        Relevance = 0.83f,
                    },
                    new Product
                    {
                        Id = 10,
                        Name = "Mouse",
                        Description = "Wireless ergonomic mouse",
                        Price = 80.00m,
                        Date = new DateTime(2024, 2, 28),
                        Popularity = 75,
                        Relevance = 0.79f,
                    }
                );
        }
    }
}
