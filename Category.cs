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
    public partial class Category : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        public Category()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            LoadCategory();
        }
        //Data retrieve from tbCategory to dgvCategory on Category from
        public void LoadCategory()
        {
            int i = 0;
            dgvCategory.Rows.Clear();
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM tbCategory ORDER BY category", conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvCategory.Rows.Add(i, dr["id"].ToString(), dr["category"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CategoryModule moduleForm = new CategoryModule(this);
            moduleForm.btnUpdate.Enabled = false;
            moduleForm.ShowDialog();
        }

        private void dgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //For update and delete brand by cell click form tbBrand
            string colName = dgvCategory.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (Utilities.MessageYesNo("Are you sure you want to delete this record?","Delete Category") == DialogResult.Yes)
                {
                    conn.Open();
                    cmd = new SqlCommand("DELETE FROM tbCategory WHERE id LIKE '" + dgvCategory[1, e.RowIndex].Value.ToString() + "'", conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    Utilities.MessageInfo("Category has been successfully deleted.");
                }
            }
            else if (colName == "Edit")
            {
                CategoryModule moduleForm = new CategoryModule(this);
                moduleForm.lblId.Text = dgvCategory[1, e.RowIndex].Value.ToString();
                moduleForm.txtCategory.Text = dgvCategory[2, e.RowIndex].Value.ToString();
                moduleForm.btnSave.Enabled = false;
                moduleForm.btnUpdate.Enabled = true;
                moduleForm.ShowDialog();
            }
            LoadCategory();
        }
    }
}
