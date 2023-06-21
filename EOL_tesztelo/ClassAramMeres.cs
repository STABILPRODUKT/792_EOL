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
using System.Windows.Forms.DataVisualization.Charting;

namespace EOL_tesztelo
{
   public struct sAramMeres
    {
        public double AramValue;
        public double FeszValue;
        public DateTime Idopont;
        public TimeSpan normalizaltDateTime;
    }
    public class ClassAramMeres
    {
     
        public List<sAramMeres> lsAramMeres;
        public int MertMennyisegLimit = 300;
        public bool Full = true;
        public bool NewMeresFlag = false;
        Chart chart;
        Chart chartNagy;

        public ClassAramMeres(ref Chart chart, ref Chart chartNagy)
        {
            lsAramMeres = new List<sAramMeres>();
            Full = true;
            this.chart = chart;
            this.chartNagy = chartNagy;
        }
        sAramMeres MertElso;
        public void Add(sReadFogyasztas[] data2)
        {
            foreach (sReadFogyasztas Mert in data2)
            {
                sAramMeres tAramMeres = new sAramMeres();
                tAramMeres.AramValue = Mert.AramValue / 1000.0;
                tAramMeres.FeszValue = Mert.FeszValue / 1000.0;
                string strIdopont = Mert.YEAR + ". " + Mert.MONTH + ". " + Mert.DAY + ". " + Mert.HOUR + ":" + Mert.MINUTE + ":" + Mert.SECOND + "." + Mert.NANOSECOND;
                tAramMeres.Idopont = Convert.ToDateTime(strIdopont);
                if (NewMeresFlag)
                {
                    NewMeresFlag = false;
                    MertElso.Idopont = tAramMeres.Idopont;
                }
                tAramMeres.normalizaltDateTime = tAramMeres.Idopont - (MertElso.Idopont);
                lsAramMeres.Add(tAramMeres);
                if (lsAramMeres.Count >= MertMennyisegLimit)
                    Full = true;
            }
        }
        public void Clear()
        {
            lsAramMeres = new List<sAramMeres>();
            Full = false;
           
        }
        public void ChartClear()
        {
            chart.Invoke((MethodInvoker)delegate
            {
              // WriteChartDefault();
            });
         
        }
        public void WriteChart(ClassSensorProperty ActParamType)
        {
            for (int j = 0; j < 5; j++)
                chart.Series[j].Points.Clear();
            for (int j = 0; j < 6; j++)
                chartNagy.Series[j].Points.Clear();

            int i = 0;
            foreach (sAramMeres tAramMeres in lsAramMeres)
            {
                chart.Series[0].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), tAramMeres.AramValue);
                chartNagy.Series[0].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), tAramMeres.AramValue);
         
                //    chartNagy.Series[5].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), tAramMeres.FeszValue);
                i++;
                foreach (DataRow row in ActParamType.dt.Rows)
                {
                    switch (row[ClassSensorProperty.cD_id])
                    {
                        case Form1.cKivantAramFogyasztasMax:
                            chart.Series[1].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[1].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cKivantAramFogyasztasMin:
                            chart.Series[2].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[2].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cNyugalmiAramFogyasztasMax:

                            chart.Series[3].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[3].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cNyugalmiAramFogyasztasMin:
                            chart.Series[4].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[4].Points.AddXY(tAramMeres.normalizaltDateTime.ToString("ss\\.fff"), row[ClassSensorProperty.cDefaultValue]);

                            break;
                    }
                }
            }
        }
        public void WriteChartDefault(ClassSensorProperty ActParamType)
        {
            chart.Invoke((MethodInvoker)delegate
            {
                chartNagy.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chartNagy.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chartNagy.ChartAreas[0].AxisY.ScaleBreakStyle.Spacing = 10;
            for (int j = 0; j < 5; j++)
                chart.Series[j].Points.Clear();
            for (int j = 0; j < 6; j++)
                chartNagy.Series[j].Points.Clear();
            chart.Series[0].Color = Color.DarkBlue;
            chart.Series[0].IsVisibleInLegend = true;
            chartNagy.Series[0].Color = Color.DarkBlue;
            chartNagy.Series[0].IsVisibleInLegend = true;
            chart.Series[1].Color = Color.Red;
            chart.Series[1].IsVisibleInLegend = true;
            chart.Series[2].Color = Color.OrangeRed;
            chart.Series[2].IsVisibleInLegend = true;
            chart.Series[3].Color = Color.Red;
            chart.Series[3].IsVisibleInLegend = true;
            chart.Series[4].Color = Color.OrangeRed;
            chart.Series[4].IsVisibleInLegend = true;


            chartNagy.Series[1].Color = Color.Red;
            chartNagy.Series[1].IsVisibleInLegend = true;
            chartNagy.Series[2].Color = Color.OrangeRed;
            chartNagy.Series[2].IsVisibleInLegend = true;
            chartNagy.Series[3].Color = Color.Red;
            chartNagy.Series[3].IsVisibleInLegend = true;
            chartNagy.Series[4].Color = Color.OrangeRed;
            chartNagy.Series[4].IsVisibleInLegend = true;
            chartNagy.Series[5].Color = Color.DarkGray;
            chartNagy.Series[5].IsVisibleInLegend = true;
            for (int j = 0; j < 5; j++)
            {
                chart.Series[j].BorderWidth = 2;
            }

            for (int j = 0; j < 6; j++)
            {
                chartNagy.Series[j].BorderWidth = 2;
            }
            for (int i = 0; i <= Form1.technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes / 10; i++)
            {
                foreach (DataRow row in ActParamType.dt.Rows)
                {
                    switch (row[ClassSensorProperty.cD_id])
                    {
                        case Form1.cKivantAramFogyasztasMax:
                            chart.Series[1].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[1].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cKivantAramFogyasztasMin:
                            chart.Series[2].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[2].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cNyugalmiAramFogyasztasMax:

                            chart.Series[3].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[3].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);

                            break;
                        case Form1.cNyugalmiAramFogyasztasMin:
                            chart.Series[4].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);
                            chartNagy.Series[4].Points.AddXY(i / 10 + "." + i % 10, row[ClassSensorProperty.cDefaultValue]);

                            break;
                    }
                }
            }
            });
        }
        public double maxAram;
        public double maxFesz;
        public double minFesz;
        public double LastAram;
        public void AramErtekekEllenorzese()
        {
            maxAram = double.MinValue;
            maxFesz = double.MinValue;
            minFesz = double.MaxValue;
            foreach (sAramMeres tAramMeres in lsAramMeres)
            {
                if (tAramMeres.AramValue > maxAram)
                    maxAram = tAramMeres.AramValue;
                if (tAramMeres.FeszValue > maxFesz)
                    maxFesz = tAramMeres.FeszValue;
                if (tAramMeres.FeszValue < minFesz)
                    minFesz = tAramMeres.FeszValue;
                LastAram = tAramMeres.AramValue;
            }

        }
    }
}
