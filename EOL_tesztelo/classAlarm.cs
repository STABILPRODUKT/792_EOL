using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EOL_tesztelo
{
    class classAlarm
    {
        public const int alarmArraySize = 100;
        sAlarm[] alarmArray=new sAlarm[alarmArraySize];
        public const int Error_PLC_connection = 0;
        public const int Error_Serial_connection = 1;
        public const int Error_Printer_connection = 2;
        public const int Error_SQL_connection = 3;
        public const int Error_Cooling_motor_protection = 10;
        public const int Error_Service_motor_protection = 11;
        public const int Error_Interface_system_motor_protection = 12;
        public const int Error_DigitalOutMotorProtection = 13;
        public const int Error_Safety_motor_protection = 14;
        public const int Error_E_stop = 15;
        public const int Error_Cabinet_temperature = 16;
        public const int Error_Air_pressure = 17;
        public const int TermekFeltoltes_failure = 18;
        public const int Error_Connection = 19;
        public const int Error_MeroTuskaVegalasNyit = 20;
        public const int Error_MeroTuskaVegalasZar = 21;
        public const int Error_ProbatestVegalasNyit = 22;
        public const int Error_ProbatestVegalasZar = 23;
        public const int GripperNyit_failure = 24;
        public const int GrippeZar_failure = 25;

        public const int Error_Camera_also_failure = 26;
        public const int TeszteloFeszultsegKismegszakito_failure = 27;
        public const int TeszteloFeszultsegElektromos_failure = 28;
        public const int TeszteloFeszultsegElektromosAramvedelem_failure = 29;

        public const int Error_SerialEOLTulindexeles = 30;
        public const int Error_Camera_felso_failure = 31;

        public bool plsAlarmReflesh = false;
        private DataBase db;
        String TableName = "AlarmLog";
        public List<sAlarm> lsAlarmLog = new List<sAlarm>();
        public classAlarm()
        {
            alarmArray[Error_PLC_connection].text = "PLC kapcsolat hiba";
            alarmArray[Error_Serial_connection].text = "EOL kapcsolat hiba";
            alarmArray[Error_Printer_connection].text = "Nyomtató kapcsolat hiba";
            alarmArray[Error_SQL_connection].text = "SQL kapcsolat hiba";
            alarmArray[Error_Cooling_motor_protection].text = "Hűtő ventillátor kismegszakító hiba";
            alarmArray[Error_Service_motor_protection].text = "Szervíz ajzat kismegszakító hiba";
            alarmArray[Error_Interface_system_motor_protection].text = "Interfész rendszer kismegszakító hiba";
            alarmArray[Error_DigitalOutMotorProtection].text = "Digitális kimenetek kismegszakító hiba";
            alarmArray[Error_Safety_motor_protection].text = "E-Stop kismegszakító hiba";
            alarmArray[Error_E_stop].text = "E-stop hiba";
            alarmArray[Error_Cabinet_temperature].text = "Szekrény hőmérséklet hiba";
            alarmArray[Error_Air_pressure].text = "Levegő nyomás hiba";
            alarmArray[TermekFeltoltes_failure].text = "Termék feltöltés során nem sikerült elérni a kivánt nyomást";

            alarmArray[Error_Connection].text = "PLC-PC kapcsolat hiba";
            alarmArray[Error_MeroTuskaVegalasNyit].text = "Mérőtüske kinyomás végállás hiba";
            alarmArray[Error_MeroTuskaVegalasZar].text = "Mérőtüske vissza végállás hiba";
            alarmArray[Error_ProbatestVegalasNyit].text = "Próbatest kinyomás végállás hiba";
            alarmArray[Error_ProbatestVegalasZar].text = "Próbatest vissza végállás hiba";

            alarmArray[Error_Camera_also_failure].text = "Alsó kamera hiba";
            alarmArray[Error_Camera_felso_failure].text = "Felső kamera hiba";
            alarmArray[GripperNyit_failure].text = "Megfogó nyit végállás hiba";
            alarmArray[GrippeZar_failure].text = "Megfogó zár végállás hiba";
            alarmArray[TeszteloFeszultsegKismegszakito_failure].text = "Tesztelő feszültség kismegszakitó hiba";
            alarmArray[TeszteloFeszultsegElektromos_failure].text = "Tesztelő feszültség hiba";
            alarmArray[TeszteloFeszultsegElektromosAramvedelem_failure].text = "Tesztelő feszültség áramvédelem hiba";
            alarmArray[Error_SerialEOLTulindexeles].text = "Soros port túlindexelés"; 
            
            for (int i=0;i< alarmArraySize;i++)
                alarmArray[i].index = i;

        }
        public List<sAlarm> getActivAlarm()
        {
            List<sAlarm> retAlarm = new List<sAlarm>();
            foreach (sAlarm alarm in alarmArray)
            {
                if (alarm.status == 1)
                {
                    retAlarm.Add(alarm);
                }
            }
            retAlarm = retAlarm.OrderByDescending(x => x.failureTime).ToList();
            return retAlarm;
        }
        public void setActiveAlarm(int index)
        {
            try
            {
                if (alarmArray[index].status == 0)
                {
                    alarmArray[index].index = index;
                    alarmArray[index].status = 1;
                    alarmArray[index].failureTime = DateTime.Now;
                    plsAlarmReflesh = true;
                }
          //      plsAlarmReflesh = true;
            }
            catch
            {
                
            }
        }
        public void AckActiveAlarm(int index)
        {
            try
            {
                if (alarmArray[index].status == 1)
                {
                    db = new DataBase();
                    string text = @"INSERT INTO [dbo].[" + TableName + "] VALUES ('" + alarmArray[index].failureTime.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',0, '" + alarmArray[index].text+ "'," +alarmArray[index].index+")";
                    db.getData(text);
                    alarmArray[index].status=0;
                    plsAlarmReflesh = true;
                }
            }
            catch
            {
            }
        }
        public void RefreshAlarmLog()
        {
            lsAlarmLog.Clear();
            db = new DataBase();
            DataTable dt = db.getData(@"SELECT TOP (100) * from " + TableName + " order by [FailureTime] desc");
            foreach (DataRow row in dt.Rows)
            {
                sAlarm sA = new sAlarm();
                sA.ID = Convert.ToInt32(row[0]);
                sA.failureTime = (DateTime)row[1];
                if (row[2] != null)
                    sA.ackTime = (DateTime)(row[2]);
                if (row[3] != null)
                    sA.status = Convert.ToInt32(row[3]);
                if (row[4] != null)
                    sA.text = row[4].ToString();
                if (row[5] != null)
                    sA.index = Convert.ToInt32(row[5]);
                lsAlarmLog.Add(sA);
            }

        }

    }

    public struct sAlarm
    {
        public int index;
        public int ID;
        public DateTime failureTime;
        public DateTime ackTime;
        public int status;
        public string text;
    }
}
