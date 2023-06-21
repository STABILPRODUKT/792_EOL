using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace EOL_tesztelo
{
    public class ClassButton
    {
        public bool Enabled =false;
        public bool SetPls = false;
        public bool ResetPls = false;
        public bool value = false;
        public int Index = 0;
        public object obj;
        public Color onColor;
        public Color offColor;
        public ClassButton(object obj, Color onColor, Color offColor, bool Enabled = false,bool value = false, int Index=0)
        {
            this.obj = obj;
            this.onColor = onColor;
            this.offColor = offColor;
            this.Enabled = Enabled;
            this.value = value;
            this.Index = Index;
            Printbtn();
        }
        public void Printbtn()
        {

            if (SetPls)
            {
                this.value = true;
                this.SetPls = false;
            }
            if (ResetPls)
            {
                this.value = false;
                this.ResetPls = false;
            }

            if (!Enabled)
            {
                this.value = false;
                ((Button)obj).FlatStyle = FlatStyle.Standard;
             //   ((Button)obj).FlatAppearance.BorderColor = Color.Black;
              //  ((Button)obj).FlatAppearance.BorderSize = 1;
            }
            if (Enabled)
            {
                ((Button)obj).FlatStyle = FlatStyle.Flat;
                ((Button)obj).FlatAppearance.BorderColor = Color.Green;
                ((Button)obj).FlatAppearance.BorderSize = 2;
            }
            if (this.value)
                ((Button)obj).BackColor = onColor;
            else
                ((Button)obj).BackColor = offColor;

        }
     /*   public void Enabledbtn(bool Enabled)
        {
            if (!Enabled)
                this.value = false;
            if (this.Enabled != Enabled)
            {
               
                    if (this.value)
                        ((Button)obj).BackColor = onColor;
                    else
                        ((Button)obj).BackColor = offColor;
              
                this.Enabled = Enabled;
            }
            ((Button)obj).Enabled = this.Enabled;
        }*/
    }
}
