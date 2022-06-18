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
    public partial class Product : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        public Product()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            LoadProduct();      
        }
        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();
            cmd = new SqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.reorder FROM tbProduct as p INNER JOIN tbBrand AS b ON b.id = p.bid INNER JOIN tbCategory AS c ON c.id = p.cid WHERE CONCAT(p.pdesc, b.brand, c.category) LIKE '%" + txtSearch.Text +"%'", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, dr["pcode"].ToString(), dr["barcode"].ToString(), dr["pdesc"].ToString(), dr["brand"].ToString(), dr["category"].ToString(), dr["price"].ToString(), dr["reorder"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProductModule productModule = new ProductModule(this);
            productModule.btnUpdate.Enabled = false;
            productModule.ShowDialog();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (Utilities.MessageYesNo("Are you sure you want to delete this product?", "Delete Product") == DialogResult.Yes)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM tbProduct WHERE pcode LIKE '" + dgvProduct[1, e.RowIndex].Value.ToString() + "'", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Utilities.MessageInfo("Product has been successfully deleted.");
                }
            }
            else if (colName == "Edit")
            {
                ProductModule productModule = new ProductModule(this);
                productModule.txtPCode.Text = dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString();
                productModule.txtBarcode.Text = dgvProduct.Rows[e.RowIndex].Cells[2].Value.ToString();
                productModule.txtPDesc.Text = dgvProduct.Rows[e.RowIndex].Cells[3].Value.ToString();
                productModule.cboBrand.Text = dgvProduct.Rows[e.RowIndex].Cells[4].Value.ToString();
                productModule.cboCategory.Text = dgvProduct.Rows[e.RowIndex].Cells[5].Value.ToString();
                productModule.txtPrice.Text = dgvProduct.Rows[e.RowIndex].Cells[6].Value.ToString();
                productModule.udReOrder.Value = int.Parse(dgvProduct.Rows[e.RowIndex].Cells[7].Value.ToString());
                productModule.txtPCode.Enabled = false;
                productModule.btnSave.Enabled = false;
                productModule.btnUpdate.Enabled = true;
                productModule.ShowDialog();
            }
            LoadProduct();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }
    }
}
