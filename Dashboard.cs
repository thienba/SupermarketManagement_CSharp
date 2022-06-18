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
    public partial class Dashboard : Form
    {
        SqlConnection conn = new SqlConnection();
        DBConnect dbconn = new DBConnect();
        public Dashboard()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            string sdate = DateTime.Now.ToShortDateString();
            lblTurnover.Text = dbconn.ExtractData("SELECT ISNULL(SUM(total),0) AS total FROM tbCart WHERE status LIKE 'Sold'").ToString("#,##0.00");
            lblTotalProduct.Text = dbconn.ExtractData("SELECT COUNT(*) FROM tbProduct").ToString("#,##0");
            lblStockOnHand.Text = dbconn.ExtractData("SELECT ISNULL(SUM(qty), 0) AS qty FROM tbProduct").ToString("#,##0");
            lblCriticalItems.Text = dbconn.ExtractData("SELECT COUNT(*) FROM viewCriticalItems").ToString("#,##0");
        }
    }
}
