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
using ZXing;
using DarrenLee.Media;

namespace SupermarketManagement
{
    public partial class Cashier : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;

        int qty;
        string id;
        string price;

        Camera captureDevice = new Camera();

        public Cashier()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            GetTranNo();
            lblDate.Text = DateTime.Now.ToShortDateString();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            if(Utilities.MessageYesNo("You want to exit applicaiton?", "Confirm") == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        public void Slide(Button button)
        {
            panelSlide.BackColor = Color.White;
            panelSlide.Height = button.Height;
            panelSlide.Top = button.Top;
        }
        #region button
        private void btnNewTransaction_Click(object sender, EventArgs e)
        {
            Slide(btnNewTransaction);
            GetTranNo();
        }

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            Slide(btnSearchProduct);
            LookUpProduct lookUp = new LookUpProduct(this);
            lookUp.LoadProduct();
            lookUp.ShowDialog();
        }

        private void btnAddDiscount_Click(object sender, EventArgs e)
        {
            Slide(btnAddDiscount);
            Discount discount = new Discount(this);
            discount.lbId.Text = id;
            discount.txtTotalPrice.Text = price;
            discount.ShowDialog();
        }

        private void btnSettlePayment_Click(object sender, EventArgs e)
        {
            Slide(btnSettlePayment);
            Settle settle = new Settle(this);
            settle.txtSale.Text = lblDisplayTotal.Text;
            settle.ShowDialog();
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            Slide(btnClearCart);
            if(Utilities.MessageYesNo("You want to remove all items from cart?", "Confirm") == DialogResult.Yes)
            {
                conn.Open();
                cmd = new SqlCommand("Delete FROM tbCart WHERE transno LIKE'" + lblTranNo.Text + "'", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                Utilities.MessageInfo("All items has been successfully remove.");
                LoadCart();
            }
        }

        private void btnDailySales_Click(object sender, EventArgs e)
        {
            Slide(btnDailySales);
            DailySale dailySale = new DailySale(new MainForm());
            dailySale.soldUser = lblUsername.Text;
            dailySale.dtFrom.Enabled = false;
            dailySale.dtTo.Enabled = false;
            dailySale.cboCashier.Enabled = false;
            dailySale.cboCashier.Text = lblUsername.Text;
            dailySale.picClose.Visible = true;
            dailySale.lblTitle.Visible = true;
            dailySale.ShowDialog();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            Slide(btnChangePassword);
            ChangePassword changePassword = new ChangePassword(this);
            changePassword.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Slide(btnLogout);
            if (dgvCashier.Rows.Count > 0)
            {
                Utilities.MessageWarning("Unable to logout. Please cancel the transaction.");
                return;
            }
            if (Utilities.MessageYesNo("You want to logout applicaiton?", "Logout") == DialogResult.Yes)
            {
                this.Hide();
                Login login = new Login();
                login.ShowDialog();
            }
        }
        #endregion button
        public void LoadCart()
        {
            try
            {
                Boolean hascart = false;
                int i = 0;
                double total = 0;
                double discount = 0;
                dgvCashier.Rows.Clear();
                conn.Open();
                cmd = new SqlCommand("SELECT c.id, c.pcode, p.pdesc, c.price, c.qty, c.disc, c.total FROM tbCart AS c INNER JOIN tbProduct AS p ON c.pcode=p.pcode WHERE c.transno LIKE @transno AND c.status LIKE 'Pending'", conn);
                cmd.Parameters.AddWithValue("@transno", lblTranNo.Text.ToString());
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    total += Convert.ToDouble(dr["total"]);
                    discount += Convert.ToDouble(dr["disc"]);
                    dgvCashier.Rows.Add(i, dr["id"].ToString(), dr["pcode"].ToString(), dr["pdesc"].ToString(), dr["price"].ToString(), dr["qty"].ToString(), Convert.ToDouble(dr["disc"]).ToString("#,##0.00"), Convert.ToDouble(dr["total"]).ToString("#,##0.00"));
                    hascart = true;
                }
                dr.Close();
                conn.Close();
                lblSalesTotal.Text = total.ToString("#,##0.00");
                lblDiscount.Text = discount.ToString("#,##0.00");
                GetCartTotal();
                if (hascart) { btnClearCart.Enabled = true; btnSettlePayment.Enabled = true; btnAddDiscount.Enabled = true; }
                else { btnClearCart.Enabled = false; btnSettlePayment.Enabled = false; btnAddDiscount.Enabled = false; }
            }
            catch(Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }
        private void GetCartTotal()
        {
            double discount = double.Parse(lblDiscount.Text);
            double sales = double.Parse(lblSalesTotal.Text) - discount;
            double vat = sales * 0.12; //VAT: 12% of VAT Payable (Output Tax Less Input Tax)
            double vatable = sales - vat;

            lblVat.Text = vat.ToString("#,##0.00");
            lblVatable.Text = vatable.ToString("#,##0.00");
            lblDisplayTotal.Text = sales.ToString("#,##0.00");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
        public void GetTranNo()
        {
            try
            {
                int count;
                string sdate = DateTime.Now.ToString("yyyyMMdd");
                string transNo;
                conn.Open();
                cmd = new SqlCommand("SELECT TOP 1 transno FROM tbCart WHERE transno LIKE '" + sdate + "%' ORDER BY id desc", conn);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    transNo = dr[0].ToString();
                    count = int.Parse(transNo.Substring(8, 4));
                    lblTranNo.Text = sdate + (count + 1);
                }
                else
                {
                    transNo = sdate + "1001";
                    lblTranNo.Text = transNo;
                }
                dr.Close();
            }
            catch(Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
            conn.Close();
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtBarcode.Text == string.Empty) return;
                else
                {
                    string _pcode;
                    double _price;
                    int _qty;
                    conn.Open();
                    cmd = new SqlCommand("SELECT * FROM tbProduct WHERE barcode LIKE '" + txtBarcode.Text +"'", conn);
                    dr = cmd.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        qty = int.Parse(dr["qty"].ToString());
                        _pcode = dr["pcode"].ToString();
                        _price = double.Parse(dr["price"].ToString());
                        _qty = int.Parse(txtQty.Text);

                        dr.Close();
                        conn.Close();

                        //Insert to tbCart
                        AddToCart(_pcode, _price, _qty);
                    }
                    dr.Close();
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }
        public void AddToCart(string _pcode, double _price, int _qty)
        {
            try
            {
                string id = "";
                int cart_qty = 0;
                bool found = false;
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM tbCart WHERE transno = @transno and pcode = @pcode", conn);
                cmd.Parameters.AddWithValue("@transno", lblTranNo.Text);
                cmd.Parameters.AddWithValue("@pcode", _pcode);
                dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    id = dr["id"].ToString();
                    cart_qty = int.Parse(dr["qty"].ToString());
                    found = true;
                } 
                else found = false;
                dr.Close();
                conn.Close();

                if (found)
                {
                    if (qty < (int.Parse(txtQty.Text) + cart_qty))
                    {
                        Utilities.MessageWarning("Unable to procced. Remaining quantity on hand is " + qty);
                        return;
                    }
                    conn.Open();
                    cmd = new SqlCommand("UPDATE tbCart SET qty = (qty + " + _qty + ") WHERE id ='"+ id +"'", conn);
                    cmd.ExecuteReader();
                    conn.Close();
                    txtBarcode.SelectionStart = 0;
                    txtBarcode.SelectionLength = txtBarcode.Text.Length; 
                    LoadCart();
                }
                else
                {
                    if (qty < (int.Parse(txtQty.Text) + cart_qty))
                    {
                        Utilities.MessageWarning("Unable to procced. Remaining quantity on hand is " + qty);
                        return;
                    }
                    conn.Open();
                    cmd = new SqlCommand("INSERT INTO tbCart(transno, pcode, price, qty, sdate, cashier) VALUES (@transno, @pcode, @price, @qty, @sdate, @cashier)", conn);
                    cmd.Parameters.AddWithValue("@transno", lblTranNo.Text);
                    cmd.Parameters.AddWithValue("@pcode", _pcode);
                    cmd.Parameters.AddWithValue("@price", _price);
                    cmd.Parameters.AddWithValue("@qty", _qty);
                    cmd.Parameters.AddWithValue("@sdate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@cashier", lblUsername.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    LoadCart();
                }
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void dgvCashier_SelectionChanged(object sender, EventArgs e)
        {
            int i = dgvCashier.CurrentRow.Index;
            id = dgvCashier[1, i].Value.ToString();
            price = dgvCashier[7, i].Value.ToString();
        }

        private void dgvCashier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvCashier.Columns[e.ColumnIndex].Name;


            if (colName == "Delete")
            {
                if (Utilities.MessageYesNo("You want to remove this item?", "Remove item") == DialogResult.Yes)
                {
                    dbconn.ExecuteQuery("DELETE FROM tbCart WHERE id LIKE'" + dgvCashier.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    Utilities.MessageInfo("Items has been successfully remove");
                    LoadCart();
                }
            }
            else if (colName == "colAdd")
            {
                int i = 0;
                conn.Open();
                cmd = new SqlCommand("SELECT SUM(qty) AS qty FROM tbProduct WHERE pcode LIKE'" + dgvCashier.Rows[e.RowIndex].Cells[2].Value.ToString() + "' GROUP BY pcode", conn);
                i = int.Parse(cmd.ExecuteScalar().ToString());
                conn.Close();
                if (int.Parse(dgvCashier.Rows[e.RowIndex].Cells[5].Value.ToString()) < i)
                {
                    dbconn.ExecuteQuery("UPDATE tbCart SET qty = qty + " + int.Parse(txtQty.Text) + " WHERE transno LIKE '" + lblTranNo.Text + "'  AND pcode LIKE '" + dgvCashier.Rows[e.RowIndex].Cells[2].Value.ToString() + "'");
                    LoadCart();
                }
                else
                {
                    Utilities.MessageWarning("Remaining qty on hand is " + i + "!");
                    return;
                }
            }
            else if (colName == "colReduce")
            {
                int i = 0;
                conn.Open();
                cmd = new SqlCommand("SELECT SUM(qty) as qty FROM tbCart WHERE pcode LIKE'" + dgvCashier.Rows[e.RowIndex].Cells[2].Value.ToString() + "' GROUP BY pcode", conn);
                i = int.Parse(cmd.ExecuteScalar().ToString());
                conn.Close();
                if (i > 1)
                {
                    dbconn.ExecuteQuery("UPDATE tbCart SET qty = qty - " + int.Parse(txtQty.Text) + " WHERE transno LIKE '" + lblTranNo.Text + "'  AND pcode LIKE '" + dgvCashier.Rows[e.RowIndex].Cells[2].Value.ToString() + "'");
                    LoadCart();
                }
                else
                {
                    MessageBox.Show("Remaining qty on cart is " + i + "!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }
        public void Noti()
        {
            int i = 0;
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM viewCriticalItems", conn);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i++;
                Alert alert = new Alert(new MainForm());
                alert.lblPcode.Text = dr["pcode"].ToString();
                alert.showAlert(i + ". " + dr["pdesc"].ToString() + " - " + dr["qty"].ToString());
            }
            dr.Close();
            conn.Close();
        }

        private void Cashier_Load(object sender, EventArgs e)
        {
            Noti();
        }

        private void Cashier_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F8)
            {
                captureDevice.OnFrameArrived += captureDevice_OnFrameArrived;
                captureDevice.Start();
            }
        }

        private void captureDevice_OnFrameArrived(object source, FrameArrivedEventArgs e)
        {
            Bitmap bitmap = (Bitmap)e.GetFrame();
            BarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(bitmap);
            if (result != null)
            {
                txtBarcode.Invoke(new MethodInvoker(delegate ()
                { txtBarcode.Text = result.ToString(); }
                )); 
            }
        }

        private void Cashier_FormClosing(object sender, FormClosingEventArgs e)
        {
            captureDevice.Stop();
        }
    }
}   
