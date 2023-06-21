using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace EOL_tesztelo
{
    public class CMeasurementData : ClassSheet
    {

        public const int cData_S_id = 0;
        public const int cData_Pc_id = 1;
        public const int cData_D_id = 2;

        public const int cData_Description = 3;
        public const int cData_ToleranceNeg = 4;
        public const int cData_MeasurementValue = 5;
        public const int cData_TolerancePoz = 6;
        public const int cData_Error = 7;
        public const int cData_Visible = 8;
        static private DataBase db = new DataBase();
        public List<DataTable> dtList;
        public CMeasurementData(ref DataGridView DGV)
        {
            // lClassMeasurementData = new List<List<string>>();

            lsDataHeadStruct = new List<classDataHeadStruct>();
            lsDataHeadStruct.Add(new classDataHeadStruct("S_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Pc_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("D_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Leírás", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 300));
            lsDataHeadStruct.Add(new classDataHeadStruct("Tolerancia -", typeof(double), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Mértérték", typeof(double), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Tolerancia +", typeof(double), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Hiba", typeof(int), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn));
            lsDataHeadStruct.Add(new classDataHeadStruct("Megjelenítés", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn));

            this.DGV = DGV;
            this.dtModify = new DataTable();
            foreach (classDataHeadStruct cdhs in lsDataHeadStruct)
            {
                this.dtModify.Columns.Add(cdhs.HeadText, cdhs.ValueType);
            }
        }


        public override void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void SaveModify()
        {
            throw new NotImplementedException();
        }

        public override void Select()
        {
            throw new NotImplementedException();
        }

        protected override void ModifyInsert(DataRow row)
        {
            throw new NotImplementedException();
        }
  



        static public string TableNameData = "MeasurementData";
    

        public void MeasurementDataSave(int RowIndex)
        {
           
            db = new DataBase();
            string text = "";
            //if (M.Visible == 3)
            if (Convert.ToInt32( dtModify.Rows[RowIndex][cData_Visible]) == Form1.scHiba)
            {
                text = @"INSERT INTO  [dbo].[" + TableNameData + "] VALUES ( " + dtModify.Rows[RowIndex][cData_S_id] + ", " + dtModify.Rows[RowIndex][cData_Pc_id] + ",  " + dtModify.Rows[RowIndex][cData_D_id] +
               " ,  " + DoubleToString(dtModify.Rows[RowIndex][cData_ToleranceNeg]) + " , " + DoubleToString(dtModify.Rows[RowIndex][cData_MeasurementValue]) +
               ", " + DoubleToString(dtModify.Rows[RowIndex][cData_TolerancePoz]) + ", " + dtModify.Rows[RowIndex][cData_Error] + " ) ";
            }
            //   if (M.Visible ==2 || M.Visible == 1)
            if (Convert.ToInt32(dtModify.Rows[RowIndex][cData_Visible]) == Form1.scElrejt)
            {
                text = @"INSERT INTO  [dbo].[" + TableNameData + "] VALUES ( " + dtModify.Rows[RowIndex][cData_S_id] + ", " + dtModify.Rows[RowIndex][cData_Pc_id] + ",  " + dtModify.Rows[RowIndex][cData_D_id] +
               " ,  " + "null" + " , " + DoubleToString(dtModify.Rows[RowIndex][cData_MeasurementValue]) +
               ", " + "null" + ", " + dtModify.Rows[RowIndex][cData_Error] + " ) ";
            }
            db.getData(text);
  
        }
        private string DoubleToString(object d)     //SQL csak pontot fogad el 
        {
            string str = d.ToString();
            return str.Replace(',', '.');
        }
    }
}
