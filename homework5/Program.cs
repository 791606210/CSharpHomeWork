using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement
{
        public class Order : IComparable<Order>
        {
            public int orderID { set; get; }
            public string orderCustomer { set; get; }
            public string customerAddress { set; get; }

            public List<OrderItem> orderItem = new List<OrderItem>();

            public DateTime time = DateTime.Now;
            public Order(int ID, string customer, string address, List<OrderItem> item)
            {
                orderID = ID;
                orderCustomer = customer;
                customerAddress = address;
                orderItem = (orderItem == null) ? new List<OrderItem>() : item;
            }
            public List<OrderItem> items
            {
                get { return orderItem; }
            }
            public void AddItem(OrderItem orderItem)
            {
                if (this.orderItem.Contains(orderItem))
                    throw new ApplicationException($"orderItem-{orderItem} is already existed!");
                this.orderItem.Add(orderItem);
            }
            public void RemoveItem(OrderItem orderItem)
            {
                this.orderItem.Remove(orderItem);
            }
            public double TotalPrice
            {
                get => items.Sum(item => item.TotalPrice);
            }
            public override bool Equals(object obj)
            {
                var order = obj as Order;
                return order != null && orderID == order.orderID;
            }
            public override int GetHashCode()
            {
                var hashCode = -531220479;
                hashCode = hashCode * -1521134295 + orderID.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(orderCustomer);
                hashCode = hashCode * -1521134295 + time.GetHashCode();
                return hashCode;
            }
            public int CompareTo(Order other)
            {
                if (other == null) return 1;
                return this.orderID.CompareTo(other.orderID);
            }
            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append($"Id:{orderID}, customer:{orderCustomer},orderTime:{time},totalPrice：{TotalPrice}");
                items.ForEach(od => strBuilder.Append("\n\t" + od));
                return strBuilder.ToString();
            }
        }

        public class OrderItem
        {
            public uint itemIndex { get; set; } //序号
            public double itemPrice { set; get; }
            public int itemQuantity { set; get; }
            public string itemName { set; get; }
            public OrderItem() { }
            public OrderItem(uint index, double price, int number, string name)
            {
                itemIndex = index;
                itemName = name;
                itemQuantity = number;
                itemPrice = price;
            }
            public double TotalPrice
            {
                get => itemPrice * itemQuantity;
            }
            public override string ToString()
            {
                return $"[No.:{itemIndex},goods:{itemName},quantity:{itemQuantity},totalPrice:{TotalPrice}]";
            }
            public override bool Equals(object obj)
            {
                var item = obj as OrderItem;
                return item != null &&
                       itemName == item.itemName;
            }
            public override int GetHashCode()
            {
                var hashCode = -2127770830;
                hashCode = hashCode * -1521134295 + itemIndex.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(itemName);
                hashCode = hashCode * -1521134295 + itemQuantity.GetHashCode();
                return hashCode;
            }
        }

        public class OrderService
        {
            public class MyComparer : IComparer<Order>
            {
                public int Compare(Order order1, Order order2)
                {
                    return order1.orderID - order2.orderID;
                }
            }
            public List<Order> Orders = new List<Order>();
            public Order GetOrder(int id)
            {
                return Orders.Where(o => o.orderID == id).FirstOrDefault();
            }
            public void addOrder(Order order)
            {
                if (Orders.Contains(order))
                    throw new ApplicationException($"Add Order Error: Order with id {order.orderID} already exists!");
                else
                {
                    Orders.Add(order);
                }
            }//添加一个新订单
            public void UpdateOrder(Order newOrder)
            {
                Order oldOrder = GetOrder(newOrder.orderID);
                if (oldOrder == null)
                    throw new ApplicationException($"Update Error：the order with id {newOrder.orderID} does not exist!");
                Orders.Remove(oldOrder);
                Orders.Add(newOrder);
            }//更改订单的内容
            public void removeOrder(int orderID)
            {
                Order order = GetOrder(orderID);
                if (Orders.Where(a => a.orderID == orderID).Count() == 0)
                    throw new ApplicationException($"Add Order Error: Order with id {order.orderID} does not exists!");
                else
                    Orders.Remove(order);
            }//移除一个订单
            public void Sort()
            {
                Orders.Sort();
            }

        public void Sort(Func<Order, Order, int> func)
        {
            Orders.Sort((o1, o2) => func(o1, o2));
        }

        public List<Order> QueryOrdersByGoodsName(string goodsName)
        {
            var query = Orders
                    .Where(order => order.orderItem.Exists(item => item.itemName == goodsName))
                    .OrderBy(o => o.TotalPrice);
            return query.ToList();
        }

        public List<Order> QueryOrdersByCustomerName(string customerName)
        {
            return Orders
                .Where(order => order.orderCustomer == customerName)
                .OrderBy(o => o.TotalPrice)
                .ToList();
        }



        //根据订单号查询一个订单
        public void sortOrder()
            {
                Orders.Sort(new MyComparer());
            }//默认的更具订单号进行排序

            
        }
        class Program
    {
        static void Main(string[] args)
        {
            try
            {
                OrderItem apple = new OrderItem(1, 10.0,80, "apple" );//public OrderItem(uint index, int price, int number, string name)
                OrderItem egg = new OrderItem(2, 1.2, 200, "eggs");
                OrderItem milk = new OrderItem(3, 50, 10, "milk");

                //public Order(int ID, string customer, string address, List<OrderItem> item)
                Order order1 = new Order(1, "Customer1","ChangSha", new List<OrderItem> { apple, egg, milk });
                Order order2 = new Order(2, "Customer2", "Wuhan",new List<OrderItem> { egg, milk });
                Order order3 = new Order(3, "Customer2","HongKong",  new List<OrderItem> { apple, milk });

                OrderService os = new OrderService();
                os.addOrder(order1);
                os.addOrder(order2);
                os.addOrder(order3);

                Console.WriteLine("GetAllOrders");
                List<Order> orders = os.Orders;
                orders.ForEach(o => Console.WriteLine(o));
                Console.WriteLine("");

                os.Sort((o1, o2) => o1.TotalPrice.CompareTo(o2.TotalPrice));
                Console.WriteLine("GetAllOrders After sort");
                orders = os.Orders;
                orders.ForEach(o => Console.WriteLine(o));
                Console.WriteLine("");

                Console.WriteLine("GetOrdersByCustomerName:'Customer2'");
                orders = os.QueryOrdersByCustomerName("Customer2");
                orders.ForEach(o => Console.WriteLine(o));
                Console.WriteLine("");

                Console.WriteLine("GetOrdersByGoodsName:'apple'");
                orders = os.QueryOrdersByGoodsName("apple");
                orders.ForEach(o => Console.WriteLine(o));
                Console.WriteLine("");

                Console.WriteLine("Remove order(id=2) and qurey all");
                os.removeOrder(2);
                orders.ForEach(o => Console.WriteLine(o));
                Console.WriteLine("");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
    }
    }
}
