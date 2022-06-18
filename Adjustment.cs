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
    public partial class Adjustment : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        MainForm mainForm;
        int _qty;
        public Adjustment(MainForm main)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            mainForm = main;
            ReferenceNo();
            LoadStock();
            lblUsername.Text = main.lblUsername.Text;
        }
        public void ReferenceNo()
        {
            Random rnd = new Random();
            lblRefNo.Text = rnd.Next().ToString(); //Next(): Trả về giá trị kiểu int, int Numrd
        }
        public void LoadStock()
        {
            int i = 0;
            dgvAdjustment.Rows.Clear();
            cmd = new SqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.qty FROM tbProduct AS p INNER JOIN tbBrand AS b ON b.id = p.bid INNER JOIN tbCategory AS c ON c.id = p.cid WHERE CONCAT(p.pdesc, b.brand, c.category) LIKE '%" + txtSearch.Text + "%'", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvAdjustment.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            conn.Close();
        }
        private void dgvAdjustment_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvAdjustment.Columns[e.ColumnIndex].Name;
            if (colName == "Select")
            {
                lblPcode.Text = dgvAdjustment.Rows[e.RowIndex].Cells[1].Value.ToString();
                lblDesc.Text = dgvAdjustment.Rows[e.RowIndex].Cells[3].Value.ToString() + " " + dgvAdjustment.Rows[e.RowIndex].Cells[4].Value.ToString() + " " + dgvAdjustment.Rows[e.RowIndex].Cells[5].Value.ToString();
                _qty = int.Parse(dgvAdjustment.Rows[e.RowIndex].Cells[7].Value.ToString());
                btnSave.Enabled = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadStock();
        }
        public void Clear()
        {
            lblDesc.Text = "";
            lblPcode.Text = "";
            txtQty.Clear();
            txtRemark.Clear();
            cbAction.Text = "";
            ReferenceNo();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //validation for empty field
                if (cbAction.Text == "")
                {
                    Utilities.MessageWarning("Please select action for add or reduce.");
                    cbAction.Focus();
                    return;
                }

                if (txtQty.Text == "")
                {
                    Utilities.MessageWarning("Please input quantity for add or reduce.");
                    txtQty.Focus();
                    return;
                }

                if (txtRemark.Text == "")
                {
                    Utilities.MessageWarning("Need reason for stock adjustment.");
                    txtRemark.Focus();
                    return;
                }

                //update stock
                if (int.Parse(txtQty.Text) > _qty)
                {
                    Utilities.MessageWarning("Stock on hand quantity should be greater than adjustment quantity.");
                    return;
                }
                if (cbAction.Text == "Remove From Inventory")
                {
                    dbconn.ExecuteQuery("UPDATE tbProduct SET qty = (qty - " + int.Parse(txtQty.Text) + ") WHERE pcode LIKE '" + lblPcode.Text + "'");
                }
                else if (cbAction.Text == "Add To Inventory")
                {
                    dbconn.ExecuteQuery("UPDATE tbProduct SET qty = (qty + " + int.Parse(txtQty.Text) + ") WHERE pcode LIKE '" + lblPcode.Text + "'");
                }

                dbconn.ExecuteQuery("INSERT INTO tbAdjustment(referenceNo, pcode, qty, action, remarks, sdate, [user]) VALUES ('" + lblRefNo.Text + "','" + lblPcode.Text + "','" + int.Parse(txtQty.Text) + "', '" + cbAction.Text + "', '" + txtRemark.Text + "', '" + DateTime.Now.ToShortDateString() + "','" + lblUsername.Text + "')");
                Utilities.MessageInfo("Stock has been successfully adjusted.");
                LoadStock();
                Clear();
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }
    }
}
