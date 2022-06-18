﻿using System;
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
    public partial class Settle : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        Cashier cashier;
        public Settle(Cashier cash)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            this.KeyPreview = true; //Bắt sự kiện nhấn phím thật (hoặc phím ảo của Windows)
            cashier = cash;
        }

        private void btnOne_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnOne.Text;
        }

        private void btnTwo_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnTwo.Text;
        }

        private void btnThree_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnThree.Text;
        }

        private void btnFour_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnFour.Text;
        }

        private void btnFive_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnFive.Text;
        }

        private void btnSix_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnSix.Text;
        }

        private void btnSeven_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnSeven.Text;
        }

        private void btnEight_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnEight.Text;
        }

        private void btnNine_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnNine.Text;
        }

        private void btnZero_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnZero.Text;
        }

        private void btnDZero_Click(object sender, EventArgs e)
        {
            txtCash.Text += btnDZero.Text;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCash.Clear();
            txtCash.Focus();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                if ((double.Parse(txtChange.Text) < 0) || (txtCash.Text.Equals("")))
                {
                    Utilities.MessageWarning("Insufficient amount, Please enter the corret amount!");
                    return;
                }
                else
                {
                    for (int i = 0; i < cashier.dgvCashier.Rows.Count; i++)
                    {
                        conn.Open();
                        cmd = new SqlCommand("UPDATE tbProduct SET qty = qty - " + int.Parse(cashier.dgvCashier.Rows[i].Cells[5].Value.ToString()) + "WHERE pcode= '" + cashier.dgvCashier.Rows[i].Cells[2].Value.ToString() + "'", conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        conn.Open();
                        cmd = new SqlCommand("UPDATE tbCart SET status = 'Sold' WHERE id= '" + cashier.dgvCashier.Rows[i].Cells[1].Value.ToString() + "'", conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    Receipt receipt = new Receipt(cashier);
                    receipt.LoadRecept(txtCash.Text, txtChange.Text);
                    receipt.ShowDialog();

                    Utilities.MessageInfo("Payment successfully saved.");
                    cashier.GetTranNo();
                    cashier.LoadCart();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double sale = double.Parse(txtSale.Text);
                double cash = double.Parse(txtCash.Text);
                double charge = cash - sale;
                txtChange.Text = charge.ToString("#,##0.00");
            }
            catch
            {
                txtChange.Text = "0.00";
            }
        }

        private void Settle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
            else if (e.KeyCode == Keys.Enter)
                btnEnter.PerformClick();
        }
    }
}
