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

   public class ClassSensorProperty : ClassSheet
    {
        public string TableName;

        public const int RendSzamDefault = 9999;  //Alapértelmemzett rendelési szám   
        public static int MaxRendSzamBelso;
        public static bool bTomegesVarhErk = false; //Tömemges státusz állítás
        bool type;

        public const int cS_id = 0;
        public const int cD_id = 1;
        public const int cPc_id = 2;
        public const int cDescription = 3;
        public const int chysteresisNeg = 4;
        public const int cDefaultValue = 5;
        public const int chysteresisPoz = 6;
        public const int cScale = 7;
        public const int cVisible = 8;
        public const int cVisibleCB = 9;
        public const int cLoginName = 10;
        public const int cModifyDate = 11;

        public static string scNemFigyel = "Nem figyel";
        public static string scMegfelelo = "Megfelelő";
        public static string scHibas = "Hibás";

        public string Megnevezes;
        private DataBase db;

        public ClassSensorProperty(string TableName, ref DataGridView DGV, string Megnevezes, bool type = false)
        {
            List<sComboBoxColumnItem> lsFigyeles = new List<sComboBoxColumnItem>();
            lsFigyeles.Add(new sComboBoxColumnItem(0, scNemFigyel));
            lsFigyeles.Add(new sComboBoxColumnItem(1, scMegfelelo));
        //    lsFigyeles.Add(new sComboBoxColumnItem(2, scHibas));
            dt = new DataTable();
            lsDataHeadStruct = new List<classDataHeadStruct>();
            lsDataHeadStruct.Add(new classDataHeadStruct("S_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("D_id", typeof(int), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Pc_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Leírás", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 300));
            lsDataHeadStruct.Add(new classDataHeadStruct("hysteresisNeg", typeof(double), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Alap érték", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("hysteresisPoz", typeof(double), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Scale", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Visible", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            if (type)
                lsDataHeadStruct.Add(new classDataHeadStruct("Megjelenítés", typeof(string), classDataHeadStruct.sNotVisible, ClassSheet.ComboBoxColumn, false, 200, lsFigyeles));
            else
                lsDataHeadStruct.Add(new classDataHeadStruct("Megjelenítés", typeof(string), classDataHeadStruct.sNotVisible, ClassSheet.ComboBoxColumn, false, 200, lsFigyeles));

            lsDataHeadStruct.Add(new classDataHeadStruct("Felhasználó", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Módosítva", typeof(DateTime), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            cCellEndEditColumn = cD_id;
            this.DGV = DGV;
            DoubleBuffered(true);

            this.Megnevezes = Megnevezes;
            this.TableName = TableName;
            this.type = type;
        }

        public override void Select()
        {
            AddRowIndex = DefaultAddRowIndex;
            db = new DataBase();
            dtDelete = new DataTable();
            dtModify = new DataTable();
            string text = "";
            if (!type)
            {
                text = @"SELECT  " + TableName + ".[S_id] , " + TableName + ".[D_id] ,[Pc_id] ,[dbo].[DiagRowsDefault].[Description] ,[HysteresisNeg] ,[DefaultValue] ,[HysteresisPoz] " +
                 " ,[Scale]," + TableName + ".[Visible] ,' ' as VisibleCB , " + TableName + ".[LoginName]" +
                 " ,[MidifyDate] from " + TableName + "  inner join [dbo].[DiagRowsDefault] on " + TableName + ".[D_id] =[dbo].[DiagRowsDefault].[D_id] where PC_id=" + Form1.options.PC_id + " and  " + TableName + ".[S_id] in (Select max(S_id) from " + TableName + ")";
          
            
            
            }
            else
            {
                text = @"SELECT [S_id] ,[D_id]  ,[Pc_id] ,[Description] ,[HysteresisNeg]  ,[DefaultValue]  ,[HysteresisPoz] ,[Scale] 
                   ,[Visible] ,' ' as VisibleCB ,[LoginName],[MidifyDate] from " + TableName + " where PC_id=" + Form1.options.PC_id + " and S_id in (Select max(S_id) from " + TableName + ")";
            }


            DataTable dt = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.
            this.dt = dt;
            this.dtModify = dt.Clone();

            foreach (DataRow row in dt.Rows)
            {
                DataRow rowtemp = dtModify.NewRow();
                rowtemp.ItemArray = (object[])row.ItemArray.Clone();
                rowtemp[cVisibleCB] = IntToStringVisibeValue((int)rowtemp[cVisible]);
                this.dtModify.Rows.Add(rowtemp);
            }
        }
        public DataTable SearchDt;
        public  void Select(string type, object S_id)
        {
        
            string text = "";
            SearchDt = new DataTable();
            if (type.Equals("Etalon"))
            {
              /*  text = @"SELECT [S_id] ,[D_id]  ,[Pc_id]  ,[HysteresisNeg]  ,[DefaultValue]  ,[HysteresisPoz] ,[Scale] 
                   ,[Visible] ,' ' as VisibleCB ,[LoginName],[MidifyDate] from ParamEtalonType where PC_id=" + Form1.options.PC_id +
                   " and S_id =" + S_id + " ";*/

                text = @" SELECT ParamEtalonType.[S_id] ,ParamEtalonType.[D_id]  ,[Pc_id] ,[Description] ,[HysteresisNeg]  ,[DefaultValue]  ,[HysteresisPoz] ,[Scale] 
  ,ParamEtalonType.[Visible] ,' ' as VisibleCB ,ParamEtalonType.[LoginName],[MidifyDate] from ParamEtalonType
 join  [EOL_792].[dbo].[DiagRowsDefault] on [DiagRowsDefault].[D_id] = ParamEtalonType.[D_id]  where PC_id = " + Form1.options.PC_id+ " and ParamEtalonType.S_id = " + S_id + " ";
            
            
            }
            else
            {
             /*   text = @"SELECT [S_id] ,[D_id]  ,[Pc_id]  ,[HysteresisNeg]  ,[DefaultValue]  ,[HysteresisPoz] ,[Scale] 
                   ,[Visible] ,' ' as VisibleCB ,[LoginName],[MidifyDate] from ParamNormalType where PC_id=" + Form1.options.PC_id +
                  " and S_id =" + S_id + " ";*/
                text = @" SELECT ParamNormalType.[S_id] ,ParamNormalType.[D_id]  ,[Pc_id] ,[Description] ,[HysteresisNeg]  ,[DefaultValue]  ,[HysteresisPoz] ,[Scale] 
  ,ParamNormalType.[Visible] ,' ' as VisibleCB ,ParamNormalType.[LoginName],[MidifyDate] from ParamNormalType
 join  [EOL_792].[dbo].[DiagRowsDefault] on [DiagRowsDefault].[D_id] = ParamNormalType.[D_id]  where PC_id = " + Form1.options.PC_id + " and ParamNormalType.S_id = " + S_id + " ";


            }

            SearchDt = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.
  
        }
        private string DoubleToString(object d)     //SQL csak pontot fogad el 
        {
            string str = d.ToString();
            return str.Replace(',', '.');
        }

        protected override void ModifyInsert(DataRow row) // SensorPropertiesModifySave()
        {
            string text = "";
            if (!type)
            {
                text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + (Convert.ToInt32(row[cS_id]) + 1) + ", " + row[cD_id] + ",  " + Form1.options.PC_id +
               ", " + DoubleToString(row[chysteresisNeg]) + " , " + DoubleToString(row[cDefaultValue]) +
                             ", " + DoubleToString(row[chysteresisPoz]) + ", " + row[cScale] + ", " + StringToIntVisibleValue(row[cVisibleCB].ToString()) + ", '" + ClassUser.LoginUser.LoginName + "', '" + Convert.ToDateTime(row[cModifyDate]).ToString("yyyy-MM-dd HH:mm") + "') ";
            }
            else
            {
                text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + (Convert.ToInt32(row[cS_id]) + 1) + ", " + row[cD_id] + ",  " + Form1.options.PC_id + ", '" + row[cDescription] +
                              "' ,  " + DoubleToString(row[chysteresisNeg]) + " , " + DoubleToString(row[cDefaultValue]) +
                              ", " + DoubleToString(row[chysteresisPoz]) + ", " + row[cScale] + ", " + StringToIntVisibleValue(row[cVisibleCB].ToString()) + ", '" + ClassUser.LoginUser.LoginName + "', '" + Convert.ToDateTime(row[cModifyDate]).ToString("yyyy-MM-dd HH:mm") + "') ";

            }
            db.getData(text);
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
           /*     case "Hibás":
                    ret = 2;
                    break;*/
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
            /*    case 2:
                    ret = "Hibás";
                    break;*/
                default:
                    ret = "Nem figyel";
                    break;
            }
            return ret;
        }

        public override void SaveModify()
        {
            foreach (DataRow r in dtModify.Rows)
            {
                ModifyInsert(r);
            }
        }
        protected void ModifyInsertType(DataRow row)// SensorPropertiesModifySaveType()
        {
            string text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + (Convert.ToInt32(row[cS_id]) + 1) + ", " + row[cD_id] + ",  " + Form1.options.PC_id + ", '" + row[cDescription] +
                              "' ,  " + DoubleToString(row[chysteresisNeg]) + " , " + DoubleToString(row[cDefaultValue]) +
                              ", " + DoubleToString(row[chysteresisPoz]) + ", " + row[cScale] + ", " + row[cVisible] + ", '" + row[cLoginName] + "', '" + Convert.ToDateTime(row[cModifyDate]).ToString("yyyy-MM-dd HH:mm") + "') ";

            db.getData(text);
        }
        public override void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cModifyDate], DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cLoginName], ClassUser.LoginUser.LoginName);
        }
        public override void WriteDataGridView(bool first = true)
        {
            WriteDataGridViewRunFlag = true;

            DGV.Rows.Clear();
            //  DGV.Visible = false;
            if (first)
            {
                DGV.Columns.Clear();
                //automatikus méretezés és a több sorok engedélyezése
                DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                DGV.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                foreach (classDataHeadStruct stval in lsDataHeadStruct)
                {
                    DGVAddColumns(stval);
                }
                first = false;
            }
            foreach (DataRow stval in dtModify.Rows)
            {
               
                   int k= DGV.Rows.Add(stval.ItemArray);
                if (type)
                {
                    DGV.Rows[k].Cells[cDefaultValue].ValueType = typeof(string);
                    if (Convert.ToInt32(stval.ItemArray[cD_id]) > 99)
                    {
                        DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                        cell.ValueType = typeof(string);
                        cell.Items.Add( "Nem figyel");
                       
                       
                        cell.Items.Add("Figyel");
                        DGV.Rows[k].Cells[cDefaultValue] = cell;
                        if (Convert.ToInt32(stval.ItemArray[cDefaultValue]) == 0)
                            DGV.Rows[k].Cells[cDefaultValue].Value = "Nem figyel";
                        else
                            DGV.Rows[k].Cells[cDefaultValue].Value = "Figyel";
                    }
                }
            }
            //  DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //   DGV.Visible = true;

            WriteDataGridViewRunFlag = false;
        }
        public  void CellEndEdit(DataGridViewRow row)
        {
            if (row.Cells[cCellEndEditColumn].Value != null)
            {
                if (dtModify.Columns.Count < 2)
                {
                    for (int i = 0; i < DGV.Columns.Count; i++)
                        dtModify.Columns.Add(DGV.Columns[i].DataPropertyName);
                }
                int k = int.MaxValue;

                for (int i = 0; i < dtModify.Rows.Count; i++)
                {
                    if (dtModify.Rows[i][cCellEndEditColumn].ToString() == row.Cells[cCellEndEditColumn].Value.ToString())
                    {
                        k = i;
                        break;
                    }
                }
                if (k == int.MaxValue)
                {
                    dtModify.Rows.Add();
                    k = dtModify.Rows.Count - 1;
                }
                for (int j = 0; j < DGV.Columns.Count; j++)
                {
                    if (row.Cells[j].Value != null && row.Cells[j].Value.ToString() != "")
                    {
                        try
                        {
                            dtModify.Rows[k][j] = row.Cells[j].Value;
                        }
                        catch
                        {
                            row.Cells[j].Value = dtModify.Rows[k][j];
                        }
                    }
                    else if (row.Cells[j].ValueType == typeof(DateTime))
                        dtModify.Rows[k][j] = new DateTime();
                    else if (row.Cells[j].ValueType == typeof(int))
                        j = j; //nem csinálunk semmit
                    else if (row.Cells[j].Value == null)
                        dtModify.Rows[k][j] = DBNull.Value;
                    else
                        dtModify.Rows[k][j] = row.Cells[j].Value;
                }
            }
        }
    }
}