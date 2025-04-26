using Microsoft.EntityFrameworkCore;

namespace OrderService
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
