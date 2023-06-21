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
using EOL_tesztelo.Properties;
using System.Reflection;

namespace EOL_tesztelo
{
     class ClassSearch : ClassSheet
    {
        DataGridView DGVdata;
        public bool Search = false;
        public ClassSearch(ref DataGridView DGVdata, ref DataGridView DGV)
        {
            this.DGVdata = DGVdata;
            this.DGV = DGV; 
        }
        
        public void btnSearchFiler_Click(object sender, EventArgs e, DateTime SaveStart, DateTime SaveFinish, string SearchSerialNumber)
        {
            if (Form1.technologia.cbtnSearchFiler.Enabled)
            {
                if (SaveStart > SaveFinish)
                {
                    System.Windows.Forms.MessageBox.Show("Dátum nem megfelelő!");
                }
                else
                {

                    Form1.technologia.cbtnSearchFiler.Enabled = false;
                    Form1.technologia.cbtnSearchExport.Enabled = false;
                    Form1.options.SaveStart = SaveStart;
                    Form1.options.SaveFinish = SaveFinish;
                    Form1.options.SearchSerialNumber = SearchSerialNumber;
                    DGVdata.Rows.Clear();
                    Form1.options.SaveOptions();
                    Search = true;
                }
            }
        }

        public static List<CMeasurement> lsME;

        public void SearchMeasurementData(int S_id)
        {
            DGVdata.Rows.Clear();
            if (lsME != null)
            {
                foreach (CMeasurement cME in lsME)
                {
                    if (Convert.ToInt32 (cME.dtModify.Rows[0][CMeasurement.cS_id]) == S_id)
                    {
                        cME.CMD.WriteDataGridView();                   
                        break;
                    }
                }
            }
        }

        public List<int> SelectedCikkszam()
        {
            List<int> lsRet = new List<int>();
            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                lsRet.Add(Convert.ToInt32(row.Cells[CMeasurement.cSerialNumber].Value));
            }
            return lsRet;
        }


        public override void Select()
        {
            lsME = ClassMeasurement.GetMeasurementExcell(ref DGV,ref DGVdata, Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()), Settings.Default["SearchSerialNumber"].ToString());

            int k;
            int i = 0;
            DGV.Rows.Clear();
            bool first = true;
       
            foreach (CMeasurement cME in lsME)
            {
                if (first)
                {
                    first = false;
                    cME.WriteDataGridView();
                    cME.CMD.WriteDataGridView();
                }
                k = DGV.Rows.Add();

                DGV.Rows[k].Cells[0].Value = cME.dtModify.Rows[0][CMeasurement.cS_id];
                DGV.Rows[k].Cells[1].Value = cME.dtModify.Rows[0][CMeasurement.cPc_id];
                DGV.Rows[k].Cells[2].Value = cME.dtModify.Rows[0][CMeasurement.cLoginName];
                DGV.Rows[k].Cells[3].Value = cME.dtModify.Rows[0][CMeasurement.cModifyDate]; ;
                DGV.Rows[k].Cells[4].Value = cME.dtModify.Rows[0][CMeasurement.cKIONPartNumber];
                DGV.Rows[k].Cells[5].Value = cME.dtModify.Rows[0][CMeasurement.cStabilPartNumber];
                DGV.Rows[k].Cells[6].Value = cME.dtModify.Rows[0][CMeasurement.cSerialNumber];
                DGV.Rows[k].Cells[7].Value = cME.dtModify.Rows[0][CMeasurement.cType];
                DGV.Rows[k].Cells[8].Value = cME.dtModify.Rows[0][CMeasurement.cMeasurementType];
                DGV.Rows[k].Cells[9].Value = cME.dtModify.Rows[0][CMeasurement.cMeasurementState];
            }
            Form1.technologia.cbtnSearchFiler.Enabled = true;
            Form1.technologia.cbtnSearchExport.Enabled = true;
            if (DGV.CurrentCell != null)
                if (DGV.CurrentCell.RowIndex >= 0)
                    SearchMeasurementData(Convert.ToInt32(DGV.Rows[DGV.CurrentCell.RowIndex].Cells[0].Value));

            Search = false;
        }

        public override void SaveModify()
        {
            throw new NotImplementedException();
        }

        protected override void ModifyInsert(DataRow row)
        {
            throw new NotImplementedException();
        }

        public override void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
