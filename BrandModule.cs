using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SupermarketManagement
{
    public partial class BrandModule : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Brand brandForm;
        public BrandModule(Brand br)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            brandForm = br;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //To insert brand name to brand table
            try
            {
                if (txtBrand.Text.Trim() != "")
                {
                    if (Utilities.MessageYesNo("Are you sure you want to save this brand?","Save Brand") == DialogResult.Yes)
                    {
                        conn.Open();
                        cmd = new SqlCommand("INSERT INTO tbBrand VALUES (@brand)", conn);
                        cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Record has been successfully saved.");
                        Clear();
                        brandForm.LoadBrand();
                    }
                }
                else
                {
                    Utilities.MessageWarning("Brand name is required.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }
        public void Clear()
        {
            txtBrand.Clear();
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            txtBrand.Focus();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBrand.Text.Trim() != "")
                {
                    //Update brand name
                    if (Utilities.MessageYesNo("Are you sure want to update this this brand?", "Update Brand") == DialogResult.Yes)
                    {
                        conn.Open();
                        cmd = new SqlCommand("UPDATE tbBrand SET brand = @brand WHERE id LIKE '" + lblId.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@brand", txtBrand.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Brand has been successfully updated.");
                        Clear();
                        this.Dispose(); //To close form after update data
                    }
                }
                else
                {
                    Utilities.MessageWarning("Brand name is required.");
                }
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
            conn.Close();
        }
    }
}
