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
using Microsoft.Reporting.WinForms;


namespace SupermarketManagement
{
    public partial class Receipt : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        string store;
        string address;
        Cashier cashier;
        public Receipt(Cashier cash)
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
            cashier = cash;
            LoadStore();
        }
        public void LoadStore()
        {
            conn.Open();
            cmd = new SqlCommand("SELECT * FROM tbStore", conn);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                store = dr["store"].ToString();
                address = dr["address"].ToString();
            }
            dr.Close();
            conn.Close();
        }
        private void Receipt_Load(object sender, EventArgs e)
        {
            this.reportViewer.RefreshReport();
        }
        public void LoadRecept(string pcash, string pchange)
        {
            ReportDataSource rptDataSoure;
            try
            {
                this.reportViewer.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptReceipt.rdlc";
                this.reportViewer.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();

                conn.Open();
                da.SelectCommand = new SqlCommand("SELECT c.id, c.transno, c.pcode, c.price, c.qty, c.disc, c.total, c.sdate, c.status, p.pdesc FROM tbCart AS c INNER JOIN tbProduct AS p ON p.pcode=c.pcode WHERE c.transno LIKE '" + cashier.lblTranNo.Text + "'", conn);
                da.Fill(ds.Tables["dtReceipt"]);
                conn.Close();

                ReportParameter pVatable = new ReportParameter("pVatable", cashier.lblVatable.Text);
                ReportParameter pVat = new ReportParameter("pVat", cashier.lblVat.Text);
                ReportParameter pDiscount = new ReportParameter("pDiscount", cashier.lblDiscount.Text);
                ReportParameter pTotal = new ReportParameter("pTotal", cashier.lblDisplayTotal.Text);
                ReportParameter pCash = new ReportParameter("pCash", pcash);
                ReportParameter pChange = new ReportParameter("pChange", pchange);
                ReportParameter pStore = new ReportParameter("pStore", store);
                ReportParameter pAddress = new ReportParameter("pAddress", address);
                ReportParameter pTransaction = new ReportParameter("pTransaction", "Invoice #: " + cashier.lblTranNo.Text);
                ReportParameter pCashier = new ReportParameter("pCashier", cashier.lblUsername.Text);

                reportViewer.LocalReport.SetParameters(pVatable);
                reportViewer.LocalReport.SetParameters(pVat);
                reportViewer.LocalReport.SetParameters(pDiscount);
                reportViewer.LocalReport.SetParameters(pTotal);
                reportViewer.LocalReport.SetParameters(pCash);
                reportViewer.LocalReport.SetParameters(pChange);
                reportViewer.LocalReport.SetParameters(pStore);
                reportViewer.LocalReport.SetParameters(pAddress);
                reportViewer.LocalReport.SetParameters(pTransaction);
                reportViewer.LocalReport.SetParameters(pCashier);

                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtReceipt"]);
                reportViewer.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer.ZoomMode = ZoomMode.Percent;
                reportViewer.ZoomPercent = 30;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void Receipt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Dispose();
        }
    }
}
