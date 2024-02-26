using E_commerce_FYP_backend.Models.AmazonNodes;
using E_commerce_FYP_backend.Models.CartProducts;
using E_commerce_FYP_backend.Models.NodeProducts;
using E_commerce_FYP_backend.Models.Orders;
using E_commerce_FYP_backend.Models.Users;
using Microsoft.EntityFrameworkCore;


namespace E_commerce_FYP_backend.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AmazonNodes> AmazonNodes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<NodeProducts> NodeProducts { get; set; }
        public DbSet<CartProducts> CartProducts { get; set; }
        public DbSet<Orders> Orders { get; set; }
    }
}
