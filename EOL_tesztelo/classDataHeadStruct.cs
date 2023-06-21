using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_tesztelo
{
    public class classDataHeadStruct
    {
        public static int sNotVisible = 0;
        public static int sVisible = 1;
        public static int sVisiblePrint = 2;

        public string HeadText;
        public Type ValueType;
        public int Visible;
        public int ColumnType;
        public bool ReadOnly;
        public int Width;
        public List<sComboBoxColumnItem> lsComboBoxColumnItem;
        public classDataHeadStruct(string HeadText, Type ValueType, int Visible, int ColumnType, bool ReadOnly = false, int Width = 200, List<sComboBoxColumnItem> lsComboBoxColumnItem = null)
        {
            this.HeadText = HeadText;
            this.ValueType = ValueType;
            this.Visible = Visible;
            this.ColumnType = ColumnType;
            this.ReadOnly = ReadOnly;
            this.Width = Width;
            if (lsComboBoxColumnItem == null)
                this.lsComboBoxColumnItem = new List<sComboBoxColumnItem>();
            else
                this.lsComboBoxColumnItem = lsComboBoxColumnItem;
        }
    }
    public struct sComboBoxColumnItem
    {
        public int Id_Data;
        public string text;
        public sComboBoxColumnItem(int Id_Data, string text)
        {
            this.Id_Data = Id_Data;
            this.text = text;
        }
    }
}
