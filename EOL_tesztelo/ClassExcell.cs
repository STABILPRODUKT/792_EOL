using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using EOL_tesztelo.Properties;
using System.Drawing.Printing;
using System.Data;

namespace EOL_tesztelo
{

    class ClassExcell
    {
        // string FileName;
        // string path;
        Excel.Application oXL;
        Excel._Workbook oWB;
        Excel._Worksheet oSheet;
        Excel.Range oRng;


        public List<string> log = new List<string>();
        public int row = 1;
        public int column = 1;
        public void Open()
        {
            //Start Excel and get Application object.
            oXL = new Excel.Application();
            oXL.Visible = false;

            //Get a new workbook.
            oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
            //    oWB = (Excel._Workbook)(oXL.Workbooks.Open(@""+ path + "\\" +FileName +".xlsx"));
            //   oSheet = (Excel._Worksheet)oWB.Worksheets[1];

        }

        /*  public void Write(ClassSensorProperty csp, bool Head = true)
          {
              column = 1;
              bool HeadFlag = Head;
              foreach (sSensorProperty tStruct in csp.lsSensorPropertiesActive)
              {
                  if (HeadFlag)
                  {
                      Write(tStruct.ToListHead());
                      HeadFlag = false;
                  }
                  foreach (string item in ((sSensorProperty)tStruct).ToList())
                  {
                      WriteCell(row, column, item);
                      column++;
                  }
                  row++;
                  column = 1;
              }
              oRng = oSheet.get_Range("A1", "D1");
              oRng.EntireColumn.AutoFit(); //automatikus méretezés
          }*/
        public void Write(ClassSensorProperty ActParamType, ClassMeasurement CM, ClassRowsDiagnostic CRDDef, bool newFile = false)
        {
            DateTime dt = DateTime.Now;

            int maxRow = 300;
            int maxColum = 10;
            string[,] str = new string[maxRow, maxColum];
            int i = 0, j = 0;
            Excel.Range c1;
            Excel.Range c2;
            Excel.Range range;


            if (newFile)
            {
                row = 1;
                writeHead(ActParamType, CRDDef);
                row++;
            }

            WriteCell(row, 1, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cSerialNumber]);
            WriteCell(row, 2, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cKIONPartNumber]);
            WriteCell(row, 3, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cStabilPartNumber]);
            WriteCell(row, 4, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementType]);
            WriteCell(row, 5, ActParamType.Megnevezes);
            WriteCell(row, 6, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cLoginName]);
            WriteCell(row, 7, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cModifyDate]);
            WriteCell(row, 8, CM.sM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementState]);


            foreach (DataRow dr in ActParamType.dt.Rows)
            {
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 1) //Kívánt induló nyomás
                    WriteCell(row, 34, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 2) //Nyomásesés figyelés késleltetése
                    WriteCell(row, 35, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 9) //Nyugalmi fogyasztás figyelés késleltetése
                    WriteCell(row, 36, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));


                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 3) //Kívánt nyomás max
                    WriteCell(row, 21, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 4) //Kívánt nyomás min
                    WriteCell(row, 19, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 5) // Kívánt feszültség max
                    WriteCell(row, 24, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 6) // Kívánt feszültség min
                    WriteCell(row, 22, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 7) // Kívánt maximum áramfogyasztás max
                    WriteCell(row, 28, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 8) //Kívánt maximum áramfogyasztás min
                    WriteCell(row, 26, dr[ClassSensorProperty.cDefaultValue]);



                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 10) //Nyugalmi fogyasztás max
                    WriteCell(row, 31, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 11) //Nyugalmi fogyasztás min
                    WriteCell(row, 29, dr[ClassSensorProperty.cDefaultValue]);
             
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 12) //Termék magasság max
                    WriteCell(row, 37, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 13) //Termék magasság min
                    WriteCell(row, 39, dr[ClassSensorProperty.cDefaultValue]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 100)
                {
                    WriteCell(row, 32, dr[ClassSensorProperty.cDefaultValue]);//Zárókupa
                    WriteCell(row, 33, dr[ClassSensorProperty.cDefaultValue]);//Zárókupa
                }
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 101)
                {
                    WriteCell(row, 9, dr[ClassSensorProperty.cDefaultValue]); //Menetrőgzítő 1
                    WriteCell(row, 10, dr[ClassSensorProperty.cDefaultValue]); //Menetrőgzítő 1
                }
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 102)
                {
                    WriteCell(row, 11, dr[ClassSensorProperty.cDefaultValue]); //Menetrőgzítő 2
                    WriteCell(row, 12, dr[ClassSensorProperty.cDefaultValue]); //Menetrőgzítő 2
                }

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 103)
                {
                    WriteCell(row, 13, dr[ClassSensorProperty.cDefaultValue]);//Menetrőgzítő 3
                    WriteCell(row, 14, dr[ClassSensorProperty.cDefaultValue]);//Menetrőgzítő 3
                }

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 104)
                {
                    WriteCell(row, 15, dr[ClassSensorProperty.cDefaultValue]);//Zsugorcső 1
                    WriteCell(row, 16, dr[ClassSensorProperty.cDefaultValue]);//Zsugorcső 1
                }

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 105)
                {
                    WriteCell(row, 17, dr[ClassSensorProperty.cDefaultValue]);//Zsugorcső 2
                    WriteCell(row, 18, dr[ClassSensorProperty.cDefaultValue]);//Zsugorcső 2
                }

                ///mértek 

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 110) //Kívánt nyomás
                    WriteCell(row, 20, dr[ClassSensorProperty.cDefaultValue]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 111) //Kívánt feszültség min
                    WriteCell(row, 23, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 112) //Kívánt feszültség max
                    WriteCell(row, 25, dr[ClassSensorProperty.cDefaultValue]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 113) //Kívánt maximum áramfogyasztás
                    WriteCell(row, 27, dr[ClassSensorProperty.cDefaultValue]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 114) //Nyugalmi fogyasztás 
                    WriteCell(row, 30, dr[ClassSensorProperty.cDefaultValue]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 115) //Termék magasság 
                    WriteCell(row, 38, dr[ClassSensorProperty.cDefaultValue]);

            }
          
            foreach (DataRow dr in CM.sM.CMD.dtModify.Rows)
            {

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id]) == 110) //Kívánt nyomás max
                    WriteCell(row, 21, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(dr[CMeasurementData.cData_D_id]) == 110) //Kívánt nyomás min
                    WriteCell(row, 19, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_ToleranceNeg]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 111) // Kívánt feszültség max
                    WriteCell(row, 24, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 111) // Kívánt feszültség min
                    WriteCell(row, 22, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_ToleranceNeg]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 113) // Kívánt maximum áramfogyasztás max
                    WriteCell(row, 28, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 113) //Kívánt maximum áramfogyasztás min
                    WriteCell(row, 26, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_ToleranceNeg]), 3));

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 114) //Nyugalmi fogyasztás max
                    WriteCell(row, 31, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 114) //Nyugalmi fogyasztás min
                    WriteCell(row, 29, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_ToleranceNeg]), 3));



                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 100)
                {
                    WriteCell(row, 32, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz])));//Zárókupa
                    WriteCell(row, 33, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue])));//Zárókupa
                }
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 101)
                {
                    WriteCell(row, 9, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz]))); //Menetrőgzítő 1
                    WriteCell(row, 10, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]))); //Menetrőgzítő 1
                }
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 102)
                {
                    WriteCell(row, 11, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz])));//Menetrőgzítő 2
                    WriteCell(row, 12, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]))); //Menetrőgzítő 2
                }

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 103)
                {
                    WriteCell(row, 13, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz])));//Menetrőgzítő 3
                    WriteCell(row, 14, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue])));//Menetrőgzítő 3
                }

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 104)
                {
                    WriteCell(row, 15, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz])));//Zsugorcső 1
                    WriteCell(row, 16, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue])));//Zsugorcső 1
                }

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 105)
                {
                    WriteCell(row, 17, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_TolerancePoz])));//Zsugorcső 2
                    WriteCell(row, 18, IntToString(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue])));//Zsugorcső 2
                }

                ///mértek 

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 110) //Kívánt nyomás
                    WriteCell(row, 20, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 111) //Kívánt feszültség min
                    WriteCell(row, 23, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 112) //Kívánt feszültség max
                    WriteCell(row, 25, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));

                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 113) //Kívánt maximum áramfogyasztás
                    WriteCell(row, 27, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 114) //Nyugalmi fogyasztás 
                    WriteCell(row, 30, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));
                if (Convert.ToInt32( dr[CMeasurementData.cData_D_id])== 115) //Termék magasság
                    WriteCell(row, 38, Math.Round(Convert.ToDouble(dr[CMeasurementData.cData_MeasurementValue]), 3));
            }

        }
        private string IntToString(double value)
        {
            if (value > 0.5)
            {
                return "Ok";
            }
            return "Nok";
        }
        private void writeHead(ClassSensorProperty ActParamType, ClassRowsDiagnostic CRDDef)
        {
            WriteCell(row, 1, "SerialNumber");
            WriteCell(row, 2, "KIONPartNumber");
            WriteCell(row, 3, "StabilPartNumber");
            WriteCell(row, 4, "MeasurementType");
            WriteCell(row, 5, "Megnevezes");
            WriteCell(row, 6, "LoginName");
            WriteCell(row, 7, "ModifyDate");
            WriteCell(row, 8, "MeasurementState");


            foreach (DataRow dr in CRDDef.dt_DiagRowsDefault.Rows)
            {
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 1) //Kívánt induló nyomás
                    WriteCell(row, 34, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 2) //Nyomásesés figyelés késleltetése
                    WriteCell(row, 35, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 3) //Kívánt nyomás max
                    WriteCell(row, 21, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 4) //Kívánt nyomás min
                    WriteCell(row, 19, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 5) // Kívánt feszültség max
                    WriteCell(row, 24, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 6) // Kívánt feszültség min
                    WriteCell(row, 22, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 7) // Kívánt maximum áramfogyasztás max
                    WriteCell(row, 28, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 8) //Kívánt maximum áramfogyasztás min
                    WriteCell(row, 26, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 9) //Nyugalmi fogyasztás figyelés késleltetése
                    WriteCell(row, 36, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 10) //Nyugalmi fogyasztás max
                    WriteCell(row, 31, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 11) //Nyugalmi fogyasztás min
                    WriteCell(row, 29, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 12) //Termék magasság max
                    WriteCell(row, 37, dr[ClassSensorProperty.cDescription]);
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 13) //Termék magasság min
                    WriteCell(row, 39, dr[ClassSensorProperty.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 100)
                {
                    WriteCell(row, 32, dr[ClassRowsDiagnostic.cDescription] + " kivánt");//Zárókupa
                    WriteCell(row, 33, dr[ClassRowsDiagnostic.cDescription]);//Zárókupa
                }
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 101)
                {
                    WriteCell(row, 9, dr[ClassRowsDiagnostic.cDescription] + " kivánt"); //Menetrőgzítő 1
                    WriteCell(row, 10, dr[ClassRowsDiagnostic.cDescription]); //Menetrőgzítő 1
                }
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 102)
                {
                    WriteCell(row, 11, dr[ClassRowsDiagnostic.cDescription] + " kivánt"); ; //Menetrőgzítő 2
                    WriteCell(row, 12, dr[ClassRowsDiagnostic.cDescription]); //Menetrőgzítő 2
                }

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 103)
                {
                    WriteCell(row, 13, dr[ClassRowsDiagnostic.cDescription] + " kivánt");//Menetrőgzítő 3
                    WriteCell(row, 14, dr[ClassRowsDiagnostic.cDescription]);//Menetrőgzítő 3
                }

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 104)
                {
                    WriteCell(row, 15, dr[ClassRowsDiagnostic.cDescription] + " kivánt");//Zsugorcső 1
                    WriteCell(row, 16, dr[ClassRowsDiagnostic.cDescription]);//Zsugorcső 1
                }

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 105)
                {
                    WriteCell(row, 17, dr[ClassRowsDiagnostic.cDescription] + " kivánt");//Zsugorcső 2
                    WriteCell(row, 18, dr[ClassRowsDiagnostic.cDescription]);//Zsugorcső 2
                }

                //mértek 
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 110) //Kívánt nyomás
                    WriteCell(row, 20, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 111) //Kívánt feszültség min
                    WriteCell(row, 23, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 112) //Kívánt feszültség max
                    WriteCell(row, 25, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 113) //Kívánt maximum áramfogyasztás
                    WriteCell(row, 27, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 114) //Nyugalmi fogyasztás 
                    WriteCell(row, 30, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 115) //Termék magasság 
                    WriteCell(row, 38, dr[ClassSensorProperty.cDescription]);

            }
        }
        /*
                    public void Write(List<CMeasurement> lsME)
                {
                    //   List<sMeasurement> lsm = ClassMeasurement.GetMeasurement(Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()));       
                    log.Clear();

                    log.Add(DateTime.Now.ToString("mm.ss.fff") + "  Start");
                    DateTime dt = DateTime.Now;

                        int maxRow = 300;
                        int maxColum = 10;
                        string[,] str = new string[maxRow, maxColum];
                        int i = 0, j = 0;
                        bool letoltve = false;
                        Excel.Range c1;
                        Excel.Range c2;
                        Excel.Range range;
                        List<string> lsMeasurementHead = sMeasurement.ToListHead();
                        string[] MeasurementStr = new string[lsMeasurementHead.Count];
                        int p = 0;
                        foreach (string item in lsMeasurementHead)
                        {
                            MeasurementStr[p] = item;
                            p++;
                        }
                        List<string> lsMeasurementDataHead = sMeasurementData.ToListHead();
                        string[] MeasurementDataStr = new string[lsMeasurementDataHead.Count];
                        p = 0;
                        foreach (string item in lsMeasurementDataHead)
                        {
                            MeasurementDataStr[p] = item;
                            p++;
                        }            

                        foreach (CMeasurement sME in lsME)
                        {
                            int RowSize = (2 + 1 + sME.lClassMeasurementData.Count() + 1 + 2 + sME.lEEPROMData.Count);
                            int columSize = lsMeasurementHead.Count;
                            letoltve = false;
                            if (j + RowSize > maxRow)
                            {
                                c1 = (Excel.Range)oSheet.Cells[row, 1];
                                c2 = (Excel.Range)oSheet.Cells[row + maxRow - 1, maxColum];
                                range = oSheet.get_Range(c1, c2);
                                range.Value = str;
                                str = new string[maxRow, maxColum];
                                row += j;
                                i = 0;
                                j = 0;
                            }
                            i = 0;
                            for (i = 0; i < MeasurementStr.Count(); i++)
                                str[j, i] = MeasurementStr[i];
                            i = 0;
                            j++;
                            foreach (string item in (sME.sM).ToList())
                            {
                                str[j, i] = item;
                                i++;
                            }
                            i = 0;
                            j++;
                            j++;
                            for (i = 0; i < MeasurementDataStr.Count(); i++)
                                str[j, i] = MeasurementDataStr[i];
                            j++;
                            foreach (List<string> sMD in sME.lClassMeasurementData)
                            {
                                i = 0;
                                foreach (string item in sMD)
                                {

                                    str[j, i] = item;
                                    i++;
                                }
                                j++;
                            }
                            i = 0;
                            j++;

                        }
                    if (j > 1)
                    {
                        c1 = (Excel.Range)oSheet.Cells[row, 1];
                        c2 = (Excel.Range)oSheet.Cells[row + j - 1, maxColum];
                        range = oSheet.get_Range(c1, c2);
                        range.Value = str;
                    }
                    log.Add(DateTime.Now.ToString("mm.ss.fff") + "  Finish");
                    log.Add((DateTime.Now - dt).ToString() + "  Finish");
                    oRng = oSheet.get_Range("A1", "D1");
                    oRng.EntireColumn.AutoFit(); //automatikus méretezés
                }
                public void WriteSeparateDay(List<CMeasurement> lsME)
                {
                    //   List<sMeasurement> lsm = ClassMeasurement.GetMeasurement(Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()));       
                    log.Clear();

                    log.Add(DateTime.Now.ToString("mm.ss.fff") + "  Start");
                    DateTime dt = DateTime.Now;

                    int maxRow = 300;
                    int maxColum = 10;
                    string[,] str = new string[maxRow, maxColum];
                    int i = 0, j = 0;
                    bool letoltve = false;
                    Excel.Range c1;
                    Excel.Range c2;
                    Excel.Range range;
                    List<string> lsMeasurementHead = sMeasurement.ToListHead();
                    string[] MeasurementStr = new string[lsMeasurementHead.Count];
                    int p = 0;
                    foreach (string item in lsMeasurementHead)
                    {
                        MeasurementStr[p] = item;
                        p++;
                    }
                    List<string> lsMeasurementDataHead = sMeasurementData.ToListHead();
                    string[] MeasurementDataStr = new string[lsMeasurementDataHead.Count];
                    p = 0;
                    foreach (string item in lsMeasurementDataHead)
                    {
                        MeasurementDataStr[p] = item;
                        p++;
                    }

                    string LastDT = "" ;
                    bool first = true;
                    int SheetIndex = 1;
                    foreach (CMeasurement sME in lsME)
                    {
                        if (first)
                        {
                            first = false;
                            LastDT = sME.sM.ModifyDate.ToString("yyyy.MM.dd");
                            Worksheets(SheetIndex, LastDT);
                        }
                        int RowSize = (2 + 1 + sME.lClassMeasurementData.Count() + 1 + 2 + sME.lEEPROMData.Count);
                        int columSize = lsMeasurementHead.Count;
                        letoltve = false;
                        if (!LastDT.Equals(sME.sM.ModifyDate.ToString("yyyy.MM.dd")))
                        {
                            LastDT = sME.sM.ModifyDate.ToString("yyyy.MM.dd");
                            c1 = (Excel.Range)oSheet.Cells[row, 1];
                            c2 = (Excel.Range)oSheet.Cells[row + maxRow - 1, maxColum];
                            range = oSheet.get_Range(c1, c2);
                            range.Value = str;
                            str = new string[maxRow, maxColum];
                            row =0;
                            i = 0;
                            j = 0;
                            Worksheets(SheetIndex, LastDT);
                                SheetIndex++;
                        }

                        if (j + RowSize > maxRow)
                        {
                            c1 = (Excel.Range)oSheet.Cells[row, 1];
                            c2 = (Excel.Range)oSheet.Cells[row + maxRow - 1, maxColum];
                            range = oSheet.get_Range(c1, c2);
                            range.Value = str;
                            str = new string[maxRow, maxColum];
                            row += j;
                            i = 0;
                            j = 0;
                        }
                        i = 0;
                        for (i = 0; i < MeasurementStr.Count(); i++)
                            str[j, i] = MeasurementStr[i];
                        i = 0;
                        j++;
                        foreach (string item in (sME.sM).ToList())
                        {
                            str[j, i] = item;
                            i++;
                        }
                        i = 0;
                        j++;
                        j++;
                        for (i = 0; i < MeasurementDataStr.Count(); i++)
                            str[j, i] = MeasurementDataStr[i];
                        j++;
                        foreach (List<string> sMD in sME.lClassMeasurementData)
                        {
                            i = 0;
                            foreach (string item in sMD)
                            {

                                str[j, i] = item;
                                i++;
                            }
                            j++;
                        }           
                    }
                    if (j > 1)
                    {
                        c1 = (Excel.Range)oSheet.Cells[row, 1];
                        c2 = (Excel.Range)oSheet.Cells[row + j - 1, maxColum];
                        range = oSheet.get_Range(c1, c2);
                        range.Value = str;
                    }
                    log.Add(DateTime.Now.ToString("mm.ss.fff") + "  Finish");
                    log.Add((DateTime.Now - dt).ToString() + "  Finish");
                    oRng = oSheet.get_Range("A1", "D1");
                    oRng.EntireColumn.AutoFit(); //automatikus méretezés
                }*/

        public void Write(List<string> ls)
        {
            column = 1;
            foreach (string item in ls)
            {
                WriteCell(row, column, item);
                column++;
            }
            column = 1;
            row++;
            oRng = oSheet.get_Range("A1", "D1");
            oRng.EntireColumn.AutoFit(); //automatikus méretezés
        }


        // public void Save(string name)
        public void Save(string path, string FileName)
        {
            try
            {
                string temp = @"" + path + "\\" + FileName.Replace(':', '_');
                oWB.SaveAs(temp);
                oWB.Close();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("A file mentése során probléma lépett fel!");
            }
        }
        public void Worksheets(int index, string name = "newsheet")
        {
            var xlSheets = oXL.Sheets as Excel.Sheets;
            oSheet = (Excel.Worksheet)xlSheets.Add(xlSheets[index], Type.Missing, Type.Missing, Type.Missing);
            oSheet.Name = name;
            row = 1;
            column = 1;
            //  oSheet = (Excel._Worksheet)oWB.Worksheets[index];
        }
        public bool WriteCell(int row, int column, object value, bool Head = false)
        {
            try
            {
                //    oSheet.Rows[1] = "kacsa";
                oSheet.Cells[row, column] = value;
                if (Head)
                    oSheet.Cells[row, column].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
