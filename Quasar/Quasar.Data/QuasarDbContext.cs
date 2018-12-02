using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quasar.Models;

namespace Quasar.Data
{
    public class QuasarDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public QuasarDbContext() { }

        public QuasarDbContext(DbContextOptions<QuasarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<UserProduct> UsersProducts { get; set; }

        public DbSet<OrderProduct> OrdersProducts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18,2)");

            base.OnModelCreating(builder);
        }

    }
}
