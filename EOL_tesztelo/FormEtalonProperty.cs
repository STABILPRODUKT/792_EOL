using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_tesztelo
{
    public partial class FormEtalonProperty : Form
    {

        public const int scNemHasznal = 0;
        public const int scHasznal = 1;
        ClassMenuDiagnostic CMD;


        public FormEtalonProperty( )
        {
            InitializeComponent();
        }


        private void btnEtalonProperty_Adds_Click(object sender, EventArgs e)        // a menűbe új etalon hozzáadása
        {
            CMD.MenuDiagnosticsAdd();
        }

        private void FormEtalonProperty_Load(object sender, EventArgs e)
        {
            CMD = new ClassMenuDiagnostic("EtalonMenu" , ref dGVEtalonMenu);
            CMD.Select();
            CMD.WriteDataGridView();
            
        }


        private void btnEtalonProperty_Delete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell obj in dGVEtalonMenu.SelectedCells)
            {
                dGVEtalonMenu.Rows[obj.RowIndex].Cells[ClassMenuDiagnostic.cDelete].Value = true;
                dGVEtalonMenu.Rows[obj.RowIndex].Visible = false;
                CMD.CellEndEdit(obj.RowIndex);
             //   CMD.MenuDiagnosticsDelete(Convert.ToInt32(dGVEtalonMenu.Rows[obj.RowIndex].Cells[0].Value));
        }
         //   CMD.Select();
          //  CMD.WriteDataGridView();
          //  ClassMenuDiagnostic.bEtalonMenu_update = true;
        }



        private void btnEtalonProperty_Save_Click(object sender, EventArgs e)
        {
            bool good = true;
            for (int i = 0; i < dGVEtalonMenu.Rows.Count; i++)
            {
                if (Convert.ToString(dGVEtalonMenu.Rows[i].Cells[2].Value).Length == 0)
                    good = false;
            }
            if (good)
            {    
                CMD.SaveModify();
                CMD.Select();
                CMD.WriteDataGridView();
               ClassMenuDiagnostic.bEtalonMenu_update = true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Sikertelen mentés. Az egyik menű neve nem megfelelő!");
            }
        }
        private void btnEtalonProperty_Cancel_Click(object sender, EventArgs e)
        {
      
            CMD.Select();
            CMD.WriteDataGridView();
        }

        private string IntToStringVisibleProperty(int index)
        {
            string ret = "Használ";
            switch (index)
            {
                case scNemHasznal:
                    ret = "Nem használ";
                    break;
                case scHasznal:
                    ret = "Használ";
                    break;
            }
            return ret;
        }
        private int StringToIntVisibleProperty(string str)
        {
            int ret = 1;
            switch (str)
            {
                case "Nem használ":
                    ret = scNemHasznal;
                    break;
                case "Használ":
                    ret = scHasznal;
                    break;
            }
            return ret;
        }

        private void dGVEtalonMenu_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CMD.CellEndEdit( e.RowIndex);
        }






        // SQL defaultja alapján updateli a hiányzó hibákat vagy ha nincs benne akkor hozzáadja 
        /*  private void btnEtalonProperty_RowsDiagnosticUpdate_Click(object sender, EventArgs e)
          {
              CD.menuRefresh();
              ClassRowsDiagnostic CRDDef = new ClassRowsDiagnostic("DiagRowsDefault");
              CRDDef.Select(1, true);
              foreach (ClassRowsDiagnostic CRD in CD.lCRD)
              {
                  foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                  {
                      int S_id = CRD.S_id;
                      bool readyFlag = false;
                      foreach (ClassRowsDiagnostic.sDiagRow sDR in CRD.lsRowsDiagnosticActive)
                      {
                        //  S_id = sDR.S_id;
                          if (sDR.D_id == sDRDef.D_id)
                          {
                              readyFlag = true;    
                          }
                      }
                      if (readyFlag == false)
                          CRD.RowsDiagnosticsAdd(sDRDef, S_id);
                  }
              }
              CD.menuRefresh();
          }*/

    }
}
