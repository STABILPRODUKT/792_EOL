using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using EOL_tesztelo.Properties;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;

namespace EOL_tesztelo
{
    public partial class Form1 : Form
    {
        //Főképernyő konstans menűválasztó
        const int spMeresiAdatok = 1;
        const int spFutesGorbe = 2;
        const int spFeltoltes = 3;


        // főképrnyő konstans indexei
        public const int scLogin = 1;
        public const int scMainOverview = 2;
        public const int scOverview = 3;
        public const int scOpMeasurementParam = 4;
        //  public const int scMeasurementData = 5;
        public const int scOpConnectionsParam = 6;
        public const int scAlarmLog = 7;
        public const int scAlarmActive = 8;
        public const int scOpEtalonCheckAlarmParam = 9;
        public const int scUsManagment = 10;
        public const int scOpInterfaceParam = 11;
        public const int scOpProductTypeParam = 12;
        public const int scOpEtalonParam = 13;
        public const int scOpMachinParam = 14;
        public const int scSearch = 15;

        //Mérés típus konstansok
        public const int scMeasurementNormal = 1;
        public const int scMeasurementEtalon = 2;
        public const string scMeasurementNormal_text = "Normál";
        public const string scMeasurementEtalon_text = "Etalon";

        //Szenzor tipus (főképernyő és a beállítások is ezt használja)
        public const int scSzenzorType_1 = 1;
        public const int scSzenzorType_2 = 2;
        public const int scSzenzorType_3 = 3;
        private int ActscSzenzorType;
        public const int scSzenzorGlobal = 4;

        //Felhasználói képernyők konstans indexei
        public const int scUserMy = 1;
        public const int scUserNew = 2;
        public const int scUsers = 3;

        //Mérés képernyők konstans indexei
        public const int scOpMeasurement_NormalType_1 = 1;
        public const int scOpMeasurement_EtalonType_1 = 4;



        //beállítás képernyők konstans indexei
        public const int scOPSQL = 1;
        public const int scOPPrinter = 2;
        public const int scOPPLC = 3;
        public const int scOPEOL = 4;

        public const int scElrejt = 0;
        public const int scHiba = 1;

        public static Font fontText = new Font("Arial", 12F, GraphicsUnit.Pixel);
        public static Font fontHead = new Font("Arial", 14F, GraphicsUnit.Pixel);
        //háttérszálak
        private Thread TScreenUpdate;
        private Thread TWriteExcell;
        private Thread TAutoWriteExcell;
        private Thread TSearch;
        private Thread TTechnologia;
        private Thread TPLC;

        bool WriteDataGridViewFlag = false;
        int ScreenVisibleIndex = 0;

        public static int ActualSzenzorTypeSelect=1;
        private bool ButtonEnableUpdateFlag = false;

        int ActSzenzorType = 0;

        private classAlarm cAlarm;
        public static ClassTech technologia = new ClassTech();
        public static sOptions options = new sOptions();
        private ClassSensorProperty OPParamSelect;  //OP ba használjuk
        ClassSensorProperty ParamNormalType_1;
        ClassSensorProperty ParamEtalonType_1;
        ClassSensorProperty ProductType_Global;
       public static ClassSensorProperty ActParamType;
      
        ClassRowsDiagnostic CRDDef;
        ClassSearch cSearch;


        ClassDiagnostic CD;
        ClassUser user = new ClassUser();
        bool Flag_btnLogin = false;
        bool StatusFlag = false;
        int StatusDelay = 0;
        string StatusText = "";

        public Form1()
        {
            InitializeComponent();
            Settings.Default["PC_id"] = 2;
            options.ReadOptions();

        }
        void FullScreen(bool fullScreen)
        {
            if (true)
            {
                if (fullScreen)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    this.Bounds = Screen.PrimaryScreen.Bounds;
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
          /*     if (Environment.MachineName == "DESKTOP-KISS-T-")
            {
                tbUserName.Text = "tibi";
                tbUserPassword.Text = "kacsa";
            }*/

            ScreenVisible(scLogin);
            TScreenUpdate = new Thread(ThreadScreenUpdate);
            TScreenUpdate.Start();
            TScreenUpdate.IsBackground = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Kijelentkezett: " + ClassUser.LoginUser.LoginName);
            TScreenUpdate.Abort();
            if (TPLC != null)
                TPLC.Abort();
            if (TTechnologia != null)
                TTechnologia.Abort();
            if (TWriteExcell != null)
                TWriteExcell.Abort();
            if (TAutoWriteExcell != null)
                TAutoWriteExcell.Abort();

            if (TSearch != null)
                TSearch.Abort();
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            Flag_btnLogin = true;
            btnLogin.Enabled = false;
            tbUserPassword.Enabled = false;
            tbUserName.Enabled = false;
        }
        private void StartLoad()
        {
            ParamNormalType_1 = new ClassSensorProperty("ParamNormalType", ref dGVOpMeasurementParam, "Normál");
            ParamEtalonType_1 = new ClassSensorProperty("ParamEtalonType", ref dGVOpMeasurementParam, "Etalon");
            CD = new ClassDiagnostic(ref dGVOpEtalonParam);
            OPParamSelect = null;
            ParamNormalType_1.Select();
            ParamEtalonType_1.Select();
            technologia.cAramMeres = new ClassAramMeres(ref chartAramMeres, ref chartAramMeresNagy);


            ProductType_Global = new ClassSensorProperty("ParamType_Global", ref dGVOpMachineParam, "", true);
            ProductType_Global.Select();

            DataGridView dgv = new DataGridView();
            CRDDef = new ClassRowsDiagnostic("DiagRowsDefault",ref dgv);
            CRDDef.SelectDefault();
            cSearch = new ClassSearch(ref dgvSearchMeasurementData, ref dgvSearchMeasurement);

            technologia.cbtnOVStart = new ClassButton(btnOVStart, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVStop = new ClassButton(btnOVStop, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVStepMode = new ClassButton(btnOVStepMode, Color.LightGreen, Color.LightGray, true, false);
            technologia.cbtnOVAllTheWay = new ClassButton(btnOVAllTheWay, Color.LightGreen, Color.LightGray, true, false);
            technologia.cbtnOVNext = new ClassButton(btnOVNext, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVMachinOn = new ClassButton(btnOVMachinOn, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVMachinOff = new ClassButton(btnOVMachinOff, Color.LightGreen, Color.LightGray, true, true);

            technologia.cbtnOVNormal = new ClassButton(btnOVNormal, Color.LightGreen, Color.LightGray, ClassUser.LoginUser.Right >= ClassUser.lsUserManager, false, scMeasurementNormal);
            technologia.cbtnOVEtalon = new ClassButton(btnOVEtalon, Color.LightGreen, Color.LightGray, true, true, scMeasurementEtalon);

            technologia.cbtnOVSzType_1 = new ClassButton(btnOVSzType_1, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_1, scSzenzorType_1);
            technologia.cbtnOVSzType_2 = new ClassButton(btnOVSzType_2, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_2, scSzenzorType_2);
            technologia.cbtnOVSzType_3 = new ClassButton(btnOVSzType_3, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_3, scSzenzorType_3);

            technologia.cbtnInterfaceService = new ClassButton(btnInterfaceService, Color.LightGreen, Color.LightGray, false, false);

            technologia.cbtnSearchFiler = new ClassButton(btnSearchFiler, Color.LightGreen, Color.LightGray, true, false);
            technologia.cbtnSearchExport = new ClassButton(btnSearchExport, Color.LightGreen, Color.LightGray, true, false);


            technologia.cbtn_Out_Interface_1 = new ClassButton(btn_Out_Interface_1, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_2 = new ClassButton(btn_Out_Interface_2, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_3 = new ClassButton(btn_Out_Interface_3, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_4 = new ClassButton(btn_Out_Interface_4, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_5 = new ClassButton(btn_Out_Interface_5, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_6 = new ClassButton(btn_Out_Interface_6, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_7 = new ClassButton(btn_Out_Interface_7, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_8 = new ClassButton(btn_Out_Interface_8, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_9 = new ClassButton(btn_Out_Interface_9, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_10 = new ClassButton(btn_Out_Interface_10, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_11 = new ClassButton(btn_Out_Interface_11, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_12 = new ClassButton(btn_Out_Interface_12, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_13 = new ClassButton(btn_Out_Interface_13, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtn_Out_Interface_14 = new ClassButton(btn_Out_Interface_14, Color.LightGreen, Color.LightGray, false, false);
    


            OVSzenzorTypeSelect(options.TypeIndex);
            CD.menuRefresh();

            OVBtnMeasurementSelect(technologia.cbtnOVEtalon.Index);
            ScreenVisible(scOverview);
            cAlarm = new classAlarm();
            technologia.MachinOn = false;

            if (TWriteExcell == null)
            {
                TWriteExcell = new Thread(ThreadWriteExcell);
                TWriteExcell.Start();
                TWriteExcell.IsBackground = true;
            }
            if (TAutoWriteExcell == null)
            {
                TAutoWriteExcell = new Thread(ThreadAutoWriteExcell);
                TAutoWriteExcell.Start();
                TAutoWriteExcell.IsBackground = true;
            }

            if (TSearch == null)
            {
                TSearch = new Thread(ThreadSearch);
                TSearch.Start();
                TSearch.IsBackground = true;
            }

            if (TTechnologia == null)
            {
                TTechnologia = new Thread(ThreadTechnologia);
                TTechnologia.Start();
                TTechnologia.IsBackground = true;
            }
            if (TPLC == null)
            {
                TPLC = new Thread(ThreadPLC);
                TPLC.Start();
                TPLC.IsBackground = true;
            }

 
            TypTextLoad();
            technologia.cAramMeres.WriteChartDefault(ActParamType);
            technologia.cSerialNumber = new ClassSerialNumber();
        }


        private void etsmUsLogOff_Click(object sender, EventArgs e)
        {
            ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Kijelentkezett: " + ClassUser.LoginUser.LoginName);
            ClassUser.LoginUser = new sUser();
            tbUserPassword.Text = "";
            ScreenVisible(scLogin);
            user.LoginSuccessfull = false;
            technologia.MachinOn = false;
        }

        private void tsmOverview_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOverview);
        }

        bool UserRightCheck(int scrrenIndex, int right)
        {
            bool rightCheck = true;
            switch (scrrenIndex)
            {
                case scOpMeasurementParam:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;
                case scOpConnectionsParam:
                    if (right < ClassUser.lsUserAdmin)
                        rightCheck = false;
                    break;
                case scAlarmLog:
                    if (right < ClassUser.lsUserOperator)
                        rightCheck = false;
                    break;
                case scAlarmActive:
                    if (right < ClassUser.lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpEtalonCheckAlarmParam:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;
                case scUsManagment:
                    if (right < ClassUser.lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpInterfaceParam:
                    if (right == ClassUser.lsUserManager || right == ClassUser.lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpProductTypeParam:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;
                case scOpEtalonParam:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;
                case scOpMachinParam:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;

                case scSearch:
                    if (right < ClassUser.lsUserManager)
                        rightCheck = false;
                    break;
            }
            return rightCheck;
        }

        //az adott indexű képernyő megjelenítése és menű színezése
        void ScreenVisible(int ScreenVisibleIndex)
        {
            this.ScreenVisibleIndex = ScreenVisibleIndex;
            if (UserRightCheck(ScreenVisibleIndex, ClassUser.LoginUser.Right)) // Fellhasználó jogosultság ellenőrzése
            {
                plLogin.Visible = false;
                plOverview.Visible = false;
                plOpMeasurementParam.Visible = false;
                plOpConnectionsParam.Visible = false;
                plAlarmActive.Visible = false;
                plAlarmLog.Visible = false;
                plOpEtalonCheckAlarmParam.Visible = false;
                plUsManagment.Visible = false;
                plOpInterfaceParam.Visible = false;
                plOpProductTypeParam.Visible = false;
                plOpEtalonParam.Visible = false;
                plOpMachinParam.Visible = false;
                plSearch.Visible = false;


                tsmColor(tsmOverview);
                tsmColor(etsmOpConnectionsParam);
                tsmColor(tsmOptions);
                tsmColor(etsmOpMeasurementParam);
                tsmColor(tsmAlarmActive);
                tsmColor(tsmAlarmLog);
                tsmColor(etsmOpEtalonCheckAlarmParam);
                tsmColor(etsmUsOptions);
                tsmColor(etsmOpInterfaceParam);
                tsmColor(etsmOpProductTypeParam);
                tsmColor(etsmOpEtalonParam);
                tsmColor(etsmOpMachinParam);
                tsmColor(tsmSearch);
                tsmColor(tsmUser);

                switch (ScreenVisibleIndex)
                {
                    case scLogin:
                        plLogin.Visible = true;
                        plMainOverview.Visible = false;
                        technologia.MachinOn = false;
                        this.AcceptButton = btnLogin;
                        FullScreen(false);
                        break;
                    case scMainOverview:
                        plOverview.Visible = true;
                        plMainOverview.Visible = true;
                        tsmColor(tsmOverview, true);
                        break;
                    case scOverview:
                        plOverview.Visible = true;
                        OVDataPanelVisible(spMeresiAdatok);
                        tsmColor(tsmOverview, true);
                        break;
                    case scOpMeasurementParam:
                        plOpMeasurementParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpMeasurementParam, true);
                        break;
                    case scOpConnectionsParam:
                        plOpConnectionsParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpConnectionsParam, true);
                        break;
                    case scAlarmLog:
                        plAlarmLog.Visible = true;
                        tsmColor(tsmAlarmLog, true);
                        break;
                    case scAlarmActive:
                        plAlarmActive.Visible = true;
                        tsmColor(tsmAlarmActive, true);
                        break;
                    case scOpEtalonCheckAlarmParam:
                        plOpEtalonCheckAlarmParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpEtalonCheckAlarmParam, true);
                        break;
                    case scUsManagment:
                        plUsManagment.Visible = true;
                        tsmUser.BackColor = Color.Gray;
                        tsmColor(etsmUsOptions, true);
                        break;
                    case scOpInterfaceParam:
                        plOpInterfaceParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpInterfaceParam, true);
                        break;
                    case scOpProductTypeParam:
                        plOpProductTypeParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpProductTypeParam, true);
                        break;
                    case scOpEtalonParam:
                        plOpEtalonParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpEtalonParam, true);
                        break;

                    case scOpMachinParam:
                        plOpMachinParam.Visible = true;
                        tsmOptions.BackColor = Color.Gray;
                        tsmColor(etsmOpMachinParam, true);
                        break;
                    case scSearch:
                        plSearch.Visible = true;
                        tsmSearch.BackColor = Color.Gray;
                        tsmColor(tsmSearch, true);
                        dTPSearchStartDate.Value = options.SaveStart;
                        dTPSearchFinishDate.Value = options.SaveFinish;
                        tbSearchSerialNumber.Text = options.SearchSerialNumber;
                        break;

                }
            }
            else
            {
                StatusFlag = true;
                StatusDelay = 10;
                StatusText = "Az Ön felhasználói jogával ez a menü nem nyitható meg!";
            }
        }
        public void tsmColor(object obj, bool set = false)
        {
            if (set)
            {
                ((ToolStripItem)obj).BackColor = Color.Gray;
                ((ToolStripItem)obj).ForeColor = Color.White;
            }
            else
            {
                ((ToolStripItem)obj).BackColor = Color.LightGray;
                ((ToolStripItem)obj).ForeColor = Color.Black;
            }
        }

        private void btnOVNormal_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVNormal.SetPls = true;
        }
        private void btnOVEtalon_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVEtalon.SetPls = true;
        }
        void OVBtnMeasurementSelect(int index)
        {
            CD.ActS_id = -1;
            CD.ActDescription = "";
            switch (index)
            {
                case scMeasurementNormal:
                    technologia.cbtnOVEtalon.ResetPls = true;
                    ActParamType = ParamNormalType_1;
                    technologia.ActMeasurementType = "Normal";
                    break;
                case scMeasurementEtalon:
                    technologia.cbtnOVNormal.ResetPls = true;
                    CD.NextActEtalon(ActualSzenzorTypeSelect);
                    ActParamType = ParamEtalonType_1;
                    technologia.ActMeasurementType = "Etalon";
                    break;
            }
            ActParamType.Select();
            PLCPropertyDataMove();
        }

        void OVSzenzorTypeSelect(int index)
        {
            switch (index)
            {
                case scSzenzorType_1:
                    technologia.cbtnOVSzType_1.Enabled = true;
                    technologia.cbtnOVSzType_2.Enabled = false;
                    technologia.cbtnOVSzType_3.Enabled = false;
                    technologia.cbtnOVSzType_1.SetPls = true;
                    technologia.ActKIONPartNumber = options.KIONPartNumber_1;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_1;
                    technologia.ActTypeText = options.Type1Text;
                    break;
                case scSzenzorType_2:
                    technologia.cbtnOVSzType_1.Enabled = false;
                    technologia.cbtnOVSzType_2.Enabled = true;
                    technologia.cbtnOVSzType_3.Enabled = false;
                    technologia.cbtnOVSzType_2.SetPls = true;
                    technologia.ActKIONPartNumber = options.KIONPartNumber_2;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_2;
                    technologia.ActTypeText = options.Type2Text;
                    break;
                case scSzenzorType_3:
                    technologia.cbtnOVSzType_1.Enabled = false;
                    technologia.cbtnOVSzType_2.Enabled = false;
                    technologia.cbtnOVSzType_3.Enabled = true;
                    technologia.cbtnOVSzType_3.SetPls = true;
                    technologia.ActKIONPartNumber = options.KIONPartNumber_3;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_3;
                    technologia.ActTypeText = options.Type3Text;
                    break;
            }
            ActualSzenzorTypeSelect = index;
            CD.ActS_id = -1;
            CD.NextActEtalon(ActualSzenzorTypeSelect);
        }

        ClassTimer AckTimer = new ClassTimer();
        bool AramMeresFullFlag = true;
        public void ThreadPLC()
        {
            technologia.plc.IP = (Settings.Default["PLC_IP_1"].ToString() + "." + Settings.Default["PLC_IP_2"].ToString() + "." + Settings.Default["PLC_IP_3"].ToString() + "." + Settings.Default["PLC_IP_4"].ToString());
            while (true)
            {
                if (ClassPLC.ConnectionLife == false)
                    technologia.plc.Con();
                technologia.plc.write.Live = !technologia.plc.write.Live;
                technologia.plc.Read();
                Thread.Sleep(50);
                technologia.plc.Write();

                if (technologia.AramMeresEredmenyLekerdezesFlag == true)
                {
                    technologia.cAramMeres.Clear();
                    technologia.cAramMeres.NewMeresFlag = true;
                    technologia.cAramMeres.Add(technologia.plc.ReadAramMeres(AramMeresFullFlag));
                    this.Invoke((MethodInvoker)delegate
                    {
                        technologia.cAramMeres.WriteChart(ActParamType);
                    });
                    technologia.cAramMeres.AramErtekekEllenorzese();
                    technologia.AramMeresEredmenyLekerdezesFlag = false;
                }
            }
        }

        public void ThreadTechnologia()
        {
            while (true)
            {
                Thread.Sleep(5);
                technologia.Tech();
                if (technologia.plc.ACK)
                {
                    AckTimer.Felhuz(50);
                    technologia.plc.ACK = false;
                }
                technologia.plc.write.ACK = AckTimer.tik_tak();
            }
        }
        public void ThreadSearch()
        {
            do
            {
                if (cSearch.Search)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        cSearch.Select();
                    });
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            while (true);
        }
        public void ThreadWriteExcell()
        {
            do
            {
                //        try
                {
                    if (SaveForm.Save)
                    {

                       /*       ClassExcell ce = new ClassExcell();
                               ce.Open();
                               //    ce.Worksheets(1, "sheet1");
                               //     ce.WriteSeparateDay(lsME);
                               //    ce.Save(Settings.Default["SaveFolderBrowser"].ToString(), Settings.Default["SaveName"].ToString());
                               SaveForm.Save = false;
                               this.Invoke((MethodInvoker)delegate
                               {
                                   technologia.cbtnSearchFiler.Enabled = false;
                                   SaveF.Save_Finish();
                               });

                       */

                        List<int> lsSelectedCikkszam = cSearch.SelectedCikkszam();
                       
                       if (lsSelectedCikkszam.Count > 0)
                        {
                            classCSV = new ClassCSV();
                            string path = Settings.Default["SaveFolderBrowser"].ToString();
                            string fileName = Settings.Default["SaveName"].ToString()+ ".csv";

                            classCSV.Open(path, fileName);
                            foreach (int cikk in lsSelectedCikkszam)
                            {

                                foreach (CMeasurement CM in ClassSearch.lsME)
                                {
                                    if (Convert.ToInt32(CM.dtModify.Rows[0].ItemArray[CMeasurement.cSerialNumber]) == cikk)
                                    {

                                        ActParamType.Select(CM.dtModify.Rows[0].ItemArray[CMeasurement.cMeasurementType].ToString(), CM.dtModify.Rows[0].ItemArray[CMeasurement.cType_S_id]);
                                        classCSV.Write(ActParamType.SearchDt, CM, CRDDef);
                                    }
                                }
                                
                               
                            }
                       
                            classCSV.Close();
                        }
                        technologia.AutoSaveFlag = false;

                       
                        technologia.cbtnSearchFiler.Enabled = true;
                        SaveForm.Save = false;
                        technologia.cbtnSearchFiler.Enabled = true;
                        technologia.cbtnSearchExport.Enabled = true;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                /* catch
                 {
                     if (SaveForm.Save)
                     {
                         SaveForm.Save = false;
                         this.Invoke((MethodInvoker)delegate
                         {
                             SaveF.Save_Finish();
                         });
                         System.Windows.Forms.MessageBox.Show("Sikertelen mentés!");
                     }

                 }*/

            }
            while (true);
        }
        public static List<CMeasurement> lsAutoME;

        ClassCSV classCSV;

        public void ThreadAutoWriteExcell()
        {
            do
            {
                //        try
                {
                    if (technologia.AutoSaveFlag)
                    {
                        lsAutoME = ClassMeasurement.GetMeasurementExcell(ref dgvSearchMeasurement,ref dgvSearchMeasurementData, Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()), Settings.Default["SearchSerialNumber"].ToString(), technologia.AutoSaveS_id);
                        if (lsAutoME.Count > 0)
                        {        
                            classCSV = new ClassCSV();
                            string path = @"C:\Log files\" + DateTime.Now.ToString("yyyy");
                            string fileName = "729905_CW" + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) + ".csv";

                            classCSV.Open(path , fileName);
                            classCSV.Write(ActParamType.dt, technologia.cmEol.sM, CRDDef);
                            classCSV.Close();
                       }
                        technologia.AutoSaveFlag = false;
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                /* catch
                 {
                     if (SaveForm.Save)
                     {
                         SaveForm.Save = false;
                         this.Invoke((MethodInvoker)delegate
                         {
                             SaveF.Save_Finish();
                         });
                         System.Windows.Forms.MessageBox.Show("Sikertelen mentés!");
                     }

                 }*/

            }
            while (true);
        }

        public void ThreadScreenUpdate()
        {
            string SelectedPrinter = "";
            bool printerConnectionOk = false;
            ClassTimer StatusTimer = new ClassTimer();
            do
            {
                if (Flag_btnLogin)
                {

                    if ((tbUserName.Text == "3i" && tbUserPassword.Text == "19510605"))
                    {
                        ClassUser.LoginUser.Right = ClassUser.lsUserAdmin;
                        ClassUser.LoginUser.LoginName = "3i";
                        this.Invoke((MethodInvoker)delegate
                        {
                            ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Bejelentkezett: " + ClassUser.LoginUser.LoginName);
                            ScreenVisible(scMainOverview);
                        });
                    }
                    else
                    {
                        if (user.Login(tbUserName.Text, tbUserPassword.Text))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Bejelentkezett: " + ClassUser.LoginUser.LoginName);
                                FullScreen(true);
                                ScreenVisible(scMainOverview);
                                StartLoad();
                                if (tbUserName.Text=="tibi")
                                    tsmFeltoltes.Visible = true;
                            else
                                    tsmFeltoltes.Visible = false;
                            });
                        }

                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                if (DataBase.ConnectionLife)
                                    System.Windows.Forms.MessageBox.Show("Nem megfelelő felhasználónév vagy jelszó!");
                                else
                                    System.Windows.Forms.MessageBox.Show("SQL kapcsolat hiba!");
                            });
                        }
                    }
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnLogin.Enabled = true;
                        tbUserPassword.Enabled = true;
                        tbUserName.Enabled = true;
                    });
                    ClassMenuDiagnostic.bEtalonMenu_update = true;
                    Flag_btnLogin = false;
                }

                if (user.LoginSuccessfull)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        toolStripStatusLabel1.Text = "Felhasználó: " + ClassUser.LoginUser.LoginName;
                        toolStripStatusLabel2.Text = "                  ";
                        toolStripStatusLabel4.Text = "                  ";
                        toolStripStatusLabel3.Text = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                        toolStripProgressBar1.Visible = false;

                        if (SaveForm.visible == false)
                        {
                            this.Enabled = true;
                            technologia.cbtnSearchFiler.Enabled = true;
                            technologia.cbtnSearchExport.Enabled = true;
                        }

                        if (technologia.updateStatus)
                        {
                            lbOVMachineStatus.Text = technologia.machineStatus;
                            lbOVUserStaus.Text = technologia.userStaus;
                        }

                        technologia.cbtnOVStart.Printbtn();
                        if (technologia.cbtnOVStart.value)
                            technologia.cbtnOVStop.value = false;

                        technologia.cbtnOVStop.Printbtn();
                        if (technologia.cbtnOVStop.value)
                            technologia.cbtnOVStart.value = false;

                        if (technologia.cbtnOVEtalon.value)
                        {
                            technologia.cbtnOVAllTheWay.value = true;
                            technologia.PrintEnabled = false;
                        }
                        else
                            technologia.PrintEnabled = true;
                        technologia.cbtnOVAllTheWay.Printbtn();
                        technologia.cbtnOVNext.Printbtn();
                        technologia.cbtnOVMachinOn.Printbtn();

                        if (technologia.cbtnOVMachinOn.value)
                            technologia.cbtnOVMachinOff.value = false;

                        technologia.cbtnOVMachinOff.Printbtn();
                        if (technologia.cbtnOVMachinOff.value)
                            technologia.cbtnOVMachinOn.value = false;

                        technologia.cbtnSearchFiler.Printbtn();
                        technologia.cbtnSearchExport.Printbtn();


                        if (technologia.cbtnOVNormal.SetPls && technologia.cbtnOVNormal.Enabled && !technologia.cbtnOVNormal.value)
                        {
                            technologia.cbtnOVAllTheWay.ResetPls = true;
                            OVBtnMeasurementSelect(technologia.cbtnOVNormal.Index);
                            plOvDataGridView.Visible = true;
                            technologia.stepCounterReset = true;
                            options.OpMeasOpt_M_Last = DateTime.Now;
                        }

                        if (technologia.cbtnOVEtalon.SetPls && technologia.cbtnOVEtalon.Enabled && !technologia.cbtnOVEtalon.value)
                        {
                            OVBtnMeasurementSelect(technologia.cbtnOVEtalon.Index);
                            plOvDataGridView.Visible = true;
                            technologia.stepCounterReset = true;
                        }


                        if (technologia.Meres) //ha meres van akkor ne lehsessen kattintani a méréstipust
                        {
                            if (!technologia.cbtnOVNormal.value)
                                technologia.cbtnOVNormal.Enabled = false;
                            if (!technologia.cbtnOVEtalon.value)
                                technologia.cbtnOVEtalon.Enabled = false;
                        }
                        if (technologia.MeresFinishFlag || ButtonEnableUpdateFlag == true)
                        {
                            ButtonEnableUpdateFlag = false;
                            technologia.MeresFinishFlag = false;              
                            if (!technologia.cbtnOVNormal.value)
                                technologia.cbtnOVNormal.Enabled = ClassUser.LoginUser.Right >= ClassUser.lsUserManager;
                            if (!technologia.cbtnOVEtalon.value)
                                technologia.cbtnOVEtalon.Enabled = ClassUser.LoginUser.Right >= ClassUser.lsUserManager;
                        }


                        if (!technologia.cbtnOVSzType_1.value && technologia.plc.read.KabelkorbacsHosszusagErzekelo_1)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_1.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_1.Index;
                            options.SaveOptions();
                        }
                        if (!technologia.cbtnOVSzType_2.value && technologia.plc.read.KabelkorbacsHosszusagErzekelo_2)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_2.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_2.Index;
                            options.SaveOptions();
                        }
                        if (!technologia.cbtnOVSzType_3.value && technologia.plc.read.KabelkorbacsHosszusagErzekelo_3)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_3.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_3.Index;
                            options.SaveOptions();
                        }
                        if (!technologia.plc.read.KabelkorbacsHosszusagErzekelo_1 &&
                        !technologia.plc.read.KabelkorbacsHosszusagErzekelo_2 &&
                        !technologia.plc.read.KabelkorbacsHosszusagErzekelo_1)
                        {
                            technologia.cbtnOVStop.SetPls = true;
                            technologia.cbtnOVSzType_1.Enabled = false;
                            technologia.cbtnOVSzType_2.Enabled = false;
                            technologia.cbtnOVSzType_3.Enabled = false;
                        }
                        technologia.cbtnOVNormal.Printbtn();
                        technologia.cbtnOVEtalon.Printbtn();
                        technologia.cbtnOVStepMode.Printbtn();
                        technologia.cbtnOVSzType_1.Printbtn();
                        technologia.cbtnOVSzType_2.Printbtn();
                        technologia.cbtnOVSzType_3.Printbtn();

                        technologia.cbtnInterfaceService.Printbtn();
                        technologia.plc.write.Service = technologia.cbtnInterfaceService.value;

                        technologia.cbtn_Out_Interface_1.Printbtn();
                        technologia.cbtn_Out_Interface_2.Printbtn();
                        technologia.cbtn_Out_Interface_3.Printbtn();
                        technologia.cbtn_Out_Interface_4.Printbtn();
                        technologia.cbtn_Out_Interface_5.Printbtn();
                        technologia.cbtn_Out_Interface_6.Printbtn();
                        technologia.cbtn_Out_Interface_7.Printbtn();
                        technologia.cbtn_Out_Interface_8.Printbtn();
                        technologia.cbtn_Out_Interface_9.Printbtn();
                        technologia.cbtn_Out_Interface_10.Printbtn();
                        technologia.cbtn_Out_Interface_11.Printbtn();
                        technologia.cbtn_Out_Interface_12.Printbtn();
                        technologia.cbtn_Out_Interface_13.Printbtn();
                        technologia.cbtn_Out_Interface_14.Printbtn();

                        if (technologia.FeltoltesMeresFlag)
                        {
                            AddFeltoltesChart(Convert.ToDouble(technologia.plc.read.ProbatestNyomasTavado) / 1000.0);
                        }


                        if (technologia.btnJoTermek)
                            btnOVMegfelelo.BackColor = Color.Green;
                        else
                            btnOVMegfelelo.BackColor = Color.Transparent;

                        if (technologia.btnRosszTermek)
                            btnOVRossz.BackColor = Color.Red;
                        else
                            btnOVRossz.BackColor = Color.Transparent;

                        if (WriteMeasurementDataGridViewFull == true)
                        {
                            dgvOVMeasurementData.Rows.Clear();
                            WriteMeasurementDataGridViewFull = false;
                        }

                        if (technologia.cbtnOVEtalon.value)
                        {
                            lbOVActEtalon.Text = CD.ActDescription;
                            lbOVActEtalonText.Visible = true;
                        }
                        else
                        {
                            lbOVActEtalon.Text = "";
                            lbOVActEtalonText.Visible = false;
                        }

                        if (technologia.MegfeleloMeresVegeFlag == true)         // ha végeztünk az aktuális etalon méréssel
                        {
                            technologia.MegfeleloMeresVegeFlag = false;
                            if (technologia.cbtnOVEtalon.value)                 // még etalon mérésben vagyunk
                            {
                                CD.NextActEtalon(ActualSzenzorTypeSelect);      //lekérjük a kövi etalont
                            }
                        }


                        if (technologia.Hiba_NincsTermek)
                        {
                            technologia.Hiba_NincsTermek = false;
                            System.Windows.Forms.MessageBox.Show("Sikertelen mérés: Helyezzen terméket a berendezésbe!");
                        }
                        if (technologia.Hiba_NincsAHelyenACsatlakozo)
                        {
                            technologia.Hiba_NincsAHelyenACsatlakozo = false;
                            System.Windows.Forms.MessageBox.Show("Sikertelen mérés: Helyezzen terméket a berendezésbe!");
                        }
                        
                         if (technologia.TermekFeltoltesHiba)
                        {
                            technologia.TermekFeltoltesHiba = false;
                            System.Windows.Forms.MessageBox.Show("Sikertelen mérés: A termékfeltöltésnél nem értük el a kivánt nyomást!");
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            lbOvSerialNumber.Text = ClassSerialNumber.SerialNumber.ToString();
                        });


                        if ((DateTime.Now.TimeOfDay > options.OpMeasOpt_M1.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M1.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres) ||
                            (DateTime.Now.TimeOfDay > options.OpMeasOpt_M2.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M2.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres) ||
                            (DateTime.Now.TimeOfDay > options.OpMeasOpt_M3.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M3.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres))
                        {
                            if (!technologia.cbtnOVEtalon.value)
                            {
                                technologia.cbtnOVEtalon.Enabled = true;
                                technologia.cbtnOVEtalon.SetPls = true;
                                technologia.cbtnOVNormal.Enabled = ClassUser.LoginUser.Right >= ClassUser.lsUserManager;
                            }
                        }

                        if (cAlarm.plsAlarmReflesh)
                        {
                            WriteAlarmActiveGridView(dgvAlarmActive);
                            cAlarm.plsAlarmReflesh = false;
                        }

                        printbtnStatus(btnOSQLConnection, DataBase.ConnectionLife, Color.Green, Color.Red);


                        if (StatusFlag)
                        {
                            StatusFlag = false;
                            StatusTimer.Felhuz(StatusDelay);
                            tsslStatus.Text = "\t\t\t" + StatusText;
                        }
                        if (!StatusTimer.tik_tak())
                        {
                            tsslStatus.Text = "";
                        }
                        if (ClassMenuDiagnostic.bEtalonMenu_update)
                        {
                            EtalonMenu_update();
                            ClassMenuDiagnostic.bEtalonMenu_update = false;



                            int EtalonSelectName = -1;
                            foreach (ToolStripItem tsi in tsmEtalon.Items)
                            {
                                EtalonSelectName = tsmEtalon.Items.IndexOf(tsi);
                                break;
                            }

                            tsmEtalonSelect(EtalonSelectName);

                        }

                        if (CD.ActS_id == -1 && technologia.cbtnOVEtalon.value)
                        {
                            technologia.cbtnOVNormal.Enabled = true;
                            technologia.cbtnOVNormal.SetPls = true;

                            technologia.cbtnOVEtalon.Enabled = ClassUser.LoginUser.Right >= ClassUser.lsUserManager;
                            technologia.cbtnOVEtalon.value = false;
                            options.OpMeasOpt_M_Last = DateTime.Now;
                            options.SaveOptions();
                        }

                        tbOVCounterBad.Text = ClassTech.Selejt.ToString();
                        tbOVCounterGood.Text = ClassTech.Megfelelo.ToString();

                        SelectedPrinter = cbOpPrinterPrinters.Text;
                        printbtnStatus(btnOPLCConnection, ClassPLC.ConnectionLife, Color.Green, Color.Red);
                        printbtnStatus(btnOpConPrinterConnection, printerConnectionOk, Color.Green, Color.Red);

                        PLCInterfaceScreenUpdate();
                    });

                    foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    {
                        if (printer == SelectedPrinter)
                            printerConnectionOk = true;
                    }
                    alarmSet(!DataBase.ConnectionLife, classAlarm.Error_SQL_connection);

                    alarmSet(!ClassPLC.ConnectionLife, classAlarm.Error_PLC_connection);

                    alarmSet(technologia.plc.read.Cooling_motor_protection_failure, classAlarm.Error_Cooling_motor_protection);
                    alarmSet(technologia.plc.read.Service_motor_protection_failure, classAlarm.Error_Service_motor_protection);
                    alarmSet(technologia.plc.read.Interface_system_motor_protection_failure, classAlarm.Error_Interface_system_motor_protection);
                    alarmSet(technologia.plc.read.DigitalOutMotorProtection_failure, classAlarm.Error_DigitalOutMotorProtection);
                    alarmSet(technologia.plc.read.Safety_motor_protection_failure, classAlarm.Error_Safety_motor_protection);
                    alarmSet(technologia.plc.read.E_stop_failure, classAlarm.Error_E_stop);
                    alarmSet(technologia.plc.read.Cabinet_temperature_failure, classAlarm.Error_Cabinet_temperature);
                    alarmSet(technologia.plc.read.Air_pressure_failure, classAlarm.Error_Air_pressure);
                 //   alarmSet(technologia.plc.read.TermekFeltoltes_failure, classAlarm.TermekFeltoltes_failure);

                    alarmSet(technologia.plc.read.Connection_failure, classAlarm.Error_Connection);
                    alarmSet(technologia.plc.read.MeroTuskaVegalasNyit_failure, classAlarm.Error_MeroTuskaVegalasNyit);
                    alarmSet(technologia.plc.read.MeroTuskaVegalasZar_failure, classAlarm.Error_MeroTuskaVegalasZar);
                    alarmSet(technologia.plc.read.ProbatestVegalasNyit_failure, classAlarm.Error_ProbatestVegalasNyit);
                    alarmSet(technologia.plc.read.ProbatestVegalasZar_failure, classAlarm.Error_ProbatestVegalasZar);


                    alarmSet(technologia.plc.read.GripperNyit_failure, classAlarm.GripperNyit_failure);
                    alarmSet(technologia.plc.read.GrippeZar_failure, classAlarm.GrippeZar_failure);

                    alarmSet(technologia.plc.read.Camera_also_failure, classAlarm.Error_Camera_also_failure);
                    alarmSet(technologia.plc.read.Camera_felso_failure, classAlarm.Error_Camera_felso_failure);

                    alarmSet(technologia.plc.read.TeszteloFeszultsegKismegszakito_failure, classAlarm.TeszteloFeszultsegKismegszakito_failure);
                    alarmSet(technologia.plc.read.TeszteloFeszultsegElektromos_failure, classAlarm.TeszteloFeszultsegElektromos_failure);
                    alarmSet(technologia.plc.read.TeszteloFeszultsegElektromosAramvedelem_failure, classAlarm.TeszteloFeszultsegElektromosAramvedelem_failure);

                    if (WriteDataGridViewFlag)
                    {
                        switch (ScreenVisibleIndex)
                        {
                            case scLogin:

                                break;
                            case scMainOverview:

                                break;
                            case scOverview:

                                break;
                            case scOpMeasurementParam:

                                break;
                            case scOpConnectionsParam:

                                break;
                            case scAlarmLog:

                                break;
                            case scAlarmActive:

                                break;
                            case scOpEtalonCheckAlarmParam:

                                break;
                            case scUsManagment:

                                break;
                            case scOpInterfaceParam:

                                break;
                            case scOpProductTypeParam:
                                plOpProductTypeParam.Invoke((MethodInvoker)delegate
                                {
                                    tsmOpProductType_1.BackColor = Color.LightGray;
                                    tsmOpProductType_1.ForeColor = Color.Black;
                                    tsmOpProductType_2.BackColor = Color.LightGray;
                                    tsmOpProductType_2.ForeColor = Color.Black;
                                    tsmOpProductType_3.BackColor = Color.LightGray;
                                    tsmOpProductType_3.ForeColor = Color.Black;
                                    switch (ActscSzenzorType)
                                    {
                                        case scSzenzorType_1:
                                            tsmOpProductType_1.BackColor = Color.Gray;
                                            tsmOpProductType_1.ForeColor = Color.White;
                                            break;
                                        case scSzenzorType_2:
                                            tsmOpProductType_2.BackColor = Color.Gray;
                                            tsmOpProductType_2.ForeColor = Color.White;
                                            break;
                                        case scSzenzorType_3:
                                            tsmOpProductType_3.BackColor = Color.Gray;
                                            tsmOpProductType_3.ForeColor = Color.White;

                                            break;
                                    }
                                    TypePartNumberrSelect(ActscSzenzorType);
                                });

                                break;
                            case scOpEtalonParam:

                                break;

                            case scOpMachinParam:

                                break;
                            case scSearch:

                                break;

                        }
                        WriteDataGridViewFlag = false;
                    }
                   

                    if (technologia.TermekLeurultNyomasErtekFlag)
                    {
                        double max = 0;
                        double min = 0;
                        double time = 0;
                        double data = Convert.ToDouble(technologia.plc.read.TermekFeltoltveNyomasErtek- technologia.plc.read.TermekLeurultNyomasErtek) / 1000.0;
                     
                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                        {
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 110)
                            {

                                foreach (DataRow sp in ActParamType.dt.Rows)
                                {
                                    switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                    {
                                        case 2:
                                            time = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 3:
                                            max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 4:
                                            min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                            break;
                                    }
                                }
                                dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                {
                                    data = (data * 12.0) / time * 60.0;
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), min, max);
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, min, max);
                                });
                                break;
                            }
                            
                        }
                        technologia.TermekLeurultNyomasErtekFlag = false;
                    }

                    if (technologia.AramErtekekEllenorzeseFlag)
                    {
                        double fesz_Max = 0;
                        double fesz_Min = 0;
                        double LastAram_Max = 0;
                        double LastAram_Min = 0;
                        double maxAram_Max = 0;
                        double maxAram_Min = 0;
                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                        {
    
                           /* if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 111)
                            {
                                foreach (DataRow sp in ActParamType.dt.Rows)
                                {
                                    switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                    {
                                        case 5:
                                            fesz_Max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 6:
                                            fesz_Min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                            break;
                                    }
                                }
                                double data = technologia.cAramMeres.minFesz;
                                dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), fesz_Min, fesz_Max);
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, fesz_Min, fesz_Max);
                                });
                            }*/
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 112)                           
                                {
                                foreach (DataRow sp in ActParamType.dt.Rows)
                                {
                                    switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                    {
                                        case 5:
                                            fesz_Max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 6:
                                            fesz_Min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                            break;
                                    }
                                }
                                double data = technologia.cAramMeres.maxFesz;
                                    dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                    {
                                        if (technologia.cbtnOVEtalon.value)
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), fesz_Min, fesz_Max);
                                        else
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, fesz_Min, fesz_Max);
                                    });
                                }

                   
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 113)    
                            {
                                foreach (DataRow sp in ActParamType.dt.Rows)
                                {
                                    switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                    {
                                        case 7:
                                            maxAram_Max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;

                                        case 8:
                                            maxAram_Min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                            break;
                                    }
                                }
                            
                                double data  = technologia.cAramMeres.maxAram;
                                dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), maxAram_Min, maxAram_Max);
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, maxAram_Min, maxAram_Max);
                                });
                            }
                                      
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 114)
                            {
                                foreach (DataRow sp in ActParamType.dt.Rows)
                                {
                                    switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                    {
                                        case 10:
                                            LastAram_Max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 11:
                                            LastAram_Min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                        
                                            break;
                                    }
                                }
                                double data= technologia.cAramMeres.LastAram;
                                dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), LastAram_Min, LastAram_Max);
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, LastAram_Min, LastAram_Max);
                                });
                            }
                        }
                      //  technologia.AramMeresEredmenyLekerdezesFlag = false;
                        technologia.AramErtekekEllenorzeseFlag = false;
                    }

                    if (technologia.TermekMagassagEllenorzesFlag)
                    {
                        double max = 0;
                        double min = 0;
 
                        double data  = Convert.ToDouble( technologia.plc.read.ProbatestMozgatoPozicio) / 1000.0;
                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                        {
                        
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 115)
                                {
                                    foreach (DataRow sp in ActParamType.dt.Rows)
                                    {
                                        switch (Convert.ToInt32(sp[ClassSensorProperty.cD_id]))
                                        {
                                        case 12:
                                            max = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) + Convert.ToDouble(sp[ClassSensorProperty.chysteresisPoz]);
                                            break;
                                        case 13:
                                                min = Convert.ToDouble(sp[ClassSensorProperty.cDefaultValue]) - Convert.ToDouble(sp[ClassSensorProperty.chysteresisNeg]);
                                           break;
                                        }
                                    }
                                    dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                    {
                                        if (technologia.cbtnOVEtalon.value)
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), min, max);
                                        else
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, min, max);
                                    });
                                }
                        }
                            technologia.TermekMagassagEllenorzesFlag = false;
                    }


                    if (technologia.ZarokupakEllenorzesFlag)
                    {
                        double data = Convert.ToDouble( technologia.plc.read.ZaroKupakPozicio);

                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 100)
                            {
                                dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                });
                                break;
                            }
                        technologia.ZarokupakEllenorzesFlag = false;
                    }

                    if (technologia.CameraFelsoFlag)
                    {
                        double data =Convert.ToDouble( technologia.plc.read.CameraMenetrogzitoCheck_1);

                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 101)
                            {
                                foreach (DataRow globalRow in ProductType_Global.dt.Rows)
                                {
                                    if (Convert.ToInt32(globalRow[ClassSensorProperty.cD_id]) == 101)
                                    {
                                        if (Convert.ToInt32(globalRow[ClassSensorProperty.cDefaultValue]) == 1)
                                        {
                                            dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                        {
                                            if (technologia.cbtnOVEtalon.value)
                                                WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                            else
                                                WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                        });
                                        }
                                        break;
                                    }
                                }
                            }
                        data = Convert.ToDouble(technologia.plc.read.CameraMenetrogzitoCheck_2);

                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 102)
                            {
                                foreach (DataRow globalRow in ProductType_Global.dt.Rows)
                                {
                                    if (Convert.ToInt32(globalRow[ClassSensorProperty.cD_id]) == 102)
                                    {
                                        if (Convert.ToInt32(globalRow[ClassSensorProperty.cDefaultValue]) == 1)
                                        {
                                            dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                            {
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                            });
                                        }
                                        break;
                                    }
                                }
                            }
           
                        technologia.CameraFelsoFlag = false;
                    }

                    if (technologia.CameraAlsoFlag)
                    {
                       double data = Convert.ToDouble(technologia.plc.read.CameraMenetrogzitoCheck_3);
                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 103)
                            {
                                foreach (DataRow globalRow in ProductType_Global.dt.Rows)
                                {
                                    if (Convert.ToInt32(globalRow[ClassSensorProperty.cD_id]) == 103)
                                    {
                                        if (Convert.ToInt32(globalRow[ClassSensorProperty.cDefaultValue]) == 1)
                                        {
                                            dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                            {
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                            });
                                        }
                                        break;
                                    }
                                }
                            }
                        data = Convert.ToDouble(technologia.plc.read.CameraZsugorcsoCheck_1);
                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 104)
                            {
                                foreach (DataRow globalRow in ProductType_Global.dt.Rows)
                                {
                                    if (Convert.ToInt32(globalRow[ClassSensorProperty.cD_id]) == 104)
                                    {
                                        if (Convert.ToInt32(globalRow[ClassSensorProperty.cDefaultValue]) == 1)
                                        {
                                            dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                            {
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                            });
                                        }
                                        break;
                                    }
                                }
                            }
                        data = Convert.ToDouble(technologia.plc.read.CameraZsugorcsoCheck_2);

                        foreach (DataRow sDRDef in CRDDef.dt_DiagRowsDefault.Rows)
                            if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]) == 105)
                            {
                                foreach (DataRow globalRow in ProductType_Global.dt.Rows)
                                {
                                    if (Convert.ToInt32(globalRow[ClassSensorProperty.cD_id]) == 105)
                                    {
                                        if (Convert.ToInt32(globalRow[ClassSensorProperty.cDefaultValue]) == 1)
                                        {
                                            dgvOVMeasurementData.Invoke((MethodInvoker)delegate
                                            {
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                            });
                                        }
                                        break;
                                    }
                                }
                            }

                        technologia.CameraAlsoFlag = false;
                    }
                

                    lbOVCimkeRagaszt.Invoke((MethodInvoker)delegate
                    {
                        if (technologia.CimkePrintFlag && technologia.cbtnOVNormal.value)
                            lbOVCimkeRagaszt.Visible = true;
                        else
                        {
                            lbOVCimkeRagaszt.Visible = false;
                            technologia.CimkePrintFlag = false;
                        }

                    });
                }

                Thread.Sleep(400);
            }
            while (true);
        }
        public void alarmSet(bool failure, int alarmIndex)
        {
            if (failure)
                cAlarm.setActiveAlarm(alarmIndex);
            else
                cAlarm.AckActiveAlarm(alarmIndex);
        }

        public void PLCInterfaceScreenUpdate()
        {
            printReadInterfaceStatus(btn_In_Interface_1, technologia.plc.read.LevegoelokeszitoNyomasKapcsolo);
            printReadInterfaceStatus(btn_In_Interface_2, technologia.plc.read.KabelkorbacsHosszusagErzekelo_1);
            printReadInterfaceStatus(btn_In_Interface_3, technologia.plc.read.KabelkorbacsHosszusagErzekelo_2);
            printReadInterfaceStatus(btn_In_Interface_4, technologia.plc.read.KabelkorbacsHosszusagErzekelo_3);
            printReadInterfaceStatus(btn_In_Interface_5, technologia.plc.read.Probatest_Alap);
            printReadInterfaceStatus(btn_In_Interface_6, technologia.plc.read.Probatest_FelhLenyom);
            printReadInterfaceStatus(btn_In_Interface_7, technologia.plc.read.Probatest_vezerelheto);
            printReadInterfaceStatus(btn_In_Interface_8, technologia.plc.read.Probatest_Lent);
            printReadInterfaceStatus(btn_In_Interface_9, technologia.plc.read.GripperNyitva);
            printReadInterfaceStatus(btn_In_Interface_10, technologia.plc.read.GripperMegfogta);
            printReadInterfaceStatus(btn_In_Interface_11, technologia.plc.read.GripperZarva);
            printReadInterfaceStatus(btn_In_Interface_12, technologia.plc.read.MerotuskePozicio_1);
            printReadInterfaceStatus(btn_In_Interface_13, technologia.plc.read.MerotuskePozicio_2);
            printReadInterfaceStatus(btn_In_Interface_14, technologia.plc.read.ZaroKupakElenorzesErentosGomb);
            printReadInterfaceStatus(btn_In_Interface_15, technologia.plc.read.ZaroKupakPozicio);
            printReadInterfaceStatus(btn_In_Interface_16, technologia.plc.read.SelesjtTaroloDarabszemlalo);
            printReadInterfaceStatus(btn_In_Interface_17, technologia.plc.read.CameraMenetrogzitoCheck_1);
            printReadInterfaceStatus(btn_In_Interface_18, technologia.plc.read.CameraMenetrogzitoCheck_2);
            printReadInterfaceStatus(btn_In_Interface_19, technologia.plc.read.CameraMenetrogzitoCheck_3);
            printReadInterfaceStatus(btn_In_Interface_20, technologia.plc.read.CameraZsugorcsoCheck_1);
            printReadInterfaceStatus(btn_In_Interface_21, technologia.plc.read.CameraZsugorcsoCheck_2);

            printReadInterfaceStatus(btn_In_Interface_22, technologia.plc.read.CameraFinish_1);
            printReadInterfaceStatus(btn_In_Interface_23, technologia.plc.read.CameraFinish_2);
            //printReadInterfaceStatus(btn_In_Interface_24, technologia.plc.read.CameraZsugorcsoCheck_1);
            //printReadInterfaceStatus(btn_In_Interface_25, technologia.plc.read.CameraZsugorcsoCheckFinish_2);
            //printReadInterfaceStatus(btn_In_Interface_26, technologia.plc.read.CameraZsugorcsoCheck_2);

    

            tb_In_Interface_1.Text = (Convert.ToDouble( technologia.plc.read.ProbatestMozgatoPozicio)/1000.0).ToString();
            tb_In_Interface_2.Text = (Convert.ToDouble(technologia.plc.read.ProbatestNyomasTavado )/ 1000.0).ToString();
           // tb_In_Interface_3.Text  = (Convert.ToDouble(technologia.plc.read.TermekLeurultNyomasErtek) / 1000.0).ToString();

            technologia.cbtn_Out_Interface_1.Enabled = technologia.plc.read.LevegoElokeszitoVezereltLeeresztoSzelep_Enabled;
         
            technologia.cbtn_Out_Interface_2.Enabled = technologia.plc.read.ProbatestMozgatoSzelepLe_Enabled;
            technologia.cbtn_Out_Interface_3.Enabled = technologia.plc.read.ProbatestMozgatoSzelepFel_Enabled;
            technologia.cbtn_Out_Interface_4.Enabled = technologia.plc.read.GripperZarSzelep_Enabled;
            technologia.cbtn_Out_Interface_5.Enabled = technologia.plc.read.GripperNyitSzelep_Enabled;
            technologia.cbtn_Out_Interface_6.Enabled = technologia.plc.read.MeroTuskeMozgatoSzelepNyit_Enabled;
            technologia.cbtn_Out_Interface_7.Enabled = technologia.plc.read.MeroTuskeMozgatoSzelepZar_Enabled;
            technologia.cbtn_Out_Interface_8.Enabled = technologia.plc.read.TermekFeltoltoSzelep_Enabled;
            technologia.cbtn_Out_Interface_9.Enabled = technologia.plc.read.MeresLog_Enabled;
            technologia.cbtn_Out_Interface_10.Enabled = technologia.plc.read.CameraAlso_Enabled;
            technologia.cbtn_Out_Interface_11.Enabled = technologia.plc.read.CameraFelso_Enabled;
           // technologia.cbtn_Out_Interface_12.Enabled = technologia.plc.read.CameraFelso_Enabled;
           // technologia.cbtn_Out_Interface_13.Enabled = technologia.plc.read.CameraFelso_Enabled;
           // technologia.cbtn_Out_Interface_14.Enabled = technologia.plc.read.CameraFelso_Enabled;      
            

            if (technologia.plc.write.Service == true && technologia.MachinOn)
            {
                technologia.plc.write.LevegoElokeszitoVezereltLeeresztoSzelep = technologia.cbtn_Out_Interface_1.value;
           
                technologia.plc.write.ProbatestMozgatoSzelepLe = technologia.cbtn_Out_Interface_2.value;
                technologia.plc.write.ProbatestMozgatoSzelepFel = technologia.cbtn_Out_Interface_3.value;
                technologia.plc.write.GripperZar = technologia.cbtn_Out_Interface_4.value;
                technologia.plc.write.GripperNyit = technologia.cbtn_Out_Interface_5.value;
                technologia.plc.write.MeroTuskeMozgatoSzelepNyit = technologia.cbtn_Out_Interface_6.value;
                technologia.plc.write.MeroTuskeMozgatoSzelepZar = technologia.cbtn_Out_Interface_7.value;
                technologia.plc.write.TermekFeltoltes = technologia.cbtn_Out_Interface_8.value;
                technologia.plc.write.AramMeresTesztelesInditasa = technologia.cbtn_Out_Interface_9.value;
                technologia.plc.write.CameraCheck_Also = technologia.cbtn_Out_Interface_10.value;
                technologia.plc.write.CameraCheck_Felso = technologia.cbtn_Out_Interface_11.value;
                //technologia.plc.write.CameraMenetrogzitoCheck_3 = technologia.cbtn_Out_Interface_12.value;
                //    technologia.plc.write.CameraZsugorcsoCheck_1 = technologia.cbtn_Out_Interface_13.value;
                //   technologia.plc.write.CameraZsugorcsoCheck_2 = technologia.cbtn_Out_Interface_14.value;
                //   technologia.plc.write.CameraMembranCheck = technologia.cbtn_Out_Interface_15.value;
            }
            else
            {
                technologia.cbtn_Out_Interface_1.value = technologia.plc.write.LevegoElokeszitoVezereltLeeresztoSzelep;
            
                technologia.cbtn_Out_Interface_2.value = technologia.plc.write.ProbatestMozgatoSzelepLe;
                technologia.cbtn_Out_Interface_3.value = technologia.plc.write.ProbatestMozgatoSzelepFel;
                technologia.cbtn_Out_Interface_4.value = technologia.plc.write.GripperZar;
                technologia.cbtn_Out_Interface_5.value = technologia.plc.write.GripperNyit;
                technologia.cbtn_Out_Interface_6.value = technologia.plc.write.MeroTuskeMozgatoSzelepNyit;
                technologia.cbtn_Out_Interface_7.value = technologia.plc.write.MeroTuskeMozgatoSzelepZar;
                technologia.cbtn_Out_Interface_8.value = technologia.plc.write.TermekFeltoltes;
                technologia.cbtn_Out_Interface_9.value = technologia.plc.write.AramMeresTesztelesInditasa;
                technologia.cbtn_Out_Interface_10.value = technologia.plc.write.CameraCheck_Also;
                technologia.cbtn_Out_Interface_11.value = technologia.plc.write.CameraCheck_Felso;
              //  technologia.cbtn_Out_Interface_12.value = technologia.plc.write.CameraMenetrogzitoCheck_3;
               //     technologia.cbtn_Out_Interface_13.value = technologia.plc.write.CameraZsugorcsoCheck_1;
                //   technologia.cbtn_Out_Interface_14.value = technologia.plc.write.CameraZsugorcsoCheck_2;
                //    technologia.cbtn_Out_Interface_15.value = technologia.plc.write.CameraMembranCheck;
            }
            printWriteInterfaceValue(tb_Out_Interface_1, technologia.plc.write.ProbatestPozicio_Alap);
            printWriteInterfaceValue(tb_Out_Interface_2, technologia.plc.write.ProbatestVezKezdoPozicio);
            printWriteInterfaceValue(tb_Out_Interface_3, technologia.plc.write.ProbatestVezVegPozicio);

        }
        public void printReadInterfaceStatus(object obj, bool value)
        {
            if (value)
                ((Button)obj).BackColor = Color.Green;
            else
                ((Button)obj).BackColor = Color.White;
        }
        public void printbtnStatus(object obj, bool value, Color onColor, Color offColor)
        {
            if (value)
                ((Button)obj).BackColor = onColor;
            else
                ((Button)obj).BackColor = offColor;
        }
        public static void printWriteInterfaceStatus(object obj, out bool value)
        {
            if (((Button)obj).BackColor == Color.Green)
            {
                value = true;
            }
            else
            {
                value = false;
                if (((Button)obj).BackColor != Color.White)
                    ((Button)obj).BackColor = Color.White;
            }

        }
        public static void printWriteInterfaceValue(object obj, object value)
        {
            ((TextBox)obj).Text = value.ToString();
        }
        private void etsmOpMeasurementParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpMeasurementParam);
            OPMeasurementParamSelect(scOpMeasurement_NormalType_1);
        }
        private void tsmAlarmActive_Click(object sender, EventArgs e)
        {
            cAlarm.plsAlarmReflesh = true;
            ScreenVisible(scAlarmActive);
        }
        private void tsmAlarmLog_Click(object sender, EventArgs e)
        {
            cAlarm.RefreshAlarmLog();
            ScreenVisible(scAlarmLog);
            WriteAlarmLogGridView(dgvAlarmLog);
        }
        void WriteAlarmLogGridView(DataGridView DGV)
        {
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            DGV.Columns[2].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            DGV.Columns[3].Visible = false;
            DGV.Rows.Clear();

            foreach (sAlarm sA in cAlarm.lsAlarmLog)
            {
                int n = DGV.Rows.Add();
                if (n % 2 == 0)
                {
                    DGV.Rows[n].DefaultCellStyle.BackColor = Color.LightGray;
                }
                DGV.Rows[n].Cells[0].Value = sA.index;
                DGV.Rows[n].Cells[1].Value = sA.failureTime;
                DGV.Rows[n].Cells[2].Value = sA.ackTime;

                DGV.Rows[n].Cells[3].Value = sA.status;
                DGV.Rows[n].Cells[4].Value = sA.text.Trim();
            }
        }
        void WriteAlarmActiveGridView(DataGridView DGV)
        {
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            DGV.Columns[2].Visible = false;
            DGV.Rows.Clear();
            tbOErrorActual.ForeColor = Color.Red;
            tbOErrorActual.Text = "";
            bool first = true;
            foreach (sAlarm sA in cAlarm.getActivAlarm())
            {
                if (first == true)
                {
                    tbOErrorActual.Text = sA.ID + "\t " + sA.failureTime + "\t " + sA.text.Trim();
                }
                int n = DGV.Rows.Add();
                if (n % 2 == 0)
                {
                    DGV.Rows[n].DefaultCellStyle.BackColor = Color.LightGray;
                }

                DGV.Rows[n].Cells[0].Value = sA.index;
                DGV.Rows[n].Cells[1].Value = sA.failureTime;
                DGV.Rows[n].Cells[2].Value = sA.status;
                DGV.Rows[n].Cells[3].Value = sA.text.Trim();
            }
        }


        public static bool WriteMeasurementDataGridViewFull = true;

        void WriteMeasurementDataGridView(DataGridView DGV, double data, DataRow sDRDef,  ClassRowsDiagnostic CRD = null, double min = 1, double max = 1)
        {
            min = Math.Round(min, 3);
            max = Math.Round(max, 3);
            //     CMD.updateMeasurementParamError(Param);
            //    CMD.updatePtoductTypeError(PtoductType);
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //     DGV.Columns[2].DefaultCellStyle.Format = "N2";
            //    DGV.Columns[3].DefaultCellStyle.Format = "N2";
            //    DGV.Columns[4].DefaultCellStyle.Format = "N2";
            //  List<sSensorProperty> lsSPA = Param.getSensorPropertiesActive();
         //   sMeasurementData sMD = new sMeasurementData();
            technologia.cmEol.sM.CMD.dtModify.Rows.Add();
            int rIndex = technologia.cmEol.sM.CMD.dtModify.Rows.Count - 1;

            int k = -1;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_S_id] = technologia.cmEol.SensorProperty_S_id;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Pc_id] = options.PC_id;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_D_id] = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]);
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Description] = sDRDef[ClassRowsDiagnostic.cDescription].ToString();
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Visible] = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]);
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = 0;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_MeasurementValue] = data;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_ToleranceNeg] = min;
            technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_TolerancePoz] = max;
            bool save = false;
            if (CRD == null)
            {
                //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét
                if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]) != 2)
                    for (int i = 0; i < DGV.RowCount; i++)
                        if (Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_D_id]) == (int)DGV.Rows[i].Cells[0].Value)
                            k = i;

                if (k == -1)
                {
                    save = true;
                    if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]) > 0)//nem figyel
                    {
                        k = DGV.Rows.Add();

                        DGV.Rows[k].Cells[0].Value = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]);
                        DGV.Rows[k].Cells[1].Value = sDRDef[ClassRowsDiagnostic.cDescription].ToString();
                        DGV.Rows[k].Cells[2].Value = min;
                        DGV.Rows[k].Cells[4].Value = max;
                        DGV.Rows[k].Cells[5].Value = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cS_id]);
                        DGV.Rows[k].Cells[6].Value = technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Pc_id];
                        DGV.Rows[k].Cells[7].Value = 1;
                        DGV.Rows[k].Cells[8].Value = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]);
                        DGV.Rows[k].Cells[9].Value = sDRDef[ClassRowsDiagnostic.cLoginName].ToString();
                    }
                }

                if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]) > 0)  //"Nem figyel":
                {
                    DGV.Rows[k].Cells[3].Value = data;
                }

                if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]) == 1) //"Megfelelő":
                {
                    if (data < Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_ToleranceNeg]))
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 1;
                        technologia.MegfeleloFlag = false;
                    }
                    else
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                    if (data > Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_TolerancePoz]))
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 2;

                        technologia.MegfeleloFlag = false;

                    }
                    else
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                }
                if (Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cVisible]) == 2)  //"Hibás"
                {

                    if (data < Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_ToleranceNeg]))
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;
                    else
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 1;
                        technologia.MegfeleloFlag = false;
                    }

                    if (data > Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_TolerancePoz]))
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                    else
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 2;
                        technologia.MegfeleloFlag = false;
                    }
                }
            }
            else
            {
                var ActsDR = CRD.dt.NewRow();

                foreach (DataRow sDR in CRD.dt.Rows)
                {
                    if (Convert.ToInt32(sDR[ClassRowsDiagnostic.cD_id]) == Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]))
                    {
                        //   var sourceRow = sDR;
                        ActsDR.ItemArray = sDR.ItemArray.Clone() as object[];
                        break;
                    }
                }
                //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét

                if (Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]) > 0)
                    for (int i = 0; i < DGV.RowCount; i++)
                        if (Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_D_id]) == (int)DGV.Rows[i].Cells[0].Value)
                            k = i;

                //ha még nincs ilyen sor a DGV-be akkor tegyük bele
                if (k == -1)
                {
                    save = true;
                    if (Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]) > 0)
                    {
                        k = DGV.Rows.Add();

                        DGV.Rows[k].Cells[0].Value = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cD_id]);
                        DGV.Rows[k].Cells[1].Value = sDRDef[ClassRowsDiagnostic.cDescription].ToString();
                        DGV.Rows[k].Cells[2].Value = min;
                        DGV.Rows[k].Cells[4].Value = max;
                        DGV.Rows[k].Cells[5].Value = Convert.ToInt32(sDRDef[ClassRowsDiagnostic.cS_id]);
                        DGV.Rows[k].Cells[6].Value = technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Pc_id];
                        DGV.Rows[k].Cells[7].Value = 1;
                        DGV.Rows[k].Cells[8].Value = Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]);
                        DGV.Rows[k].Cells[9].Value = sDRDef[ClassRowsDiagnostic.cLoginName].ToString();
                    }
                }


                //töltsük bele az értéket 

                if (Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]) > 0)  //"Nem figyel":
                {
                    DGV.Rows[k].Cells[3].Value = data;
                }
                if (Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]) == 1) //megfelelő
                {
                    if (data < Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_ToleranceNeg]))
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 1;
                        technologia.MegfeleloFlag = false;
                    }
                    else
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                    if (data > Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_TolerancePoz]))
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 2;
                        technologia.MegfeleloFlag = false;
                    }
                    else
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                }
                if (Convert.ToInt32(ActsDR.ItemArray[ClassRowsDiagnostic.cVisible]) == 2)   //hibás
                {
                    bool NotGood = false;

                    if (data < Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_ToleranceNeg])
                        || Convert.ToDouble(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_TolerancePoz]) < data)
                    {
                        NotGood = true;
                    }

                    if (NotGood)
                        DGV.Rows[k].Cells[3].Style.BackColor = Color.Green;
                    else
                    {
                        DGV.Rows[k].Cells[3].Style.BackColor = Color.Red;
                        technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error] = Convert.ToInt32(technologia.cmEol.sM.CMD.dtModify.Rows[rIndex][CMeasurementData.cData_Error]) + 1;
                        technologia.MegfeleloFlag = false;
                    }
                }
            }
            if (save == true)
            {
                technologia.cmEol.sM.CMD.MeasurementDataSave(rIndex);
             //   technologia.cmEol.SetMeasurementData(sMD);
            }
        }
        

        private void dGVOpMeasurementParam_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //    Param_EditingControlShowing(e, dGVOpMeasurementParam);
        }
        private void dGVOpTypeParam_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //        Param_EditingControlShowing(e, dGVOpProductTypeParam);
        }
        void Param_EditingControlShowing(DataGridViewEditingControlShowingEventArgs e, DataGridView DGV)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(tbOpDouble_KeyPress);
            if (DGV.CurrentCell.ColumnIndex == 2 || DGV.CurrentCell.ColumnIndex == 3 || DGV.CurrentCell.ColumnIndex == 4 || DGV.CurrentCell.ColumnIndex == 7) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(tbOpDouble_KeyPress);
                }
            }
        }
      
        private void tsmOpMeasurement_Normal_Click(object sender, EventArgs e)
        {
            OPMeasurementParamSelect(scOpMeasurement_NormalType_1);
        }

        private void tsmOpMeasurement_Etalon_Click(object sender, EventArgs e)
        {
            OPMeasurementParamSelect(scOpMeasurement_EtalonType_1);
        }


        private void tsmOpSQL_Click(object sender, EventArgs e)
        {
            OPtParamSelect(scOPSQL);
        }

        private void tsmOpPrinter_Click(object sender, EventArgs e)
        {
            cbOpPrinterPrinters.Items.Clear();

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cbOpPrinterPrinters.Items.Add(printer);
            }
            cbOpPrinterPrinters.Text = options.OpPrinter;

            tbOpConPrinter_CimkeYearDay.Text = options.CimkeYearDay;
            tbOpConPrinter_CimkeStabilPartNumber.Text = options.CimkeStabilPartNumber;
            tbOpConPrinter_CimkexyPartNumber.Text = options.CimkexyPartNumber;
            tbOpConPrinter_CimkeQR_xyPartNumber.Text = options.CimkeQR_xyPartNumber;
            tbOpConPrinter_CimkeQR_SerialNumber.Text = options.CimkeQR_SerialNumber;
            tbOpConPrinter_CimkeQR_YYYYMMDD.Text = options.CimkeQR_YYYYMMDD;

            tbOpConPrinter_CimkeHibasSerialNumber.Text = options.CimkeHibasSerialNumber;
            tbOpConPrinter_CimkeHibasDatum.Text = options.CimkeHibasDatum;
            tbOpConPrinter_CimkeHibasHiba.Text = options.CimkeHibasHiba;

            OPtParamSelect(scOPPrinter);
        }

        private void tsmOpPLC_Click(object sender, EventArgs e)
        {
            OPtParamSelect(scOPPLC);
        }

        private void etsmOpConnectionsParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpConnectionsParam);
            OPParamSQLWrite();
            OPtParamSelect(scOPSQL);
        }

        private void etsmOpMeasurementAlarmParam_Click(object sender, EventArgs e)
        {
            OpMeasOptWrite();
            ScreenVisible(scOpEtalonCheckAlarmParam);
        }

        private void etsmOpMachinParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpMachinParam);
            ProductType_Global.WriteDataGridView();
        }

        private void etsmOpEtalonParam_Click(object sender, EventArgs e)
        {
            EtalonMenu_update();

            int EtalonSelectName = -1;
 
            foreach (ToolStripItem tsi in tsmEtalon.Items)
            {
                EtalonSelectName = tsmEtalon.Items.IndexOf(tsi);
                break;
            }
             tsmEtalonSelect(EtalonSelectName);
            ScreenVisible(scOpEtalonParam);
        }
        void OPtParamSelect(int index)
        {
            tsmOpSQL.BackColor = Color.LightGray;
            tsmOpSQL.ForeColor = Color.Black;
            tsmOpPrinter.BackColor = Color.LightGray;
            tsmOpPrinter.ForeColor = Color.Black;
            tsmOpPLC.BackColor = Color.LightGray;
            tsmOpPLC.ForeColor = Color.Black;
            plOpConSQL.Visible = false;
            plOpConPrinter.Visible = false;
            plOpConPLC.Visible = false;
            switch (index)
            {
                case scOPSQL:
                    tsmOpSQL.BackColor = Color.Gray;
                    tsmOpSQL.ForeColor = Color.White;
                    plOpConSQL.Visible = true;
                    OPParamSQLWrite();
                    break;
                case scOPPrinter:
                    tsmOpPrinter.BackColor = Color.Gray;
                    tsmOpPrinter.ForeColor = Color.White;
                    plOpConPrinter.Visible = true;

                    break;
                case scOPPLC:
                    tsmOpPLC.BackColor = Color.Gray;
                    tsmOpPLC.ForeColor = Color.White;
                    plOpConPLC.Visible = true;
                    OPParamPLCWrite();
                    break;
            }
        }
        void OPMeasurementParamSelect(int index)
        {
            tsmOpMeasurement_Normal.BackColor = Color.LightGray;
            tsmOpMeasurement_Normal.ForeColor = Color.Black;
            tsmOpMeasurement_Etalon.BackColor = Color.LightGray;
            tsmOpMeasurement_Etalon.ForeColor = Color.Black;

            switch (index)
            {
                case scOpMeasurement_NormalType_1:
                    tsmOpMeasurement_Normal.BackColor = Color.Gray;
                    tsmOpMeasurement_Normal.ForeColor = Color.White;
                    OPParamSelect = ParamNormalType_1;
                    break;

                case scOpMeasurement_EtalonType_1:
                    tsmOpMeasurement_Etalon.BackColor = Color.Gray;
                    tsmOpMeasurement_Etalon.ForeColor = Color.White;
                    OPParamSelect = ParamEtalonType_1;
                    break;

            }
            OPParamSelect.WriteDataGridView();
        }

        void OPProductTypeSelect(int index)
        {
            tsmOpProductType_1.BackColor = Color.LightGray;
            tsmOpProductType_1.ForeColor = Color.Black;
            tsmOpProductType_2.BackColor = Color.LightGray;
            tsmOpProductType_2.ForeColor = Color.Black;
            tsmOpProductType_3.BackColor = Color.LightGray;
            tsmOpProductType_3.ForeColor = Color.Black;
            switch (index)
            {
                case scSzenzorType_1:
                    tsmOpProductType_1.BackColor = Color.Gray;
                    tsmOpProductType_1.ForeColor = Color.White;
                    //      OPParamSelect = ProductType_1;
                    break;
                case scSzenzorType_2:
                    tsmOpProductType_2.BackColor = Color.Gray;
                    tsmOpProductType_2.ForeColor = Color.White;
                    //    OPParamSelect = ProductType_2;
                    break;
                case scSzenzorType_3:
                    tsmOpProductType_3.BackColor = Color.Gray;
                    tsmOpProductType_3.ForeColor = Color.White;
                    //     OPParamSelect = ProductType_3;

                    break;
            }
            //   OPParamSelect.WriteDataGridView();
        }

        void UserManegmentSelect(int index, int right)
        {
            tsmUserMy.BackColor = Color.LightGray;
            tsmUserMy.ForeColor = Color.Black;
            tsmUserNew.BackColor = Color.LightGray;
            tsmUserNew.ForeColor = Color.Black;
            tlpUserMy.Visible = false;
            tlpUserNew.Visible = false;
            tlpUsers.Visible = false;
            tsmUsers.BackColor = Color.LightGray;
            tsmUsers.ForeColor = Color.Black;
            if (right < ClassUser.lsUserManager)
            {
                tsmUserNew.Enabled = false;
                tsmUsers.Enabled = false;
            }
            else
            {
                tsmUserNew.Enabled = true;
                tsmUsers.Enabled = true;
            }

            switch (index)
            {
                case scUserMy:
                    tsmUserMy.BackColor = Color.Gray;
                    tsmUserMy.ForeColor = Color.White;
                    tbUserMyFirstName.Text = ClassUser.LoginUser.FirstName;
                    tbUserMyLastName.Text = ClassUser.LoginUser.LastName;
                    tbUserMyLoginName.Text = ClassUser.LoginUser.LoginName;
                    tbUserMyPassword.Text = "";
                    tbUserMyPasswordNew1.Text = "";
                    tbUserMyPasswordNew2.Text = "";
                    tlpUserMy.Visible = true;
                    break;
                case scUserNew:
                    tsmUserNew.BackColor = Color.Gray;
                    tsmUserNew.ForeColor = Color.White;
                    cbUserNewRight.SelectedIndex = 0; //"Kezelő"
                    tlpUserNew.Visible = true;
                    break;
                case scUsers:
                    tsmUsers.BackColor = Color.Gray;
                    tsmUsers.ForeColor = Color.White;
                    cbUserSelected();
                    tlpUsers.Visible = true;
                    break;
            }
        }

        private void tbOpDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }
        private void tbOpDouble_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 44))
            {
                e.Handled = true;
                return;
            }
            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 44)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void btnOpConSQLSave_Click(object sender, EventArgs e)
        {
            options.SQLDataSource = tbOSQLDataSource.Text;
            options.SQLInitialCatalog = tbOSQLInitialCatalog.Text;
            options.SQLUserID = tbOSQLUserID.Text;
            options.SQLPassword = tbOSQLPassword.Text;
            options.SaveOptions();
            technologia.plc.IP = (Settings.Default["PLC_IP_1"].ToString() + "." + Settings.Default["PLC_IP_2"].ToString() + "." + Settings.Default["PLC_IP_3"].ToString() + "." + Settings.Default["PLC_IP_4"].ToString());
            StartLoad();
        }
        private void btnOpConSQLCancel_Click(object sender, EventArgs e)
        {
            OPParamSQLWrite();
        }
        private void btnOpMeasurementParamSave_Click(object sender, EventArgs e)
        {
            OPParamSelect.SaveModify();
            OPParamSelect.Select();
            OPParamSelect.WriteDataGridView();
            ActParamType.Select();
            PLCPropertyDataMove();
        }

        private void btnOpMeasurementParamCancel_Click(object sender, EventArgs e)
        {
            OPParamSelect.WriteDataGridView();
        }

        private void btnOpConPLCSave_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(tbOpPLCIP_1.Text) > 255 || Convert.ToInt32(tbOpPLCIP_2.Text) > 255 ||
                Convert.ToInt32(tbOpPLCIP_3.Text) > 255 || Convert.ToInt32(tbOpPLCIP_4.Text) > 255)
                System.Windows.Forms.MessageBox.Show("Mentés sikertelen, nem megfelelő IP");
            else
            {
                options.PLC_IP_1 = Convert.ToInt32(tbOpPLCIP_1.Text);
                options.PLC_IP_2 = Convert.ToInt32(tbOpPLCIP_2.Text);
                options.PLC_IP_3 = Convert.ToInt32(tbOpPLCIP_3.Text);
                options.PLC_IP_4 = Convert.ToInt32(tbOpPLCIP_4.Text);
                options.SaveOptions();
            }
        }

        private void btnOpConPLCCancel_Click(object sender, EventArgs e)
        {
            OPParamPLCWrite();
        }
        private void btnOpEtalonCheckAlarmParamSave_Click(object sender, EventArgs e)
        {

            options.OpMeasOpt_M1_Active = cbOpEtalonCheckAlarm_M1_Active.Checked;
            options.OpMeasOpt_M2_Active = cbOpEtalonCheckAlarm_M2_Active.Checked;
            options.OpMeasOpt_M3_Active = cbOpEtalonCheckAlarm_M3_Active.Checked;
            options.OpMeasOpt_M1 = dtpOpEtalonCheckAlarm_M1.Value;
            options.OpMeasOpt_M2 = dtpOpEtalonCheckAlarm_M2.Value;
            options.OpMeasOpt_M3 = dtpOpEtalonCheckAlarm_M3.Value;
            options.SaveOptions();
        }
        private void btnOpEtalonCheckAlarmParamCancel_Click(object sender, EventArgs e)
        {
            OpMeasOptWrite();
        }

        public void OpMeasOptWrite()
        {
            cbOpEtalonCheckAlarm_M1_Active.Checked = options.OpMeasOpt_M1_Active;
            cbOpEtalonCheckAlarm_M2_Active.Checked = options.OpMeasOpt_M2_Active;
            cbOpEtalonCheckAlarm_M3_Active.Checked = options.OpMeasOpt_M3_Active;
            dtpOpEtalonCheckAlarm_M1.Value = options.OpMeasOpt_M1;
            dtpOpEtalonCheckAlarm_M2.Value = options.OpMeasOpt_M2;
            dtpOpEtalonCheckAlarm_M3.Value = options.OpMeasOpt_M3;
        }

        public void OPParamPLCWrite()
        {
            tbOpPLCIP_1.Text = options.PLC_IP_1.ToString();
            tbOpPLCIP_2.Text = options.PLC_IP_2.ToString();
            tbOpPLCIP_3.Text = options.PLC_IP_3.ToString();
            tbOpPLCIP_4.Text = options.PLC_IP_4.ToString();
        }
        public void OPParamSQLWrite()
        {
            tbOSQLDataSource.Text = options.SQLDataSource;
            tbOSQLInitialCatalog.Text = options.SQLInitialCatalog;
            tbOSQLUserID.Text = options.SQLUserID;
            tbOSQLPassword.Text = options.SQLPassword;
        }

        private void etsmUsOptions_Click(object sender, EventArgs e)
        {
            ScreenVisible(scUsManagment);
            UserManegmentSelect(scUserMy, ClassUser.LoginUser.Right);
        }

        private void tsmUserMy_Click(object sender, EventArgs e)
        {
            UserManegmentSelect(scUserMy, ClassUser.LoginUser.Right);
        }

        private void tsmUserNew_Click(object sender, EventArgs e)
        {
            cbUserNewRight.Items.Clear();
            cbUserNewRight.Items.Insert(ClassUser.lsUserOperator - 1, ClassUser.lsUserOperator_text);
            cbUserNewRight.Items.Insert(ClassUser.lsUserMaintenance - 1, ClassUser.lsUsermaintenance_text);
            cbUserNewRight.Items.Insert(ClassUser.lsUserManager - 1, ClassUser.lsUserManager_text);
            cbUserNewRight.Items.Insert(ClassUser.lsUserAdmin - 1, ClassUser.lsUserAdmin_text);
            UserManegmentSelect(scUserNew, ClassUser.LoginUser.Right);
        }

        private void tsmUsers_Click(object sender, EventArgs e)
        {
            cbUsersRight.Items.Clear();
            cbUsersRight.Items.Insert(ClassUser.lsUserOperator - 1, ClassUser.lsUserOperator_text);
            cbUsersRight.Items.Insert(ClassUser.lsUserMaintenance - 1, ClassUser.lsUsermaintenance_text);
            cbUsersRight.Items.Insert(ClassUser.lsUserManager - 1, ClassUser.lsUserManager_text);
            cbUsersRight.Items.Insert(ClassUser.lsUserAdmin - 1, ClassUser.lsUserAdmin_text);
            UserManegmentSelect(scUsers, ClassUser.LoginUser.Right);
        }

        private void btnUserMySave_Click(object sender, EventArgs e)
        {
            if (tbUserMyPasswordNew1.Text.Length > 0)
            {
                if (tbUserMyPasswordNew1.Text == tbUserMyPasswordNew2.Text)
                {
                    if (user.userCheked(ClassUser.LoginUser.LoginName, tbUserMyPassword.Text))
                    {
                        sUser su = new sUser();
                        su.FirstName = tbUserMyFirstName.Text;
                        su.LastName = tbUserMyLastName.Text;
                        su.LoginName = ClassUser.LoginUser.LoginName;
                        su.Right = ClassUser.LoginUser.Right;
                        su.Password = tbUserMyPasswordNew1.Text;
                        user.userUpdate(su);
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("A jelenlegi jelszó nem megfelelő!");
                }
                else
                    System.Windows.Forms.MessageBox.Show("Az új jelszók nem egyeznek!");
            }
            else
            {
                sUser su = new sUser();
                su.FirstName = tbUserMyFirstName.Text;
                su.LastName = tbUserMyLastName.Text;
                su.LoginName = ClassUser.LoginUser.LoginName;
                su.Right = ClassUser.LoginUser.Right;
                su.Password = tbUserMyPassword.Text;
                user.userUpdate(su);
            }
        }

        private void btnUserMyCancel_Click(object sender, EventArgs e)
        {
            tbUserMyFirstName.Text = ClassUser.LoginUser.FirstName;
            tbUserMyLastName.Text = ClassUser.LoginUser.LastName;
            tbUserMyLoginName.Text = ClassUser.LoginUser.LoginName;
            tbUserMyPassword.Text = "";
            tbUserMyPasswordNew1.Text = "";
            tbUserMyPasswordNew2.Text = "";
        }

        private void btnUserNewSave_Click(object sender, EventArgs e)
        {
            if (tbUserNewPasswordNew1.Text == tbUserNewPasswordNew2.Text)
            {
                sUser su = new sUser();
                su.FirstName = tbUserNewFirstName.Text;
                su.LastName = tbUserNewLastName.Text;
                su.Password = tbUserNewPasswordNew1.Text;
                su.LoginName = tbUserNewLoginName.Text;
                su.Right = cbUserNewRight.SelectedIndex + 1;
                user.AddUser(su);
            }
        }
        void cbUserSelected()
        {
            user.SelectedUsers();
            cbUsers.Items.Clear();
            foreach (sUser su in ClassUser.lsUser)
            {
                cbUsers.Items.Add(su.LoginName);
            }
        }

        private void btnUserNewCancel_Click(object sender, EventArgs e)
        {
            tbUserNewFirstName.Text = "";
            tbUserNewLastName.Text = "";
            tbUserNewPasswordNew1.Text = "";
            tbUserNewPasswordNew2.Text = "";
            tbUserNewLoginName.Text = "";
            cbUserNewRight.SelectedIndex = 0;
        }

        private void btnUsersSave_Click(object sender, EventArgs e)
        {
            if (cbUsers.Text.Length != 0)
            {
                if (tbUsersPasswordNew1.Text.Length > 0)
                {
                    if (tbUsersPasswordNew1.Text == tbUsersPasswordNew2.Text)
                    {
                        sUser su = new sUser();
                        su.LoginName = tbUsersLoginName.Text;
                        su.FirstName = tbUsersFirstName.Text;
                        su.LastName = tbUsersLastName.Text;
                        su.Right = cbUsersRight.SelectedIndex + 1;
                        su.Password = tbUsersPasswordNew1.Text;
                        if (cbUsersDelete.Checked)
                        {
                            su.Delet = 1;

                        }
                        else
                            su.Delet = 0;
                        user.userUpdate(su);
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Az új jelszók nem egyeznek!");
                }
                else
                {
                    sUser su = new sUser();
                    su.FirstName = tbUsersFirstName.Text;
                    su.LastName = tbUsersLastName.Text;
                    su.LoginName = tbUsersLoginName.Text;
                    su.Right = cbUsersRight.SelectedIndex + 1;
                    su.Password = "";
                    if (cbUsersDelete.Checked)
                        su.Delet = 1;
                    else
                        su.Delet = 0;
                    user.userUpdate(su);
                }
                cbUserSelected();
                cbUsers.Text = tbUsersLoginName.Text;
            }
            else
                System.Windows.Forms.MessageBox.Show("Nincs felhasználó kiválasztva!");
        }

        private void cbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (sUser su in ClassUser.lsUser)
            {
                if (su.LoginName == cbUsers.Text)
                {
                    tbUsersFirstName.Text = su.FirstName;
                    tbUsersLastName.Text = su.LastName;
                    tbUsersLoginName.Text = su.LoginName;
                    tbUsersPasswordNew1.Text = "";
                    tbUsersPasswordNew2.Text = "";
                    cbUsersRight.SelectedIndex = su.Right - 1;
                    if (su.Delet == 1)
                        cbUsersDelete.Checked = true;
                    else
                        cbUsersDelete.Checked = false;
                }
            }
        }

        private void btnUsersCancel_Click(object sender, EventArgs e)
        {
            foreach (sUser su in ClassUser.lsUser)
            {
                if (su.LoginName == cbUsers.Text)
                {
                    tbUsersFirstName.Text = su.FirstName;
                    tbUsersLastName.Text = su.LastName;
                    tbUsersLoginName.Text = su.LoginName;
                    tbUsersPasswordNew1.Text = "";
                    tbUsersPasswordNew2.Text = "";
                    cbUsersRight.SelectedIndex = su.Right - 1;
                }
            }
        }

        private void etsmOpInterfaceParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpInterfaceParam);
        }

        SaveForm SaveF;


        private void btnOpPrinterTestPagePrint_Jo_Click(object sender, EventArgs e)
        {
            CimkePrint(technologia.ActKIONPartNumber, technologia.ActStabilPartNumber, "1234567", DateTime.Now,true);
        }

        private void btnOpPrinterTestPagePrint_Rossz_Click(object sender, EventArgs e)
        {
            CimkePrint(technologia.ActKIONPartNumber, technologia.ActStabilPartNumber, "1234567", DateTime.Now,false);
        }

        public static void CimkePrint(string KION_Partumber, string STABIL_Partumber, string SerialNumber, DateTime date,bool JoRossz)
        {
            ClassPrinter classPrinter = new ClassPrinter();
            if (JoRossz)
            {
                string path =  @"C:\Cimke_felepetese\792 cimke ZD420.prn";
                while(SerialNumber.Length<9)
                {
                    SerialNumber = "0" + SerialNumber;
                }
                if ( classPrinter.ReadPrinterFile(path))
                classPrinter.Print(KION_Partumber, STABIL_Partumber, SerialNumber, date);
               else
                    System.Windows.Forms.MessageBox.Show("A file nem található:" + path);
            }
            else
            {
                string hiba = "";
                if (technologia.cmEol != null)
                {
                    foreach (DataRow row in technologia.cmEol.sM.CMD.dtModify.Rows)
                    {
                        if (Convert.ToInt32(row[CMeasurementData.cData_Error]) > 0)
                        {
                            hiba = row[CMeasurementData.cData_Description].ToString();
                            break;
                        }
                    }
                }
                else
                    hiba = "Teszt";
                if (hiba == "")
                    hiba = "Megszakítva";
                string path = @"C:\Cimke_felepetese\\792 cimke ZD420_Rossz.prn";
                if (classPrinter.ReadPrinterFile(path))
                classPrinter.Print( SerialNumber, hiba, date);
                else
                    System.Windows.Forms.MessageBox.Show("A file nem található:"+ path);
            }
        }
       

        private void btnOpConPrinterSave_Click(object sender, EventArgs e)
        {
            options.OpPrinter = cbOpPrinterPrinters.Text;
            options.CimkeYearDay = tbOpConPrinter_CimkeYearDay.Text;
            options.CimkeStabilPartNumber = tbOpConPrinter_CimkeStabilPartNumber.Text;
            options.CimkexyPartNumber = tbOpConPrinter_CimkexyPartNumber.Text;
            options.CimkeQR_xyPartNumber = tbOpConPrinter_CimkeQR_xyPartNumber.Text;
            options.CimkeQR_SerialNumber = tbOpConPrinter_CimkeQR_SerialNumber.Text;
            options.CimkeQR_YYYYMMDD = tbOpConPrinter_CimkeQR_YYYYMMDD.Text;

            options.CimkeHibasSerialNumber = tbOpConPrinter_CimkeHibasSerialNumber.Text;
            options.CimkeHibasDatum = tbOpConPrinter_CimkeHibasDatum.Text;
            options.CimkeHibasHiba = tbOpConPrinter_CimkeHibasHiba.Text;
            options.SaveOptions();
        }

        private void btnOpConPrinterCancel_Click(object sender, EventArgs e)
        {
            cbOpPrinterPrinters.Text = options.OpPrinter;
        }

        private void btnOVSzType_1_Click(object sender, EventArgs e)
        {
        //    technologia.cbtnOVSzType_1.SetPls = true;
         //   technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVSzType_2_Click(object sender, EventArgs e)
        {
         //   technologia.cbtnOVSzType_2.SetPls = true;
         //   technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVSzType_3_Click(object sender, EventArgs e)
        {
          //  technologia.cbtnOVSzType_3.SetPls = true;
          //  technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVMachinOn_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVMachinOn.SetPls = true;
        }


        private void etsmOpProductTypeParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpProductTypeParam);
            ActscSzenzorType = scSzenzorType_1;
            WriteDataGridViewFlag = true;
        }

        private void tsmOpProductType_1_Click(object sender, EventArgs e)
        {
            ActscSzenzorType = scSzenzorType_1;
            WriteDataGridViewFlag = true;
        }

        private void tsmOpProductType_2_Click(object sender, EventArgs e)
        {
            ActscSzenzorType = scSzenzorType_2;
            WriteDataGridViewFlag = true;
        }

        private void tsmOpProductType_3_Click(object sender, EventArgs e)
        {
            ActscSzenzorType = scSzenzorType_3;
            WriteDataGridViewFlag = true;
        }

        // type beállítások save gomb megnyomása
        private void btnOpProductTypeParamSave_Click(object sender, EventArgs e)
        {
            TypePartNumberrSave(ActSzenzorType);
            TypePartNumberrSelect(ActSzenzorType);
            OVSzenzorTypeSelect(ActualSzenzorTypeSelect);
            TypTextLoad();
        }
        void TypePartNumberrSave(int index)
        {
            if (tbOpProductTypeKIONPartNumber.Text.Length < 20 && tbOpProductTypeKIONPartNumber.Text.Length > 0)
            {
                if (tbOpProductTypeStabilPartNumber.Text.Length < 20 && tbOpProductTypeStabilPartNumber.Text.Length > 0)
                {
                    if (tbOpProductTypeStabilPartNumber.Text.Length < 20 && tbOpProductTypeStabilPartNumber.Text.Length > 0)
                    {
                        switch (index)
                        {
                            case scSzenzorType_1:
                                options.KIONPartNumber_1 = tbOpProductTypeKIONPartNumber.Text;
                                options.StabilPartNumber_1 = tbOpProductTypeStabilPartNumber.Text;
                                options.Type1Text = tbOpProductTypeName.Text;
                                break;
                            case scSzenzorType_2:
                                options.KIONPartNumber_2 = tbOpProductTypeKIONPartNumber.Text;
                                options.StabilPartNumber_2 = tbOpProductTypeStabilPartNumber.Text;
                                options.Type2Text = tbOpProductTypeName.Text;
                                break;
                            case scSzenzorType_3:
                                options.KIONPartNumber_3 = tbOpProductTypeKIONPartNumber.Text;
                                options.StabilPartNumber_3 = tbOpProductTypeStabilPartNumber.Text;
                                options.Type3Text = tbOpProductTypeName.Text;
                                break;
                            default:
                                break;
                        }
                        options.SaveOptions();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Tipus megnavazés nem megfelelő hosszú");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Stabil cikkszám nem megfelelő hosszú");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("KION cikkszám nem megfelelő hosszú");
            }

        }
        void TypePartNumberrSelect(int index)
        {

            switch (index)
            {
                case scSzenzorType_1:
                    tbOpProductTypeKIONPartNumber.Visible = true;
                    tbOpProductTypeStabilPartNumber.Visible = true;
                    lOPTypeKIONPartNumber.Visible = true;
                    lOPTypeStabilPartNumber.Visible = true;
                    tbOpProductTypeKIONPartNumber.Text = options.KIONPartNumber_1;
                    tbOpProductTypeStabilPartNumber.Text = options.StabilPartNumber_1;
                    tbOpProductTypeName.Text = options.Type1Text;
                    break;
                case scSzenzorType_2:
                    tbOpProductTypeKIONPartNumber.Visible = true;
                    tbOpProductTypeStabilPartNumber.Visible = true;
                    lOPTypeKIONPartNumber.Visible = true;
                    lOPTypeStabilPartNumber.Visible = true;
                    tbOpProductTypeKIONPartNumber.Text = options.KIONPartNumber_2;
                    tbOpProductTypeStabilPartNumber.Text = options.StabilPartNumber_2;
                    tbOpProductTypeName.Text = options.Type2Text;
                    break;
                case scSzenzorType_3:
                    tbOpProductTypeKIONPartNumber.Visible = true;
                    tbOpProductTypeStabilPartNumber.Visible = true;
                    lOPTypeKIONPartNumber.Visible = true;
                    lOPTypeStabilPartNumber.Visible = true;
                    tbOpProductTypeKIONPartNumber.Text = options.KIONPartNumber_3;
                    tbOpProductTypeStabilPartNumber.Text = options.StabilPartNumber_3;
                    tbOpProductTypeName.Text = options.Type3Text;
                    break;
                default:
                    tbOpProductTypeKIONPartNumber.Visible = false;
                    tbOpProductTypeStabilPartNumber.Visible = false;
                    lOPTypeKIONPartNumber.Visible = false;
                    lOPTypeStabilPartNumber.Visible = false;
                    break;
            }

            ActSzenzorType = index;

        }
        // type beállítások cancel gomb megnyomása
        private void btnOpProductTypeParamCancel_Click(object sender, EventArgs e)
        {
            OPParamSelect.WriteDataGridView();
            TypePartNumberrSelect(ActSzenzorType);
        }

        // A kiválasztott typ-okhoz tartozó értéket valamint a glogál paramétereket beletöltja a plc structurába


        public const int cKivantInduloNyomas = 1;
        public const int cNyomasFigyelesKesleltetes = 2;
        public const int cKivantNyomasMax = 3;
        public const int cKivantNyomasMin = 4;
        public const int cKivantFeszultsegMax = 5;
        public const int cKivantFeszultsegMin = 6;
        public const int cKivantAramFogyasztasMax = 7;
        public const int cKivantAramFogyasztasMin = 8;
        public const int cNyugalmiFogyasztasFigyelesKesleltetes = 9;
        public const int cNyugalmiAramFogyasztasMax = 10;
        public const int cNyugalmiAramFogyasztasMin = 11;
        void PLCPropertyDataMove()
        {
            foreach (DataRow row in ActParamType.dt.Rows)
            {
                switch (row[ClassSensorProperty.cD_id])
                {
                    case cKivantInduloNyomas:
                        technologia.plc.write.KivantInduloNyomas = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case cNyomasFigyelesKesleltetes:
                        technologia.plc.write.NyomasFigyelesKesleltetes = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case cKivantNyomasMax:
                        technologia.plc.write.KivantNyomasMax = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case cKivantNyomasMin:
                        technologia.plc.write.KivantNyomasMin = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
      
                    case cNyugalmiFogyasztasFigyelesKesleltetes:
                        technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        if (technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes > 5000)
                            technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes = 5000;
                        break;

                }
            }

            foreach (DataRow row in ProductType_Global.dt.Rows)
            {
                switch (row[ClassSensorProperty.cD_id])
                {
                    case 1:
                        technologia.plc.write.ProbatestPozicio_Alap = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 2:
                        technologia.plc.write.ProbatestVezKezdoPozicio = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 3:
                        technologia.plc.write.ProbatestVezVegPozicio = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 4:
                        technologia.plc.write.ProbatesLenyomvaDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 5:
                        technologia.plc.write.ProbatesLeszoritvaDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 6:
                        technologia.plc.write.ProbatestCloseFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 7:
                        technologia.plc.write.GripperOpenFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 8:
                        technologia.plc.write.GripperCloseFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 9:
                        technologia.plc.write.GripperMegfogDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 10:
                        technologia.plc.write.MeroTuskeOpenFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 11:
                        technologia.plc.write.MeroTuskeCloseFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 12:
                        technologia.plc.write.TermekFeltoltesFailureDelay = (UInt32)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                    case 13:
                        technologia.plc.write.ProbatestPozicioKompenzalas = (ushort)(Convert.ToDouble(row[ClassSensorProperty.cDefaultValue]) * Convert.ToInt32(row[ClassSensorProperty.cScale]));
                        break;
                }
            }
        }
        private void btnInterfaceService_Click(object sender, EventArgs e)
        {
            if (!technologia.cbtnInterfaceService.value)
            {
                technologia.cbtnInterfaceService.SetPls = true;
                technologia.cbtnOVStop.SetPls = true;
            }
            else
            {
                technologia.cbtnInterfaceService.ResetPls = true;
            }
        }

        private void btnOACK_Click(object sender, EventArgs e)
        {
            technologia.plc.ACK = true;
        }

        private void btnOVMachinOff_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVMachinOn.ResetPls = true;
            technologia.cbtnOVMachinOff.SetPls = true;
            technologia.cbtnOVStart.Enabled = false;
            technologia.cbtnOVStop.Enabled = false;
        }

        private void btnAlarmActivAck_Click(object sender, EventArgs e)
        {
            technologia.plc.ACK = true;
        }
        private void tsmEtalonModification_Click(object sender, EventArgs e)
        {
            FormEtalonProperty FEP = new FormEtalonProperty();
            FEP.Visible = true;
        }
        private void tsmEtalonSelect_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = sender as ToolStripItem;
            tsmEtalonSelect(tsmEtalon.Items.IndexOf(clickedItem));
        }
        int ClassRowsDiagnosticActive_S_id;


        //  ClassRowsDiagnostic CRD = new ClassRowsDiagnostic("DiagRows");
        void tsmEtalonSelect(int tsi_id)
        {
            dGVOpEtalonParam.Visible = true;
            btnEtalonModification.Visible = true;
            //  dGVOpMeasurementParam.Visible = false;
            bool megvan = false;
            dGVOpEtalonParam.Rows.Clear();
            dGVOpEtalonParam.Columns.Clear();
            foreach (ToolStripItem tsi in tsmEtalon.Items)
            {

                if (tsi_id == tsmEtalon.Items.IndexOf(tsi))
                {
                    ClassRowsDiagnosticActive_S_id = 0;
                    foreach (sTSI stsi in CD.lTSI_S_ID)
                    {
                        if (tsi_id == stsi.index)
                            ClassRowsDiagnosticActive_S_id = stsi.S_id;
                    }

                    megvan = true;
                    tsi.BackColor = Color.Gray;
                    tsi.ForeColor = Color.White;
                    CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id);
                    if (CD.CRDp != null)
                        CD.CRDp.WriteDataGridView();

                }
                else
                {
                    tsi.BackColor = Color.LightGray;
                    tsi.ForeColor = Color.Black;
                }
            }
            if (!megvan)
            {
                CD.GetClassRowsDiagnostic(-1);
                if (CD.CRDp != null)
                    CD.CRDp.WriteDataGridView();
            }

        }
        private void EtalonMenu_update()
        {

            CD.menuRefresh();
            tsmEtalon.Items.Clear();
            CD.lTSI_S_ID.Clear();
            foreach (DataRow row in CD.CMD.dt.Rows)
            {
                if ((int)row[ClassMenuDiagnostic.cUse] == FormEtalonProperty.scHasznal)
                {
                    sTSI tsi = new sTSI();
                    tsi.S_id = (int)row[ClassMenuDiagnostic.cS_id];
                    ToolStripItem subItemF = new ToolStripMenuItem(row[ClassMenuDiagnostic.cDescription].ToString());
                    subItemF.Click += new EventHandler(tsmEtalonSelect_Click);
                    tsmEtalon.Items.Add(subItemF);
                    tsi.index = tsmEtalon.Items.IndexOf(subItemF);
                    CD.lTSI_S_ID.Add(tsi);

                }
            }
        }


        private void btnKCounterClear_Click(object sender, EventArgs e)
        {
            ClassTech.CounterClear();
        }

        private void btnOpMachinParamSave_Click(object sender, EventArgs e)
        {
            ProductType_Global.SaveModify();
            ProductType_Global.Select();
            ProductType_Global.WriteDataGridView();

            PLCPropertyDataMove();
        }

        private void btnOpMachinParamCancel_Click(object sender, EventArgs e)
        {
            ProductType_Global.Select();
            ProductType_Global.WriteDataGridView();
        }

        private void btnOpEtalonParamSave_Click(object sender, EventArgs e)
        {
            CD.CRDp.SaveModify();
            CD.menuRefresh();
            CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id);
            CD.CRDp.WriteDataGridView();
        }

        private void btnOpEtalonParamCancel_Click(object sender, EventArgs e)
        {
            CD.menuRefresh();
            CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id);
            CD.CRDp.WriteDataGridView();
        }


        private void btnOVStart_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVStart.SetPls = true;
        }

        private void btnOVNext_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVNext.SetPls = true;
        }

        private void btnOVStop_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVStop.SetPls = true;
        }

        private void btnOVStepMode_Click(object sender, EventArgs e)
        {
            if (!technologia.cbtnOVStepMode.value)
                technologia.cbtnOVStepMode.SetPls = true;
            else
                technologia.cbtnOVStepMode.ResetPls = true;
        }

        private void btnOVAllTheWay_Click(object sender, EventArgs e)
        {
            if (technologia.cbtnOVAllTheWay.value)
            {
                if (!technologia.cbtnOVEtalon.value)
                    technologia.cbtnOVAllTheWay.ResetPls = true;
            }
            else
                technologia.cbtnOVAllTheWay.SetPls = true;
        }
        void btnServiceInterfaceOut(ClassButton CB)
        {
            if (technologia.plc.write.Service == true)
            {
                if (CB.value)
                    CB.ResetPls = true;
                else
                    CB.SetPls = true;
            }
        }
        private void btn_Out_Interface_1_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_1);
        }

        private void btn_Out_Interface_2_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_2);
        }

        private void btn_Out_Interface_3_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_3);
        }

        private void btn_Out_Interface_4_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_4);
        }

        private void btn_Out_Interface_5_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_5);
        }

        private void btn_Out_Interface_6_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_6);
        }

        private void btn_Out_Interface_7_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_7);
        }

        private void btn_Out_Interface_8_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_8);
        }

        private void btn_Out_Interface_9_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_9);
        }

        private void btn_Out_Interface_10_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_10);
        }

        private void btn_Out_Interface_11_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_11);
        }

        private void btn_Out_Interface_12_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_12);
        }

        private void btn_Out_Interface_13_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_13);
        }

        private void btn_Out_Interface_14_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_14);
        }

    
        private void tsmRestoration_Click(object sender, EventArgs e)
        {
            ScreenVisible(scSearch);
        }


    
        private void btnSearchFiler_Click(object sender, EventArgs e)
        {
             cSearch.btnSearchFiler_Click(sender, e, dTPSearchStartDate.Value, dTPSearchFinishDate.Value, tbSearchSerialNumber.Text);
        }

        

        private void dgvSearchMeasurementData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearchMeasurement.CurrentCell != null)
                if (dgvSearchMeasurement.CurrentCell.RowIndex >= 0)
                   cSearch.SearchMeasurementData(Convert.ToInt32(dgvSearchMeasurement.Rows[dgvSearchMeasurement.CurrentCell.RowIndex].Cells[0].Value));
        }
        


        private void btnSearchExport_Click(object sender, EventArgs e)
        {
            if (technologia.cbtnSearchExport.Enabled)
            {
                if (SaveForm.visible == false)
                {
                   
                    SaveF = new SaveForm();
                    SaveF.Visible = true;
                    SaveForm.visible = true;
                }
                this.Enabled = false;
                technologia.cbtnSearchFiler.Enabled = false;
                technologia.cbtnSearchExport.Enabled = false;
            }
        }
        void TypTextLoad()
        {
            btnOVSzType_1.Text = options.Type1Text;
            btnOVSzType_2.Text = options.Type2Text;
            btnOVSzType_3.Text = options.Type3Text;
            tsmOpProductType_1.Text = options.Type1Text;
            tsmOpProductType_2.Text = options.Type2Text;
            tsmOpProductType_3.Text = options.Type3Text;
        }


        private void dGVOpMeasurementParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            OPParamSelect.CellValueChanged(sender, e);
            OPParamSelect.CellEndEdit(e.RowIndex);
        }

        private void dGVOpEtalonParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CD.CRDp.CellValueChanged(sender, e);
            CD.CRDp.CellEndEdit(e.RowIndex);
        }


        private void dGVOpProductTypeParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            OPParamSelect.CellValueChanged(sender, e);
            OPParamSelect.CellEndEdit(e.RowIndex);
        }

        private void dGVOpMachineParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ProductType_Global.CellValueChanged(sender, e);
            DataGridViewRow row = ProductType_Global.DGV.Rows[e.RowIndex];
            if (Convert.ToInt32(row.Cells[ClassSensorProperty.cD_id].Value) > 99)
            {
                if (row.Cells[ClassSensorProperty.cDefaultValue].Value.ToString() == "Nem figyel")
                {
                    row.Cells[ClassSensorProperty.cDefaultValue].Value = 0;
                }
                else
                {
                    row.Cells[ClassSensorProperty.cDefaultValue].Value = 1;
                }
            }
            ProductType_Global.CellEndEdit(row);
            if (Convert.ToInt32(row.Cells[ClassSensorProperty.cD_id].Value) > 99)
            {
                if (row.Cells[ClassSensorProperty.cDefaultValue].Value.ToString() == "0")
                {
                    row.Cells[ClassSensorProperty.cDefaultValue].Value = "Nem figyel";
                }
                else
                {
                    row.Cells[ClassSensorProperty.cDefaultValue].Value = "Figyel";
                }
            }
        }
        Point? prevPosition = null;
        ToolTip tooltip = new ToolTip();
        private void chartAramMeres_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.Location;
                if (prevPosition.HasValue && pos == prevPosition.Value)
                    return;
                tooltip.RemoveAll();
                prevPosition = pos;
                var results = chartAramMeres.HitTest(pos.X, pos.Y, false,
                                                ChartElementType.DataPoint);
                foreach (var result in results)
                {
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        var prop = result.Object as DataPoint;
                        if (prop != null)
                        {
                            var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                            var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                            // check if the cursor is really close to the point (2 pixels around the point)
                            //                if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            //            Math.Abs(pos.Y - pointYPixel) < 2)
                            {
                                tooltip.Show("X=" + prop.AxisLabel + ", Y=" + prop.YValues[0], this.chartAramMeres,
                                                pos.X, pos.Y - 15);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void chartAramMeresNagy_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.Location;
                if (prevPosition.HasValue && pos == prevPosition.Value)
                    return;
                tooltip.RemoveAll();
                prevPosition = pos;
                var results = chartAramMeresNagy.HitTest(pos.X, pos.Y, false,
                                                ChartElementType.DataPoint);
                foreach (var result in results)
                {
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        var prop = result.Object as DataPoint;
                        if (prop != null)
                        {
                            var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                            var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                            // check if the cursor is really close to the point (2 pixels around the point)
                            //                if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            //            Math.Abs(pos.Y - pointYPixel) < 2)
                            {
                                tooltip.Show("X=" + prop.AxisLabel + ", Y=" + prop.YValues[0], this.chartAramMeresNagy,
                                                pos.X, pos.Y - 15);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void tsmMeresiAdatokToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OVDataPanelVisible(spMeresiAdatok);
        }

        private void tsmFutesGorbeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OVDataPanelVisible(spFutesGorbe);
        }
        private void tsmFeltoltes_Click(object sender, EventArgs e)
        {
            OVDataPanelVisible(spFeltoltes);
        }
        private void OVDataPanelVisible(int sp)
        {
            tsmColor(tsmMeresiAdatok, false);
            tsmColor(tsmFutesGorbe, false);
            tsmColor(tsmFeltoltes, false);
            switch (sp)
            {
                case spMeresiAdatok:
                    plOvAramMeresData.Visible = false;
                    plOvDataGridView.Visible = true;
                    plOvFeltoltes.Visible = false;
                    tsmKicsinyites.Visible = false;
                    tsmNagyitas.Visible = false;
                    tsmColor(tsmMeresiAdatok, true);
                    break;
                case spFutesGorbe:
                    plOvAramMeresData.Visible = true;
                    plOvDataGridView.Visible = false;
                    plOvFeltoltes.Visible = false;
                    tsmKicsinyites.Visible = true;
                    tsmNagyitas.Visible = true;
                    tsmColor(tsmFutesGorbe, true);
                    break;
                case spFeltoltes:
                
                    var yAxis = chart1.ChartAreas[0].AxisY;
                    var posYStart = 4.9;
                    var posYFinish = 5.05;
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                    plOvAramMeresData.Visible = false;
                    plOvDataGridView.Visible = false;
                    plOvFeltoltes.Visible = true;
                    tsmKicsinyites.Visible = true;
                    tsmNagyitas.Visible = true;
                    tsmColor(tsmFeltoltes, true);
                    break;
            }
        }

        private void tsmKicsinyites_Click(object sender, EventArgs e)
        {
            var chart = chartAramMeresNagy;
            if (plOvAramMeresData.Visible == true)
            {
                 chart = chartAramMeresNagy;
            }
            if (plOvFeltoltes.Visible == true)
            {
                 chart = chart1;
            }
            var xAxis = chart.ChartAreas[0].AxisX;
                var yAxis = chart.ChartAreas[0].AxisY;
                xAxis.ScaleView.ZoomReset();
                yAxis.ScaleView.ZoomReset();
                ZoomCounter = 0;
           
        }
        int ZoomCounter = 0;
        private void tsmNagyitas_Click(object sender, EventArgs e)
        {
            var chart = chartAramMeresNagy;
            if (plOvAramMeresData.Visible == true)
            {
                chart = chartAramMeresNagy;
            }
            if (plOvFeltoltes.Visible == true)
            {
                chart = chart1;
            }
            if (ZoomCounter < 3)
            {
                var xAxis = chart.ChartAreas[0].AxisX;
                var yAxis = chart.ChartAreas[0].AxisY;
                var xMin = xAxis.ScaleView.ViewMinimum;
                var xMax = xAxis.ScaleView.ViewMaximum;
                var yMin = yAxis.ScaleView.ViewMinimum;
                var yMax = yAxis.ScaleView.ViewMaximum;

                var posXStart = xAxis.PixelPositionToValue(chartAramMeresNagy.Location.X) - (xMax - xMin) / 1.5;
                var posXFinish = xAxis.PixelPositionToValue(chartAramMeresNagy.Location.X) + (xMax - xMin) / 1.5;
                var posYStart = yAxis.PixelPositionToValue(chartAramMeresNagy.Location.Y) - (yMax - yMin) / 1.5;
                var posYFinish = yAxis.PixelPositionToValue(chartAramMeresNagy.Location.Y) + (yMax - yMin) / 1.5;

                xAxis.ScaleView.Zoom(posXStart, posXFinish);
                yAxis.ScaleView.Zoom(posYStart, posYFinish);
                ZoomCounter++;
            }
        }

        void AddFeltoltesChart(double value)
        {
         
            chart1.Series[0].Points.AddXY( DateTime.Now.ToString("HH:mm:ss"),value);
        }
        private void dGVOpMachineParam_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.Location;
                if (prevPosition.HasValue && pos == prevPosition.Value)
                    return;
                tooltip.RemoveAll();
                prevPosition = pos;
                var results = chart1.HitTest(pos.X, pos.Y, false,
                                                ChartElementType.DataPoint);
                foreach (var result in results)
                {
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        var prop = result.Object as DataPoint;
                        if (prop != null)
                        {
                            var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                            var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                            // check if the cursor is really close to the point (2 pixels around the point)
                            //                if (Math.Abs(pos.X - pointXPixel) < 2 &&
                            //            Math.Abs(pos.Y - pointYPixel) < 2)
                            {
                                tooltip.Show("X=" + prop.AxisLabel + ", Y=" + prop.YValues[0], this.chart1,
                                                pos.X, pos.Y - 15);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

    // program beállításait tartalmazó struktura
    public struct sOptions
    {
        public string SQLDataSource;
        public string SQLInitialCatalog;
        public string SQLPersistSecurityInfo;
        public string SQLUserID;
        public string SQLPassword;
        public int PLC_IP_1;
        public int PLC_IP_2;
        public int PLC_IP_3;
        public int PLC_IP_4;
        public int PLC_Port;
        public bool OpMeasOpt_M1_Active;
        public bool OpMeasOpt_M2_Active;
        public bool OpMeasOpt_M3_Active;
        public string OpPrinter;
        public string EOL_Port;
        public string KIONPartNumber_1;
        public string StabilPartNumber_1;
        public string KIONPartNumber_2;
        public string StabilPartNumber_2;
        public string KIONPartNumber_3;
        public string StabilPartNumber_3;
        public int PC_id;
        public DateTime OpMeasOpt_M1;
        public DateTime OpMeasOpt_M2;
        public DateTime OpMeasOpt_M3;
        public DateTime OpMeasOpt_M_Last;
        public DateTime SaveStart;
        public DateTime SaveFinish;
        public string SearchSerialNumber;
        public string Type1Text;
        public string Type2Text;
        public string Type3Text;
        public int TypeIndex;
        public string CimkeYearDay;
        public string CimkeStabilPartNumber;
        public string CimkexyPartNumber;
        public string CimkeQR_xyPartNumber;
        public string CimkeQR_SerialNumber;
        public string CimkeQR_YYYYMMDD;
        public string CimkeHibasSerialNumber;
        public string CimkeHibasDatum;
        public string CimkeHibasHiba;

        // beállítások betöltése
        public void ReadOptions()
        {
            SQLDataSource = Settings.Default["SQLDataSource"].ToString();
            SQLInitialCatalog = Settings.Default["SQLInitialCatalog"].ToString();
            SQLUserID = Settings.Default["SQLUserID"].ToString();
            SQLPassword = Settings.Default["SQLPassword"].ToString();
            PLC_IP_1 = Convert.ToInt32(Settings.Default["PLC_IP_1"]);
            PLC_IP_2 = Convert.ToInt32(Settings.Default["PLC_IP_2"]);
            PLC_IP_3 = Convert.ToInt32(Settings.Default["PLC_IP_3"]);
            PLC_IP_4 = Convert.ToInt32(Settings.Default["PLC_IP_4"]);
            PLC_Port = Convert.ToInt32(Settings.Default["PLC_Port"]);

            OpMeasOpt_M1_Active = Convert.ToBoolean(Settings.Default["OpMeasOpt_M1_Active"]);
            OpMeasOpt_M2_Active = Convert.ToBoolean(Settings.Default["OpMeasOpt_M2_Active"]);
            OpMeasOpt_M3_Active = Convert.ToBoolean(Settings.Default["OpMeasOpt_M3_Active"]);
            OpPrinter = Settings.Default["OpPrinter"].ToString();
            EOL_Port = Settings.Default["EOL_Port"].ToString();
            KIONPartNumber_1 = Settings.Default["KIONPartNumber_1"].ToString();
            StabilPartNumber_1 = Settings.Default["StabilPartNumber_1"].ToString();
            KIONPartNumber_2 = Settings.Default["KIONPartNumber_2"].ToString();
            StabilPartNumber_2 = Settings.Default["StabilPartNumber_2"].ToString();
            KIONPartNumber_3 = Settings.Default["KIONPartNumber_3"].ToString();
            StabilPartNumber_3 = Settings.Default["StabilPartNumber_3"].ToString();
            PC_id = Convert.ToInt32(Settings.Default["PC_id"]);
            OpMeasOpt_M1 = Convert.ToDateTime(Settings.Default["OpMeasOpt_M1"]);
            OpMeasOpt_M2 = Convert.ToDateTime(Settings.Default["OpMeasOpt_M2"]);
            OpMeasOpt_M3 = Convert.ToDateTime(Settings.Default["OpMeasOpt_M3"]);
            OpMeasOpt_M_Last = Convert.ToDateTime(Settings.Default["OpMeasOpt_M_Last"]);
            SaveStart = Convert.ToDateTime(Settings.Default["SaveStart"]);
            SaveFinish = Convert.ToDateTime(Settings.Default["SaveFinish"]);
            SearchSerialNumber = Settings.Default["SearchSerialNumber"].ToString();
            Type1Text = Settings.Default["Type1Text"].ToString();
            Type2Text = Settings.Default["Type2Text"].ToString();
            Type3Text = Settings.Default["Type3Text"].ToString();
            TypeIndex = Convert.ToInt32(Settings.Default["TypeIndex"]);
            CimkeYearDay = Settings.Default["CimkeYearDay"].ToString();
            CimkeStabilPartNumber = Settings.Default["CimkeStabilPartNumber"].ToString();
            CimkexyPartNumber = Settings.Default["CimkexyPartNumber"].ToString();
            CimkeQR_xyPartNumber = Settings.Default["CimkeQR_xyPartNumber"].ToString();
            CimkeQR_SerialNumber = Settings.Default["CimkeQR_SerialNumber"].ToString();
            CimkeQR_YYYYMMDD = Settings.Default["CimkeQR_YYYYMMDD"].ToString();
            CimkeHibasSerialNumber = Settings.Default["CimkeHibasSerialNumber"].ToString();
            CimkeHibasDatum = Settings.Default["CimkeHibasDatum"].ToString();
            CimkeHibasHiba = Settings.Default["CimkeHibasHiba"].ToString();

        }

        // bállítások mentése
        public void SaveOptions()
        {
            SaveSettings("SQLDataSource", SQLDataSource);
            SaveSettings("SQLInitialCatalog", SQLInitialCatalog);
            SaveSettings("SQLUserID", SQLUserID);
            SaveSettings("SQLPassword", SQLPassword);

            SaveSettings("PLC_IP_1", PLC_IP_1);
            SaveSettings("PLC_IP_2", PLC_IP_2);
            SaveSettings("PLC_IP_3", PLC_IP_3);
            SaveSettings("PLC_IP_4", PLC_IP_4);
            SaveSettings("PLC_Port", PLC_Port);

            SaveSettings("OpMeasOpt_M1_Active", OpMeasOpt_M1_Active);
            SaveSettings("OpMeasOpt_M2_Active", OpMeasOpt_M2_Active);
            SaveSettings("OpMeasOpt_M3_Active", OpMeasOpt_M3_Active);

            SaveSettings("OpPrinter", OpPrinter);
            SaveSettings("EOL_Port", EOL_Port);

            SaveSettings("KIONPartNumber_1", KIONPartNumber_1);
            SaveSettings("StabilPartNumber_1", StabilPartNumber_1);
            SaveSettings("KIONPartNumber_2", KIONPartNumber_2);
            SaveSettings("StabilPartNumber_2", StabilPartNumber_2);
            SaveSettings("KIONPartNumber_3", KIONPartNumber_3);
            SaveSettings("StabilPartNumber_3", StabilPartNumber_3);

            SaveSettings("PC_id", PC_id);

            SaveSettings("OpMeasOpt_M1", OpMeasOpt_M1);
            SaveSettings("OpMeasOpt_M2", OpMeasOpt_M2);
            SaveSettings("OpMeasOpt_M3", OpMeasOpt_M3);
            SaveSettings("OpMeasOpt_M_Last", OpMeasOpt_M_Last);
            SaveSettings("SaveStart", SaveStart.ToString("yyyy.MM.dd HH:mm"));
            SaveSettings("SaveFinish", SaveFinish.ToString("yyyy.MM.dd HH:mm"));
            SaveSettings("SearchSerialNumber", SearchSerialNumber);

            SaveSettings("Type1Text", Type1Text);
            SaveSettings("Type2Text", Type2Text);
            SaveSettings("Type3Text", Type3Text);
            SaveSettings("TypeIndex", TypeIndex);

            SaveSettings("CimkeYearDay", CimkeYearDay);
            SaveSettings("CimkeStabilPartNumber", CimkeStabilPartNumber);
            SaveSettings("CimkexyPartNumber", CimkexyPartNumber);
            SaveSettings("CimkeQR_xyPartNumber", CimkeQR_xyPartNumber);
            SaveSettings("CimkeQR_SerialNumber", CimkeQR_SerialNumber);
            SaveSettings("CimkeQR_YYYYMMDD", CimkeQR_YYYYMMDD);

            SaveSettings("CimkeHibasSerialNumber", CimkeHibasSerialNumber);
            SaveSettings("CimkeHibasDatum", CimkeHibasDatum);
            SaveSettings("CimkeHibasHiba", CimkeHibasHiba);

            Settings.Default.Save();
        }
        // adot beállítás mentése és logolása
        private void SaveSettings(string setting, object variant)
        {
            object obj = Settings.Default[setting];
            if (obj.ToString() != variant.ToString())
            {
                ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + setting + ": " + Settings.Default[setting].ToString() + "  ->  " + variant.ToString());
                Settings.Default[setting] = variant;
            }
        }
    };
}