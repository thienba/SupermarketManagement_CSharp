using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupermarketManagement
{
    class DBConnect
    {
        SqlConnection conn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        private string cn;
        public string myConnection()
        {
            cn = @"Data Source=LAPTOP-GB0TUTPL\SQLEXPRESS;Initial Catalog=SupermarketManagement;Integrated Security=True";
            return cn;
        }
        public DataTable getTable(string query)
        {
            conn.ConnectionString = myConnection();
            cmd = new SqlCommand(query, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }
        public void ExecuteQuery(String sql)
        {
            try
            {
                conn.ConnectionString = myConnection();
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Utilities.MessageWarning(ex.Message);
            }
        }
        public String getPassword(string username)
        {
            string password = "";
            conn.ConnectionString = myConnection();
            conn.Open();
            cmd = new SqlCommand("SELECT password FROM tbUser WHERE username = '" + username + "'", conn);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                password = dr["password"].ToString();
            }
            dr.Close();
            conn.Close();
            return password;
        }
        public double ExtractData(string sql)
        {

            conn = new SqlConnection();
            conn.ConnectionString = myConnection();
            conn.Open();
            cmd = new SqlCommand(sql, conn);
            double data = double.Parse(cmd.ExecuteScalar().ToString());
            conn.Close();
            return data;

        }
    }
}
