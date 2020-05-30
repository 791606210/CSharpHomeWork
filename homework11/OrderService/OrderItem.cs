using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp {

  /**
   **/
  public class OrderItem {
        
        [Key]
        //为订单项增添一个新的唯一标识的ID
        public string Id { get; set; }

        public string GoodsItemId { get; set; }

        public uint Index { get; set; } //序号

        //添加外键
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("GoodsItemId")]
    public Goods GoodsItem { get; set; }

    public String GoodsName { get => GoodsItem!=null? this.GoodsItem.Name:""; }

    public double UnitPrice { get => GoodsItem != null ? this.GoodsItem.Price : 0.0; }

        //订单项对应所在的订单
        public string OrderId { get; set; }


        public uint Quantity { get; set; }

    public OrderItem() {
            Id = Guid.NewGuid().ToString();
        }

    public OrderItem(uint index, Goods goods, uint quantity) {
      this.Index = index;
      this.GoodsItem = goods;
      this.Quantity = quantity;
    }

    public double TotalPrice {
      get => GoodsItem==null?0.0: GoodsItem.Price * Quantity;
    }

    public override string ToString() {
      return $"[No.:{Index},goods:{GoodsName},quantity:{Quantity},totalPrice:{TotalPrice}]";
    }

    public override bool Equals(object obj) {
      var item = obj as OrderItem;
      return item != null &&
             GoodsName == item.GoodsName;
    }

    public override int GetHashCode() {
      var hashCode = -2127770830;
      hashCode = hashCode * -1521134295 + Index.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GoodsName);
      hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
      return hashCode;
    }
  }
}
