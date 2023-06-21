using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_tesztelo
{
    class ClassTimer
    {
        int ActualTime;
        int FinishTime;
        bool value;

        public ClassTimer()
        {
            this.FinishTime = 0;
            this.ActualTime = 0;
        }
        public void Felhuz(int FinishTime)
        {
            this.FinishTime = FinishTime;
            this.ActualTime = 0;
        }
        public bool tik_tak()
        {

            if (ActualTime < FinishTime)
            {
                ActualTime++;
                return true;

            }
            else
            {
                return false;
            }
        }
    }
}
