using Microsoft.EntityFrameworkCore;

namespace TypicalTechTools.Models.Data
{
    public class TypistTechToolsDBContext : DbContext
    {
        public TypistTechToolsDBContext(DbContextOptions options) : base(options)
        {
        
        
        
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasData(
               new AppUser
               {
                   Id = 1,
                   Email = "jason@gmail.com",
                   UserName = "Test",
                   Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1"),
                   Role = "ADMIN"
               }



               );

            builder.Entity<Product>().HasData(
                new Product { Id = 12345, Name = "Generic Headphones", Price = 84.99m, Description = "bluetooth headphones with fair battery life and a 1 month warranty", UpdatedDate=default(DateTime) },
                new Product { Id = 12346, Name = "Expensive Headphones", Price = 149.99m, Description = "bluetooth headphones with good battery life and a 6 month warranty", UpdatedDate = default(DateTime) },
                new Product { Id = 12347, Name = "Name Brand Headphones", Price = 199.99m, Description = "bluetooth headphones with good battery life and a 12 month warranty", UpdatedDate = default(DateTime) },
                new Product { Id = 12348, Name = "Generic Wireless Mouse", Price = 39.99m, Description = "simple bluetooth pointing device", UpdatedDate = default(DateTime) },
                new Product { Id = 12349, Name = "Logitech Mouse and Keyboard", Price = 73.99m, Description = "mouse and keyboard wired combination", UpdatedDate = default(DateTime) },
                new Product { Id = 12350, Name = "Logitech Wireless Mouse", Price = 149.99m, Description = "quality wireless mouse", UpdatedDate = default(DateTime) }

                );

            builder.Entity<Comment>().HasData(
                new Comment { Id = 1, Text = "This is a great product. Highly Recommended.", ProductId = 12345 },
                new Comment { Id = 2, Text = "Not worth the excessive price. Stick with a cheaper generic one.", ProductId = 12350 },
                new Comment { Id = 3, Text = "A great budget buy. As good as some of the expensive alternatives.", ProductId = 12345 },
                new Comment { Id = 4, Text = "Total garbage. Never buying this brand again.", ProductId = 12347 }
                );
        }

    }
}
