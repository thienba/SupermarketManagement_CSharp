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
    public partial class UserProperties : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        UserAccount userAccount;
        public string username;
        public UserProperties(UserAccount user)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            userAccount = user;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (Utilities.MessageYesNo("Are you sure you want to change this account properties?", "Change Properties") == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new SqlCommand("UPDATE tbUser SET name=@name, role=@role, isactivate=@isactivate WHERE username='" + username + "'", cn);
                    cm.Parameters.AddWithValue("@name", txtFullname.Text);
                    cm.Parameters.AddWithValue("@role", cboRole.Text);
                    cm.Parameters.AddWithValue("@isactivate", cboActivate.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    Utilities.MessageInfo("Account properties has been successfully changed!");
                    userAccount.LoadUser();
                    this.Dispose();
                }
            }                                           
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void UserProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
