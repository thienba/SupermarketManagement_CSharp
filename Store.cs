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
    public partial class Store : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        bool haveStoreInfo = false;
        public Store()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            LoadStore();
        }
        public void LoadStore()
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM tbStore", conn);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    haveStoreInfo = true;
                    txtStName.Text = dr["store"].ToString();
                    txtAddress.Text = dr["address"].ToString();
                }
                else
                {
                    txtStName.Clear();
                    txtAddress.Clear();
                }
                dr.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtStName.Text.Trim() != "" && txtAddress.Text.Trim() != "")
            {
                try
                {
                    if (Utilities.MessageYesNo("You want to save store details?", "Confirm") == DialogResult.Yes)
                        if (haveStoreInfo)
                        {
                            dbconn.ExecuteQuery("UPDATE tbStore SET store = '" + txtStName.Text + "', address= '" + txtAddress.Text + "'");
                        }
                        else
                        {
                            dbconn.ExecuteQuery("INSERT INTO tbStore (store,address) VALUES ('" + txtStName.Text + "','" + txtAddress.Text + "')");
                        }
                    Utilities.MessageInfo("Store detail has been successfully saved!");
                    this.Dispose();
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
            else
            {
                Utilities.MessageWarning("Please enter store name and address.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Store_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
