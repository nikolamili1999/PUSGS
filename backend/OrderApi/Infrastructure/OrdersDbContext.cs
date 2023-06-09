using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace Backend.Infrastructure
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions options): base(options) { 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Kazemo mu da pronadje sve konfiguracije u Assembliju i da ih primeni nad bazom
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
