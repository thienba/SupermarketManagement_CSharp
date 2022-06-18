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
    public partial class POSReport : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        DBConnect dbconn = new DBConnect();
        SqlDataReader dr;
        string store;
        string address;
        public POSReport()
        {
            InitializeComponent();
            conn = new SqlConnection(dbconn.myConnection());
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
        private void reportViewer1_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void LoadDailyReport(string sql, string param, string cashier)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptSoldReport.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtSoldReport"]);
                conn.Close();

                ReportParameter pDate = new ReportParameter("pDate", param);
                ReportParameter pCashier = new ReportParameter("pCashier", cashier);
                ReportParameter pHeader = new ReportParameter("pHeader", "Daily Sales Report");
                ReportParameter pStore = new ReportParameter("pStore", store);
                ReportParameter pAddress = new ReportParameter("pAddress", address);

                reportViewer1.LocalReport.SetParameters(pDate);
                reportViewer1.LocalReport.SetParameters(pCashier);
                reportViewer1.LocalReport.SetParameters(pHeader);
                reportViewer1.LocalReport.SetParameters(pStore);
                reportViewer1.LocalReport.SetParameters(pAddress);

                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtSoldReport"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 30;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }
        public void LoadTopSelling(string sql, string param, string header)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptTopSell.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtTopSell"]);
                conn.Close();

                ReportParameter pDate = new ReportParameter("pDate", param);
                ReportParameter pHeader = new ReportParameter("pHeader", header);

                reportViewer1.LocalReport.SetParameters(pDate);
                reportViewer1.LocalReport.SetParameters(pHeader);

                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtTopSell"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 30;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }
        public void LoadSoldItems(string sql, string param)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptSoldItems.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtSoldItems"]);
                conn.Close();

                ReportParameter pDate = new ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);
                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtSoldItems"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        public void LoadInventory(string sql)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptInventory.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtInventory"]);
                conn.Close();

                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtInventory"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        public void LoadCancelledOrder(string sql, string param)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptCancelled.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtCancelOrder"]);
                conn.Close();

                ReportParameter pDate = new ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);
                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtCancelOrder"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        public void LoadStockInHist(string sql, string param)
        {
            try
            {
                ReportDataSource rptDataSoure;
                this.reportViewer1.LocalReport.ReportPath = Application.StartupPath + @"\Reports\rptStockInHist.rdlc";
                this.reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds = new DataSet1();
                SqlDataAdapter da = new SqlDataAdapter();
                conn.Open();
                da.SelectCommand = new SqlCommand(sql, conn);
                da.Fill(ds.Tables["dtStockInHist"]);
                conn.Close();

                ReportParameter pDate = new ReportParameter("pDate", param);

                reportViewer1.LocalReport.SetParameters(pDate);
                rptDataSoure = new ReportDataSource("DataSet1", ds.Tables["dtStockInHist"]);
                reportViewer1.LocalReport.DataSources.Add(rptDataSoure);
                reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
            }
            catch (Exception ex)
            {
                conn.Close();
                Utilities.MessageWarning(ex.Message);
            }
        }

        private void POSReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
