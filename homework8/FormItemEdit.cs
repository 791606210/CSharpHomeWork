using OrderManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework8
{
    public partial class FormItemEdit : Form
    {
        public OrderItem OrderItem { get; set; }

        public FormItemEdit()
        {
            InitializeComponent();
        }
        public FormItemEdit(OrderItem orderItem) : this()
        {
            this.OrderItem = orderItem;
            this.ItemBindingSource.DataSource = orderItem;
            goodsBindingSource.Add(new Goods("1", "苹果", 100.0));
            goodsBindingSource.Add(new Goods("2", "鸡蛋", 200.0));
            goodsBindingSource.Add(new Goods("3", "牛奶", 300.0));
            goodsBindingSource.Add(new Goods("4", "面包", 200.0));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemBindingSource.ResetBindings(false);
        }
    }
}
