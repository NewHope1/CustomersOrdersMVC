using CustomersOrdersMVC.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomersOrdersMVC.Data
{
    public class CustomersOrdersContext : DbContext
    {
        public CustomersOrdersContext(DbContextOptions options)
            : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)      
        {

            modelBuilder.Entity<Customer>().ToTable("customers");
            modelBuilder.Entity<Order>().ToTable("orders");

            modelBuilder.Entity<Customer>()
                .HasMany(r => r.Orders)
                .WithOne(r => r.Customer)

                .HasForeignKey(r => r.CustomerID);
            //.HasPrincipalKey(r => r.CustomerID);


            modelBuilder.Entity<Customer>().HasKey(x => x.CustomerID);
            modelBuilder.Entity<Order>().HasKey(x => x.OrderID);

        }

        public DbSet<CustomersOrdersMVC.Entities.Customer> Customers { get; set; }
        public DbSet<CustomersOrdersMVC.Entities.Order> Orders { get; set; }
    }
}
