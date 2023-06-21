using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_tesztelo
{
   public class ClassSerialNumber
    {
        public static int SerialNumber;
        DataBase db;
        public ClassSerialNumber()
        {
            Select();
        }
        public void Select()
        {

            db = new DataBase();

            string text = "";

            text = @"SELECT  [SerialNumber] FROM [EOL_792].[dbo].[SerialNumber]";


            DataTable dt = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.

            foreach (DataRow row in dt.Rows)
            {

                SerialNumber = (int)row[0];
            }
        }
        public void Update(int SerialNumber)
        {

            db = new DataBase();

            string text = "";

            text = @"UPDATE [dbo].[SerialNumber]
   SET [SerialNumber] = "+ SerialNumber + @"
   where [SerialNumber]=[SerialNumber]";

            db.setData(text); //DataTable-be belerakom az SQL Query eredményét.

        }

    }
}
