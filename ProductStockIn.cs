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
    public partial class ProductStockIn : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        StockIn stockIn;
        public ProductStockIn(StockIn stock)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            stockIn = stock;
            LoadProduct();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void LoadProduct()
        {
            int i = 0;
            dgvProduct.Rows.Clear();
            cmd = new SqlCommand("SELECT pcode, pdesc, qty FROM tbProduct WHERE pdesc LIKE '%" + txtSearch.Text + "%'", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvProduct.Rows.Add(i, dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["qty"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void dgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvProduct.Columns[e.ColumnIndex].Name;
            if (colName == "Selected")
            {
                if(stockIn.txtStockInBy.Text == string.Empty)
                {
                    Utilities.MessageWarning("Please enter stock in by name");
                    stockIn.txtStockInBy.Focus();
                    this.Dispose();
                    return;
                }

                if (Utilities.MessageYesNo("You want to add this item?", "Question") == DialogResult.Yes)
                {
                    addStockIn(dgvProduct.Rows[e.RowIndex].Cells[1].Value.ToString());
                    Utilities.MessageInfo("Successfully added.");
                }
            }
        }
        public void addStockIn(string pcode)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("INSERT INTO tbStockIn (refno, pcode, sdate, stockinby, supplierid)VALUES (@refno, @pcode, @sdate, @stockinby, @supplierid)", conn);
                cmd.Parameters.AddWithValue("@refno", stockIn.txtRefNo.Text);
                cmd.Parameters.AddWithValue("@pcode", pcode);
                cmd.Parameters.AddWithValue("@sdate", stockIn.dtpStockInDate.Value);
                cmd.Parameters.AddWithValue("@stockinby", stockIn.txtStockInBy.Text);
                cmd.Parameters.AddWithValue("@supplierid", stockIn.lblId.Text);
                cmd.ExecuteNonQuery();
                conn.Close();
                stockIn.LoadStockIn();

            }
            catch (Exception ex)
            {
                Utilities.MessageInfo(ex.Message);
            }
        }

        private void ProductStockIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }
    }
}
