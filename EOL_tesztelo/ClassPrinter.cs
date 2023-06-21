using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Globalization;

namespace EOL_tesztelo
{

    class ClassPrinter
    {
        List<string> PrinterFile;
        Encoding enc = Encoding.UTF8;

        public ClassPrinter()
        {
            PrinterFile = new List<string>();
        }
        //Beolvassa a ZebraDesigner átal kigenerált file-t
        public bool ReadPrinterFile(string path)
        {
            PrinterFile.Clear();
            try
            {
                using (StreamReader sr = new StreamReader(path, enc))
                {
                    string row;
                    while ((row = sr.ReadLine()) != null)
                    {
                        PrinterFile.Add(row);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        //a beolvasott fileba lecsélketjük az adott szövegeket az aktuális értékre és kinyomtatjuk a jó cimkét
        public void Print(string KION_Partumber, string STABIL_Partumber, string SerialNumber, DateTime date)
        {
            string PrintText = "";
            CultureInfo cul = CultureInfo.CurrentCulture;
            string Day = cul.Calendar.GetDayOfYear(date).ToString();
            string Year = Convert.ToString(cul.Calendar.GetYear(date) % 100);

            foreach (string str in PrinterFile)
            {
                string tempstr = str;
                tempstr = tempstr.Replace(Form1.options.CimkeYearDay, Day + Year);
                tempstr = tempstr.Replace(Form1.options.CimkeStabilPartNumber, STABIL_Partumber);
                tempstr = tempstr.Replace(Form1.options.CimkexyPartNumber, KION_Partumber);
                tempstr = tempstr.Replace(Form1.options.CimkeQR_xyPartNumber, KION_Partumber.Replace(".", string.Empty));
                tempstr = tempstr.Replace(Form1.options.CimkeQR_SerialNumber, SerialNumber);
                tempstr = tempstr.Replace(Form1.options.CimkeQR_YYYYMMDD, date.ToString("yyyyMMdd"));
                PrintText += tempstr;//+ '\n';
            }
            ClassRawPrinterHelper.SendStringToPrinter(Form1.options.OpPrinter, PrintText);
        }
        //a beolvasott fileba lecsélketjük az adott szövegeket az aktuális értékre és kinyomtatjuk a rossz cimkét
        public void Print(string SerialNumber, string hiba, DateTime date)
        {
            string PrintText = "";
            CultureInfo cul = CultureInfo.CurrentCulture;
            string Day = cul.Calendar.GetDayOfYear(date).ToString();
            string Year = Convert.ToString(cul.Calendar.GetYear(date) % 100);

            foreach (string str in PrinterFile)
            {
                string tempstr = str;
                tempstr = tempstr.Replace(Form1.options.CimkeHibasSerialNumber, SerialNumber);
                tempstr = tempstr.Replace(Form1.options.CimkeHibasDatum, date.ToString("yyyy.MM.dd HH:MM:ss"));
                tempstr = tempstr.Replace(Form1.options.CimkeHibasHiba, EkezetTelanites(hiba + " hiba"));
                PrintText += tempstr;
            }
            ClassRawPrinterHelper.SendStringToPrinter(Form1.options.OpPrinter, PrintText);
        }
        //Ékezetes karakterek lecserélése
        private string EkezetTelanites(string str)
        {
            str = str.Replace('Á', 'A');
            str = str.Replace('á', 'a');
            str = str.Replace('É', 'E');
            str = str.Replace('é', 'e');
            str = str.Replace('Í', 'I');
            str = str.Replace('í', 'i');
            str = str.Replace('Ó', 'o');
            str = str.Replace('ó', 'o');
            str = str.Replace('Ö', 'o');
            str = str.Replace('ö', 'o');
            str = str.Replace('Ő', 'o');
            str = str.Replace('ő', 'o');
            str = str.Replace('Ú', 'u');
            str = str.Replace('ú', 'u');
            str = str.Replace('Ü', 'u');
            str = str.Replace('ü', 'u');
            str = str.Replace('Ű', 'u');
            str = str.Replace('ű', 'u');
            return str;
        }
    }
}
