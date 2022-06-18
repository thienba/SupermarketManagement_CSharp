﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zen.Barcode;

namespace SupermarketManagement
{
    public partial class Barcode : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        string fname;
        public Barcode()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            LoadProduct();
        }
        public void LoadProduct()
        {
            int i = 0;
            dgvBarcode.Rows.Clear();
            cmd = new SqlCommand("SELECT p.pcode, p.barcode, p.pdesc, b.brand, c.category, p.price, p.reorder FROM tbProduct as p INNER JOIN tbBrand AS b ON b.id = p.bid INNER JOIN tbCategory AS c ON c.id = p.cid WHERE CONCAT(p.pdesc, b.brand, c.category) LIKE '%" + txtSearch.Text + "%'", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvBarcode.Rows.Add(i, dr["pcode"].ToString(), dr["barcode"].ToString(), dr["pdesc"].ToString(), dr["brand"].ToString(), dr["category"].ToString(), dr["price"].ToString(), dr["reorder"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void dgvBarcode_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvBarcode.Columns[e.ColumnIndex].Name;
            if (colName == "Select")
            {
                Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                picBarcode.Image = barcode.Draw(dgvBarcode.Rows[e.RowIndex].Cells[2].Value.ToString(),60,2);
                fname = dgvBarcode.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.FileName = fname;
            saveFile.Filter = "Image File(*.jpg, *.png)| *.jpg, *.png";
            ImageFormat image = ImageFormat.Png;
            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                string ftype = System.IO.Path.GetExtension(saveFile.FileName);
                switch (ftype)
                {
                    case ".jpg":
                        image = ImageFormat.Jpeg;
                        break;
                    case ".png":
                        image = ImageFormat.Png;
                        break;
                }
                picBarcode.Image.Save(saveFile.FileName, image);
            }
            picBarcode.Image = null;
        }
    }
}
