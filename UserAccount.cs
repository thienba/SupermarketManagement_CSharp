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
    public partial class UserAccount : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        MainForm mainForm;
        public string username;
        string name;
        string role;
        string accStatus;
        public UserAccount(MainForm main)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            mainForm = main;
            LoadUser();
        }
        public void LoadUser()
        {
            int i = 0;
            dgvUser.Rows.Clear();
            cmd = new SqlCommand("SELECT * FROM tbUser", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvUser.Rows.Add(i, dr[0].ToString(), dr[3].ToString(), dr[4].ToString(), dr[2].ToString());
            }
            dr.Close();
            conn.Close();
        }
        private bool CheckUserExists()
        {
            cmd = new SqlCommand("SELECT COUNT(*) FROM tbUser WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("@username", txtUsername.Text);
            conn.Open();
            int TotalRows = 0;
            TotalRows = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            if (TotalRows > 0)
            {
                Utilities.MessageWarning("Username already exist");
                txtUsername.Focus();
                return false;
            }
            return true;
        }
        private bool CheckEmpty()
        {
            if (txtFullname.Text.Trim() == "")
            {
                Utilities.MessageWarning("FullName is required.");
                txtFullname.Clear();
                return false;
            }
            if (txtUsername.Text.Trim() == "")
            {
                Utilities.MessageWarning("Username is required.");
                txtUsername.Clear();
                return false;
            }
            if (txtPassword.Text.Trim() == "")
            {
                Utilities.MessageWarning("Password is required.");
                txtPassword.Clear();
                return false;
            }
            if (txtRetypePassword.Text.Trim() == "")
            {
                Utilities.MessageWarning("Retype password is required.");
                txtRetypePassword.Clear();
                return false;
            }
            return true;
        }
        private bool CheckRePassword()
        {
            if(txtPassword.Text != txtRetypePassword.Text)
            {
                Utilities.MessageWarning("Re-password does not match");
                return false;
            }
            return true;
        }
        public void Clear()
        {
            txtFullname.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtRetypePassword.Clear();

            txtFullname.Focus();
        }
        private void btnAccSave_Click(object sender, EventArgs e)
        {
            if (CheckEmpty() && CheckUserExists() && CheckRePassword())
            {
                try
                {
                    if (txtPassword.Text != txtRetypePassword.Text)
                    {
                        Utilities.MessageWarning("Password did not March!");
                        return;
                    }
                    cmd = new SqlCommand("INSERT INTO tbUser(username, password, role, name) VALUES(@username, @password, @role, @name)", conn);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", Encode_Decode.Cryptography.Encrypt(txtPassword.Text));
                    cmd.Parameters.AddWithValue("@role", cboRole.Text);
                    cmd.Parameters.AddWithValue("@name", txtFullname.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Utilities.MessageInfo("New account has been successfully saved.");
                    Clear();
                    LoadUser();
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
        }

        private void btnAccCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnPassSave_Click(object sender, EventArgs e)
        {
            if (txtCurPass.Text.Trim() !="" && txtNPass.Text.Trim() != "" && txtRePass2.Text.Trim() != "")
            {
                try
                {
                    if (Encode_Decode.Cryptography.Encrypt(txtCurPass.Text) != mainForm._pass)
                    {
                        Utilities.MessageWarning("Current password did not martch!");
                        return;
                    }
                    if (txtNPass.Text != txtRePass2.Text)
                    {
                        Utilities.MessageWarning("Confirm new password did not martch!");
                        return;
                    }

                    dbconn.ExecuteQuery("UPDATE tbUser SET password= '" + Encode_Decode.Cryptography.Encrypt(txtNPass.Text) + "' WHERE username='" + lblUsername.Text + "'");
                    ClearAllTextBox();
                    Utilities.MessageInfo("Password has been succefully changed!");
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
            else
            {
                Utilities.MessageWarning("Please enter current password, new password and re-type password!");
            }
        }

        private void UserAccount_Load(object sender, EventArgs e)
        {
            lblUsername.Text = mainForm.lblUsername.Text;
        }
        public void ClearAllTextBox()
        {
            txtCurPass.Clear();
            txtNPass.Clear();
            txtRePass2.Clear();
        }
        private void btnPassCancel_Click(object sender, EventArgs e)
        {
            ClearAllTextBox();
        }

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {
            int i = dgvUser.CurrentRow.Index;
            username = dgvUser[1, i].Value.ToString();
            name = dgvUser[2, i].Value.ToString();
            role = dgvUser[4, i].Value.ToString();
            accStatus = dgvUser[3, i].Value.ToString();
            if (lblUsername.Text == username)
            {
                btnRemove.Enabled = false;
                btnResetPass.Enabled = false;
                lblAccNote.Text = "To change your password, go to change password tag.";
            }
            else
            {
                btnRemove.Enabled = true;
                btnResetPass.Enabled = true;
                lblAccNote.Text = "To change the password for " + username + ", click Reset Password.";
            }
            gbUser.Text = "Password For " + username;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You chose to remove this account from this Point Of Sales System's user list. \n\n Are you sure you want to remove '" + username + "' \\ '" + role + "'", "User Account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                dbconn.ExecuteQuery("DELETE FROM tbUser WHERE username = '" + username + "'");
                Utilities.MessageInfo("Account has been successfully deleted");
                LoadUser();
            }
        }

        private void btnResetPass_Click(object sender, EventArgs e)
        {
            ResetPassword resetPassword = new ResetPassword(this);
            resetPassword.ShowDialog();
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            UserProperties properties = new UserProperties(this);
            properties.Text = name + "\\" + username + " Properties";
            properties.txtFullname.Text = name;
            properties.cboRole.Text = role;
            properties.cboActivate.Text = accStatus;
            properties.username = username;
            properties.ShowDialog();
        }
    }
}
