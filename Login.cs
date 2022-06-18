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
    public partial class Login : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        public string _pass = "";
        public bool _isActivate;
        public Login()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            txtName.Focus();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            if (Utilities.MessageYesNo("You want to exit applicaiton?", "Confirm") == DialogResult.Yes)
                Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string _username = "", _name = "", _role = "";
            try
            {
                bool found;
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM tbUser WHERE username = @username and password = @password", conn);
                cmd.Parameters.AddWithValue("@username", txtName.Text);
                cmd.Parameters.AddWithValue("@password", Encode_Decode.Cryptography.Encrypt(txtPass.Text));
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    found = true;
                    _username = dr["username"].ToString();
                    _name = dr["name"].ToString();
                    _role = dr["role"].ToString();
                    _pass = dr["password"].ToString();
                    _isActivate = bool.Parse(dr["isactivate"].ToString());
                }
                else
                {
                    found = false;
                }
                dr.Close();
                conn.Close();

                if (found)
                {
                    if (!_isActivate)
                    {
                        Utilities.MessageWarning("Account is deactivate. Unable to login!");
                        return;
                    }
                    if (_role == "Cashier")
                    {
                        Utilities.MessageInfo("Welcome " + _name + " !");
                        txtName.Clear();
                        txtPass.Clear();
                        this.Hide();
                        Cashier cashier = new Cashier();
                        cashier.lblUsername.Text = _username;
                        cashier.lblNameAndRole.Text = _name + " | " + _role;
                        cashier.ShowDialog();
                    }
                    else
                    {
                        Utilities.MessageInfo("Welcome " + _name + " !");
                        txtName.Clear();
                        txtPass.Clear();
                        this.Hide();
                        MainForm main = new MainForm();
                        main.lblUsername.Text = _username;
                        main.lblName.Text = _name;
                        main._pass = _pass;
                        main.ShowDialog();
                    }
                }
                else
                {
                    Utilities.MessageWarning("Invalid username or password!");
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Utilities.MessageYesNo("You want to exit application?", "Confirm") == DialogResult.Yes)
                Application.Exit();
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                btnLogin.PerformClick();
            }
        }
    }
}
