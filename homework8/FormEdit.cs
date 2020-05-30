using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderManagement;

namespace Homework8
{
    public partial class FormEdit : Form
    {
        public Order CurrentOrder { get; set; }
        

        public FormEdit()
        {
            InitializeComponent();
            
            customerBindingSource.Add(new Customer("1", "杨先生"));
            customerBindingSource.Add(new Customer("2", "张先生"));
            customerBindingSource.Add(new Customer("3", "王女士"));
            customerBindingSource.Add(new Customer("4", "刘先生"));
        }

        public FormEdit(Order order, bool editMode = false) : this()
        {
            CurrentOrder = order;
            orderBindingSource.DataSource = CurrentOrder;
            txtOrderId.Enabled = !editMode;
            if (!editMode)
            {
                CurrentOrder.Customer = (Customer)customerBindingSource.Current;
            }
        }

        private void dgvItems_CellContentDoubleClick(object sender, EventArgs e)
        {
            EditItem();
        }

        private void EditItem()
        {
            OrderItem orderItem = itemsBindingSource.Current as OrderItem;
            if (orderItem == null)
            {
                MessageBox.Show("请选择一个订单项进行修改");
                return;
            }
            FormItemEdit formItemEdit = new FormItemEdit(orderItem);
            if (formItemEdit.ShowDialog() == DialogResult.OK)
            {
                itemsBindingSource.ResetBindings(false);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            FormItemEdit formItemEdit = new FormItemEdit(new OrderItem());
            try
            {
                if (formItemEdit.ShowDialog() == DialogResult.OK)
                {
                    uint index = 0;
                    if (CurrentOrder.Items.Count != 0)
                    {
                        index = CurrentOrder.Items.Max(i => i.Index) + 1;
                    }
                    formItemEdit.OrderItem.Index = index;
                    CurrentOrder.AddItem(formItemEdit.OrderItem);
                    itemsBindingSource.ResetBindings(false);
                }
            }
            catch (Exception e2)
            {
                MessageBox.Show(e2.Message);
            }

        }

        private void btnEditItem_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        private void btnDeleteItem_Click(object sender, EventArgs e)
        {
            OrderItem orderItem = itemsBindingSource.Current as OrderItem;
            if (orderItem == null)
            {
                MessageBox.Show("请选择一个订单项进行删除");
                return;
            }
            CurrentOrder.RemoveItem(orderItem);
            itemsBindingSource.ResetBindings(false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
