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
    public abstract class ClassSheet
    {
        public const int TextBoxColumn = 1;
        public const int CheckBoxColumn = 2;
        public const int ComboBoxColumn = 3;
        public const int ButtonColumn = 4;

        protected int AddRowIndex;
        protected const int DefaultAddRowIndex = 2147460000;// int max értéke: 2147483647

        public DataGridView DGV;
        public static DataGridView DGV_Print;
        public DataTable dtDelete;
        public DataTable dtModify;
        public DataTable dt;
        protected DataTable dtPrint;
        public DataTable dtExcell;
        public bool WriteDataGridViewRunFlag = true;
        public List<classDataHeadStruct> lsDataHeadStruct;



        //annak az olszlopnak az indexe ahol az id található
        public static int cCellEndEditColumn;

        public abstract void Select();
        public abstract void SaveModify();

     //   protected abstract void ModifyUpdate(DataRow row);
        protected abstract void ModifyInsert(DataRow row);
     //   protected abstract void ModifyDelete(DataRow row);

        public virtual void WriteDataGridView(bool first = true)
        {
            WriteDataGridViewRunFlag = true;

            DGV.Rows.Clear();
          //  DGV.Visible = false;
            if (first)
            {
                DGV.Columns.Clear();
                //automatikus méretezés és a több sorok engedélyezése
                DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                DGV.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                foreach (classDataHeadStruct stval in lsDataHeadStruct)
                {
                    DGVAddColumns(stval);
                }
                first = false;
            }
            foreach (DataRow stval in dtModify.Rows)
            {
                DGV.Rows.Add(stval.ItemArray);
            }
          //  DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //   DGV.Visible = true;

            WriteDataGridViewRunFlag = false;
        }

        public void DoubleBuffered( bool setting)
        {
            Type dgvType = DGV.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(DGV, setting, null);
        }

        //DGV_Print fejlécét tölti ki
      /*  public virtual void WriteDataGridViewHeadPrint()
        {
            DGV_Print.Columns.Clear();
            //automatikus méretezés és a több sorok engedélyezése
            DGV_Print.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            DGV_Print.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            foreach (classDataHeadStruct stval in lsDataHeadStruct)
                DGVAddColumns(stval, true);
        }*/
      //  public abstract void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e);
        public abstract void CellValueChanged(object sender, DataGridViewCellEventArgs e);
      //  public abstract void RowsAdded(object sender, DataGridViewRowEventArgs e);

        //classDataHeadStruct alapján eldönti hogy milyen típusu és beállítású oszlopot kell beszurnia
        protected void DGVAddColumns(classDataHeadStruct stval, bool PrintGrid = false)
        {

            if (stval.ColumnType == CheckBoxColumn)
            {
                DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
                dgvCmb.ValueType = stval.ValueType;
                dgvCmb.Name = stval.HeadText;
                dgvCmb.HeaderText = stval.HeadText;
                if ((stval.Visible != classDataHeadStruct.sNotVisible && !PrintGrid) || (PrintGrid && stval.Visible == classDataHeadStruct.sVisiblePrint))
                    dgvCmb.Visible = true;
                else
                    dgvCmb.Visible = false;
                dgvCmb.ReadOnly = stval.ReadOnly;
                dgvCmb.Width = stval.Width;

                dgvCmb.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (PrintGrid == false)
                    DGV.Columns.Add(dgvCmb);
                else
                    DGV_Print.Columns.Add(dgvCmb);
            }
            else if (stval.ColumnType == TextBoxColumn)
            {
                DataGridViewTextBoxColumn dgvCmb = new DataGridViewTextBoxColumn();
                dgvCmb.ValueType = stval.ValueType;
                dgvCmb.Name = stval.HeadText;
                dgvCmb.HeaderText = stval.HeadText;
                if ((stval.Visible != classDataHeadStruct.sNotVisible && !PrintGrid) || (PrintGrid && stval.Visible == classDataHeadStruct.sVisiblePrint))
                    dgvCmb.Visible = true;
                else
                    dgvCmb.Visible = false;
                dgvCmb.ReadOnly = stval.ReadOnly;

                dgvCmb.HeaderCell.Style.Font = Form1.fontHead;
                dgvCmb.DefaultCellStyle.Font = Form1.fontText;
                dgvCmb.Width = stval.Width;
                dgvCmb.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (PrintGrid == false)
                    DGV.Columns.Add(dgvCmb);
                else
                    DGV_Print.Columns.Add(dgvCmb);
            }
            else if (stval.ColumnType == ComboBoxColumn)
            {
                DataGridViewComboBoxColumn dgvCmb = new DataGridViewComboBoxColumn();
                dgvCmb.ValueType = stval.ValueType;
                dgvCmb.Name = stval.HeadText;
                dgvCmb.HeaderText = stval.HeadText;
                dgvCmb.HeaderCell.Style.Font = Form1.fontHead;
                dgvCmb.DefaultCellStyle.Font = Form1.fontText;
                if ((stval.Visible != classDataHeadStruct.sNotVisible && !PrintGrid) || (PrintGrid && stval.Visible == classDataHeadStruct.sVisiblePrint))
                    dgvCmb.Visible = true;
                else
                    dgvCmb.Visible = false;
                dgvCmb.ReadOnly = stval.ReadOnly;
                dgvCmb.Width = stval.Width;
                foreach (sComboBoxColumnItem item in stval.lsComboBoxColumnItem)
                    dgvCmb.Items.Add(item.text);

                dgvCmb.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (PrintGrid == false)
                    DGV.Columns.Add(dgvCmb);
                else
                    DGV_Print.Columns.Add(dgvCmb);
            }
            if (stval.ColumnType == ButtonColumn)
            {
                DataGridViewButtonColumn dgvCmb = new DataGridViewButtonColumn();
                dgvCmb.ValueType = stval.ValueType;
                dgvCmb.Text = "+";
                dgvCmb.UseColumnTextForButtonValue = true;
                dgvCmb.Name = stval.HeadText;
                dgvCmb.HeaderText = stval.HeadText;
                dgvCmb.HeaderCell.Style.Font = Form1.fontHead;
                dgvCmb.DefaultCellStyle.Font = Form1.fontText;
                if ((stval.Visible != classDataHeadStruct.sNotVisible && !PrintGrid) || (PrintGrid && stval.Visible == classDataHeadStruct.sVisiblePrint))
                    dgvCmb.Visible = true;
                else
                    dgvCmb.Visible = false;
                dgvCmb.ReadOnly = stval.ReadOnly;
                //   dgvCmb.MinimumWidth = stval.MinimumWidth;
                dgvCmb.Width = stval.Width;
                dgvCmb.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                if (PrintGrid == false)
                    DGV.Columns.Add(dgvCmb);
                else
                    DGV_Print.Columns.Add(dgvCmb);
            }
            DGV.Columns[DGV.Columns.Count - 1].SortMode = DataGridViewColumnSortMode.NotSortable;

        }
        public void GetModifyList()
        {
            SaveModify();
            Select();

            DGV.Invoke((MethodInvoker)delegate
            {
                WriteDataGridView();
            });
        }


        //ha befejeztük a cella modosítását akkor beletesszük a modosítás datatabléba
        public virtual void CellEndEdit(int RowIndex)
        {
            if (DGV.Rows[RowIndex].Cells[cCellEndEditColumn].Value != null)
            {
                if (dtModify.Columns.Count < 2)
                {
                    for (int i = 0; i < DGV.Columns.Count; i++)
                        dtModify.Columns.Add(DGV.Columns[i].DataPropertyName);
                }
                int k = int.MaxValue;

                for (int i = 0; i < dtModify.Rows.Count; i++)
                {
                    if (dtModify.Rows[i][cCellEndEditColumn].ToString() == DGV.Rows[RowIndex].Cells[cCellEndEditColumn].Value.ToString())
                    {
                        k = i;
                        break;
                    }
                }
                if (k == int.MaxValue)
                {
                    dtModify.Rows.Add();
                    k = dtModify.Rows.Count - 1;
                }
                for (int j = 0; j < DGV.Columns.Count; j++)
                {
                    if (DGV.Rows[RowIndex].Cells[j].Value != null && DGV.Rows[RowIndex].Cells[j].Value.ToString() != "")
                    {
                        try
                        {
                            dtModify.Rows[k][j] = DGV.Rows[RowIndex].Cells[j].Value;
                        }
                        catch
                        {
                            DGV.Rows[RowIndex].Cells[j].Value = dtModify.Rows[k][j];
                        }
                        }
                    else if (DGV.Rows[RowIndex].Cells[j].ValueType == typeof(DateTime))
                        dtModify.Rows[k][j] = new DateTime();
                    else if (DGV.Rows[RowIndex].Cells[j].ValueType == typeof(int))
                        j = j; //nem csinálunk semmit
                    else if (DGV.Rows[RowIndex].Cells[j].Value == null)
                        dtModify.Rows[k][j] = DBNull.Value;
                    else
                        dtModify.Rows[k][j] = DGV.Rows[RowIndex].Cells[j].Value;
                }
            }
        }


        //boolból intet készít
        protected int BoolToInt(object b)
        {
            if (b == DBNull.Value)
            {
                return 0;
            }
            if (Convert.ToBoolean(b) == false)
                return 0;
            else
                return 1;
        }

        public static void DGVSetData(DataGridViewCell cell, string value)
        {
            if (cell.ValueType == typeof(bool))
            {
                cell.Value = Convert.ToBoolean(value);
            }
            else if (cell.ValueType == typeof(int))
            {
                cell.Value = Convert.ToInt32(value);
            }
            else if (cell.ValueType == typeof(string))
            {
                cell.Value = Convert.ToString(value);
            }
            else if (cell.ValueType == typeof(double))
            {
                cell.Value = Convert.ToDouble(value);
            }
            else if (cell.ValueType == typeof(DateTime))
            {
                cell.Value = Convert.ToDateTime(value);
            }
        }
    }
}
