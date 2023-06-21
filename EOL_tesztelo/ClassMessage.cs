using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_tesztelo
{
    class ClassMessage
    {
        private DateTime ModifyDate;
        private string User;
        private string Message;
        private static string TableName = "Message";

        public static void SetMessage(string User, string Message)
        {
            DataBase db = new DataBase();
            string text = @"INSERT INTO [dbo].[" + TableName + "] VALUES ('" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") +
                "' ,'" + User + "', '" + Message + "')";
            db.getData(text);
        }
    }
}
