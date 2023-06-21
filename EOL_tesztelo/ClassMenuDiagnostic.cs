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

    class ClassMenuDiagnostic : ClassSheet
    {
        public const int cS_id = 0;
        public const int cPc_id = 1;
        public const int cDescription = 2;
        public const int cNumber = 3;
        public const int cType_1_Selected = 4;
        public const int cType_2_Selected = 5;
        public const int cType_3_Selected = 6;
        public const int cUse = 7;
        public const int cUseCB = 8;
        public const int cDelete = 9;
        public const int cLoginName = 10;
        public const int cModifyDate = 11;

        public  const string scHasznal_text = "Használ";
        public  const string scNemHasznal_text = "Nem használ";
        public const int scHasznal = 1;
        public const int scNemHasznal = 0;


        public string TableName;
        private DataBase db;
        public static bool bEtalonMenu_update = false;
        public ClassMenuDiagnostic(string TableName, ref DataGridView DGV )
        {
            List<sComboBoxColumnItem> lsHasznal = new List<sComboBoxColumnItem>();
            lsHasznal.Add(new sComboBoxColumnItem(0, scNemHasznal_text));
            lsHasznal.Add(new sComboBoxColumnItem(1, scHasznal_text));
            dt = new DataTable();
            lsDataHeadStruct = new List<classDataHeadStruct>();
            lsDataHeadStruct.Add(new classDataHeadStruct("S_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Pc_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Leírás", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Sorszám", typeof(int), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Type_1_Selected", typeof(bool), classDataHeadStruct.sNotVisible, ClassSheet.CheckBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Type_2_Selected", typeof(bool), classDataHeadStruct.sNotVisible, ClassSheet.CheckBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Type_3_Selected", typeof(bool), classDataHeadStruct.sNotVisible, ClassSheet.CheckBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Használat", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Használat", typeof(string), classDataHeadStruct.sVisible, ClassSheet.ComboBoxColumn, false, 200, lsHasznal));
            lsDataHeadStruct.Add(new classDataHeadStruct("Törlés", typeof(bool), classDataHeadStruct.sNotVisible, ClassSheet.CheckBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Felhasználó", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Dátum", typeof(DateTime), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            cCellEndEditColumn = cS_id;
            this.DGV = DGV;
            DoubleBuffered(true);

            this.TableName = TableName;
        }
      
        public override void Select()
        {
            AddRowIndex = DefaultAddRowIndex;
            db = new DataBase();
            dtDelete = new DataTable();
            dtModify = new DataTable();
            string text = @"SELECT [S_id] ,[PC_id] ,[Description] ,[Number] ,[Type_1_Selected] ,[Type_2_Selected] ,[Type_3_Selected] "+
                           " ,[Use]  ,'' as UseCB ,[Delete]   ,[LoginName]  ,[ModifyDate] from " + TableName + " where PC_id = " + Form1.options.PC_id + " " +
                           "and[Delete] = '" + false + "' Order by Number";

            DataTable dt = db.getData(text); //DataTable-be belerakom az SQL Query eredményét.
            this.dt = dt;
            this.dtModify = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                DataRow rowtemp = dtModify.NewRow();
                rowtemp.ItemArray = (object[])row.ItemArray.Clone();
                rowtemp[cUseCB] = IntToStringVisibeValue((int)rowtemp[cUse]);
                this.dtModify.Rows.Add(rowtemp);
            }
        }


        private int StringToIntVisibleValue(string str)
        {
            int ret = scHasznal;
            switch (str)
            {
                case scNemHasznal_text:
                    ret = scNemHasznal;
                    break;
                case scHasznal_text:
                    ret = scHasznal;
                    break;

            }
            return ret;
        }
        private string IntToStringVisibeValue(int index)
        {
            string ret = scNemHasznal_text;
            switch (index)
            { 
                case scNemHasznal:
                    ret = scNemHasznal_text;
                    break;
                case scHasznal:
                    ret = scHasznal_text;
                    break;
            }
            return ret;
        }

        public int maxNumber()
        {
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT max( Number) FROM " + TableName + " ");
            foreach (DataRow row in dt.Rows)
            {
                if(!row.IsNull(0))
                    return Convert.ToInt32(row[0]);
            }
            return 0;
        }
 
        public int maxS_idSelect()
        {
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT max( S_id) FROM " + TableName + " ");
            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull(0))
                   return  Convert.ToInt32(row[0])+1;
            }
            return 1;
        }

        public override void SaveModify()
        {
            foreach (DataRow r in dtModify.Rows)
            {

                if (Convert.ToInt32(r[cS_id]) < DefaultAddRowIndex)
                    ModifyUpdate(r);
                else
                    if(Convert.ToBoolean(r[cDelete])==false)
                    ModifyInsert(r);
            }
            dtModify.Rows.Clear();
        }

        public void MenuDiagnosticsAdd() //a táblázat értékeit fel updeteljük az SQL-be 
        {
            int NextS_id = maxS_idSelect();
            string MenuName = "Menü_" + NextS_id.ToString();
            int k = DGV.Rows.Add();
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cS_id].Value = AddRowIndex;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cPc_id].Value = Form1.options.PC_id;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cDescription].Value = MenuName;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cNumber].Value = NextS_id;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cLoginName].Value = ClassUser.LoginUser.LoginName;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cType_1_Selected].Value = true;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cType_2_Selected].Value = true;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cType_3_Selected].Value = true;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cUseCB].Value = scHasznal_text;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cDelete].Value = false;
            DGV.Rows[k].Cells[ClassMenuDiagnostic.cModifyDate].Value = DateTime.Now.ToString("yyyy.MM.dd H:mm:ss");
            CellEndEdit(k);
            AddRowIndex++;
        }
        protected override void ModifyInsert(DataRow row) // SensorPropertiesModifySave()
        {
            string MenuName = "Menü_" + Convert.ToInt32(maxNumber() + 1).ToString();

            string text = @"INSERT INTO  [dbo].[" + TableName + "] OUTPUT Inserted.S_id  VALUES ( " + Form1.options.PC_id +
           ",  '" + MenuName + "' ,  " + (maxNumber() + 1) + ", '" + row[cType_1_Selected] + "','" + row[cType_2_Selected] + "', '" 
           + row[cType_3_Selected] + "', " + StringToIntVisibleValue(row[cUseCB].ToString()) + ",'" + false + "',  '" 
           + ClassUser.LoginUser.LoginName + "', '" + DateTime.Now.ToString("yyyy.MM.dd H:mm:ss") + "') ";

           DataTable tempDt= db.getData(text);

            int S_id = (int)tempDt.Rows[0].ItemArray[0];
            DataGridView dgv = new DataGridView();
            ClassRowsDiagnostic tempCRD = new ClassRowsDiagnostic("DiagRows", ref dgv);
            tempCRD.SelectDefault();
            foreach (DataRow CRDrow in tempCRD.dt_DiagRowsDefault.Rows)
            {
                tempCRD.RowsDiagnosticsAdd(CRDrow, S_id);
            }

        }
        public void MenuDiagnosticsDelete(int iS_id) //2 delete
        {
            db = new DataBase();
            string text = @"UPDATE  [dbo].[" + TableName + "] SET [Delete]='" + true + "' WHERE S_id=" + iS_id;
            db.getData(text);

        }
        public void ModifyUpdate(DataRow row) //2 delete
        {
            db = new DataBase();
            string text = @"UPDATE  [dbo].[" + TableName + "] SET[PC_id] = " + row[cPc_id] +
                " ,[Description] = '" + row[cDescription] +"'"+
                " ,[Number] = " + row[cNumber] +
                " ,[Type_1_Selected] = '" +Convert.ToBoolean( row[cType_1_Selected]) + "'" +
                ",[Type_2_Selected] =  '" + Convert.ToBoolean(row[cType_2_Selected]) + "'" +
                ",[Type_3_Selected] = '" + Convert.ToBoolean(row[cType_3_Selected]) + "'" +
                ",[Use] = " + StringToIntVisibleValue(row[cUseCB].ToString()) +
                ",[Delete] = '" + Convert.ToBoolean(row[cDelete]) + "'" +
                " ,[LoginName] = '" + row[cLoginName] + "'" +
                ",[ModifyDate] = '" + Convert.ToDateTime(row[cModifyDate]).ToString("yyyy-MM-dd HH:mm") + "'" +
                " WHERE S_id=" + row[cS_id] ;
            db.getData(text);

        }
        public override void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cModifyDate], DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            DGVSetData(DGV.Rows[DGV.CurrentRow.Index].Cells[cLoginName], ClassUser.LoginUser.LoginName);
        }
    }
}