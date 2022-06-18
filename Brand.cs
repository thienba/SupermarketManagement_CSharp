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
    public partial class Brand : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        public Brand()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            LoadBrand();
        }
        //Data retrieve from tbBrand to dgbBrand on Brand from
        public void LoadBrand()
        {
            int i = 0;
            dgvBrand.Rows.Clear();
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM tbBrand ORDER BY brand", conn);
            dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                i++;
                dgvBrand.Rows.Add(i, dr["id"].ToString(), dr["brand"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            BrandModule moduleForm = new BrandModule(this);
            moduleForm.btnUpdate.Enabled = false;
            moduleForm.ShowDialog();
        }

        private void dgvBrand_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //For update and delete brand by cell click form tbBrand
            string colName = dgvBrand.Columns[e.ColumnIndex].Name;
            if(colName == "Delete")
            {
                if(Utilities.MessageYesNo("Are you sure you want to delete this record?", "Delete Record") == DialogResult.Yes)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM tbBrand WHERE id LIKE '"+ dgvBrand[1,e.RowIndex].Value.ToString() + "'", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Utilities.MessageInfo("Brand has been successfully deleted.");
                }
            }
            else if(colName == "Edit")
            {
                BrandModule brandModule = new BrandModule(this);
                brandModule.lblId.Text = dgvBrand[1, e.RowIndex].Value.ToString();
                brandModule.txtBrand.Text = dgvBrand[2, e.RowIndex].Value.ToString();
                brandModule.btnSave.Enabled = false;
                brandModule.btnUpdate.Enabled = true;
                brandModule.ShowDialog();
            }
            LoadBrand();
        }
    }
}
