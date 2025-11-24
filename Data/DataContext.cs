using Microsoft.EntityFrameworkCore;
using Foodopia.Models;

namespace Foodopia.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Product -> Admin (Many-to-One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Admin)
                .WithMany() // An admin can have many products
                .HasForeignKey(p => p.Admin_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Order -> Product
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.Product_ID)
                .OnDelete(DeleteBehavior.Cascade);

            // Order -> User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.User_ID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
