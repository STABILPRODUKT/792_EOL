using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace EOL_tesztelo
{
    class ClassCSV
    {
        System.IO.StreamWriter sw;
        Encoding enc = Encoding.UTF8;
        bool newFileFlag = false;
        int maxColum = 39;
        string[] dataArray;
        string[] HeadArray;
        public ClassCSV()
        {
            dataArray = new string[maxColum];
            HeadArray = new string[maxColum];
        }

        public void Open(string Path,string File)
        {
         
            bool exits = System.IO.Directory.Exists(Path);
            if (!exits)
                System.IO.Directory.CreateDirectory(Path);
            newFileFlag = NewFileCheck(Path + "\\" + File);
            sw = new StreamWriter(Path + "\\" + File, true, enc);

            //első paraméter: a kimenő fájl neve illetve ha nem az exe mellett van akkor elérési útvonala
            //második paraméter: append: ha igaz akkor hozzáfűzi ha hamis akkor felülírja
            //harmadik paraméter: a kimeneti fájl kódolása

        }
        private void WriteHead()
        {
            string row = "";
            for (int i = 1; i < maxColum; i++)
            {
                row += HeadArray[i] + ";";
            }
            sw.WriteLine(row);
        }
        private void WriteData()
        {
            string row = "";
            for (int i = 1; i < maxColum; i++)
            {
                row += dataArray[i] + ";";
            }
            sw.WriteLine(row);
        }
        public void Close()
        {
            sw.Dispose();
        }

        private bool NewFileCheck(string path)
        {

            int counter = 0;
            try
            {
                using (StreamReader sr = new StreamReader(path, enc))
                {
                    string row;
                    while ((row = sr.ReadLine()) != null)
                    {
                        counter++;
                    }
                }

                if (counter > 0)
                    return false;
            }
            catch
            {
            }
            return true;
        }
        public void Write(DataTable dtProperty, CMeasurement CM, ClassRowsDiagnostic CRDDef)
        {
            if (newFileFlag)
            {
                CreateHead( CRDDef);
                WriteHead();
                newFileFlag = false;

            }
            CreateData(dtProperty, CM);
            WriteData();
        }
        private void CreateData(DataTable dtProperty, CMeasurement CM)
        {
            DateTime dt = DateTime.Now;

            int row = 1;

            WriteCell(row, 1, CM.dtModify.Rows[0].ItemArray[CMeasurement.cSerialNumber]);
            WriteCell(row, 2, CM.dtModify.Rows[0].ItemArray[CMeasurement.cKIONPartNumber]);
            WriteCell(row, 3, CM.dtModify.Rows[0].ItemArray[CMeasurement.cStabilPartNumber]);
            WriteCell(row, 4, CM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementType]);
            WriteCell(row, 5, Form1.technologia.ActTypeText);
            WriteCell(row, 6, CM.dtModify.Rows[0].ItemArray[CMeasurement.cLoginName]);
            WriteCell(row, 7, CM.dtModify.Rows[0].ItemArray[CMeasurement.cModifyDate]);
            WriteCell(row, 8, CM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementState ]);


            foreach (DataRow dr in dtProperty.Rows)
            {
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 1) //Kívánt induló nyomás
                    WriteCell(row, 33, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 2) //Nyomásesés figyelés késleltetése
                    WriteCell(row, 34, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));
                if (Convert.ToInt32(dr[ClassSensorProperty.cD_id]) == 9) //Nyugalmi fogyasztás figyelés késleltetése
                    WriteCell(row, 35, Math.Round(Convert.ToDouble(dr[ClassSensorProperty.cDefaultValue]), 3));
            }


            foreach (DataRow drow in CM.CMD.dtModify.Rows)
            {
             
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 110) //Kívánt nyomás max
                    WriteCell(row, 21, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 110) //Kívánt nyomás min
                    WriteCell(row, 19, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_ToleranceNeg]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 112) // Kívánt feszültség max
                    WriteCell(row, 24, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 112) // Kívánt feszültség min
                    WriteCell(row, 22, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_ToleranceNeg]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 113) // Kívánt maximum áramfogyasztás max
                    WriteCell(row, 27, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 113) //Kívánt maximum áramfogyasztás min
                    WriteCell(row, 25, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_ToleranceNeg]), 3));

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 114) //Nyugalmi fogyasztás max
                    WriteCell(row, 30, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 114) //Nyugalmi fogyasztás min
                    WriteCell(row, 28, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_ToleranceNeg]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 115) //
                    WriteCell(row, 38, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 115) //
                    WriteCell(row, 36, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_ToleranceNeg]), 3));



                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 100)
                {
                    WriteCell(row, 31, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz])));//Zárókupa
                    WriteCell(row, 32, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue])));//Zárókupa
                }
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 101)
                {
                    WriteCell(row, 9, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz]))); //Menetrőgzítő 1
                    WriteCell(row, 10, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]))); //Menetrőgzítő 1
                }
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 102)
                {
                    WriteCell(row, 11, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz])));//Menetrőgzítő 2
                    WriteCell(row, 12, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]))); //Menetrőgzítő 2
                }

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 103)
                {
                    WriteCell(row, 13, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz])));//Menetrőgzítő 3
                    WriteCell(row, 14, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue])));//Menetrőgzítő 3
                }

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 104)
                {
                    WriteCell(row, 15, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz])));//Zsugorcső 1
                    WriteCell(row, 16, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue])));//Zsugorcső 1
                }

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 105)
                {
                    WriteCell(row, 17, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_TolerancePoz])));//Zsugorcső 2
                    WriteCell(row, 18, IntToString(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue])));//Zsugorcső 2
                }

                ///mértek 

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 110) //Kívánt nyomás
                    WriteCell(row, 20, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));

              /*  if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 111) //Kívánt feszültség min
                    WriteCell(row, 23, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));*/
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 112) //Kívánt feszültség max
                    WriteCell(row, 23, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));

                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 113) //Kívánt maximum áramfogyasztás
                    WriteCell(row, 26, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 114) //Nyugalmi fogyasztás 
                    WriteCell(row, 29, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));
                if (Convert.ToInt32(drow[CMeasurementData.cData_D_id]) == 115) //
                {
                   
                    WriteCell(row, 37, Math.Round(Convert.ToDouble(drow[CMeasurementData.cData_MeasurementValue]), 3));
                }


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
        private void CreateHead( ClassRowsDiagnostic CRDDef)
        {
            int row = 0;
            WriteCell(row, 1, "SerialNumber");
            WriteCell(row, 2, "XY PartNumber");
            WriteCell(row, 3, "StabilPartNumber");
            WriteCell(row, 4, "Measurement type");
            WriteCell(row, 5, "Product type");
            WriteCell(row, 6, "LoginName");
            WriteCell(row, 7, "ModifyDate");
            WriteCell(row, 8, "MeasurementState");
            WriteCell(row, 22, "Kívánt feszültség min (V)");
            WriteCell(row, 24, "Kívánt feszültség max (V)");

            foreach (DataRow dr in CRDDef.dt_DiagRowsDefault.Rows)
            {
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 1) //Kívánt induló nyomás
                    WriteCell(row, 33, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 2) //Nyomásesés figyelés késleltetése
                    WriteCell(row, 34, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 3) //Kívánt nyomás max
                    WriteCell(row, 21, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 4) //Kívánt nyomás min
                    WriteCell(row, 19, dr[ClassRowsDiagnostic.cDescription]);
            /*    if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 5) // Kívánt feszültség max
                    WriteCell(row, 24, dr[ClassRowsDiagnostic.cDescription]);*/
               /* if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 6) // Kívánt feszültség min
                    WriteCell(row, 22, dr[ClassRowsDiagnostic.cDescription]);*/
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 7) // Kívánt maximum áramfogyasztás max
                    WriteCell(row, 27, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 8) //Kívánt maximum áramfogyasztás min
                    WriteCell(row, 25, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 9) //Nyugalmi fogyasztás figyelés késleltetése
                    WriteCell(row, 35, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 10) //Nyugalmi fogyasztás max
                    WriteCell(row, 30, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 11) //Nyugalmi fogyasztás min
                    WriteCell(row, 28, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 12) //
                    WriteCell(row, 38, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 13) //
                    WriteCell(row, 36, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 100)
                {
                    WriteCell(row, 31, dr[ClassRowsDiagnostic.cDescription] + " kivánt");//Zárókupa
                    WriteCell(row, 32, dr[ClassRowsDiagnostic.cDescription]);//Zárókupa
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

                /*if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 111) //Kívánt feszültség min
                    WriteCell(row, 23, dr[ClassRowsDiagnostic.cDescription] + "imum");*/

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 112) //Kívánt feszültség max
                    WriteCell(row, 23, dr[ClassRowsDiagnostic.cDescription] );

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 113) //Kívánt maximum áramfogyasztás
                    WriteCell(row, 26, dr[ClassRowsDiagnostic.cDescription]);

                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 114) //Nyugalmi fogyasztás 
                    WriteCell(row, 29, dr[ClassRowsDiagnostic.cDescription]);
                if (Convert.ToInt32(dr[ClassRowsDiagnostic.cD_id]) == 115) //Nyugalmi fogyasztás 
                    WriteCell(row, 37, dr[ClassRowsDiagnostic.cDescription]);
            }
        }
        private void WriteCell(int row, int column, object value)
        {
            if (row == 0)
                HeadArray[column] = value.ToString();
            else
                dataArray[column] = value.ToString();
        }
    }
}
