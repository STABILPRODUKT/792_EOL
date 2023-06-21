using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EOL_tesztelo
{
    
    class DataBase
    {
        SqlConnection myConnection;
        public static bool ConnectionLife;
        public DataBase()
        {
            string SQLDataSource=EOL_tesztelo.Form1.options.SQLDataSource;
            // myConnection = new SqlConnection(@"Data Source=192.168.19.7\Dev;Initial Catalog=EOL_tester;Integrated Security=True");
          //  myConnection = new SqlConnection(@"Data Source=" + EOL_tesztelo.Form1.options.SQLDataSource + ";Initial Catalog=" + EOL_tesztelo.Form1.options.SQLInitialCatalog + ";Persist Security Info=False;User ID=" + EOL_tesztelo.Form1.options.SQLUserID + ";Password=" + EOL_tesztelo.Form1.options.SQLPassword + ";");
            myConnection = new SqlConnection(@"Data Source=" + EOL_tesztelo.Form1.options.SQLDataSource + ";Initial Catalog=" + EOL_tesztelo.Form1.options.SQLInitialCatalog + ";Integrated Security=True");
            //  connectionString = "Data Source=myserver;Initial Catalog=myDB;Persist Security Info=False;User ID=USER_NAME;Password=USER_PASS;"
        }
        public bool Con()
        {
            try
            {
                myConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public DataTable getData(string str)
        {
            DataTable dt = new DataTable();
            try
            {
                bool retCon = Con();
                
                SqlCommand myCommand = new SqlCommand(str);

                myCommand.Connection = myConnection;
                SqlDataAdapter da = new SqlDataAdapter(myCommand);
              da.Fill(dt);
          
                myConnection.Close();
                ConnectionLife = true;
            }
            catch
            {
            //    System.Windows.Forms.MessageBox.Show("SQL kapcsolat hiba");
                ConnectionLife = false;
            }
            return dt;
        }
        public void setData(string cmdString)
        {
            try
            {
                Con();
                SqlCommand myCommand = new SqlCommand(cmdString, myConnection);
                myCommand.ExecuteNonQuery();
                myConnection.Close();
                ConnectionLife = true;
            }
            catch
            {
                ConnectionLife = false;
                //      System.Windows.Forms.MessageBox.Show("SQL kapcsolat hiba");
            }
        }
    }

}
