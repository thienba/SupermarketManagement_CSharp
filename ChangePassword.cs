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
    public partial class ChangePassword : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        Cashier cashier;
        public ChangePassword(Cashier cash)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            cashier = cash;
            lblUsername.Text = cashier.lblUsername.Text;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                string oldpass = dbconn.getPassword(lblUsername.Text);
                if (oldpass != Encode_Decode.Cryptography.Encrypt(txtPass.Text))
                {
                    Utilities.MessageWarning("Wrong password, please try again!");
                    return;
                }
                else
                {
                    txtPass.Visible = false;
                    btnNext.Visible = false;

                    txtNewPass.Visible = true;
                    txtConfirmPass.Visible = true;
                    btnSave.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtConfirmPass.Text.Trim() != "" && txtNewPass.Text.Trim() != "")
            {
                try
                {
                    if (txtNewPass.Text != txtConfirmPass.Text)
                    {
                        Utilities.MessageWarning("New password and confirm password did not matched!");
                    }
                    else
                    {
                        if (Utilities.MessageYesNo("You want to change password?", "Change password") == DialogResult.Yes)
                        {
                            dbconn.ExecuteQuery("UPDATE tbUser SET password = '" + Encode_Decode.Cryptography.Encrypt(txtNewPass.Text) + "' WHERE username = '" + lblUsername.Text + "'");
                            Utilities.MessageInfo("Password has been sucessfully update!");
                            this.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
            else
            {
                Utilities.MessageWarning("Please enter new and confirm password!");
            }
        }

        private void ChangePassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
