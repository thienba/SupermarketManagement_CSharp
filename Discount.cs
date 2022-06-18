using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupermarketManagement
{
    public partial class Discount : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Cashier cashier;
        public Discount(Cashier cash)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            cashier = cash;
            txtDiscount.Focus();
            this.KeyPreview = true; //Bắt sự kiện nhấn phím thật (hoặc phím ảo của Windows)
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Discount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
            else if (e.KeyCode == Keys.Enter)
                btnConfirm.PerformClick();
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double disc = double.Parse(txtTotalPrice.Text) * double.Parse(txtDiscount.Text) * 0.01;
                txtDiscAmount.Text = disc.ToString("#,##0.00");
            }
            catch(Exception)
            {
                txtDiscAmount.Text = "0.00";
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if(txtDiscount.Text.Trim() == "")
            {
                Utilities.MessageWarning("Please enter discount."); 
            }
            else
            {
                try
                {
                    if (Utilities.MessageYesNo("Are you sure want to add discount?", "Notification") == DialogResult.Yes)
                    {
                        conn.Open();
                        cmd = new SqlCommand("UPDATE tbCart SET disc_percent = @disc_percent WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@disc_percent", double.Parse(txtDiscount.Text));
                        cmd.Parameters.AddWithValue("@id", int.Parse(lbId.Text));
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        cashier.LoadCart();
                        this.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    conn.Open();
                    Utilities.MessageInfo(ex.Message);
                }
            }
        }
    }
}
