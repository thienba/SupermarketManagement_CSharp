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
    public partial class Void : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        CancelOrder cancelOrder;
        public Void(CancelOrder cancel)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            txtUsername.Focus();
            cancelOrder = cancel;
        }

        private void btnVoid_Click(object sender, EventArgs e)
        {
            try
            {
                if(txtUsername.Text.ToLower().Trim() != "" && txtPass.Text.ToLower().Trim() != "")
                {
                    if (txtUsername.Text.ToLower() == cancelOrder.txtCancelBy.Text.ToLower())
                    {
                        Utilities.MessageWarning("Void by name and cancelled by name are same! Please void by another person.");
                        return;
                    }
                }
                else
                {
                    Utilities.MessageWarning("Please enter username and password!");
                }
                string user;
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM tbUser WHERE username = @username and password = @password", conn);
                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@password", Encode_Decode.Cryptography.Encrypt(txtPass.Text));
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    user = dr["username"].ToString();
                    dr.Close();
                    conn.Close();
                    SaveCancelOrder(user);
                    if (cancelOrder.cboInventory.Text == "yes")
                    {
                        dbconn.ExecuteQuery("UPDATE tbProduct SET qty = qty + " + cancelOrder.udCancelQty.Value + " where pcode= '" + cancelOrder.txtPcode.Text + "'");
                    }
                    dbconn.ExecuteQuery("UPDATE tbCart SET qty = qty + " + cancelOrder.udCancelQty.Value + " where id LIKE '" + cancelOrder.txtId.Text + "'");
                    Utilities.MessageInfo("Order transaction successfully cancelled!");
                    this.Dispose();
                    cancelOrder.ReloadSoldList();
                    cancelOrder.Dispose();

                }
                dr.Close();
                conn.Close();
            }
            catch(Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }
        public void SaveCancelOrder(string user)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("INSERT INTO tbCancel (transno, pcode, price, qty, total, sdate, voidby, cancelledby, reason, action) values (@transno, @pcode, @price, @qty, @total, @sdate, @voidby, @cancelledby, @reason, @action)", conn);
                cmd.Parameters.AddWithValue("@transno", cancelOrder.txtTransno.Text);
                cmd.Parameters.AddWithValue("@pcode", cancelOrder.txtPcode.Text);
                cmd.Parameters.AddWithValue("@price", double.Parse(cancelOrder.txtPrice.Text));
                cmd.Parameters.AddWithValue("@qty", int.Parse(cancelOrder.txtQty.Text));
                cmd.Parameters.AddWithValue("@total", double.Parse(cancelOrder.txtTotal.Text));
                cmd.Parameters.AddWithValue("@sdate", DateTime.Now);
                cmd.Parameters.AddWithValue("@voidby", user);
                cmd.Parameters.AddWithValue("@cancelledby", cancelOrder.txtCancelBy.Text);
                cmd.Parameters.AddWithValue("@reason", cancelOrder.txtReason.Text);
                cmd.Parameters.AddWithValue("@action", cancelOrder.cboInventory.Text);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Void_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
