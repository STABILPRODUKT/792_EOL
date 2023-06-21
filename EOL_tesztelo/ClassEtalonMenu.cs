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
    public struct sTSI
    {
        public int index;
        public int S_id;
    }
    class ClassDiagnostic
    {
        public string TableName_Etalon = "EtalonMenu";
        public string TableName_DiagRows = "DiagRows";
        public ClassMenuDiagnostic CMD;
        public List<ClassRowsDiagnostic> lCRD;
        public List<sTSI> lTSI_S_ID = new List<sTSI>();

        public int ActS_id = -1;
        public string ActDescription = "";
        public ClassRowsDiagnostic CRDp;

        DataGridView DGV;
        public ClassDiagnostic(ref DataGridView DGV)
        {
            DataGridView asd = new DataGridView();
            CMD = new ClassMenuDiagnostic(TableName_Etalon, ref asd);
            lCRD = new List<ClassRowsDiagnostic>();
            ActS_id = -1;
            this.DGV = DGV;
        }
        public void menuRefresh()
        {
            CMD.Select();
            lCRD.Clear();
            foreach (DataRow row in CMD.dt.Rows)
            {
                ClassRowsDiagnostic CRD = new ClassRowsDiagnostic(TableName_DiagRows, ref DGV);
                CRD.Select((int)row[ClassRowsDiagnostic.cS_id]);
                lCRD.Add(CRD);
            }
        }
        public void NextActEtalon(int index)
        {
            int tempActS_id = ActS_id;
            ActDescription = "";
            /*  switch (index)
              {
                  case Form1.scSzenzorType_1:
                      foreach (DataRow row in CMD.dt.Rows)
                      {
                          if (ActS_id < (int)row[ClassMenuDiagnostic.cS_id] && (bool)row[ClassMenuDiagnostic.cType_1_Selected] == true && (int)row[ClassMenuDiagnostic.cUse] == FormEtalonProperty.scHasznal)
                          {
                              ActS_id = (int)row[ClassMenuDiagnostic.cS_id];
                              ActDescription = row[ClassMenuDiagnostic.cDescription].ToString();
                              break;
                          }
                      }

                      break;

                  case Form1.scSzenzorType_2:
                      foreach (DataRow row in CMD.dt.Rows)
                      {
                          if (ActS_id < (int)row[ClassMenuDiagnostic.cS_id] && (bool)row[ClassMenuDiagnostic.cType_2_Selected] == true && (int)row[ClassMenuDiagnostic.cUse] == FormEtalonProperty.scHasznal)
                          {
                              ActS_id = (int)row[ClassMenuDiagnostic.cS_id];
                              ActDescription = row[ClassMenuDiagnostic.cDescription].ToString();
                              break;
                          }
                      }
                      break;
                  case Form1.scSzenzorType_3:
                      foreach (DataRow row in CMD.dt.Rows)
                      {
                          if (ActS_id <(int)row[ClassMenuDiagnostic.cS_id] && (bool)row[ClassMenuDiagnostic.cType_3_Selected] == true&& (int)row[ClassMenuDiagnostic.cUse] == FormEtalonProperty.scHasznal)
                          {
                              ActS_id = (int)row[ClassMenuDiagnostic.cS_id];
                              ActDescription = row[ClassMenuDiagnostic.cDescription].ToString();
                              break;
                          }
                      }
                      break;

              }*/
            foreach (DataRow row in CMD.dt.Rows)
            {
                if (ActS_id < (int)row[ClassMenuDiagnostic.cS_id] && (int)row[ClassMenuDiagnostic.cUse] == FormEtalonProperty.scHasznal)
                {
                    ActS_id = (int)row[ClassMenuDiagnostic.cS_id];
                    ActDescription = row[ClassMenuDiagnostic.cDescription].ToString();
                    break;
                }
            }
            if (tempActS_id == ActS_id)  //ha megvolt az utolsó akkor -1 el térünk vissza 
            {
                ActS_id = -1;
            }


        }
        // public ClassRowsDiagnostic GetClassRowsDiagnostic(string text)
        public ClassRowsDiagnostic GetClassRowsDiagnostic()
        {
            /*   int S_id = 0;
               foreach (sDiagMenu sDM in CMD.lsDiagnosticsMenuActive)
               {
                   if (sDM.Description == text)
                   {
                       S_id = sDM.S_id;
                       break;
                   }
               }*/
            foreach (ClassRowsDiagnostic CRD in lCRD)
            {
                if (CRD.S_id == ActS_id)
                {
                    return CRD;
                }
            }
            return null;
        }

        public void GetClassRowsDiagnostic(int S_id)
        {
            this.CRDp = null;
            foreach (ClassRowsDiagnostic CRD in lCRD)
            {
                if (CRD.S_id == S_id)
                {
                    this.CRDp = CRD;
                    return;
                }
            }

        }

    }
}
