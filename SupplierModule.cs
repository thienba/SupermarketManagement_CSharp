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
    public partial class SupplierModule : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Supplier supplier;
        public SupplierModule(Supplier sp)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            supplier = sp;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private bool CheckEmpty()
        {
            if (txtSupplierName.Text.Trim() == "")
            {
                Utilities.MessageWarning("Supplier Name is required.");
                txtSupplierName.Clear();
                return false;
            }
            if (txtAddress.Text.Trim() == "")
            {
                Utilities.MessageWarning("Address is required.");
                txtAddress.Clear();
                return false;
            }
            if (txtContactPerson.Text.Trim() == "")
            {
                Utilities.MessageWarning("Contact Person is required.");
                txtContactPerson.Clear();
                return false;
            }
            if (txtPhoneNo.Text.Trim() == "")
            {
                Utilities.MessageWarning("Phone No is required.");
                txtPhoneNo.Clear();
                return false;
            }
            if (txtEmailAddress.Text.Trim() == "")
            {
                Utilities.MessageWarning("Email Address is required.");
                txtEmailAddress.Clear();
                return false;
            }
            return true;
        }
        private bool SupplierCheckExists()
        {
            cmd = new SqlCommand("SELECT COUNT(*) FROM tbSupplier WHERE supplier = @supplier", conn);
            cmd.Parameters.AddWithValue("@supplier", txtSupplierName.Text);
            conn.Open();
            int TotalRows = 0;
            TotalRows = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            if (TotalRows > 0)
            {
                Utilities.MessageWarning("Supplier already exist");
                txtSupplierName.Focus();
                return false;
            }
            return true;
        }
        public void Clear()
        {
            txtAddress.Clear();
            txtContactPerson.Clear();
            txtEmailAddress.Clear();
            txtFaxNo.Clear();
            txtPhoneNo.Clear();
            txtSupplierName.Clear();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtSupplierName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckEmpty() && SupplierCheckExists() && Utilities.IsPhoneNumber(txtPhoneNo.Text) && Utilities.IsValidEmail(txtEmailAddress.Text))
            {
                try
                {
                    if (Utilities.MessageYesNo("Are you sure want to save this supplier?", "Save Supplier") == DialogResult.Yes)
                    {
                        cmd = new SqlCommand("INSERT INTO tbSupplier(supplier, address, contactperson, phone, email, fax) VALUES(@supplier, @address, @contactperson, @phone, @email, @fax)", conn);
                        cmd.Parameters.AddWithValue("@supplier", txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmailAddress.Text);
                        cmd.Parameters.AddWithValue("@fax", txtFaxNo.Text);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Supplier has been successfully saved.");
                        Clear();
                        supplier.LoadSupplier();
                    }
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (CheckEmpty() && Utilities.IsPhoneNumber(txtPhoneNo.Text) && Utilities.IsValidEmail(txtEmailAddress.Text))
            {
                try
                {
                    if (Utilities.MessageYesNo("Are you sure want to update this supplier?", "Update Supplier") == DialogResult.Yes)
                    {
                        cmd = new SqlCommand("UPDATE tbSupplier SET supplier = @supplier, address = @address, contactperson =  @contactperson, phone = @phone, email = @email, fax = @fax where id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", lblId.Text);
                        cmd.Parameters.AddWithValue("@supplier", txtSupplierName.Text);
                        cmd.Parameters.AddWithValue("@address", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@contactperson", txtContactPerson.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmailAddress.Text);
                        cmd.Parameters.AddWithValue("@fax", txtFaxNo.Text);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Supplier has been successfully updated.");
                        Clear();
                        supplier.LoadSupplier();
                        this.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
        }

        private void SupplierModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
