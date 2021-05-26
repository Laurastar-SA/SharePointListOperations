using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace SharePointListOperations
{
    public class LSSqlOperations
    {
        //Connect to the SQL server
        public static bool ConnectToSqlServer(SqlConnection conn, string conn_string)
        {
            bool is_connected = false;

            try
            {
                conn.Open();
                is_connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nem sikerült csatalkozni az adatbázishoz" + "\n\r" + ex);
            }

            return is_connected;
        }

        //Query from the SQL server with datareader, returns with the value of the "result field"
        public static string SqlQuery(SqlConnection conn, string query, bool date_based, string param1, string paramvalue1, string param2, string paramvalue2, string result_field)
        {
            string result = "";

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = query;

            if (date_based)
            {
                command.Parameters.AddWithValue("@start_date", DateTime.Parse(DateTime.Now.ToShortDateString().ToString() + " 0:00:00"));
                command.Parameters.AddWithValue("@end_date", DateTime.Parse(DateTime.Now.ToShortDateString().ToString() + " 23:59:00"));
            }

            if (param1 != null && paramvalue1 != null)
            {
                command.Parameters.AddWithValue(param1, paramvalue1);
            }

            if (param2 != null && paramvalue2 != null)
            {
                command.Parameters.AddWithValue(param2, paramvalue2);
            }

            SqlDataReader reader = (null);

            try
            {
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = (reader[result_field].ToString());
                    //Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("A lekérdezés futtatása nem sikerült.\r\n" + ex);
            }

            if (reader != null)
            {
                reader.Close();
            }
            command.Cancel();

            return result;
        }

        //SQL Query to Datatable
        public static DataTable SQLDataTableQuery(SqlConnection conn, string query, bool date_based, string param1, string paramvalue1, string param2, string paramvalue2, string result_field)
        {
            DataTable d_table = new DataTable();

            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandText = query;

            if (date_based)
            {
                command.Parameters.AddWithValue("@start_date", DateTime.Parse(DateTime.Now.ToShortDateString().ToString() + " 0:00:00"));
                command.Parameters.AddWithValue("@end_date", DateTime.Parse(DateTime.Now.ToShortDateString().ToString() + " 23:59:00"));
            }

            if (param1 != null && paramvalue1 != null)
            {
                command.Parameters.AddWithValue(param1, paramvalue1);
            }

            if (param2 != null && paramvalue2 != null)
            {
                command.Parameters.AddWithValue(param2, paramvalue2);
            }

            SqlDataAdapter d_adapter = new SqlDataAdapter();

            return d_table;
        }
    }
}
