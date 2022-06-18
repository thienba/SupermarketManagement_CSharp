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
    public partial class CategoryModule : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Category categoryForm;
        public CategoryModule(Category cat)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            categoryForm = cat;
        }
        public void Clear()
        {
            txtCategory.Clear();
            btnUpdate.Enabled = false;
            btnSave.Enabled = true;
            txtCategory.Focus();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            //To insert brand name to brand table
            try
            {
                if (txtCategory.Text.Trim() != "")
                {
                    if (Utilities.MessageYesNo("Are you sure you want to save this category?", "Save Category") == DialogResult.Yes)
                    {
                        conn.Open();
                        cmd = new SqlCommand("INSERT INTO tbCategory VALUES (@category)", conn);
                        cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Record has been successfully saved.");
                        Clear();
                        categoryForm.LoadCategory();
                    }
                }
                else
                {
                    Utilities.MessageWarning("Category is required.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCategory.Text.Trim() != "")
                {
                    //Update brand name
                    if (Utilities.MessageYesNo("Are you sure want to update this this category?", "Save Category") == DialogResult.Yes)
                    {
                        conn.Open();
                        cmd = new SqlCommand("UPDATE tbCategory SET category = @category WHERE id LIKE '" + lblId.Text + "'", conn);
                        cmd.Parameters.AddWithValue("@category", txtCategory.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Brand has been successfully updated.");
                        Clear();
                        this.Dispose(); //To close form after update data
                    }
                }
                else
                {
                    Utilities.MessageWarning("Category name is required.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void CategoryModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
