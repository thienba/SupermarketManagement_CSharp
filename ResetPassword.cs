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
    public partial class ResetPassword : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DBConnect dbcon = new DBConnect();
        SqlDataReader dr;
        UserAccount userAccount;
        public ResetPassword(UserAccount account)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.myConnection());
            userAccount = account;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtNpass.Text != txtResPass.Text)
            {
                Utilities.MessageWarning("The password you typed do not match. Type the password for this account in both text boxes.");
                return;
            }
            else
            {
                if (Utilities.MessageYesNo("You want to reset password?", "Confirm") == DialogResult.Yes)
                {
                    dbcon.ExecuteQuery("UPDATE tbUser SET password = '" + Encode_Decode.Cryptography.Encrypt(txtNpass.Text) + "'WHERE username = '" + userAccount.username + "'");
                    Utilities.MessageInfo("Password has been successfully reset");
                    this.Dispose();
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ResetPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
