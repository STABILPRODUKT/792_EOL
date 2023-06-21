using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Office.Interop;

namespace EOL_tesztelo
{ 

    class ClassRowsDiagnostic : ClassSheet
    {
        public int S_id;
        public string TableName;
        private DataBase db;
        public static bool bEtalonMenu_update=false;


        public const int cS_id = 0;
        public const int cD_id = 1;
        public const int cDescription = 2;
        public const int cVisible = 3;
        public const int cVisibleCB = 4;
        public const int cLoginName = 5;
        public const int cModifyDate = 6;


        public static string scNemFigyel = "Nem figyel";
        public static string scMegfelelo = "Megfelelő";
        public static string scHibas = "Hibás";
       public DataTable dt_DiagRowsDefault;

        public ClassRowsDiagnostic( string TableName, ref DataGridView DGV)
        {
            List<sComboBoxColumnItem> lsFigyeles = new List<sComboBoxColumnItem>();
            lsFigyeles.Add(new sComboBoxColumnItem(0, scNemFigyel));
            lsFigyeles.Add(new sComboBoxColumnItem(1, scMegfelelo));
            lsFigyeles.Add(new sComboBoxColumnItem(2, scHibas));
            dt = new DataTable();
            lsDataHeadStruct = new List<classDataHeadStruct>();
            lsDataHeadStruct.Add(new classDataHeadStruct("S_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn,true));
            lsDataHeadStruct.Add(new classDataHeadStruct("D_id", typeof(int), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Leírás", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 500));
            lsDataHeadStruct.Add(new classDataHeadStruct("Megjelenítés", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Megjelenítés", typeof(string), classDataHeadStruct.sVisible, ClassSheet.ComboBoxColumn, false, 200, lsFigyeles));
            lsDataHeadStruct.Add(new classDataHeadStruct("Felhasználó", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 250));
            lsDataHeadStruct.Add(new classDataHeadStruct("Módosítva", typeof(DateTime), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            cCellEndEditColumn = cD_id;
            this.DGV = DGV;
            DoubleBuffered( true);
            this.TableName = TableName;
            dt_DiagRowsDefault = new DataTable();
        }
        private int StringToIntVisibleValue(string str)
        {

            int ret = 1;
            switch (str)
            {
                case "Nem figyel":
                    ret = 0;
                    break;

                case "Megfelelő":
                    ret = 1;
                    break;
                case "Hibás":
                    ret = 2;
                    break;
            }
            return ret;
        }
        private string IntToStringVisibeValue(int index)
        {
            string ret = "Nem figyel";
            switch (index)
            {
                case 0:
                    ret = "Nem figyel";
                    break;
                case 1:
                    ret = "Megfelelő";
                    break;
                case 2:
                    ret = "Hibás";
                    break;
                default:
                    ret = "Nem figyel";
                    break;
            }
            return ret;
        }
        public override void Select()
        {
      
        }
        public void Select(int S_id)
        {
            this.S_id = S_id;
            AddRowIndex = DefaultAddRowIndex;
            db = new DataBase();
            dtDelete = new DataTable();
            dtModify = new DataTable();
            string text = "";

            text = "SELECT  " + TableName + ".[S_id] ," + TableName + ".[D_id] ,[Description] ," + TableName + ".[Visible], '' as VisibleCB," + TableName + ".[LoginName]" +
     " ," + TableName + ".[ModifyDate] from " + TableName + " inner join [dbo].[DiagRowsDefault] on " + TableName + ".[D_id] =[dbo].[DiagRowsDefault].[D_id]  where " + TableName + ".S_id =" + S_id + " and [DiagRowsDefault].Visible=1 order by " + TableName + ".[D_id]";
            DataTable dt = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.
            this.dt = dt;
            dtModify = new DataTable();
            this.dtModify = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                DataRow rowtemp = dtModify.NewRow();
                rowtemp.ItemArray = (object[])row.ItemArray.Clone();
                rowtemp[cVisibleCB] = IntToStringVisibeValue((int)rowtemp[cVisible]);
                this.dtModify.Rows.Add(rowtemp);
            }
        }
        public void SelectDefault()
        {
            db = new DataBase();
            dt_DiagRowsDefault = new DataTable();
            string text = "";

            string TableNameTemp = "DiagRowsDefault";
            text = "SELECT * from " + TableNameTemp + "  where " + TableNameTemp + ".S_id =1";
            dt_DiagRowsDefault = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.
        }

        public override void SaveModify()
        { 
            foreach (DataRow r in dtModify.Rows)
            {
                if (Convert.ToInt32(r[cD_id]) < DefaultAddRowIndex)
                    ModifyUpdate(r);
                else
                    ModifyInsert(r);
            }
            dtModify.Rows.Clear();
            try
            {
               /* if (dtDelete.Rows != null)
                {
                    foreach (DataRow r in dtDelete.Rows)
                        ModifyDelete(r);
                }*/
            }
            catch (System.NullReferenceException) { }
        }


        public void RowsDiagnosticsAdd() 
        {
            db = new DataBase();
            foreach (DataRow row in dt.Rows)
            {
                string text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + (Convert.ToInt32(row[cS_id]) +1) + ", " + row[cD_id] + 
                " , " + StringToIntVisibleValue(row[cVisibleCB].ToString()) + ", '" + ClassUser.LoginUser.LoginName + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "') ";
                db.getData(text);
            }
        }
        public void RowsDiagnosticsAdd(DataRow row, int S_id)//a táblázat értékeit fel teszük az SQL-be 
        {
                string text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + (S_id) + ", " + row[cD_id] + 
                 " ,  " + StringToIntVisibleValue(row[cVisibleCB].ToString()) + ", '" + ClassUser.LoginUser.LoginName + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "') ";
                db.getData(text);   
        }
 
        public void RowsDiagnosticsAdd(List<string> ls, int S_id) //a táblázat értékeit fel teszük az SQL-be 
        {
            db = new DataBase();
            int d_id = 1;
            foreach (string str in ls)
            {
                string text = @"INSERT INTO  [dbo].["+ TableName + "] VALUES ( " + S_id + ", " + d_id + 
                " ,   " + FormEtalonProperty.scNemHasznal + ", '" + ClassUser.LoginUser.LoginName + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "') ";
                db.getData(text);
                d_id++;
            }
        }

        protected void ModifyUpdate(DataRow row)
        {
            string text = @"UPDATE [dbo].[" + TableName + "] SET    Visible=" + StringToIntVisibleValue(row[cVisibleCB].ToString()) + ", LoginName='" + ClassUser.LoginUser.LoginName + "', ModifyDate= '" + Convert.ToDateTime(row[cModifyDate]).ToString("yyyy-MM-dd HH:mm") + "'  " +
                            " WHERE S_id=" + row[cS_id] + " AND D_id=" + row[cD_id];
            db.getData(text);
        }
        protected override void ModifyInsert(DataRow row) // SensorPropertiesModifySave()
        {
        }

        public void RowsDiagnosticsDelete(int S_id) 
        {
            db = new DataBase();
            string text = @"DELETE  [dbo].[" + TableName + "] " + " WHERE S_id=" + S_id;
            db.getData(text);

        }

        protected  void ModifyDelete(DataRow row)
        {
            string text = @"DELETE  [dbo].[" + TableName + "] " + " WHERE S_id=" + row[cS_id];
            db.getData(text);
        }
        public override void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cModifyDate], DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cLoginName], ClassUser.LoginUser.LoginName);
        }
    }
}
