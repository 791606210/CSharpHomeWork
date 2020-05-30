using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;



namespace program
{
    public class OrderServiceSource
    {
        public class MatchException : Exception
        {
            private int code;
            public int Code { get => code; }
            public MatchException(string message, int code) : base(message)
            {
                this.code = code;
            }
        }

        public class OrderService
        {
            private List<Order> orders;
            public List<Order> Orders { get { return orders; } }
            public int order_amount { get => orders.Count; }

            public OrderService()
            {
                orders = new List<Order>();
            }

            private bool CheckOrder(Order order)
            {
                foreach (Order order_this in orders)
                {
                    if (order_this.Equals(order))
                        return true;
                }
                return false;
            }

            private Order FindOrderID(int ID)
            {
                foreach (Order order_this in orders)
                {
                    if (order_this.ID == ID)
                    {
                        return order_this;
                    }
                }
                return null;
            }

            public void AddOrder(Order order)
            {
                if (CheckOrder(order))
                {
                    throw new MatchException("Add order match error!", 1);
                }
                orders.Add(new Order(order));
            }

            public void AddOrderItem(int ID, OrderItem orderitem)
            {
                Order order = FindOrderID(ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                order.AddOrderItem(orderitem);
            }

            public void DeleteOrder(int ID)
            {
                Order order = FindOrderID(ID);
                if (order == null)
                {
                    throw new MatchException("Delete order match error!", 2);
                }
                orders.Remove(order);
            }

            public void DeleteOrderItem(int Order_ID, int OrderItem_ID)
            {
                Order order = FindOrderID(Order_ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                order.DeleteOrderItem(OrderItem_ID);
            }

            public void UpdateOrder(int ID, Order order)
            {
                DeleteOrder(ID);
                AddOrder(order);
            }

            public void UpdateOrderItem(int Order_ID, int OrderItem_ID, OrderItem orderitem)
            {
                Order order = FindOrderID(Order_ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                order.DeleteOrderItem(OrderItem_ID);
                order.AddOrderItem(orderitem);
            }

            public Order FindOrder(int ID)
            {
                Order order = FindOrderID(ID);
                if(order == null)
                {
                    throw new MatchException("Delete order match error!", 2);
                }
                return new Order(order);
            }

            public List<Order> FindOrder(Func<Order, bool> func)
            {
                var query = orders.Where(func).OrderBy(order => order.TotalPrice);
                return query.ToList<Order>();
            }

            public List<Order> FindOrder(string CustomerName)
            {
                var query = orders.Where(order => order.CustomerName == CustomerName).OrderBy(order => order.TotalPrice);
                return query.ToList<Order>();
            }
            
            public OrderItem FindOrderItem(int Order_ID, int OrderItem_ID)
            {
                Order order = FindOrderID(Order_ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                OrderItem orderitem = order.FindOrderItem(OrderItem_ID);
                return orderitem;
            }

            public List<OrderItem> FindOrderItem(int Order_ID, Func<OrderItem, bool> func)
            {
                Order order = FindOrderID(Order_ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                var query = order.orderitems.Where(func).OrderBy(orderitem => orderitem.Price * orderitem.Quantity);
                return query.ToList<OrderItem>();
            }
            
            public List<OrderItem> FindOrderItem(int Order_ID, string ProductName)
            {
                Order order = FindOrderID(Order_ID);
                if (order == null)
                {
                    throw new MatchException("Order match error!", 0);
                }
                var query = order.orderitems.Where(orderitem => orderitem.ProductName == ProductName)
                    .OrderBy(orderitem => orderitem.Price * orderitem.Quantity);
                return query.ToList();
            }
            
            public void SortOrder()
            {
                orders.Sort((order1, order2) => order1.ID - order2.ID);
            }

            public void SortOrder(Func<Order, Order, int> func)
            {
                orders.Sort((order1, order2) => func(order1, order2));
            }

            public Order GetFirst()
            {
                if (order_amount > 0)
                    return new Order(orders[0]);
                else
                    return null;
            }

            public void Export(string Filename)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Order>));
                using (FileStream fs = new FileStream(Filename, FileMode.Create))
                {
                    xmlSerializer.Serialize(fs, orders);
                }
            }

            public void Export()
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Order>));
                using (FileStream fs = new FileStream("OrderList.xml", FileMode.Create))
                {
                    xmlSerializer.Serialize(fs, orders);
                }
            }

            public void Import(string Filename)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Order>));
                using (FileStream fs = new FileStream(Filename, FileMode.Open))
                {
                    orders = (List<Order>)xmlSerializer.Deserialize(fs);
                }
            }
        }

        [Serializable]
        public class Order
        {
            public int ID { get; set; }
            public string CustomerName { get; set; }
            public double TotalPrice
            {
                get
                {
                    var query = from orderitem in orderitems
                                select orderitem.Price * orderitem.Quantity;
                    return query.Sum();
                }
            }
            public List<OrderItem> orderitems;

            [XmlIgnore]
            public List<OrderItem> OrderItems { get { return orderitems; } }

            public Order(int ID, string CustomerName)
            {
                this.ID = ID;
                this.CustomerName = CustomerName;
                orderitems = new List<OrderItem>();
            }

            public Order(Order order)
            {
                ID = order.ID;
                CustomerName = order.CustomerName;
                orderitems = new List<OrderItem>();
                foreach (OrderItem orderItem in order.orderitems)
                {
                    orderitems.Add(new OrderItem(orderItem));
                }
            }

            public Order()
            {
                orderitems = new List<OrderItem>();
            }

            public override bool Equals(object obj)
            {
                Order order = obj as Order;
                return order != null && ID == order.ID;
            }

            public override int GetHashCode()
            {
                var hashCode = -221969603;
                hashCode = hashCode * -1521134295 + ID.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CustomerName);
                hashCode = hashCode * -1521134295 + TotalPrice.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<List<OrderItem>>.Default.GetHashCode(orderitems);
                return hashCode;
            }

            public override string ToString()
            {
                return $"Order_ID: {ID} CustomerName: {CustomerName} TotalPrice: {TotalPrice}";
            }

            private bool CheckOrderItem(OrderItem orderitem)
            {
                foreach (OrderItem orderitem_this in orderitems)
                {
                    if (orderitem_this.Equals(orderitem))
                    {
                        return true;
                    }
                }
                return false;
            }

            private OrderItem FindOrderItemID(int ID)
            {
                foreach (OrderItem orderitem in orderitems)
                {
                    if (orderitem.ID == ID)
                    {
                        return orderitem;
                    }
                }
                return null;
            }
            
            public OrderItem FindOrderItem(int ID)
            {
                OrderItem orderitem = FindOrderItemID(ID);
                if (orderitem == null)
                {
                    throw new MatchException("OrderItem match error!", 3);
                }
                return new OrderItem(orderitem);
            }

            public void AddOrderItem(OrderItem orderitem)
            {
                if (CheckOrderItem(orderitem))
                {
                    throw new MatchException("Add orderItem match error!", 4);
                }
                orderitems.Add(new OrderItem(orderitem));
            }

            public void DeleteOrderItem(int ID)
            {
                OrderItem orderitem = FindOrderItemID(ID);
                if (orderitem == null)
                {
                    throw new MatchException("Delete orderItem match error!", 5);
                }
                orderitems.Remove(orderitem);
            }
        }

        [Serializable]
        public class OrderItem
        {
            public int ID { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }

            public OrderItem() { }

            public OrderItem(int ID, string ProductName, double Price, int Quantity)
            {
                this.ID = ID;
                this.ProductName = ProductName;
                this.Price = Price;
                this.Quantity = Quantity;
            }

            public OrderItem(OrderItem orderitem)
            {
                ID = orderitem.ID;
                ProductName = orderitem.ProductName;
                Price = orderitem.Price;
                Quantity = orderitem.Quantity;
            }

            public override bool Equals(object obj)
            {
                OrderItem orderitem = obj as OrderItem;
                return orderitem != null && ID == orderitem.ID;
            }

            public override int GetHashCode()
            {
                var hashCode = -1799128887;
                hashCode = hashCode * -1521134295 + ID.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProductName);
                hashCode = hashCode * -1521134295 + Price.GetHashCode();
                hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
                return hashCode;
            }

            public override string ToString()
            {
                return $"OrderItem_ID: {ID} ProductName: {ProductName} Price: {Price} Quantity: {Quantity}";
            }
        }

        static void Main(string[] args)
        {
            OrderService orderService = new OrderService();
            Order order = new Order(0, "customer1");
            Console.WriteLine(order.TotalPrice);
            for(int i = 0; i < 10; i++)
            {
                order.AddOrderItem(new OrderItem(i, "product" + i, 300, 10));
            }
            Console.WriteLine(order.TotalPrice);
            orderService.AddOrder(order);
            orderService.Export();
            OrderService orderService_1 = new OrderService();
            orderService_1.Import("OrderList.xml");
            Console.WriteLine(orderService_1.GetFirst().ToString());
            Console.ReadKey();
        }
    }
}
