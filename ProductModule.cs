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
    public partial class ProductModule : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Product product;
        public ProductModule(Product pd)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            product = pd;
            LoadCategory();
            LoadBrand();
        }
        private bool CheckEmpty()
        {
            if (txtPCode.Text.Trim() == "")
            {
                Utilities.MessageWarning("Product code is required.");
                txtPCode.Clear();
                return false;
            }
            if (txtBarcode.Text.Trim() == "")
            {
                Utilities.MessageWarning("Barcode is required.");
                txtBarcode.Clear();
                return false;
            }
            if (txtPDesc.Text.Trim() == "")
            {
                Utilities.MessageWarning("Description is required.");
                txtPDesc.Clear();
                return false;
            }
            if (txtPrice.Text.Trim() == "")
            {
                Utilities.MessageWarning("Price is required.");
                txtPrice.Clear();
                return false;
            }
            return true;
        }
        private bool PCodeCheckExists()
        {
            cmd = new SqlCommand("SELECT COUNT(*) FROM tbProduct WHERE pcode = @pcode", conn);
            cmd.Parameters.AddWithValue("@pcode", txtPCode.Text);
            conn.Open();
            int TotalRows = 0;
            TotalRows = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();
            if (TotalRows > 0)
            {
                Utilities.MessageWarning("Product code already exist");
                txtPCode.Focus();
                return false;
            }
            return true;
        }
        public void LoadCategory()
        {
            cboCategory.Items.Clear();
            cboCategory.DataSource = dbconn.getTable("SELECT * FROM tbCategory");
            cboCategory.DisplayMember = "category";
            cboCategory.ValueMember = "id";
        }
        public void LoadBrand()
        {
            cboBrand.Items.Clear();
            cboBrand.DataSource = dbconn.getTable("SELECT * FROM tbBrand");
            cboBrand.DisplayMember = "brand";
            cboBrand.ValueMember = "id";
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void Clear()
        {
            txtPCode.Clear();
            txtBarcode.Clear();
            txtPDesc.Clear();
            txtPrice.Clear();
            cboBrand.SelectedIndex = 0;
            cboCategory.SelectedIndex = 0;
            udReOrder.Value = 1;

            txtPCode.Enabled = true;
            txtPCode.Focus();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckEmpty() && PCodeCheckExists())
            {
                try
                {
                    if (Utilities.MessageYesNo("Are you sure want to save this product?", "Save Product") == DialogResult.Yes)
                    {
                        cmd = new SqlCommand("INSERT INTO tbProduct(pcode, barcode, pdesc, bid, cid, price, reorder) VALUES(@pcode, @barcode, @pdesc, @bid, @cid, @price, @reorder)", conn);
                        cmd.Parameters.AddWithValue("@pcode", txtPCode.Text);
                        cmd.Parameters.AddWithValue("@barcode", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@pdesc", txtPDesc.Text);
                        cmd.Parameters.AddWithValue("@bid", cboBrand.SelectedValue);
                        cmd.Parameters.AddWithValue("@cid", cboCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@price", double.Parse(txtPrice.Text));
                        cmd.Parameters.AddWithValue("@reorder", udReOrder.Text);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Product has been successfully saved.");
                        Clear();
                        product.LoadProduct();
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
            if (CheckEmpty())
            {
                try
                {
                    if (Utilities.MessageYesNo("Are you sure want to update this product?", "Update Product") == DialogResult.Yes)
                    {
                        cmd = new SqlCommand("UPDATE tbProduct SET pcode = @pcode, barcode = @barcode, pdesc = @pdesc, bid = @bid, cid = @cid, price = @price, reorder = @reorder WHERE pcode LIKE @pcode", conn);
                        cmd.Parameters.AddWithValue("@pcode", txtPCode.Text);
                        cmd.Parameters.AddWithValue("@barcode", txtBarcode.Text);
                        cmd.Parameters.AddWithValue("@pdesc", txtPDesc.Text);
                        cmd.Parameters.AddWithValue("@bid", cboBrand.SelectedValue);
                        cmd.Parameters.AddWithValue("@cid", cboCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@price", double.Parse(txtPrice.Text));
                        cmd.Parameters.AddWithValue("@reorder", udReOrder.Text);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        Utilities.MessageInfo("Product has been successfully updated.");
                        Clear();
                        this.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Utilities.MessageWarning(ex.Message);
                }
            }
        }

        private void ProductModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
