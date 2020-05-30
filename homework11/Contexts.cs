using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp
{
    class OrderContexts: DbContext
    {
        public OrderContexts() : base("OrderDatabase")
        {
            Database.SetInitializer(
            new DropCreateDatabaseIfModelChanges<OrderContexts>());
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Goods> GoodItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
