using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OrderApp {

  /**
   * The service class to manage orders
   * */
  public class OrderService {

    //不需要再使用List存数据了
    //the order list
   // private List<Order> orders;


    public OrderService() {
      //orders = new List<Order>();
    }

        /* public List<Order> Orders {
           get => orders;
         }
         */
        //

        private static IQueryable<Order> AllOrders(OrderContexts db)
        {
            return db.Orders.Include(o => o.Items.Select(i => i.GoodsItem))
                      .Include("Customer");
        }


        public static List<Order> GetAllOrders()
        {
            using (var db = new OrderContexts())
            {
                return AllOrders(db).ToList();
            }
        }
        public static Order GetOrder(string id) {
            using (var db = new OrderContexts())
            {
                return AllOrders(db).FirstOrDefault(o => o.Id == id);
            }
        }

        public static Order AddOrder(Order order)
        {
            try
            {
                using (var db = new OrderContexts())
                {
                    db.Orders.Add(order);
                    db.SaveChanges();
                }
                return order;
            }
            catch (Exception e)
            {
                
                throw new ApplicationException($"添加错误: {e.Message}");
            }
        }

        public static void RemoveOrder(string id)
        {
            try
            {
                using (var db = new OrderContexts())
                {
                    var order = db.Orders.Include("Items").Where(o => o.Id == id).FirstOrDefault();
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                //TODO 需要更加错误类型返回不同错误信息
                throw new ApplicationException($"删除订单错误!");
            }
        }

        public static List<Order> QueryOrdersByGoodsName(string goodsName)
        {
            using (var db = new OrderContexts())
            {
                var query = AllOrders(db)
                  .Where(o => o.Items.Count(i => i.GoodsItem.Name == goodsName) > 0);
                return query.ToList();
            }
        }

        public static List<Order> QueryOrdersByCustomerName(string customerName)
        {
            using (var db = new OrderContexts())
            {
                var query = AllOrders(db)
                  .Where(o => o.Customer.Name == customerName);
                return query.ToList();
            }
        }


        private static void RemoveItems(string orderId)
        {
            using (var db = new OrderContexts())
            {
                var oldItems = db.OrderItems.Where(item => item.OrderId == orderId);
                db.OrderItems.RemoveRange(oldItems);
                db.SaveChanges();
            }
        }
        public static void UpdateOrder(Order newOrder)
        {
            RemoveItems(newOrder.Id);
            using (var db = new OrderContexts())
            {
                db.Entry(newOrder).State = EntityState.Modified;
                db.OrderItems.AddRange(newOrder.Items);
                db.SaveChanges();
            }
        }



        public static void Export(String fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Order>));
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                xs.Serialize(fs, GetAllOrders());
            }
        }

        public static void Import(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Order>));
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                List<Order> temp = (List<Order>)xs.Deserialize(fs);
                temp.ForEach(order => {
                    try
                    {
                        AddOrder(order);
                    }
                    catch
                    {
                      
                    }
                });
            }
        }

        public static object QueryByTotalAmount(float amout)
        {
            using (var db = new OrderContexts())
            {
                return AllOrders(db)
                  .Where(o => o.Items.Sum(item => item.GoodsItem.Price * item.Quantity) > amout)
                  .ToList();
            }
        }
    }

    public class CustomerService {

        public static void Add(Customer customer)
        {
            try
            {
                using (var db = new OrderContexts())
                {
                    //获取对象，进行更改，保存更改，EF框架会更改对应的数据项
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
              
                throw new ApplicationException($"添加错误!");
            }
        }

        public static List<Customer> GetAll()
        {
            using (var db = new OrderContexts())
            {
                return db.Customers.ToList();
            }
        }
    }

    public class GoodsService {

        //获取对象，进行更改，保存更改，EF框架会更改对应的数据项
        public static void Add(Goods goods) {
      try {
        using (var db = new OrderContexts()) {
          db.GoodItems.Add(goods);
          db.SaveChanges();
        }
      }
      catch (Exception e) {
       
        throw new ApplicationException($"添加错误!");
      }
    }

    public static List<Goods> GetAll() {
      using (var db = new OrderContexts()) {
        return db.GoodItems.ToList();
      }
    }
  } 

}
