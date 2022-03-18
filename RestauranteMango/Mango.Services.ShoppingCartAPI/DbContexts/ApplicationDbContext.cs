using Mango.Services.ShoppingCartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<CartDetails> CartDetails { get; set; }

        public DbSet<CartHeader> CartHeaders { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Product>().HasData(new Product
        //    {
        //        ProductId = 1,
        //        Name = "Petacon",
        //        Price = 14,
        //        Description = "Mango presentativo de Veracruz",
        //        ImageUrl = "",
        //        CategoryName = "Appetizer"
        //    });

        //    modelBuilder.Entity<Product>().HasData(new Product
        //    {
        //        ProductId = 2,
        //        Name = "Petacon",
        //        Price = 10.99,
        //        Description = "Mango presentativo de Veracruz",
        //        ImageUrl = "",
        //        CategoryName = "Appetizer"
        //    });


        //    modelBuilder.Entity<Product>().HasData(new Product
        //    {
        //        ProductId = 3,
        //        Name = "Petacon",
        //        Price = 14.222,
        //        Description = "Mango presentativo de Veracruz",
        //        ImageUrl = "",
        //        CategoryName = "Dessert"
        //    });

        //    modelBuilder.Entity<Product>().HasData(new Product
        //    {
        //        ProductId = 4,
        //        Name = "Petacon",
        //        Price = 18,
        //        Description = "Mango presentativo de Veracruz",
        //        ImageUrl = "",
        //        CategoryName = "Entree"
        //    });
        //}
    }
}
