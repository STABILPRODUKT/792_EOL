using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace EOL_tesztelo
{
/*    public struct sMeasurementData
    {
        public int S_id;
        public int Pc_id;
        public int D_id;

        public string Description;
        public double ToleranceNeg;
        public double MeasurementValue;
        public double TolerancePoz;
        public int Error;
        public int Visible;

        public List<string> ToList()
        {
            List<string> ret = new List<string>();
       //     ret.Add(this.S_id.ToString());
       //     ret.Add(this.Pc_id.ToString());
            ret.Add(this.D_id.ToString());
            ret.Add(this.Description.ToString());
            if (this.Visible != Form1.scElrejt)
                ret.Add(this.ToleranceNeg.ToString());
            else
                ret.Add("");
            ret.Add(this.MeasurementValue.ToString());
            if (this.Visible != Form1.scElrejt)
                ret.Add(this.TolerancePoz.ToString());
              else
                ret.Add("");
            ret.Add(this.Error.ToString());
       //     ret.Add(this.Visible.ToString());

            return ret;
        }

    }*/
    public class CMeasurement : ClassSheet
    {

        public const int cS_id = 0;
        public const int cPc_id = 1;
        public const int cLoginName = 2;
        public const int cModifyDate = 3;
        public const int cKIONPartNumber = 4;
        public const int cStabilPartNumber = 5;
        public const int cSerialNumber = 6;
        public const int cType = 7;
        public const int cMeasurementType = 8;
        public const int cMeasurementState = 9;
        public const int cType_S_id = 10;
        

        public CMeasurementData CMD;
        public  CMeasurement(ref DataGridView DGV,ref DataGridView DGV_Data)
        {
            lsDataHeadStruct = new List<classDataHeadStruct>();
            lsDataHeadStruct.Add(new classDataHeadStruct("S_id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Pc id", typeof(int), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("Felhasználó", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 250));
            lsDataHeadStruct.Add(new classDataHeadStruct("Dátum", typeof(DateTime), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true));
            lsDataHeadStruct.Add(new classDataHeadStruct("KION cikkszám", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 100));
            lsDataHeadStruct.Add(new classDataHeadStruct("Stabil cikkszám", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 100));
            lsDataHeadStruct.Add(new classDataHeadStruct("Serial szám", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 100));
            lsDataHeadStruct.Add(new classDataHeadStruct("Type", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 100));
            lsDataHeadStruct.Add(new classDataHeadStruct("MeasurementType", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 100));
            lsDataHeadStruct.Add(new classDataHeadStruct("Hiba", typeof(string), classDataHeadStruct.sVisible, ClassSheet.TextBoxColumn, true, 20));
            lsDataHeadStruct.Add(new classDataHeadStruct("Type_S_id", typeof(string), classDataHeadStruct.sNotVisible, ClassSheet.TextBoxColumn, true, 20));


            this.DGV = DGV;
            this.dtModify = new DataTable();

            foreach (classDataHeadStruct cdhs in lsDataHeadStruct)
            {
                this.dtModify.Columns.Add(cdhs.HeadText, cdhs.ValueType);
            }
       //     dtModify.Rows.Add();
            CMD = new CMeasurementData(ref DGV_Data);
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
    }
    

    public class ClassMeasurement
    {
        public CMeasurement sM;
   //   public  ClassMeasurementData ActCMD = new ClassMeasurementData();

        public int SensorProperty_S_id;
        public static string TableName = "Measurement";
        static private DataBase db = new DataBase();
        public bool Olvashat = false;
        public ClassMeasurement( )
        {
            DataGridView asd = new DataGridView();
            sM = new CMeasurement(ref asd, ref asd);
            sM.dtModify.Rows.Add();
            SetMeasurement();
            Olvashat = false;
        }
    
        
        private static int ByteArrayToInt(byte b1, byte b2)
        {
            int ret = b1 * 256 + b2;
            return ret;
        }
        public void SetMeasurement()
        {
            // sM.S_id = GetS_idNext();  //Automatikus
           
            sM.dtModify.Rows[0][CMeasurement.cPc_id] = Form1.options.PC_id;
            sM.dtModify.Rows[0][CMeasurement.cLoginName] = ClassUser.LoginUser.LoginName;
            sM.dtModify.Rows[0][CMeasurement.cModifyDate] = DateTime.Now;
            sM.dtModify.Rows[0][CMeasurement.cSerialNumber] = "---";
        }

        public void MeasurementSave()
        {
            db = new DataBase();
            string text = @"INSERT INTO  [dbo].[" + TableName + "] VALUES ( " + Form1.options.PC_id + ", '"
                + sM.dtModify.Rows[0].ItemArray[CMeasurement.cLoginName] + "',  '" + Convert.ToDateTime( sM.dtModify.Rows[0].ItemArray[CMeasurement.cModifyDate]).ToString("yyyy.MM.dd HH:mm:ss") +
                            "',  '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cKIONPartNumber] + "' ,  '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cStabilPartNumber] + "' , '"
                            + sM.dtModify.Rows[0].ItemArray[CMeasurement.cSerialNumber] + "' , '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cType] + "' , '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementType] + "', '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementState] + "', '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cType_S_id] + "' ) ";
            
            db.getData(text);
            GetSensorProperty_S_id();
        }

        public void MeasurementUpdate()
        {
            db = new DataBase();
            string text = @"UPDATE  [dbo].[" + TableName + "] SET MeasurementState= '" + sM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementState] + "' Where S_id= " + SensorProperty_S_id;
            db.getData(text);
        }

        private void GetSensorProperty_S_id()
        {

            db = new DataBase();
            DataTable dt = db.getData(@"SELECT Max([S_id])  from " + TableName + " where PC_id=" + Form1.options.PC_id);
            foreach (DataRow row in dt.Rows)
            {
                SensorProperty_S_id = Convert.ToInt32(row[0]);
            }
        }  
        static public List<CMeasurement> GetMeasurementExcell(ref DataGridView DGV,ref DataGridView DGV_Data,DateTime Start, DateTime Finish, string SearchSerialNumber, string S_id = "")
        {
            List<CMeasurement> lsME = new List<CMeasurement>();
            string AdWhere = "";
            if (S_id.Length > 0)
            {
                AdWhere = " where [Measurement].[S_id]= '" + S_id + "' ";
            }
            else
            {
                AdWhere = " where [Measurement].[ModifyDate] between '" + Start.ToString("yyyy.MM.dd HH:mm:ss") + "' and '" + Finish.ToString("yyyy.MM.dd HH:mm:ss") + "'";
                if (SearchSerialNumber.Length > 0)
                {
                    AdWhere += " and [SerialNumber]='" + SearchSerialNumber + "' ";
                }
            }


            string text = @" SELECT [Measurement].[S_id] ,[Measurement].[Pc_id] ,[MeasurementData].[D_id] as A1 ,[DiagRowsDefault].[Description]as A2 ,[MeasurementData].[ToleranceNeg]as A3" +
     " ,[MeasurementData].[MeasuremenValue]as A4 ,[MeasurementData].[TolerancePoz]as A5 ,[MeasurementData].[Error]as A6  ,'d'as A7 ,[Measurement].[LoginName]" +
     " ,[Measurement].[ModifyDate] ,[KIONPartNumber] ,[StabilPartNumber] ,[SerialNumber],[Type] ,[MeasurementType]  ,[MeasurementState],[Type_S_id]" +
 " FROM [MeasurementData] INNER JOIN [Measurement] ON [Measurement].[S_id]=[MeasurementData].[S_id] " +
 " INNER JOIN [dbo].[DiagRowsDefault] ON [dbo].[DiagRowsDefault].D_id=[MeasurementData].D_id " +
  AdWhere +
           /* "   union SELECT  [Measurement].[S_id] ,[Measurement].[Pc_id] ,[EEPROMData].[D_id] as A1 ,''as A2 ,''as A3,[EEPROMData].[Value] as A4 ,''as A5  ,''as A6" +
                        "  ,'e'as A7 ,[LoginName] ,[ModifyDate]  ,[KIONPartNumber] ,[StabilPartNumber] ,[SerialNumber] ,[PRODDATE] ,[HW_VER] ,[MeasurementState]"+

                       " FROM [EEPROMData] INNER JOIN [Measurement] ON [Measurement].[S_id]=[EEPROMData].[S_id]"+

                       AdWhere +*/
           " Order by [Measurement].[S_id] ";
            DataTable dt_temp = db.getData(text);

            int lastS_id = -1;
            CMeasurement sMe = new CMeasurement(ref DGV,ref DGV_Data);
            bool first = true;
            bool LastFinish = true;
          

            // foreach (DataRow rowMeasurementData in dtMeasurementData.Rows)
            foreach (DataRow row in dt_temp.Rows)
            {
                if (first)
                {
 
                    DataRow sm = sMe.dtModify.NewRow();
                 //   sm.ItemArray = (object[])row.ItemArray.Clone();


                    int offset = 7;
                    sm[CMeasurement.cS_id] = Convert.ToInt32(row[0]);
                    sm[CMeasurement.cPc_id] = Convert.ToInt32(row[1]);
                    sm[CMeasurement.cLoginName ]= row[offset + 2].ToString().Trim();
                    sm[CMeasurement.cModifyDate] = Convert.ToDateTime(row[offset + 3]);
                    sm[CMeasurement.cKIONPartNumber] = row[offset + 4].ToString().Trim();
                    sm[CMeasurement.cStabilPartNumber] = row[offset + 5].ToString().Trim();
                    sm[CMeasurement.cSerialNumber] = row[offset + 6].ToString().Trim();
                    sm[CMeasurement.cType ]= row[offset + 7].ToString().Trim();
                    sm[CMeasurement.cMeasurementType] = row[offset + 8].ToString().Trim();
                    sm[CMeasurement.cMeasurementState] = row[offset + 9].ToString().Trim();
                    sm[CMeasurement.cType_S_id] = row[offset + 10].ToString().Trim();
                    sMe.dtModify.Rows.Add(sm);

                    first = false;
                    lastS_id = Convert.ToInt32(sm[CMeasurement.cS_id]);
                }
                if (lastS_id != Convert.ToInt32(row[0]))
                {
                    lsME.Add(sMe);
                    sMe =  new CMeasurement(ref DGV, ref DGV_Data);

                    DataRow sm = sMe.dtModify.NewRow();
                 //   sm.ItemArray = (object[])row.ItemArray.Clone();
                    int offset = 7;
                    sm[CMeasurement.cS_id] = Convert.ToInt32(row[0]);
                    sm[CMeasurement.cPc_id] = Convert.ToInt32(row[1]);
                    sm[CMeasurement.cLoginName] = row[offset + 2].ToString().Trim();
                    sm[CMeasurement.cModifyDate] = Convert.ToDateTime(row[offset + 3]);
                    sm[CMeasurement.cKIONPartNumber] = row[offset + 4].ToString().Trim();
                    sm[CMeasurement.cStabilPartNumber] = row[offset + 5].ToString().Trim();
                    sm[CMeasurement.cSerialNumber] = row[offset + 6].ToString().Trim();
                    sm[CMeasurement.cType] = row[offset + 7].ToString().Trim();
                    sm[CMeasurement.cMeasurementType] = row[offset + 8].ToString().Trim();
                    sm[CMeasurement.cMeasurementState] = row[offset + 9].ToString().Trim();
                    sm[CMeasurement.cType_S_id] = row[offset + 10].ToString().Trim();
                    sMe.dtModify.Rows.Add(sm);
                    lastS_id = Convert.ToInt32(sm[CMeasurement.cS_id]);
                    LastFinish = true;
            
                }

                if (row[8].ToString() == "d")
                {
    
                    DataRow sMD = sMe.CMD.dtModify.NewRow();
                    sMD[CMeasurementData.cData_S_id] = Convert.ToInt32(row[0]);
                    sMD[CMeasurementData.cData_Pc_id ]= Convert.ToInt32(row[1]);
                    sMD[CMeasurementData.cData_D_id] = Convert.ToInt32(row[2]);
                    sMD[CMeasurementData.cData_Visible ]= Form1.scHiba;
                    sMD[CMeasurementData.cData_Description] = row[3].ToString().Trim();
                    if (row[4] != System.DBNull.Value)
                    {
                        sMD[CMeasurementData.cData_ToleranceNeg ]= Math.Round(Convert.ToDouble(row[4]), 3);
                    }
                    else
                    {
                        sMD[CMeasurementData.cData_ToleranceNeg] = new double();
                        sMD[CMeasurementData.cData_Visible] = Form1.scElrejt;
                    }
                    if (row[5] != System.DBNull.Value)
                        sMD[CMeasurementData.cData_MeasurementValue ]= Math.Round( Convert.ToDouble(row[5]),3);
                    else
                        sMD[CMeasurementData.cData_ToleranceNeg] = new double();
                    if (row[6] != System.DBNull.Value)
                    {
                        sMD[CMeasurementData.cData_TolerancePoz] = Math.Round(Convert.ToDouble(row[6]), 3);
                    }
                    else
                    {
                        sMD[CMeasurementData.cData_ToleranceNeg] = new double();
                        sMD[CMeasurementData.cData_Visible] = Form1.scElrejt;
                    }
                    sMD[CMeasurementData.cData_Error] = Convert.ToInt32(row[7]);

                    sMe.CMD.dtModify.Rows.Add(sMD);
                    LastFinish = false;
                }

            }
            if (LastFinish == false)
                lsME.Add(sMe);

            return lsME;
        }
       
      /*  static public List<List<string>> GetMeasurementData(int S_id)
        {
            List<List<string>> lSMD = new List<List<string>>();
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT *  from " + ClassMeasurementData.TableNameData + " where S_id =" + S_id + "  order by [D_id]");
            foreach (DataRow row in dt.Rows)
            {
                sMeasurementData sMD = new sMeasurementData();

                sMD.S_id = Convert.ToInt32(row[0]);
                sMD.Pc_id = Convert.ToInt32(row[1]);
                sMD.D_id = Convert.ToInt32(row[2]);
                sMD.Visible = Form1.scHiba;
                sMD.Description = row[3].ToString().Trim();
                if (row[4] != System.DBNull.Value)
                {
                    sMD.ToleranceNeg = Convert.ToDouble(row[4]);
                }
                else
                {
                    sMD.ToleranceNeg = new double();
                    sMD.Visible = Form1.scElrejt;
                }
                if (row[5] != System.DBNull.Value)
                    sMD.MeasurementValue = Convert.ToDouble(row[5]);
                else
                    sMD.ToleranceNeg = new double();
                if (row[6] != System.DBNull.Value)
                {
                    sMD.TolerancePoz = Convert.ToDouble(row[6]);
                }
                else
                {
                    sMD.ToleranceNeg = new double();
                    sMD.Visible = Form1.scElrejt;
                }
                sMD.Error = Convert.ToInt32(row[7]);

                lSMD.Add(sMD.ToList());
            }
            return lSMD;
        }*/



    }
}
