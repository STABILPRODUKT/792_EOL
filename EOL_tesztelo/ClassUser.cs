using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EOL_tesztelo
{
    public struct sUser
    {
        private int ID;
        public string LoginName;
        public string Password;
        public string FirstName;
        public string LastName;
        public int Right;
        public int Delet;
    }

    class ClassUser
    {
        private String TableName = "[User]";
        public static sUser LoginUser = new sUser();
        public static List<sUser> lsUser = new List<sUser>();
        private DataBase db;
        public bool LoginSuccessfull = false;


        //felhasználó típusok
        public const int lsUserOperator = 1;
        public const int lsUserMaintenance = 2;
        public const int lsUserManager = 3;
        public const int lsUserAdmin = 4;

        public const string lsUserOperator_text = "Operátor";
        public const string lsUsermaintenance_text = "Karbantartó";
        public const string lsUserManager_text = "Minőségügyi";
        public const string lsUserAdmin_text = "Admin";

        public void SelectedUsers()
        {
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT * from " + TableName + " ");
            lsUser.Clear();
            foreach (DataRow row in dt.Rows)
            {
                sUser su = new sUser();
                su.LoginName = row[1].ToString().Trim();
                su.FirstName = row[3].ToString().Trim();
                su.LastName = row[4].ToString().Trim();
                su.Right = Convert.ToInt32(row[5]);
                su.Delet= Convert.ToInt32(row[6]);
                lsUser.Add(su);
            }
        }
        public bool Login(string LoginName, string Password)
        {
            ulong pass= PasswordGenerator(Password);
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT * from " + TableName + " where [LoginName]= '"+LoginName+"' and [Password] ='"+ pass+ "' and Delet=0");
            foreach (DataRow row in dt.Rows)
            {
                LoginUser.LoginName = row[1].ToString().Trim();
                LoginUser.FirstName = row[3].ToString().Trim();
                LoginUser.LastName = row[4].ToString().Trim();
                LoginUser.Right =Convert.ToInt32( row[5]);
                LoginSuccessfull = true;
                return true;
            }
            LoginSuccessfull = false;
            return false;
        }
        public bool userCheked(string LoginName, string Password)
        {
            ulong pass = PasswordGenerator(Password);
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT * from " + TableName + " where [LoginName]= '" + LoginName + "' and [Password] ='" + pass + "' and Delet=0");
            foreach (DataRow row in dt.Rows)
            {
                return true;
            }
            return false;
        }
        private ulong PasswordGenerator(string Password)
        {
            ulong pass = 0;
            foreach (char c in Password)
            {
                pass = pass * 128 + Convert.ToByte(c);
            }
            return pass;
        }
        public bool userUpdate(sUser su)
        {
            if (su.LoginName.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid felhasználónév!");
                return false;
            }
            if (su.LastName.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid vezetéknév!");
                return false;
            }
            if (su.FirstName.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid keresztnév!");
                return false;
            }
            foreach (sUser suc in lsUser)
            {
                if (suc.LoginName == su.LoginName)
                {
                    if(suc.LastName!=su.LastName)
                        ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + su.LoginName + " vezetékneve : " + suc.LastName.ToString() + "  ->  " + su.LastName.ToString());
                    if (suc.FirstName != su.FirstName)
                        ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + su.LoginName + " keresztneve : " + suc.FirstName.ToString() + "  ->  " + su.FirstName.ToString());
                    if (su.Password.Length>0)
                        ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + su.LoginName + " jelszava ");
                    if (suc.Right != su.Right)
                        ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + su.LoginName + " felhasználói joga: " + suc.Right.ToString() + "  ->  " + su.Right.ToString());
                    if (suc.Delet != su.Delet)
                        ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + su.LoginName + " törlési állapota : " + suc.Delet.ToString() + "  ->  " + su.Delet.ToString());
                    break;
                }
            }
           
            ulong pass = PasswordGenerator(su.Password);
            db = new DataBase();
            string text;
            if(su.Password.Length>0)
             text = @"UPDATE [dbo]." + TableName + " SET [Password]='" + pass + "', [FirstName] ='" + su.FirstName + "', [LastName] ='" + su.LastName + "', [UserType] = " + su.Right + ", [Delet] = " + su.Delet + " where LoginName='"+ su.LoginName+"'";
            else
                text = @"UPDATE [dbo]." + TableName + " SET  [FirstName] ='" + su.FirstName + "', [LastName] ='" + su.LastName + "', [UserType] = " + su.Right + ", [Delet] = " + su.Delet + " where LoginName='" + su.LoginName + "'";
            db.getData(text);
            Login(su.LoginName, su.Password);
            SelectedUsers();
            return true;
        }

        public bool AddUser(sUser su)
        {
            if (su.LoginName.Length < 3 )
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid felhasználónév!");
                return false;
            }
            if ( su.LastName.Length < 3 )
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid vezetéknév!");
                return false;
            }
            if (su.FirstName.Length < 3 )
            {
                System.Windows.Forms.MessageBox.Show("Túl rövid keresztnévnév!");
                return false;
            }
            if (su.Password.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Nincs megadva jelszó!");
                return false;
            }
            DataTable dt = db.getData(@"SELECT * from " + TableName + " where [LoginName]= '" + su.LoginName + "' ");
            foreach (DataRow row in dt.Rows)
            {
                System.Windows.Forms.MessageBox.Show("A felhasználó már létezik!");
                return false;
            }
            ulong pass = PasswordGenerator(su.Password);
            db = new DataBase();
            string text = @"INSERT INTO [dbo]." + TableName + " VALUES ('" + su.LoginName+ "','" + pass + "','" + su.FirstName + "','" + su.LastName + "'," + su.Right + "," + 0 + ")";
            db.getData(text);
            SelectedUsers();
            return true;
        }
    }
}

