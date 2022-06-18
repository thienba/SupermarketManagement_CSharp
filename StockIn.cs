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
    public partial class StockIn : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        MainForm mainForm;
        public StockIn(MainForm main)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            mainForm = main;
            LoadSupplier();
            GetRefNo();
            txtStockInBy.Text = mainForm.lblUsername.Text;
        }
        public void GetRefNo()
        {
            Random rnd = new Random();
            txtRefNo.Clear();
            txtRefNo.Text += rnd.Next();
        }
        public void LoadSupplier()
        {
            cboSupplier.Items.Clear();
            cboSupplier.DataSource = dbconn.getTable("SELECT * FROM tbSupplier");
            cboSupplier.DisplayMember = "supplier";
        }
        public void ProductForSupplier(string pcode)
        {
            string supplier = "";
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM viewStockIn WHERE pcode LIKE '" + pcode + "'", conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                supplier = dr["supplier"].ToString();
            }
            dr.Close();
            conn.Close();
            cboSupplier.Text = supplier;
        }
        public void LoadStockIn()
        {
            int i = 0;
            dgvStockIn.Rows.Clear();
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM viewStockIn WHERE refno LIKE '"+ txtRefNo.Text +"' AND status LIKE 'Pending'", conn);
            dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dgvStockIn.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString(), dr[8].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void cboSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void linkGenerate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GetRefNo();
        }

        private void linkProduct_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProductStockIn productStockIn = new ProductStockIn(this);
            productStockIn.ShowDialog();
        }

        private void btnEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if(dgvStockIn.Rows.Count > 0)
                {
                    if (Utilities.MessageYesNo("Are you sure you want to save this records?", "Save Records") == DialogResult.Yes)
                    {
                        for(int i = 0; i < dgvStockIn.Rows.Count; i++)
                        {
                            //Update product quantity
                            conn.Open();
                            cmd = new SqlCommand("UPDATE tbProduct SET qty = qty + " + int.Parse(dgvStockIn.Rows[i].Cells[5].Value.ToString()) + " WHERE pcode LIKE'" + dgvStockIn.Rows[i].Cells[3].Value.ToString() + "'", conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            //Update stockin quantity
                            conn.Open();
                            cmd = new SqlCommand("UPDATE tbStockIn SET qty = qty + " + int.Parse(dgvStockIn.Rows[i].Cells[5].Value.ToString()) + ", status = 'Done' WHERE id LIKE'" + dgvStockIn.Rows[i].Cells[1].Value.ToString() + "'", conn);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                        Clear();
                        LoadStockIn();
                    }
                }
            }
            catch(Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }
        public void Clear()
        {
            txtRefNo.Clear();
            txtStockInBy.Clear();
            dtpStockInDate.Value = DateTime.Now;
        }

        private void dgvStockIn_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvStockIn.Columns[e.ColumnIndex].Name;
            if(colName == "Delete")
            {
                if(Utilities.MessageYesNo("You want to remove this item?", "Remove Item") == DialogResult.Yes)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM tbStockIn WHERE id ='" + dgvStockIn.Rows[e.RowIndex].Cells[1].Value.ToString() +"'", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Utilities.MessageInfo("Item has been successfully removed");
                    LoadStockIn();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                dgvInStockHistory.Rows.Clear();
                conn.Open();
                //CAST chuyển đổi kiễu dữ liệu từ kiễu này sang kiễu khác (sdate kiễu date)
                cmd = new SqlCommand("SELECT * FROM viewStockIn WHERE CAST(sdate as date) BETWEEN '" + dtFrom.Value.ToShortDateString() + "' AND '" + dtTo.Value.ToShortDateString() + "' AND status LIKE 'Done'", conn);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvInStockHistory.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), DateTime.Parse(dr[5].ToString()).ToShortDateString(), dr[6].ToString(), dr["supplier"].ToString());

                }
                dr.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void cboSupplier_TextChanged(object sender, EventArgs e)
        {
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM tbSupplier WHERE supplier LIKE '" + cboSupplier.Text + "'", conn);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblId.Text = dr["id"].ToString();
                txtContactPerson.Text = dr["contactperson"].ToString();
                txtAddress.Text = dr["address"].ToString();

            }
            dr.Close();
            conn.Close();
        }
    }
}
