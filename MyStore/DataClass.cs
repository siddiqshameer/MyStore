using System;
using System.Data.SqlClient;

using System.Configuration;
using System.Data;

namespace MyStore
{
    public class DataClass
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["storedataConnectionString"].ConnectionString);
        public DataTable StockLoad()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "select * from stock";
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
            
        }

        
    }
}
