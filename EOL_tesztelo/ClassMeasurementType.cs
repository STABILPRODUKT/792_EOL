using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_tesztelo
{
    struct sMeasurementEtalonWarning
    {

        public int Hour;
        public int Min;
        public bool Active;

    }

    
       
    class ClassMeasurementType
    {
        public sMeasurementEtalonWarning sMEW_1;
        public sMeasurementEtalonWarning sMEW_2;
        public sMeasurementEtalonWarning sMEW_3;

        //Ha kell tesztelni akkor bebököm az EtalonTest-et
        private bool EtalonTest=true;
        public int EtaloTestStepCounter = 0;
        //Ha Megvolt a teszt bebököm a EtalonTestFinish
        public bool EtalonTestFinish=false;
        public ClassMeasurementType()
        {
                 sMEW_1 = new sMeasurementEtalonWarning();
          sMEW_2 = new sMeasurementEtalonWarning();
          sMEW_3 = new sMeasurementEtalonWarning();
        }
        public void SetClassMeasurementType( sMeasurementEtalonWarning sMEW , int Hour, int Min, bool Active)
        {
            sMEW.Hour = Hour;
            sMEW.Min = Min;
            sMEW.Active = Active;
        }
        public bool EtalonTestCheck()
        {
            int ActHour = Convert.ToInt32( DateTime.Now.ToString("HH"));
            int ActMin = Convert.ToInt32(DateTime.Now.ToString("mm"));
            if (!sMEW_1.Active&& !sMEW_1.Active&& !sMEW_1.Active)  //ha nem aktív akkor ki kell falseolni
            {
                EtalonTest = false;
                EtalonTestFinish = false;
            }
            bool EtalonTestTemp = EtalonTest;

            if (ActHour == sMEW_1.Hour && ActMin == sMEW_1.Min && sMEW_1.Active)
            {
                EtalonTest = true;
            }

            if (ActHour == sMEW_2.Hour && ActMin == sMEW_2.Min && sMEW_2.Active)
            {
                EtalonTest = true;
            }

            if (ActHour == sMEW_3.Hour && ActMin == sMEW_3.Min && sMEW_3.Active)
            {
                EtalonTest = true;
            }

            if (EtalonTestTemp == false && EtalonTest == true)
            {
                ResetEtaloTestStepCounter();
            }

            if (EtalonTest)
                return true;
            else
                return false;
        }

        private void ResetEtaloTestStepCounter()
        {
            
        }
    }
}
