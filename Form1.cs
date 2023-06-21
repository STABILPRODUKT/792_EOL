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


namespace EOL_tesztelo
{
    public partial class Form1 : Form
    {
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

        public const int scSzenzorGlobal = 4;

        //Felhasználói képernyők konstans indexei
        public const int scUserMy = 1;
        public const int scUserNew = 2;
        public const int scUsers = 3;

        //Mérés képernyők konstans indexei
        public const int scOpMeasurement_NormalType_1 = 1;
        public const int scOpMeasurement_NormalType_2 = 2;
        public const int scOpMeasurement_NormalType_3 = 3;
        public const int scOpMeasurement_EtalonType_1 = 4;
        public const int scOpMeasurement_EtalonType_2 = 5;
        public const int scOpMeasurement_EtalonType_3 = 6;



        //beállítás képernyők konstans indexei
        public const int scOPSQL = 1;
        public const int scOPPrinter = 2;
        public const int scOPPLC = 3;
        public const int scOPEOL = 4;

        //felhasználó típusok
        public const int lsUserOperator = 1;
        public const int lsUserMaintenance = 2;
        public const int lsUserManager = 3;
        public const int lsUserAdmin = 4;

        public const string lsUserOperator_text = "Operátor";
        public const string lsUsermaintenance_text = "Karbantartó";
        public const string lsUserManager_text = "Minőségügyi";
        public const string lsUserAdmin_text = "Admin";


        public const int scElrejt = 1;
        //    public const int scMegjelenit = 2;
        public const int scHiba = 2;

        public const int scEtalonNemFigyel = 1;
        public const int scEtalonMegfelelo = 2;
        public const int scEtalonHibas = 3;

        //háttérszálak
        private Thread TScreenUpdate;
        private Thread TPrintEolData; //mérés megjelenítése
        private Thread TWriteExcell;
        private Thread TAutoWriteExcell;
        private Thread TSearch;
        private Thread TEolSensor;
        private Thread TTechnologia;
        private Thread TPLC;

        private string ActualMeasurementSelect_text;

        private int ActualSzenzorTypeSelect;

        int ActSzenzorType = 0;

        private classAlarm cAlarm;
        ClassTech technologia = new ClassTech();
        public static sOptions options = new sOptions();

        ClassSensorProperty OPParamSelect;   //OP ba használjuk
        ClassSensorProperty MeasurementParamSelect;
        ClassSensorProperty ParamNormalType_1;
        ClassSensorProperty ParamNormalType_2;
        ClassSensorProperty ParamNormalType_3;
        ClassSensorProperty ParamEtalonType_1;
        ClassSensorProperty ParamEtalonType_2;
        ClassSensorProperty ParamEtalonType_3;
        // ClassSensorProperty ParamOil;
        ClassSensorProperty ProductTypeSelect; // a választott 
        ClassSensorProperty ActProductType; // a menűbe 
        ClassSensorProperty ProductType_1;
        ClassSensorProperty ProductType_2;
        ClassSensorProperty ProductType_3;
        ClassSensorProperty ProductType_Global;
        ClassRowsDiagnostic CRDDef;

        ClassDiagnostic CD = new ClassDiagnostic();
        ClassUser user = new ClassUser();
        bool Flag_btnLogin = false;
        bool StatusFlag = false;
        int StatusDelay = 0;
        string StatusText = "";

        public Form1()
        {
            InitializeComponent();
            Settings.Default["PC_id"] = 1;
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
            //   if (Environment.MachineName == "DESKTOP-KISS-T-")
            {
                tbUserName.Text = "tibi";
                tbUserPassword.Text = "kacsa";
            }

            ScreenVisible(scLogin);
            TScreenUpdate = new Thread(ThreadScreenUpdate);
            TScreenUpdate.Start();
            TScreenUpdate.IsBackground = true;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Kijelentkezett: " + ClassUser.LoginUser.LoginName);
            TScreenUpdate.Abort();
            if (TPrintEolData != null)
                TPrintEolData.Abort();
            if (TEolSensor != null)
                TEolSensor.Abort();
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
            ParamNormalType_1 = new ClassSensorProperty("ParamNormalType1", "Normál típus 1");
            ParamNormalType_2 = new ClassSensorProperty("ParamNormalType2", "Normál típus 2");
            ParamNormalType_3 = new ClassSensorProperty("ParamNormalType3", "Normál típus 3");
            ParamEtalonType_1 = new ClassSensorProperty("ParamEtalonType1", "Etalon típus 1");
            ParamEtalonType_2 = new ClassSensorProperty("ParamEtalonType2", "Etalon típus 2");
            ParamEtalonType_3 = new ClassSensorProperty("ParamEtalonType3", "Etalon típus 3");
            ProductType_1 = new ClassSensorProperty("ParamType_1", "Típus1",true);
            ProductType_2 = new ClassSensorProperty("ParamType_2", "Típus2", true);
            ProductType_3 = new ClassSensorProperty("ParamType_3", "Típus3", true);
            cbEtalonTypeSelectStartValue();

            ProductType_Global = new ClassSensorProperty("ParamType_Global", "", true);
            CRDDef = new ClassRowsDiagnostic("DiagRowsDefault");
            CRDDef.SelectRowsDiagnosticActive(1, true);


            technologia.cbtnOVStart = new ClassButton(btnOVStart, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVStop = new ClassButton(btnOVStop, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVStepMode = new ClassButton(btnOVStepMode, Color.LightGreen, Color.LightGray, true, false);
            technologia.cbtnOVAllTheWay = new ClassButton(btnOVAllTheWay, Color.LightGreen, Color.LightGray, true, false);
            technologia.cbtnOVNext = new ClassButton(btnOVNext, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVMachinOn = new ClassButton(btnOVMachinOn, Color.LightGreen, Color.LightGray, false, false);
            technologia.cbtnOVMachinOff = new ClassButton(btnOVMachinOff, Color.LightGreen, Color.LightGray, true, true);

            technologia.cbtnOVNormal = new ClassButton(btnOVNormal, Color.LightGreen, Color.LightGray, ClassUser.LoginUser.Right >= lsUserManager, false, scMeasurementNormal);
            technologia.cbtnOVEtalon = new ClassButton(btnOVEtalon, Color.LightGreen, Color.LightGray, true, true, scMeasurementEtalon);

            technologia.cbtnOVSzType_1 = new ClassButton(btnOVSzType_1, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_1, scSzenzorType_1);
                technologia.cbtnOVSzType_2 = new ClassButton(btnOVSzType_2, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_2, scSzenzorType_2);
                technologia.cbtnOVSzType_3 = new ClassButton(btnOVSzType_3, Color.LightGreen, Color.LightGray, true, options.TypeIndex == scSzenzorType_3, scSzenzorType_3);

            technologia.cbtnInterfaceService = new ClassButton(btnInterfaceService, Color.LightGreen, Color.LightGray, false, false);

            technologia.cbtnFirstStartRecalibration = new ClassButton(btnFirstStartRecalibration, Color.LightGreen, Color.LightGray,true,options.FirstStartRecalibration);
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
            technologia.cbtn_Out_Interface_15 = new ClassButton(btn_Out_Interface_15, Color.LightGreen, Color.LightGray, false, false);

            OVSzenzorTypeSelect(options.TypeIndex);
            PLCPropertyDataMove(ProductType_Global);
            CD.menuRefresh();

            OVBtnMeasurementSelect(technologia.cbtnOVEtalon.Index);
            plOvDataGridView.Visible = true;

            cAlarm = new classAlarm();
            technologia.MachinOn = false;
            technologia.csEOL = new ClassSerial();

            if (TPrintEolData == null)
            {
                TPrintEolData = new Thread(ThreadPrintEolData);
                TPrintEolData.Start();
                TPrintEolData.IsBackground = true;
            }
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
            technologia.csEOL.PortName = Settings.Default["EOL_Port"].ToString();
            technologia.csEOL.connect();

            if (TEolSensor == null)
            {
                TEolSensor = new Thread(ThreadEolSensor);
                TEolSensor.IsBackground = true;
                TEolSensor.Start();
            }


            SearchDGVHead();
            TypTextLoad();
            //     CD.NextActEtalon(ActualSzenzorTypeSelect);
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
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;
                case scOpConnectionsParam:
                    if (right < lsUserAdmin)
                        rightCheck = false;
                    break;
                case scAlarmLog:
                    if (right < lsUserOperator)
                        rightCheck = false;
                    break;
                case scAlarmActive:
                    if (right < lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpEtalonCheckAlarmParam:
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;
                case scUsManagment:
                    if (right < lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpInterfaceParam:
                    if (right == lsUserManager || right == lsUserOperator)
                        rightCheck = false;
                    break;
                case scOpProductTypeParam:
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;
                case scOpEtalonParam:
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;
                case scOpMachinParam:
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;

                case scSearch:
                    if (right < lsUserManager)
                        rightCheck = false;
                    break;
            }
            return rightCheck;
        }

        //az adott indexű képernyő megjelenítése és menű színezése
        void ScreenVisible(int index)
        {
            if (UserRightCheck(index, ClassUser.LoginUser.Right)) // Fellhasználó jogosultság ellenőrzése
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

                switch (index)
                {
                    case scLogin:
                        plLogin.Visible = true;
                        plMainOverview.Visible = false;
                        technologia.MachinOn = false;
                        FullScreen(false);
                        break;
                    case scMainOverview:
                        plOverview.Visible = true;
                        plMainOverview.Visible = true;
                        tsmColor(tsmOverview, true);
                        break;
                    case scOverview:
                        plOverview.Visible = true;
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
                    if (technologia.cbtnOVSzType_1.value)
                        MeasurementParamSelect = ParamNormalType_1;
                    if (technologia.cbtnOVSzType_2.value)
                        MeasurementParamSelect = ParamNormalType_2;
                    if (technologia.cbtnOVSzType_3.value)
                        MeasurementParamSelect = ParamNormalType_3;
                    ActualMeasurementSelect_text = scMeasurementNormal_text;

                    break;
                case scMeasurementEtalon:
                    technologia.cbtnOVNormal.ResetPls = true;
                    CD.NextActEtalon(ActualSzenzorTypeSelect);
                    if (technologia.cbtnOVSzType_1.value)
                        MeasurementParamSelect = ParamEtalonType_1;
                    if (technologia.cbtnOVSzType_2.value)
                        MeasurementParamSelect = ParamEtalonType_2;
                    if (technologia.cbtnOVSzType_3.value)
                        MeasurementParamSelect = ParamEtalonType_3;
                    ActualMeasurementSelect_text = scMeasurementEtalon_text;

                    break;
            }

            //    WriteMeasurementDataGridViewFull = true;
        }

        void OVSzenzorTypeSelect(int index)
        {
            switch (index)
            {
                case scSzenzorType_1:
                    technologia.cbtnOVSzType_2.ResetPls = true;
                    technologia.cbtnOVSzType_3.ResetPls = true;
                    if (technologia.cbtnOVNormal.value)
                        MeasurementParamSelect = ParamNormalType_1;
                    if (technologia.cbtnOVEtalon.value)
                        MeasurementParamSelect = ParamEtalonType_1;

                    ProductTypeSelect = ProductType_1;
                    technologia.ActKIONPartNumber = options.KIONPartNumber_1;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_1;
                    break;
                case scSzenzorType_2:
                    technologia.cbtnOVSzType_1.ResetPls = true;
                    technologia.cbtnOVSzType_3.ResetPls = true;
                    if (technologia.cbtnOVNormal.value)
                        MeasurementParamSelect = ParamNormalType_2;
                    if (technologia.cbtnOVEtalon.value)
                        MeasurementParamSelect = ParamEtalonType_2;

                    ProductTypeSelect = ProductType_2;
                    technologia.ActKIONPartNumber = options.KIONPartNumber_2;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_2;
                    break;
                case scSzenzorType_3:
                    technologia.cbtnOVSzType_1.ResetPls = true;
                    technologia.cbtnOVSzType_2.ResetPls = true;
                    if (technologia.cbtnOVNormal.value)
                        MeasurementParamSelect = ParamNormalType_3;
                    if (technologia.cbtnOVEtalon.value)
                        MeasurementParamSelect = ParamEtalonType_3;

                    technologia.ActKIONPartNumber = options.KIONPartNumber_3;
                    technologia.ActStabilPartNumber = options.StabilPartNumber_3;
                    ProductTypeSelect = ProductType_3;
                    break;
            }
            ActualSzenzorTypeSelect = index;
            CD.ActS_id = -1;
            CD.NextActEtalon(ActualSzenzorTypeSelect);
            PLCPropertyDataMove(ProductTypeSelect);
        }


        byte[] rdata = new byte[ClassMeasurement.EOLDataSize];
        bool tPrint = false;
        int csomag = 0;
        ClassTimer AckTimer = new ClassTimer();

        public void ThreadPLC()
        {
            technologia.plc.IP = (Settings.Default["PLC_IP_1"].ToString() + "." + Settings.Default["PLC_IP_2"].ToString() + "." + Settings.Default["PLC_IP_3"].ToString() + "." + Settings.Default["PLC_IP_4"].ToString());

            while (true)
            {
                try
                {
                    if (ClassPLC.ConnectionLife == false)
                        technologia.plc.Con();
                    technologia.plc.write.Live = !technologia.plc.write.Live;
                    technologia.plc.Read();
                    Thread.Sleep(5);
                    technologia.plc.Write();
                }
                catch
                {
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
        public static List<cMeasurementExcell> lsME;
        public void ThreadSearch()
        {
            do
            {
                if (Search)
                {
                    lsME = ClassMeasurement.GetMeasurementExcell(Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()), Settings.Default["SearchSerialNumber"].ToString());
                    this.Invoke((MethodInvoker)delegate
                    {
                        int k;
                        int i = 0;
                        dgvSearchMeasurement.Rows.Clear();
                        foreach (cMeasurementExcell cME in lsME)
                        {
                            k = dgvSearchMeasurement.Rows.Add();
                            dgvSearchMeasurement.Rows[k].Cells[0].Value = cME.sM.S_id;
                            dgvSearchMeasurement.Rows[k].Cells[1].Value = cME.sM.Pc_id;
                            dgvSearchMeasurement.Rows[k].Cells[2].Value = cME.sM.LoginName;
                            dgvSearchMeasurement.Rows[k].Cells[3].Value = cME.sM.ModifyDate; ;
                            dgvSearchMeasurement.Rows[k].Cells[4].Value = cME.sM.KIONPartNumber;
                            dgvSearchMeasurement.Rows[k].Cells[5].Value = cME.sM.StabilPartNumber;
                            dgvSearchMeasurement.Rows[k].Cells[6].Value = cME.sM.SerialNumber;
                            dgvSearchMeasurement.Rows[k].Cells[7].Value = cME.sM.PRODDATE;
                            dgvSearchMeasurement.Rows[k].Cells[8].Value = cME.sM.HW_VER;
                            dgvSearchMeasurement.Rows[k].Cells[9].Value = cME.sM.MeasurementState;
                        }
                        technologia.cbtnSearchFiler.Enabled = true;
                        technologia.cbtnSearchExport.Enabled = true;
                        if (dgvSearchMeasurement.CurrentCell != null)
                            if (dgvSearchMeasurement.CurrentCell.RowIndex >= 0)
                                SearchMeasurementDataAndEEPROM(Convert.ToInt32(dgvSearchMeasurement.Rows[dgvSearchMeasurement.CurrentCell.RowIndex].Cells[0].Value));
                    });
                    Search = false;
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

                        ClassExcell ce = new ClassExcell();
                        ce.Open();
                   //    ce.Worksheets(1, "sheet1");
                        ce.WriteSeparateDay(lsME);
                        ce.Save(Settings.Default["SaveFolderBrowser"].ToString(), Settings.Default["SaveName"].ToString());
                        SaveForm.Save = false;
                        this.Invoke((MethodInvoker)delegate
                        {
                            technologia.cbtnSearchFiler.Enabled = false;
                            SaveF.Save_Finish();
                        });
                        this.Invoke((MethodInvoker)delegate
                        {

                            //az eol connect es képernyőn kirakjuk mogy mi volt a csomagban amit kaptunk a kupakból
                            lbOpEOLData.Items.Clear();
                            foreach (string str in ce.log)
                            {
                                lbOpEOLData.Items.Add(str);
                            }
                            technologia.cbtnSearchFiler.Enabled = true;
                        });
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
        public static List<cMeasurementExcell> lsAutoME;
        public void ThreadAutoWriteExcell()
        {
            do
            {
                //        try
                {
                    if (technologia.AutoSaveFlag)
                    {
                        lsAutoME = ClassMeasurement.GetMeasurementExcell(Convert.ToDateTime(Settings.Default["SaveStart"].ToString()), Convert.ToDateTime(Settings.Default["SaveFinish"].ToString()), Settings.Default["SearchSerialNumber"].ToString(), technologia.AutoSaveS_id);
                        if (lsAutoME.Count > 0)
                        {
                            ClassExcell ce = new ClassExcell();
                            ce.Open();
                           ce.Worksheets(1, "sheet1");
                            ce.Write(lsAutoME);
                            string path = @"C:\Log files\"+DateTime.Now.ToString("yyyy")+"\\KW" + CultureInfo.CurrentCulture.Calendar.GetWeekOfYear( DateTime.Now, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
                            System.IO.Directory.CreateDirectory(path);
                            ce.Save(path, DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HH_mm_ss") +"_"+ technologia.AutoSaveS_id);
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

        public void ThreadEolSensor()
        {
            int i = 0;
            byte strLast = 0;
            byte[] tdata = new byte[ClassMeasurement.EOLDataSize];
            do
            {
                try
                {
                    while (!technologia.csEOL.IsOpen())  // ha nem elérhető a sorosportos eszköz
                    {
                        technologia.csEOL.connect();  //megpróbálunk kapcsolodni
                        Thread.Sleep(30);
                        technologia.csEOL.ConnectionLife = false;
                        i = 0;
                    }
                    technologia.csEOL.ConnectionLife = true;
                    byte str1 = Convert.ToByte(technologia.csEOL.getData());
                    //Beolvasuk a következő adatot

                    if (strLast == 85 && str1 == 170) //kezdő bytok
                    {
                        if (i == ClassMeasurement.EOLDataSize)      //ha nem volt jó a hossz akkor nem volt jó a mérés
                        {
                            tdata[i - 1] = 0;       //85 leszedése a végéről
                            csomag++;

                            lock (rdata)
                            {
                                for (int j = 0; j < ClassMeasurement.EOLDataSize; j++)          //átrakjuk aza adatot bytonként
                                    rdata[j] = tdata[j];

                            }
                            tPrint = true;      //jelezzük a feldolgozó szálnak hogy van adat
                        }
                        tdata = new byte[ClassMeasurement.EOLDataSize];     //kigyepáljuk a tömbünket
                        i = 0;
                        tdata[i++] = strLast;               //85 megy az elejére
                        technologia.csEOL.ConnectionLife = true;
                        technologia.csEOL.Tulindexeles = false;
                    }
                    if (i == ClassMeasurement.EOLDataSize)          //ha beleírás előtt már elértük a tömb végét akkor ez egy első hibásan összeszedet csomag volt ezért újra kezdjük a gyüjtést
                    {
                        i = 0;
                        technologia.csEOL.ConnectionLife = false;
                        technologia.csEOL.Tulindexeles = true;
                        tdata = new byte[ClassMeasurement.EOLDataSize];
                    }
                    tdata[i++] = str1;      //beolvasott byt mentése 
                    strLast = str1;         //Last eltárolása
                }
                catch
                {
                }
            }
            while (true);
        }

        public void ThreadPrintEolData()
        {
            byte[] temprdata = new byte[ClassMeasurement.EOLDataSize];      //temp létrehozása
            do
            {
                if (tPrint == true)         //ha kaptunk jelet hogy van csomag akkor feldolgozzuk
                {
                    lock (rdata)
                    {
                        temprdata = rdata;      //lementjük 
                    }
                    technologia.cmEol.Set(temprdata);  //feldolgozzuk 
                    tPrint = false;                    //jöhet a következő csomag
                  
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            while (true);
        }
        private bool ButtonEnableUpdateFlag = false;
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
                        ClassUser.LoginUser.Right = lsUserAdmin;
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
                    ButtonEnableUpdateFlag = true;
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
                            technologia.cbtnOVAllTheWay.value = true;
                        technologia.cbtnOVAllTheWay.Printbtn();
                        technologia.cbtnOVNext.Printbtn();
                        technologia.cbtnOVMachinOn.Printbtn();

                        if (technologia.cbtnOVMachinOn.value)
                            technologia.cbtnOVMachinOff.value = false;

                        technologia.cbtnOVMachinOff.Printbtn();
                        if (technologia.cbtnOVMachinOff.value)
                            technologia.cbtnOVMachinOn.value = false;

                        technologia.cbtnFirstStartRecalibration.Printbtn();
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

                       

                        if (technologia.cbtnOVEtalon.value)
                            technologia.PrintEnabled = false;
                        else
                            technologia.PrintEnabled = true;

                        if (technologia.Meres) //ha meres van akkor ne lehsessen kattintani a typokat meg a méréstipust
                        {
                            if (!technologia.cbtnOVSzType_1.value)
                                technologia.cbtnOVSzType_1.Enabled = false;
                            if (!technologia.cbtnOVSzType_2.value)
                                technologia.cbtnOVSzType_2.Enabled = false;
                            if (!technologia.cbtnOVSzType_3.value)
                                technologia.cbtnOVSzType_3.Enabled = false;
                            if (!technologia.cbtnOVNormal.value)
                                technologia.cbtnOVNormal.Enabled = false;
                            if (!technologia.cbtnOVEtalon.value)
                                technologia.cbtnOVEtalon.Enabled = false;
                        }
                        if (technologia.MeresFinishFlag || ButtonEnableUpdateFlag == true)
                        {
                            ButtonEnableUpdateFlag = false;
                            technologia.MeresFinishFlag = false;
                            if (!technologia.cbtnOVSzType_1.value)
                                technologia.cbtnOVSzType_1.Enabled = true;
                            if (!technologia.cbtnOVSzType_2.value)
                                technologia.cbtnOVSzType_2.Enabled = true;
                            if (!technologia.cbtnOVSzType_3.value)
                                technologia.cbtnOVSzType_3.Enabled = true;
                            if (!technologia.cbtnOVNormal.value)
                                technologia.cbtnOVNormal.Enabled = ClassUser.LoginUser.Right >= lsUserManager;
                            if (!technologia.cbtnOVEtalon.value)
                                technologia.cbtnOVEtalon.Enabled = ClassUser.LoginUser.Right >= lsUserManager;
                        }

     

                        if (technologia.cbtnOVSzType_1.SetPls && technologia.cbtnOVSzType_1.Enabled)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_1.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_1.Index;
                            options.SaveOptions();
                        }
                        if (technologia.cbtnOVSzType_2.SetPls && technologia.cbtnOVSzType_2.Enabled)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_2.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_2.Index;
                            options.SaveOptions();
                        }
                        if (technologia.cbtnOVSzType_3.SetPls && technologia.cbtnOVSzType_3.Enabled)
                        {
                            OVSzenzorTypeSelect(technologia.cbtnOVSzType_3.Index);
                            technologia.cbtnOVEtalon.Enabled = true;
                            technologia.cbtnOVEtalon.SetPls = true;
                            technologia.cbtnOVNormal.ResetPls = true;
                            ButtonEnableUpdateFlag = true;
                            options.TypeIndex = technologia.cbtnOVSzType_3.Index;
                            options.SaveOptions();
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
                        technologia.cbtn_Out_Interface_15.Printbtn();


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

                        //Kapcsolás ellenőrzés eredmény kirakása
                        if (technologia.KapcsolasProbatestBeKapcsFlag)
                        {
                            technologia.KapcsolasProbatestBeKapcsFlag = false;
                            int[] data = new int[1];
                            if (technologia.KapcsolasProbatestBeKapcsAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 103)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }
                        //Kapcsolás ellenőrzés eredmény kirakása
                        if (technologia.KapcsolasProbatestKiKapcsFlag)
                        {
                            technologia.KapcsolasProbatestKiKapcsFlag = false;
                            int[] data = new int[1];
                            if (technologia.KapcsolasProbatestKiKapcsAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 105)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }
                        //Kapcsolás ellenőrzés eredmény kirakása
                        if (technologia.KapcsolasKiKapcsFlag)
                        {
                            technologia.KapcsolasKiKapcsFlag = false;
                            int[] data = new int[1];
                            if (technologia.KapcsolasKiKapcsAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 102)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }
                        // dátum ellenőrzése
                        if (technologia.EEPROMDataDateCheckFlag)
                        {
                            technologia.EEPROMDataDateCheckFlag = false;
                            int[] data = new int[1];
                            if (technologia.EEPROMDataDateCheck)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 111)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }
                        // EEPROM ellenőrzés
                        if (technologia.cmEol != null)
                            if (technologia.cmEol.EEPROMKeszFlag)
                            {
                                technologia.cmEol.EEPROMKeszFlag = false;
                                int[] data = new int[1];
                                if (!technologia.cmEol.EEPROMdataError)
                                    data[0] = 1;
                                else
                                    data[0] = 0;

                                foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                    if (sDRDef.D_id == 112)
                                    {
                                        if (technologia.cbtnOVEtalon.value)
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                        else
                                            WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                        break;
                                    }
                            }


                        //Kapcsolás pont ellenőrzés eredmény kirakása
                        if (technologia.KapcsoltPozMegvanFlag)
                        {
                            technologia.KapcsoltPozMegvanFlag = false;
                            int[] data = new int[1];
                            data[0] = technologia.KapcsoltPoz;
                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                            {
                                if (sDRDef.D_id == 101)
                                {
                                    foreach (sSensorProperty sp in ProductTypeSelect.lsSensorPropertiesActive)
                                    {
                                        switch (sp.D_id)
                                        {
                                            case 1:
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        //Verzíószám
                        if (technologia.EEPROMDataVersionCheckFlag)
                        {
                            technologia.EEPROMDataVersionCheckFlag = false;
                            int[] data = new int[1];
                            data[0] = technologia.cmEol.sEEPROMData.HW_VER;
                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                            {
                                if (sDRDef.D_id == 110)
                                {
                                    foreach (sSensorProperty sp in ProductTypeSelect.lsSensorPropertiesActive)
                                    {
                                        switch (sp.D_id)
                                        {
                                            case 2:
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }

                        //Tömítés ellenőrzés eredmény kirakása
                        if (technologia.TomitesFlag)
                        {
                            technologia.TomitesFlag = false;
                            int[] data = new int[1];
                            if (technologia.TomitesAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 104)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }

                        if (technologia.CimkeRagasztFlag)
                        {
                            lbOVCimkeRagaszt.Visible = true;
                        }
                        else
                            lbOVCimkeRagaszt.Visible = false;

                        //Cimke ellenőrzés eredmény kirakása
                        if (technologia.CimkeFlag)
                        {
                            technologia.CimkeFlag = false;
                            int[] data = new int[1];
                            if (technologia.CimkeAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 106)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }
                        //Membrán ellenőrzés eredmény kirakása
                        if (technologia.MembranFlag)
                        {
                            technologia.MembranFlag = false;
                            int[] data = new int[1];
                            if (technologia.MembranAllapot)
                                data[0] = 1;
                            else
                                data[0] = 0;

                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                                if (sDRDef.D_id == 107)
                                {
                                    if (technologia.cbtnOVEtalon.value)
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic());
                                    else
                                        WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef);
                                    break;
                                }
                        }


                        if (technologia.MegfeleloMeresVegeFlag == true)         // ha végeztünk az aktuális etalon méréssel
                        {
                            technologia.MegfeleloMeresVegeFlag = false;
                            if (technologia.cbtnOVEtalon.value)                 // még etalon mérésben vagyunk
                            {
                                CD.NextActEtalon(ActualSzenzorTypeSelect);      //lekérjük a kövi etalont
                            }
                        }


                        if (technologia.cmEol != null)
                            lbOVActCikkszam.Text = technologia.cmEol.sM.SerialNumber;

                        if (technologia.SikertelenVisszaAllitasFlag)
                        {
                            technologia.SikertelenVisszaAllitasFlag = false;
                            System.Windows.Forms.MessageBox.Show("Sikertelen visszaállítás! Zárlatok száma: "+technologia.ZarlatCounter+".");
                        }

                        /*       if (technologia.TapFeszCheckErrorFlag)
                               {
                                   technologia.TapFeszCheckErrorFlag = false;
                                   System.Windows.Forms.MessageBox.Show("A termék áramfelvétele nem megfelelő!");
                               }*/

                        //Áramfelvétel
                        if (technologia.TapFeszCheckFlag)
                        {

                            int[] data = new int[1];
                            data[0] = technologia.plc.read.EOL_aramfelvetel0_8V_mA;
                            foreach (ClassRowsDiagnostic.sDiagRow sDRDef in CRDDef.lsRowsDiagnosticActive)
                            {
                                if (sDRDef.D_id == 100)
                                {
                                    foreach (sSensorProperty sp in ProductTypeSelect.lsSensorPropertiesActive)
                                    {
                                        switch (sp.D_id)
                                        {
                                            case 3:
                                                if (technologia.cbtnOVEtalon.value)
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, CD.GetClassRowsDiagnostic(), (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                else
                                                    WriteMeasurementDataGridView(dgvOVMeasurementData, data, sDRDef, null, (ushort)((sp.DefaultValue - sp.hysteresisNeg) * sp.Scale), (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale));
                                                break;
                                        }
                                    }
                                    break;
                                }
                            }
                            technologia.TapFeszCheckFlag = false;
                        }
                        if ((DateTime.Now.TimeOfDay > options.OpMeasOpt_M1.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M1.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres) ||
                            (DateTime.Now.TimeOfDay > options.OpMeasOpt_M2.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M2.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres) ||
                            (DateTime.Now.TimeOfDay > options.OpMeasOpt_M3.TimeOfDay && options.OpMeasOpt_M_Last.AddMinutes(10).TimeOfDay < options.OpMeasOpt_M3.TimeOfDay && options.OpMeasOpt_M1_Active && !technologia.Meres))
                        {
                            if (!technologia.cbtnOVEtalon.value)
                            {
                                technologia.cbtnOVEtalon.Enabled = true;
                                technologia.cbtnOVEtalon.SetPls = true;
                                technologia.cbtnOVNormal.Enabled = ClassUser.LoginUser.Right >= lsUserManager;
                            }
                        }


                        if (cAlarm.plsAlarmReflesh)
                        {
                            WriteAlarmActiveGridView(dgvAlarmActive);
                            cAlarm.plsAlarmReflesh = false;
                        }

                        printbtnStatus(btnOSQLConnection, DataBase.ConnectionLife, Color.Green, Color.Red);

                        printbtnStatus(btnOEOLConnection, technologia.csEOL.ConnectionLife && !technologia.csEOL.Tulindexeles, Color.Green, Color.Red);

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
                            if (cbEtalonTypeSelect.Text != null)
                            {
                                EtalonMenu_update(cbEtalonTypeSelect.Text);
                                ClassMenuDiagnostic.bEtalonMenu_update = false;
                                if (tsmEtalon.Items.Count > 0)
                                {
                                    sTSI tsi = CD.lTSI_S_ID.First();
                                    tsmEtalonSelect(tsi.S_id);
                                }
                                else
                                    tsmEtalonSelect(-1);
                            }
                            /////////////////////////////////////////////////////////////////////////////////////nem tudom kell e 
                            //  OPMeasurementParamSelect(scOpMeasurement_Etalon);
                        }

                        if (CD.ActS_id == -1 && technologia.cbtnOVEtalon.value)
                        {
                            technologia.cbtnOVNormal.Enabled = true;
                            technologia.cbtnOVNormal.SetPls = true;
                            
                            technologia.cbtnOVEtalon.Enabled = ClassUser.LoginUser.Right >= lsUserManager;
                            technologia.cbtnOVEtalon.value = false;
                            options.OpMeasOpt_M_Last = DateTime.Now;
                            options.SaveOptions();
                        }

                        tbOVCounterBad.Text = ClassTech.Selejt.ToString();
                        tbOVCounterGood.Text = ClassTech.Megfelelo.ToString();

                        //    lblkCounterSum.Text = (ClassTech.Selejt + ClassTech.Megfelelo).ToString();
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
                    //   alarmSet(!technologia.csEOL.ConnectionLife, classAlarm.Error_Serial_connection);
                    //     alarmSet(technologia.csEOL.Tulindexeles, classAlarm.Error_SerialEOLTulindexeles);

                    alarmSet(!ClassPLC.ConnectionLife, classAlarm.Error_PLC_connection);
                    // alarmSet(!printerConnectionOk, classAlarm.Error_Printer_connection);

                    alarmSet(technologia.plc.read.Cooling_motor_protection_failure, classAlarm.Error_Cooling_motor_protection);
                    alarmSet(technologia.plc.read.Service_motor_protection_failure, classAlarm.Error_Service_motor_protection);
                    alarmSet(technologia.plc.read.Interface_system_motor_protection_failure, classAlarm.Error_Interface_system_motor_protection);
                    alarmSet(technologia.plc.read.DigitalOutMotorProtection_failure, classAlarm.Error_DigitalOutMotorProtection);
                    alarmSet(technologia.plc.read.Safety_motor_protection_failure, classAlarm.Error_Safety_motor_protection);
                    alarmSet(technologia.plc.read.E_stop_failure, classAlarm.Error_E_stop);
                    alarmSet(technologia.plc.read.Cabinet_temperature_failure, classAlarm.Error_Cabinet_temperature);
                    alarmSet(technologia.plc.read.Air_pressure_failure, classAlarm.Error_Air_pressure);
                    alarmSet(technologia.plc.read.Connection_failure, classAlarm.Error_Connection);
                    alarmSet(technologia.plc.read.MeroTuskaVegalasNyit_failure, classAlarm.Error_MeroTuskaVegalasNyit);
                    alarmSet(technologia.plc.read.MeroTuskaVegalasZar_failure, classAlarm.Error_MeroTuskaVegalasZar);
                    alarmSet(technologia.plc.read.ProbatestVegalasNyit_failure, classAlarm.Error_ProbatestVegalasNyit);
                    alarmSet(technologia.plc.read.ProbatestVegalasZar_failure, classAlarm.Error_ProbatestVegalasZar);
                    alarmSet(technologia.plc.read.Camera_failure, classAlarm.Error_Camera_failure);
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
            printReadInterfaceStatus(btn_In_Interface_1, technologia.plc.read.EOLKupakJelzes);
            printReadInterfaceStatus(btn_In_Interface_2, technologia.plc.read.LevegoelokeszitoNyomasKapcsolo);
            printReadInterfaceStatus(btn_In_Interface_3, technologia.plc.read.MerotuskePozicio_1);
            printReadInterfaceStatus(btn_In_Interface_4, technologia.plc.read.MerotuskePozicio_2);
            printReadInterfaceStatus(btn_In_Interface_5, technologia.plc.read.ProbatestPozicio_1);
            printReadInterfaceStatus(btn_In_Interface_6, technologia.plc.read.ProbatestPozicio_2);
            printReadInterfaceStatus(btn_In_Interface_7, technologia.plc.read.TermekRogzitve);
            printReadInterfaceStatus(btn_In_Interface_8, technologia.plc.read.SelesjtTaroloDarabszemlalo);
            printReadInterfaceStatus(btn_In_Interface_9, technologia.plc.read.TomitesJelenletErzekeloSenzor);
            printReadInterfaceStatus(btn_In_Interface_10, technologia.plc.read.UzemmodAllitoCsatlakozasHiba);
            printReadInterfaceStatus(btn_In_Interface_11, technologia.plc.read.UzemmodAllitoMozgatoPozicio_2);
            printReadInterfaceStatus(btn_In_Interface_12, technologia.plc.read.UzemmodAllitoMozgatoPozicio_1);
            printReadInterfaceStatus(btn_In_Interface_13, technologia.plc.read.CameraMunkadarabCheck);
            printReadInterfaceStatus(btn_In_Interface_14, technologia.plc.read.CameraCimkeCheck);
            printReadInterfaceStatus(btn_In_Interface_15, technologia.plc.read.CameraMembranCheck);

            tb_In_Interface_1.Text = technologia.plc.read.EOL_aramfelvetel0_8V_mA.ToString();
            tb_In_Interface_2.Text = technologia.plc.read.ProbatestMozgatoPozicio.ToString();
            tb_In_Interface_3.Text = technologia.plc.read.ProbatestKapcsolasiPozicio.ToString();

            technologia.cbtn_Out_Interface_1.Enabled = technologia.plc.read.LevegoElokeszitoVezereltLeeresztoSzelep_Enabled;
            technologia.cbtn_Out_Interface_2.Enabled = technologia.plc.read.MeroTuskeMozgatoSzelepNyit_Enabled;
            technologia.cbtn_Out_Interface_3.Enabled = technologia.plc.read.MeroTuskeMozgatoSzelepZar_Enabled;
            technologia.cbtn_Out_Interface_4.Enabled = technologia.plc.read.ProbatestMozgatoSzelepNyit_Enabled;
            technologia.cbtn_Out_Interface_5.Enabled = technologia.plc.read.ProbatestMozgatoSzelepZar_Enabled;
            technologia.cbtn_Out_Interface_6.Enabled = technologia.plc.read.EOLx_xV_Enabled;
            technologia.cbtn_Out_Interface_7.Enabled = technologia.plc.read.EOLx_xV_Enabled;
            technologia.cbtn_Out_Interface_8.Enabled = technologia.plc.read.EOLx_xV_Enabled;
            technologia.cbtn_Out_Interface_9.Enabled = technologia.plc.read.EOLx_xV_Enabled;
            technologia.cbtn_Out_Interface_10.Enabled = technologia.plc.read.EOL_tapfeszBekapcsolas_Enabled;
            technologia.cbtn_Out_Interface_11.Enabled = technologia.plc.read.UzemmodAllitoMozgatoSzelepNyit_Enabled;
            technologia.cbtn_Out_Interface_12.Enabled = technologia.plc.read.EOL_beallitasiFeszBekapcsolas_Enabled;
            technologia.cbtn_Out_Interface_13.Enabled = technologia.plc.read.CameraMunkadarabCheck_Enabled;
            technologia.cbtn_Out_Interface_14.Enabled = technologia.plc.read.CameraCimkeCheck_Enabled;
            technologia.cbtn_Out_Interface_15.Enabled = technologia.plc.read.CameraMembranCheck_Enabled;

            if (technologia.plc.write.Service == true && technologia.MachinOn)
            {
                technologia.plc.write.LevegoElokeszitoVezereltLeeresztoSzelep = technologia.cbtn_Out_Interface_1.value;
                technologia.plc.write.MeroTuskeMozgatoSzelepNyit = technologia.cbtn_Out_Interface_2.value;
                technologia.plc.write.MeroTuskeMozgatoSzelepZar = technologia.cbtn_Out_Interface_3.value;
                technologia.plc.write.ProbatestMozgatoSzelepNyit = technologia.cbtn_Out_Interface_4.value;
                technologia.plc.write.ProbatestMozgatoSzelepZar = technologia.cbtn_Out_Interface_5.value;
                technologia.plc.write.EOL0_0V = technologia.cbtn_Out_Interface_6.value;
                technologia.plc.write.EOL0_4V = technologia.cbtn_Out_Interface_7.value;
                technologia.plc.write.EOL1_0V = technologia.cbtn_Out_Interface_8.value;
                technologia.plc.write.EOL2_1V = technologia.cbtn_Out_Interface_9.value;
                technologia.plc.write.EOL_tapfeszBekapcsolas = technologia.cbtn_Out_Interface_10.value;
                technologia.plc.write.UzemmodAllitoMozgatoSzelepNyit = technologia.cbtn_Out_Interface_11.value;
                technologia.plc.write.EOL_beallitasiFeszBekapcsolas = technologia.cbtn_Out_Interface_12.value;
                technologia.plc.write.CameraMunkadarabCheck = technologia.cbtn_Out_Interface_13.value;
                technologia.plc.write.CameraCimkeCheck = technologia.cbtn_Out_Interface_14.value;
                technologia.plc.write.CameraMembranCheck = technologia.cbtn_Out_Interface_15.value;
            }
            else
            {
                technologia.cbtn_Out_Interface_1.value = technologia.plc.write.LevegoElokeszitoVezereltLeeresztoSzelep;
                technologia.cbtn_Out_Interface_2.value = technologia.plc.write.MeroTuskeMozgatoSzelepNyit;
                technologia.cbtn_Out_Interface_3.value = technologia.plc.write.MeroTuskeMozgatoSzelepZar;
                technologia.cbtn_Out_Interface_4.value = technologia.plc.write.ProbatestMozgatoSzelepNyit;
                technologia.cbtn_Out_Interface_5.value = technologia.plc.write.ProbatestMozgatoSzelepZar;
                technologia.cbtn_Out_Interface_6.value = technologia.plc.write.EOL0_0V;
                technologia.cbtn_Out_Interface_7.value = technologia.plc.write.EOL0_4V;
                technologia.cbtn_Out_Interface_8.value = technologia.plc.write.EOL1_0V;
                technologia.cbtn_Out_Interface_9.value = technologia.plc.write.EOL2_1V;
                technologia.cbtn_Out_Interface_10.value = technologia.plc.write.EOL_tapfeszBekapcsolas;
                technologia.cbtn_Out_Interface_11.value = technologia.plc.write.UzemmodAllitoMozgatoSzelepNyit;
                technologia.cbtn_Out_Interface_12.value = technologia.plc.write.EOL_beallitasiFeszBekapcsolas;
                technologia.cbtn_Out_Interface_13.value = technologia.plc.write.CameraMunkadarabCheck;
                technologia.cbtn_Out_Interface_14.value = technologia.plc.write.CameraCimkeCheck;
                technologia.cbtn_Out_Interface_15.value = technologia.plc.write.CameraMembranCheck;
            }
            printWriteInterfaceValue(tb_Out_Interface_1, technologia.plc.write.ProbatestMozgatoPozivio_Open);
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



        void WriteDataGridView(DataGridView DGV, ClassSensorProperty Param)
        {
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
 


            if (Param == ProductType_Global)
            {
                DGV.Columns[2].Visible = false;
                DGV.Columns[4].Visible = false;
                DGV.Columns[7].Visible = false;
                DGV.Columns[8].Visible = false;
            }
            else if (Param == ProductType_1 || Param == ProductType_2 || Param == ProductType_3)
            {
                DGV.Columns[2].Visible = true;
                DGV.Columns[4].Visible = true;
                DGV.Columns[7].Visible = false;
                DGV.Columns[8].Visible = false;
            }
            else
            {
                DGV.Columns[2].Visible = true;
                DGV.Columns[4].Visible = true;
                DGV.Columns[7].Visible = false;
                DGV.Columns[8].Visible = true;
            }





            if (Param.TableName == ParamEtalonType_1.TableName || Param.TableName == ParamEtalonType_2.TableName || Param.TableName == ParamEtalonType_3.TableName)
                DGV.Columns[8].Visible = false;
            DGV.Rows.Clear();
            foreach (sSensorProperty sSP in Param.lsSensorPropertiesActive)
            {
                int n = DGV.Rows.Add();
                DGV.Rows[n].Cells[0].Value = sSP.D_id;
                DGV.Rows[n].Cells[1].Value = sSP.Description;
                DGV.Rows[n].Cells[2].Value = sSP.hysteresisNeg;
                DGV.Rows[n].Cells[3].Value = sSP.DefaultValue;
                DGV.Rows[n].Cells[4].Value = sSP.hysteresisPoz;
                DGV.Rows[n].Cells[5].Value = sSP.S_id;
                DGV.Rows[n].Cells[6].Value = sSP.Pc_id;
                DGV.Rows[n].Cells[7].Value = sSP.Scale;
                DGV.Rows[n].Cells[8].Value = IntToStringVisibleProperty(sSP.Visible);
                DGV.Rows[n].Cells[9].Value = sSP.LoginName;
                DGV.Rows[n].Cells[10].Value = sSP.ModifyDate;
            }
        }

        private string IntToStringVisibleProperty(int index)
        {
            string ret = "Hiba";
            switch (index)
            {
                case scElrejt:
                    ret = "Elrejt";
                    break;
                // case scMegjelenit:
                //    ret = "Megjelenít";
                //      break;
                case scHiba:
                    ret = "Hiba";
                    break;
            }
            return ret;
        }
        private int StringToIntVisibleProperty(string str)
        {
            int ret = 1;
            switch (str)
            {
                case "Elrejt":
                    ret = scElrejt;
                    break;
                //     case "Megjelenít":
                //        ret = scMegjelenit;
                //       break;
                case "Hiba":
                    ret = scHiba;
                    break;
            }
            return ret;
        }
        public static bool WriteMeasurementDataGridViewFull = true;
        void WriteMeasurementDataGridView(DataGridView DGV, int[] data, List<sSensorProperty> Param, ClassRowsDiagnostic CRD = null)
        {
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[7].Visible = false;
            if (CRD == null)
            {
                foreach (sSensorProperty sSP in Param)
                {

                    sMeasurementData sMD = new sMeasurementData();
                    int k = -1;
                    sMD.Pc_id = sSP.Pc_id;
                    sMD.D_id = sSP.D_id;
                    sMD.Description = sSP.Description;
                    sMD.Visible = sSP.Visible;
                    sMD.Error = 0;
                    sMD.ToleranceNeg = sSP.DefaultValue - sSP.hysteresisNeg;
                    sMD.MeasurementValue = data[sSP.D_id + 2];
                    sMD.TolerancePoz = sSP.DefaultValue + sSP.hysteresisPoz;


                    //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét
                    if (sSP.Visible > scElrejt)
                        for (int i = 0; i < DGV.RowCount; i++)
                            if (sMD.D_id == (int)DGV.Rows[i].Cells[0].Value)
                                k = i;

                    if (k == -1)
                    {
                        if (sSP.Visible > scElrejt)
                        {
                            k = DGV.Rows.Add();

                            DGV.Rows[k].Cells[0].Value = sSP.D_id;
                            DGV.Rows[k].Cells[1].Value = sSP.Description;
                            DGV.Rows[k].Cells[2].Value = sSP.DefaultValue - sSP.hysteresisNeg;
                            DGV.Rows[k].Cells[4].Value = sSP.DefaultValue + sSP.hysteresisPoz;
                            DGV.Rows[k].Cells[5].Value = sSP.S_id;
                            DGV.Rows[k].Cells[6].Value = sSP.Pc_id;
                            DGV.Rows[k].Cells[7].Value = sSP.Scale;
                            DGV.Rows[k].Cells[8].Value = sSP.Visible;
                            DGV.Rows[k].Cells[9].Value = sSP.LoginName;
                        }
                    }
                    if (sSP.Visible > scElrejt)
                    {
                        DGV.Rows[k].Cells[3].Value = data[sSP.D_id + 2] / sSP.Scale;
                    }

                    if (sSP.Visible == scHiba)
                    {
                        if (sSP.DefaultValue - sSP.hysteresisNeg > data[sSP.D_id + 2] / sSP.Scale)
                        {
                            DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                            //                 CMD.SetError(sSP.D_id);
                            sMD.Error = sMD.Error + 1;
                        }
                        else
                            DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                        if (sSP.DefaultValue + sSP.hysteresisPoz < data[sSP.D_id + 2] / sSP.Scale)
                        {
                            DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                            //                  CMD.SetError(sSP.D_id);
                            sMD.Error = sMD.Error + 2;
                        }
                        else
                            DGV.Rows[k].Cells[4].Style.BackColor = Color.White;

                    }
                    technologia.cmEol.EOLDataSaveCounter++;
                    technologia.cmEol.SetMeasurementData(sMD);
                    /*    if (sSP.Visible == scMegjelenit)
                     {
                      DGV.Rows[k].Cells[2].Style.BackColor = Color.White;
                      DGV.Rows[k].Cells[2].Value = "-";
                      DGV.Rows[k].Cells[4].Value = "-";
                      DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                     }*/
                }


            }
            else
            {
                foreach (sSensorProperty sSP in Param)
                {
                    sMeasurementData sMD = new sMeasurementData();
                    int k = -1;
                    sMD.Pc_id = sSP.Pc_id;
                    sMD.D_id = sSP.D_id;
                    sMD.Description = sSP.Description;
                    sMD.Visible = sSP.Visible;
                    sMD.Error = 0;
                    sMD.ToleranceNeg = sSP.DefaultValue - sSP.hysteresisNeg;
                    sMD.MeasurementValue = data[sSP.D_id + 2];
                    sMD.TolerancePoz = sSP.DefaultValue + sSP.hysteresisPoz;
                    ClassRowsDiagnostic.sDiagRow ActsDR = new ClassRowsDiagnostic.sDiagRow();
                    foreach (ClassRowsDiagnostic.sDiagRow sDR in CRD.lsRowsDiagnosticActive)
                    {
                        if (sDR.D_id == sSP.D_id)
                        {
                            ActsDR = sDR;
                            break;
                        }
                    }



                    //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét
                    if (ActsDR.Visible > scEtalonNemFigyel)
                        for (int i = 0; i < DGV.RowCount; i++)
                            if (sMD.D_id == (int)DGV.Rows[i].Cells[0].Value)
                                k = i;

                    if (k == -1)
                    {
                        if (ActsDR.Visible > scEtalonNemFigyel)
                        {
                            k = DGV.Rows.Add();

                            DGV.Rows[k].Cells[0].Value = sSP.D_id;
                            DGV.Rows[k].Cells[1].Value = sSP.Description;
                            DGV.Rows[k].Cells[2].Value = sSP.DefaultValue - sSP.hysteresisNeg;
                            DGV.Rows[k].Cells[4].Value = sSP.DefaultValue + sSP.hysteresisPoz;
                            DGV.Rows[k].Cells[5].Value = sSP.S_id;
                            DGV.Rows[k].Cells[6].Value = sSP.Pc_id;
                            DGV.Rows[k].Cells[7].Value = sSP.Scale;
                            DGV.Rows[k].Cells[8].Value = sSP.Visible;
                            DGV.Rows[k].Cells[9].Value = sSP.LoginName;
                        }
                    }
                    if (ActsDR.Visible > scEtalonNemFigyel)
                    {
                        DGV.Rows[k].Cells[3].Value = data[sSP.D_id + 2] / sSP.Scale;
                    }

                    if (ActsDR.Visible == scEtalonMegfelelo)
                    {
                        if (sSP.DefaultValue - sSP.hysteresisNeg > data[sSP.D_id + 2] / sSP.Scale)
                        {
                            DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                            //                 CMD.SetError(sSP.D_id);
                            sMD.Error = sMD.Error + 1;
                        }
                        else
                            DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                        if (sSP.DefaultValue + sSP.hysteresisPoz < data[sSP.D_id + 2] / sSP.Scale)
                        {
                            DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                            //                  CMD.SetError(sSP.D_id);
                            sMD.Error = sMD.Error + 2;
                        }
                        else
                            DGV.Rows[k].Cells[4].Style.BackColor = Color.White;

                    }
                    if (ActsDR.Visible == scEtalonHibas)
                    {
                        bool NotGood = false;
                        if (sSP.DefaultValue - sSP.hysteresisNeg > data[sSP.D_id + 2] / sSP.Scale)
                        {
                            //     DGV.Rows[k].Cells[2].Style.BackColor = Color.Green;
                            //                 CMD.SetError(sSP.D_id);
                            NotGood = true;
                        }

                        if (sSP.DefaultValue + sSP.hysteresisPoz < data[sSP.D_id + 2] / sSP.Scale)
                        {
                            //    DGV.Rows[k].Cells[4].Style.BackColor = Color.Green;
                            //                  CMD.SetError(sSP.D_id);
                            NotGood = true;
                        }


                        if (NotGood)
                            DGV.Rows[k].Cells[3].Style.BackColor = Color.Green;
                        else
                        {
                            DGV.Rows[k].Cells[3].Style.BackColor = Color.Red;
                            sMD.Error = sMD.Error + 1;
                        }


                    }
                    technologia.cmEol.EOLDataSaveCounter++;
                    technologia.cmEol.SetMeasurementData(sMD);
                }
            }
            technologia.cmEol.Upload();
        }

        void WriteMeasurementDataGridView(DataGridView DGV, int[] data, ClassRowsDiagnostic.sDiagRow sDRDef, ClassRowsDiagnostic CRD = null, int min = 1, int max = 1)
        {
            //     CMD.updateMeasurementParamError(Param);
            //    CMD.updatePtoductTypeError(PtoductType);
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //     DGV.Columns[2].DefaultCellStyle.Format = "N2";
            //    DGV.Columns[3].DefaultCellStyle.Format = "N2";
            //    DGV.Columns[4].DefaultCellStyle.Format = "N2";
            //  List<sSensorProperty> lsSPA = Param.getSensorPropertiesActive();
            sMeasurementData sMD = new sMeasurementData();
            int k = -1;
            sMD.Pc_id = options.PC_id;
            sMD.D_id = sDRDef.D_id;
            sMD.Description = sDRDef.Description;
            sMD.Visible = sDRDef.Visible;
            sMD.Error = 0;
            //  sMD.ToleranceNeg = sSP.DefaultValue - sSP.hysteresisNeg;
            sMD.MeasurementValue = data[0];
            //  sMD.TolerancePoz = sSP.DefaultValue + sSP.hysteresisPoz;

            if (CRD == null)
            {


                //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét
                if (sDRDef.Visible != 2)
                    for (int i = 0; i < DGV.RowCount; i++)
                        if (sMD.D_id == (int)DGV.Rows[i].Cells[0].Value)
                            k = i;

                if (k == -1)
                {
                    if (sDRDef.Visible > scEtalonNemFigyel)
                    {
                        k = DGV.Rows.Add();

                        DGV.Rows[k].Cells[0].Value = sDRDef.D_id;
                        DGV.Rows[k].Cells[1].Value = sDRDef.Description;
                        DGV.Rows[k].Cells[2].Value = min;
                        DGV.Rows[k].Cells[4].Value = max;
                        DGV.Rows[k].Cells[5].Value = sDRDef.S_id;
                        DGV.Rows[k].Cells[6].Value = sMD.Pc_id;
                        DGV.Rows[k].Cells[7].Value = 1;
                        DGV.Rows[k].Cells[8].Value = sDRDef.Visible;
                        DGV.Rows[k].Cells[9].Value = sDRDef.LoginName;
                    }
                }

                if (sDRDef.Visible > scEtalonNemFigyel)  //"Nem figyel":
                {
                    DGV.Rows[k].Cells[3].Value = data[0];
                }

                if (sDRDef.Visible == scEtalonMegfelelo) //"Megfelelő":
                {
                    sMD.ToleranceNeg = min;
                    sMD.TolerancePoz = max;
                    if (data[0] < sMD.ToleranceNeg)
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        //                 CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 1;

                    }
                    else
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                    if (data[0] > sMD.TolerancePoz)
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        //                  CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 2;

                    }
                    else
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                }
                if (sDRDef.Visible == scEtalonHibas)  //"Hibás"
                {
                    sMD.ToleranceNeg = min;
                    sMD.TolerancePoz = max;
                    if (data[0] < sMD.ToleranceNeg)
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;
                    else
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        //                 CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 1;
                    }

                    if (data[0] > sMD.TolerancePoz)
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                    else
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        //                  CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 2;
                    }

                }
            }
            else
            {
                ClassRowsDiagnostic.sDiagRow ActsDR = new ClassRowsDiagnostic.sDiagRow();
                foreach (ClassRowsDiagnostic.sDiagRow sDR in CRD.lsRowsDiagnosticActive)
                {
                    if (sDR.D_id == sDRDef.D_id)
                    {
                        ActsDR = sDR;
                        break;
                    }
                }
                //ha meg kell jeleniteni akkor megkeresük a k-ba az adott sor indexét
                if (ActsDR.Visible > scEtalonNemFigyel)
                    for (int i = 0; i < DGV.RowCount; i++)
                        if (sMD.D_id == (int)DGV.Rows[i].Cells[0].Value)
                            k = i;

                if (k == -1)
                {
                    if (ActsDR.Visible > scEtalonNemFigyel)
                    {
                        k = DGV.Rows.Add();

                        DGV.Rows[k].Cells[0].Value = sDRDef.D_id;
                        DGV.Rows[k].Cells[1].Value = sDRDef.Description;
                        DGV.Rows[k].Cells[2].Value = "1";
                        DGV.Rows[k].Cells[4].Value = "1";
                        DGV.Rows[k].Cells[5].Value = sDRDef.S_id;
                        DGV.Rows[k].Cells[6].Value = sMD.Pc_id;
                        DGV.Rows[k].Cells[7].Value = 1;
                        DGV.Rows[k].Cells[8].Value = ActsDR.Visible;
                        DGV.Rows[k].Cells[9].Value = sDRDef.LoginName;
                    }
                }



                if (ActsDR.Visible > scEtalonNemFigyel)  //"Nem figyel":
                {
                    DGV.Rows[k].Cells[3].Value = data[0];
                }

                if (ActsDR.Visible == scEtalonMegfelelo) //megfelelő
                {
                    sMD.ToleranceNeg = min;
                    sMD.TolerancePoz = max;
                    if (data[0] < sMD.ToleranceNeg)
                    {
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.Red;
                        //                 CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 1;

                    }
                    else
                        DGV.Rows[k].Cells[2].Style.BackColor = Color.White;

                    if (data[0] > sMD.TolerancePoz)
                    {
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                        //                  CMD.SetError(sSP.D_id);
                        sMD.Error = sMD.Error + 2;

                    }
                    else
                        DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                }
                if (ActsDR.Visible == scEtalonHibas)   //hibás
                {
                    bool NotGood = false;
                    sMD.ToleranceNeg = min;
                    sMD.TolerancePoz = max;
                    if (data[0] < sMD.ToleranceNeg)
                    {
                        //    DGV.Rows[k].Cells[2].Style.BackColor = Color.White;
                        NotGood = true;
                    }
                    else
                    {
                        //       DGV.Rows[k].Cells[3].Style.BackColor = Color.Red;
                    }

                    if (data[0] > sMD.TolerancePoz)
                    {
                        //      DGV.Rows[k].Cells[4].Style.BackColor = Color.White;
                        NotGood = true;
                    }
                    else
                    {
                        //      DGV.Rows[k].Cells[4].Style.BackColor = Color.Red;
                    }

                    if (NotGood)
                        DGV.Rows[k].Cells[3].Style.BackColor = Color.Green;
                    else
                    {
                        DGV.Rows[k].Cells[3].Style.BackColor = Color.Red;
                        sMD.Error = sMD.Error + 1;
                    }
                }
            }
            technologia.cmEol.SetMeasurementData(sMD);
            technologia.cmEol.Upload();
        }


        private void dGVOpMeasurementParam_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Param_EditingControlShowing(e, dGVOpMeasurementParam);
        }
        private void dGVOpTypeParam_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Param_EditingControlShowing(e, dGVOpProductTypeParam);
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
        void ParamSave(DataGridView DGV, ClassSensorProperty CSP, bool Type)
        {
            bool error = false;
            List<sSensorProperty> lsSSP = new List<sSensorProperty>();
            try
            {
                for (int i = 0; i < DGV.Rows.Count; i++)
                {
                    sSensorProperty sSP = new sSensorProperty();
                    sSP.D_id = Convert.ToInt32(DGV.Rows[i].Cells[0].Value);
                    sSP.Description = Convert.ToString(DGV.Rows[i].Cells[1].Value);
                    sSP.hysteresisNeg = Convert.ToDouble(DGV.Rows[i].Cells[2].Value.ToString().Trim());
                    sSP.DefaultValue = Convert.ToDouble(DGV.Rows[i].Cells[3].Value.ToString().Trim());
                    sSP.hysteresisPoz = Convert.ToDouble(DGV.Rows[i].Cells[4].Value.ToString().Trim());
                    sSP.S_id = Convert.ToInt32(DGV.Rows[i].Cells[5].Value);
                    sSP.Pc_id = Convert.ToInt32(DGV.Rows[i].Cells[6].Value);
                    sSP.Scale = Convert.ToInt32(DGV.Rows[i].Cells[7].Value);
                    sSP.Visible = StringToIntVisibleProperty(DGV.Rows[i].Cells[8].Value.ToString());
                    sSP.LoginName = DGV.Rows[i].Cells[9].Value.ToString();
                    sSP.ModifyDate = Convert.ToDateTime(DGV.Rows[i].Cells[10].Value);
                    lsSSP.Add(sSP);
                }
                CSP.setSensorPropertiesModify(lsSSP);
            }
            catch
            {
                error = true;
            }
            if (!error)
            {
                if (!Type)
                    CSP.SensorPropertiesModifySave();
                else
                    CSP.SensorPropertiesModifySaveType();
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
            OPtParamSelect(scOPPrinter);
        }

        private void tsmOpPLC_Click(object sender, EventArgs e)
        {
            OPtParamSelect(scOPPLC);
        }

        private void tsmOpEOL_Click(object sender, EventArgs e)
        {
            cbOpEOLPorts.Items.Clear();
            foreach (string port in SerialPort.GetPortNames())
            {
                cbOpEOLPorts.Items.Add(port);
            }
            cbOpEOLPorts.Text = options.EOL_Port;
            OPtParamSelect(scOPEOL);
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
            WriteDataGridView(dGVOpMachineParam, ProductType_Global);
        }

        private void etsmOpEtalonParam_Click(object sender, EventArgs e)
        {
            EtalonMenu_update(cbEtalonTypeSelect.Text);

            int EtalonSelectName = -1;
            foreach (sDiagMenu sDM in CD.CMD.lsDiagnosticsMenuActive)
            {
                if (sDM.Use == FormEtalonProperty.scHasznal)
                {
                    EtalonSelectName = sDM.S_id;
                    break;
                }
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
            tsmOpEOL.BackColor = Color.LightGray;
            tsmOpEOL.ForeColor = Color.Black;
            plOpConSQL.Visible = false;
            plOpConPrinter.Visible = false;
            plOpConPLC.Visible = false;
            plOpConEOL.Visible = false;
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
                case scOPEOL:
                    tsmOpEOL.BackColor = Color.Gray;
                    tsmOpEOL.ForeColor = Color.White;
                    plOpConEOL.Visible = true;
                    break;
            }
        }
        void OPMeasurementParamSelect(int index)
        {
            tsmOpMeasurement_Normal.BackColor = Color.LightGray;
            tsmOpMeasurement_Normal.ForeColor = Color.Black;
            tsmOpMeasurement_Etalon.BackColor = Color.LightGray;
            tsmOpMeasurement_Etalon.ForeColor = Color.Black;
            tsmOpMeasurement_Type1.BackColor = Color.LightGray;
            tsmOpMeasurement_Type1.ForeColor = Color.Black;
            tsmOpMeasurement_Type2.BackColor = Color.LightGray;
            tsmOpMeasurement_Type2.ForeColor = Color.Black;
            tsmOpMeasurement_Type3.BackColor = Color.LightGray;
            tsmOpMeasurement_Type3.ForeColor = Color.Black;


            dGVOpEtalonParam.Visible = false;
            dGVOpMeasurementParam.Visible = true;

            switch (index)
            {
                case scOpMeasurement_NormalType_1:
                    tsmOpMeasurement_Normal.BackColor = Color.Gray;
                    tsmOpMeasurement_Normal.ForeColor = Color.White;
                    tsmOpMeasurement_Type1.BackColor = Color.Gray;
                    tsmOpMeasurement_Type1.ForeColor = Color.White;
                    OPParamSelect = ParamNormalType_1;
                    btnEtalonModification.Visible = false;
                    break;
                case scOpMeasurement_NormalType_2:
                    tsmOpMeasurement_Normal.BackColor = Color.Gray;
                    tsmOpMeasurement_Normal.ForeColor = Color.White;
                    tsmOpMeasurement_Type2.BackColor = Color.Gray;
                    tsmOpMeasurement_Type2.ForeColor = Color.White;
                    OPParamSelect = ParamNormalType_2;
                    btnEtalonModification.Visible = false;
                    break;
                case scOpMeasurement_NormalType_3:
                    tsmOpMeasurement_Normal.BackColor = Color.Gray;
                    tsmOpMeasurement_Normal.ForeColor = Color.White;
                    tsmOpMeasurement_Type3.BackColor = Color.Gray;
                    tsmOpMeasurement_Type3.ForeColor = Color.White;
                    OPParamSelect = ParamNormalType_3;
                    btnEtalonModification.Visible = false;
                    break;
                case scOpMeasurement_EtalonType_1:
                    tsmOpMeasurement_Etalon.BackColor = Color.Gray;
                    tsmOpMeasurement_Etalon.ForeColor = Color.White;
                    tsmOpMeasurement_Type1.BackColor = Color.Gray;
                    tsmOpMeasurement_Type1.ForeColor = Color.White;
                    btnEtalonModification.Visible = true;

                    OPParamSelect = ParamEtalonType_1;
                    break;
                case scOpMeasurement_EtalonType_2:
                    tsmOpMeasurement_Etalon.BackColor = Color.Gray;
                    tsmOpMeasurement_Etalon.ForeColor = Color.White;
                    tsmOpMeasurement_Type2.BackColor = Color.Gray;
                    tsmOpMeasurement_Type2.ForeColor = Color.White;
                    btnEtalonModification.Visible = true;

                    OPParamSelect = ParamEtalonType_2;
                    break;
                case scOpMeasurement_EtalonType_3:
                    tsmOpMeasurement_Etalon.BackColor = Color.Gray;
                    tsmOpMeasurement_Etalon.ForeColor = Color.White;
                    tsmOpMeasurement_Type3.BackColor = Color.Gray;
                    tsmOpMeasurement_Type3.ForeColor = Color.White;
                    btnEtalonModification.Visible = true;

                    OPParamSelect = ParamEtalonType_3;
                    break;
            }
            WriteDataGridView(dGVOpMeasurementParam, OPParamSelect);
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
                    ActProductType = ProductType_1;
                    break;
                case scSzenzorType_2:
                    tsmOpProductType_2.BackColor = Color.Gray;
                    tsmOpProductType_2.ForeColor = Color.White;
                    ActProductType = ProductType_2;
                    break;
                case scSzenzorType_3:
                    tsmOpProductType_3.BackColor = Color.Gray;
                    tsmOpProductType_3.ForeColor = Color.White;
                    ActProductType = ProductType_3;
                    break;
            }
            WriteDataGridView(dGVOpProductTypeParam, ActProductType);
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
            if (right < lsUserManager)
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

            WriteDataGridView(dGVOpMeasurementParam, OPParamSelect);
        }
        private void btnOpConSQLCancel_Click(object sender, EventArgs e)
        {
            OPParamSQLWrite();
        }
        private void btnOpMeasurementParamSave_Click(object sender, EventArgs e)
        {
            ParamSave(dGVOpMeasurementParam, OPParamSelect,false);
            ParamNormalType_1.SelectSensorPropertiesActive();
            ParamNormalType_2.SelectSensorPropertiesActive();
            ParamNormalType_3.SelectSensorPropertiesActive();
            ParamEtalonType_1.SelectSensorPropertiesActive();
            ParamEtalonType_2.SelectSensorPropertiesActive();
            ParamEtalonType_3.SelectSensorPropertiesActive();
            if (ParamNormalType_1.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamNormalType_1;
            if (ParamNormalType_2.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamNormalType_2;
            if (ParamNormalType_3.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamNormalType_3;
            if (ParamEtalonType_1.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamEtalonType_1;
            if (ParamEtalonType_2.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamEtalonType_2;
            if (ParamEtalonType_3.TableName == OPParamSelect.TableName)
                OPParamSelect = ParamEtalonType_3;
            WriteMeasurementDataGridViewFull = true;
            WriteDataGridView(dGVOpMeasurementParam, OPParamSelect);
        }

        void MeasuremenParamSave(DataGridView DGV, int S_id)
        {
            List<ClassRowsDiagnostic.sDiagRow> lsSSP = new List<ClassRowsDiagnostic.sDiagRow>();

            for (int i = 0; i < DGV.Rows.Count; i++)
            {
                ClassRowsDiagnostic.sDiagRow sDR = new ClassRowsDiagnostic.sDiagRow();
                sDR.S_id = Convert.ToInt32(DGV.Rows[i].Cells[0].Value);
                sDR.D_id = Convert.ToInt32(DGV.Rows[i].Cells[1].Value);
                sDR.Description = DGV.Rows[i].Cells[2].Value.ToString().Trim();
                sDR.Visible = Convert.ToInt32(StringToIntVisibleErrorValue(DGV.Rows[i].Cells[3].Value.ToString()));
                sDR.LoginName = DGV.Rows[i].Cells[4].Value.ToString().Trim();
                sDR.ModifyDate = Convert.ToDateTime(DGV.Rows[i].Cells[5].Value);
                lsSSP.Add(sDR);
            }
            CD.setSensorPropertiesModify(S_id, lsSSP);
        }

        private void btnOpMeasurementParamCancel_Click(object sender, EventArgs e)
        {
            WriteDataGridView(dGVOpMeasurementParam, OPParamSelect);
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
            cbUserNewRight.Items.Insert(lsUserOperator - 1, lsUserOperator_text);
            cbUserNewRight.Items.Insert(lsUserMaintenance - 1, lsUsermaintenance_text);
            cbUserNewRight.Items.Insert(lsUserManager - 1, lsUserManager_text);
            cbUserNewRight.Items.Insert(lsUserAdmin - 1, lsUserAdmin_text);
            UserManegmentSelect(scUserNew, ClassUser.LoginUser.Right);
        }

        private void tsmUsers_Click(object sender, EventArgs e)
        {
            cbUsersRight.Items.Clear();
            cbUsersRight.Items.Insert(lsUserOperator - 1, lsUserOperator_text);
            cbUsersRight.Items.Insert(lsUserMaintenance - 1, lsUsermaintenance_text);
            cbUsersRight.Items.Insert(lsUserManager - 1, lsUserManager_text);
            cbUsersRight.Items.Insert(lsUserAdmin - 1, lsUserAdmin_text);
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

        ClassButton StringtToClassButton(object a)
        {
            return (ClassButton)a;
        }
        SaveForm SaveF;


        private void btnOpPrinterTestPagePrint_Click(object sender, EventArgs e)
        {
            CimkePrint(technologia.ActKIONPartNumber, technologia.ActStabilPartNumber, "1234567", DateTime.Now);
        }
        public static void CimkePrint(string KION_Partumber, string STABIL_Partumber, string SerialNumber, DateTime date)
        {
            /*     string s = "^XA" +         //kezdete
        "^FO220,20" +            //kezdő pozició (mm,mm)
      //  "^BQ,2,5" +             //QR code méret
                                //   "^FDQA," + tbOpPrinterTestPageQRcode.Text +       //QR code
      "^BQ,2,2,H" +             //QR code méret
        "^FDQA," + "7917415116;1234567;727910;46/20" +       //QR code
        "^FS" +                 //aktuális parancs vége
        "^FO370,0" +            //kezdő pozició (mm,mm)
       // "^A0N30,30" +           //betű méret (mm,mm)
         "^A0N10,10" +           //betű méret (mm,mm)
        "^FD " + tbOpPrinterTestPageText.Text +    //ki írt szöveg
        "^FS" +                 //aktuális parancs vége
        "^FO370,40" +            //kezdő pozició (mm,mm)
        "^A0N25,25" +           //betű méret (mm,mm)
        "^FD " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") +    //ki írt szöveg
        "^FS" +                 //aktuális parancs vége
        "^XZ";                  //vége*/

            CultureInfo cul = CultureInfo.CurrentCulture;

            int week = cul.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            /*
            string s = "^XA" +         //kezdete
           "^JMB" +
            "^FO220,20" +            //kezdő pozició (mm,mm)                       
             "^A0N19,19" +           //betű méret (mm,mm)
            "^FD " + KION_Partumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^FO220,30" +            //kezdő pozició (mm,mm)    
            "^BQ,1,1,M" +             //QR code méret
            "^FDQA," + KION_Partumber + ";" + SerialNumber + ";" + STABIL_Partumber + ";" + week + "/" + date.ToString("yy") +       //QR code
            "^FS" +                 //aktuális parancs vége

            "^FO280,40" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + STABIL_Partumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

             "^FO280,60" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + week + "/" + date.ToString("yy") +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^FO280,80" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + SerialNumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^XZ";                  //vége
            */
            /*     int offset = 260;
                 int offset2 = offset + 60;
                 string s = "^XA" +         //kezdete
                "^JMA" +
                 "^FO"+ offset + ",19" +            //kezdő pozició (mm,mm)                       
                  "^A0N19,19" +           //betű méret (mm,mm)
                 "^FD " + KION_Partumber +    //ki írt szöveg
                 "^FS" +                 //aktuális parancs vége

                 "^FO" + offset + ",29" +            //kezdő pozició (mm,mm)    
                 "^BQ,2,2,M" +             //QR code méret
                 "^FDQA," + KION_Partumber + ";" + SerialNumber + ";" + STABIL_Partumber + ";" + week + "/" + date.ToString("yy") +       //QR code
                 "^FS" +                 //aktuális parancs vége

                 "^FO" + offset2  + ",39" +            //kezdő pozició (mm,mm)                       
                 "^A0N14,14" +           //betű méret (mm,mm)
                 "^FD " + STABIL_Partumber +    //ki írt szöveg
                 "^FS" +                 //aktuális parancs vége

                  "^FO" + offset2  + ",59" +            //kezdő pozició (mm,mm)                       
                 "^A0N14,14" +           //betű méret (mm,mm)
                 "^FD " + week + "/" + date.ToString("yy") +    //ki írt szöveg
                 "^FS" +                 //aktuális parancs vége

                 "^FO" + offset2  + ",79" +            //kezdő pozició (mm,mm)                       
                 "^A0N14,14" +           //betű méret (mm,mm)
                 "^FD " + SerialNumber +    //ki írt szöveg
                 "^FS" +                 //aktuális parancs vége

                 "^XZ";                  //vége
                 */

            int offset = 355;//260
            int offset2 = offset + 60;
            string s = "^XA" +         //kezdete
           "^JMA" +
            "^FO" + offset + ",19" +            //kezdő pozició (mm,mm)                       
             "^A0N19,19" +           //betű méret (mm,mm)
            "^FD " + KION_Partumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^FO" + offset + ",29" +            //kezdő pozició (mm,mm)    
            "^BQ,2,2,M" +             //QR code méret
            "^FDQA," + KION_Partumber + ";" + SerialNumber + ";" + STABIL_Partumber + ";" + week + "/" + date.ToString("yy") +       //QR code
            "^FS" +                 //aktuális parancs vége

            "^FO" + offset2 + ",39" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + STABIL_Partumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

             "^FO" + offset2 + ",54" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + week + "/" + date.ToString("yy") +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^FO" + offset2 + ",69" +            //kezdő pozició (mm,mm)                       
            "^A0N14,14" +           //betű méret (mm,mm)
            "^FD " + SerialNumber +    //ki írt szöveg
            "^FS" +                 //aktuális parancs vége

            "^XZ";                  //vége


            //        PrintDialog pd = new PrintDialog();               // felugro ablakos kivalasztas
            //       pd.PrinterSettings = new PrinterSettings();
            //     if (DialogResult.OK == pd.ShowDialog(this))
            {
                //   ClassRawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, s);
                //   ClassRawPrinterHelper.SendStringToPrinter("ZDesigner GC420t (EPL)", s);
                ClassRawPrinterHelper.SendStringToPrinter(options.OpPrinter, s);
            }
        }

        private void btnOpConPrinterSave_Click(object sender, EventArgs e)
        {
            options.OpPrinter = cbOpPrinterPrinters.Text;
            options.SaveOptions();
        }

        private void btnOpConPrinterCancel_Click(object sender, EventArgs e)
        {
            cbOpPrinterPrinters.Text = options.OpPrinter;
        }

        private void btnOpConEOLSave_Click(object sender, EventArgs e)
        {
            options.EOL_Port = cbOpEOLPorts.Text;
            options.SaveOptions();
            technologia.csEOL.DisConnet();
            technologia.csEOL.PortName = Settings.Default["EOL_Port"].ToString();
            technologia.csEOL.connect();
        }

        private void btnOpConEOLClose_Click(object sender, EventArgs e)
        {
            cbOpEOLPorts.Text = options.EOL_Port;
        }

        private void btnOVSzType_1_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVSzType_1.SetPls = true;
            technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVSzType_2_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVSzType_2.SetPls = true;
            technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVSzType_3_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVSzType_3.SetPls = true;
            technologia.cbtnOVEtalon.SetPls = true;
        }

        private void btnOVMachinOn_Click(object sender, EventArgs e)
        {
            technologia.cbtnOVMachinOn.SetPls = true;
        }

        private void btnOpEOLClear_Click(object sender, EventArgs e)
        {
            lbOpEOLData.Items.Clear();
        }

        private void etsmOpProductTypeParam_Click(object sender, EventArgs e)
        {
            ScreenVisible(scOpProductTypeParam);
            OPProductTypeSelect(scSzenzorType_1);
            TypePartNumberrSelect(scSzenzorType_1);
        }

        private void tsmOpProductType_1_Click(object sender, EventArgs e)
        {
            OPProductTypeSelect(scSzenzorType_1);
            TypePartNumberrSelect(scSzenzorType_1);
        }

        private void tsmOpProductType_2_Click(object sender, EventArgs e)
        {
            OPProductTypeSelect(scSzenzorType_2);
            TypePartNumberrSelect(scSzenzorType_2);
        }

        private void tsmOpProductType_3_Click(object sender, EventArgs e)
        {
            OPProductTypeSelect(scSzenzorType_3);
            TypePartNumberrSelect(scSzenzorType_3);
        }

        // type beállítások save gomb megnyomása
        private void btnOpProductTypeParamSave_Click(object sender, EventArgs e)
        {

            ParamSave(dGVOpProductTypeParam, ActProductType, true); // menti a dgv-ből az aktuálisan beállított type hoz tartozó értékeket
            TypePartNumberrSave(ActSzenzorType);
            ProductType_1.SelectSensorPropertiesActiveType(); // letölti az sql-ből a legfrissebb type beállításokat
            ProductType_2.SelectSensorPropertiesActiveType(); // letölti az sql-ből a legfrissebb type beállításokat
            ProductType_3.SelectSensorPropertiesActiveType(); // letölti az sql-ből a legfrissebb type beállításokat
                                                          //   ProductType_Global.SelectSensorPropertiesActive(); // letölti az sql-ből a legfrissebb type beállításokat
            WriteMeasurementDataGridViewFull = true;     // a méréshez trartozó szálnak adunk egy triggert hogy frissitse a megjelent mérést
            OPProductTypeSelect(ActSzenzorType); 
            //       WriteDataGridView(dGVOpTypeParam, ActProductType); // megjelenő dgv frissitése 
            //   SzenzorTypeSelect(ActualSzenzorTypeSelect);   // meghívjuk a szenzor type választó függvényt hogy frissüljön a plc-be leküldött beállítások
         
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
            WriteDataGridView(dGVOpProductTypeParam, ProductTypeSelect);
            TypePartNumberrSelect(ActSzenzorType);
        }

        // A kiválasztott typ-okhoz tartozó értéket valamint a glogál paramétereket beletöltja a plc structurába

        void PLCPropertyDataMove(ClassSensorProperty csp)
        {
            ProductType_1 = new ClassSensorProperty("ParamType_1", "Típus 1", true);
            ProductType_2 = new ClassSensorProperty("ParamType_2", "Típus 2", true);
            ProductType_3 = new ClassSensorProperty("ParamType_3", "Típus 3", true);
            ProductType_Global = new ClassSensorProperty("ParamType_Global", "", true);
            switch (csp.TableName)
            {
                case "ParamType_1":
                    foreach (sSensorProperty sp in ProductType_1.lsSensorPropertiesActive)
                    {
                        switch (sp.D_id)
                        {
                            case 1:
                                technologia.plc.write.ProbatestMozgatoPozivio_Open = (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale);
                                break;
                        }
                        technologia.plc.write.ProbatestMozgatoPozivio_Close = 13;
                    }
                    break;
                case "ParamType_2":
                    foreach (sSensorProperty sp in ProductType_2.lsSensorPropertiesActive)
                    {
                        switch (sp.D_id)
                        {
                            case 1:
                                technologia.plc.write.ProbatestMozgatoPozivio_Open = (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale);
                                break;
                        }
                        technologia.plc.write.ProbatestMozgatoPozivio_Close = 13;
                    }
                    break;
                case "ParamType_3":
                    foreach (sSensorProperty sp in ProductType_3.lsSensorPropertiesActive)
                    {
                        switch (sp.D_id)
                        {
                            case 1:
                                technologia.plc.write.ProbatestMozgatoPozivio_Open = (ushort)((sp.DefaultValue + sp.hysteresisPoz) * sp.Scale);
                                break;
                        }
                        technologia.plc.write.ProbatestMozgatoPozivio_Close = 13;
                    }
                    break;
                case "ParamType_Global":
                    foreach (sSensorProperty sp in ProductType_Global.lsSensorPropertiesActive)
                    {
                        switch (sp.D_id)
                        {
                            case 1:
                                technologia.plc.write.MeroTuskeOpenFailureDelay = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                            case 2:
                                technologia.plc.write.MeroTuskeCloseFailureDelay = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                            case 3:
                                technologia.plc.write.ProbatestOpenFailureDelay = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                            case 4:
                                technologia.plc.write.ProbatestCloseFailureDelay = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                            case 5:
                                technologia.plc.write.UzemmodAllitoMozgatoPozicio_Delay_Open = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                            case 6:
                                technologia.plc.write.UzemmodAllitoMozgatoPozicio_Delay_Close = (UInt32)(sp.DefaultValue * sp.Scale);
                                break;
                        }
                    }
                    break;
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
            dGVOpMeasurementParam.Visible = false;
            bool megvan = false;
            btnEtalonType_1Select.BackColor = Color.LightGray;
            btnEtalonType_2Select.BackColor = Color.LightGray;
            btnEtalonType_3Select.BackColor = Color.LightGray;

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

                    tsi.ForeColor = Color.White;

                    //     if (text != "Etalon")
                    WriteEtalonDataGridView(dGVOpEtalonParam, CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id));
                    foreach (sDiagMenu sDM in CD.CMD.lsDiagnosticsMenuActive)
                    {
                        if (sDM.S_id == ClassRowsDiagnosticActive_S_id)
                        {
                            if (sDM.Type_1_Selected)
                                btnEtalonType_1Select.BackColor = Color.Green;

                            if (sDM.Type_2_Selected)
                                btnEtalonType_2Select.BackColor = Color.Green;

                            if (sDM.Type_3_Selected)
                                btnEtalonType_3Select.BackColor = Color.Green;
                        }
                    }
                }
                else
                {
                    tsi.BackColor = Color.LightGray;
                    tsi.ForeColor = Color.Black;
                }
            }
            if (!megvan)
                WriteEtalonDataGridView(dGVOpEtalonParam, CD.GetClassRowsDiagnostic(-1));
        }
        private void EtalonMenu_update(string type)
        {
            CD.menuRefresh();
            tsmEtalon.Items.Clear();
            CD.lTSI_S_ID.Clear();
            //     ToolStripItem subItem = new ToolStripMenuItem("Etalon");
            //    subItem.Click += new EventHandler(tsmOpEtalon_Click);
            //    tsmEtalon.Items.Add(subItem);
            foreach (sDiagMenu sDM in CD.CMD.lsDiagnosticsMenuActive)
            {
                if (sDM.Use == FormEtalonProperty.scHasznal)
                {
                    sTSI tsi = new sTSI();
                    tsi.S_id = sDM.S_id;

                    if (type == options.Type1Text)
                    {
                        if (sDM.Type_1_Selected)
                        {
                            ToolStripItem subItemF = new ToolStripMenuItem(sDM.Description);
                            subItemF.Click += new EventHandler(tsmEtalonSelect_Click);
                            tsmEtalon.Items.Add(subItemF);
                            tsi.index = tsmEtalon.Items.IndexOf(subItemF);
                            CD.lTSI_S_ID.Add(tsi);
                        }
                    }
                    if (type == options.Type2Text)
                    {
                        if (sDM.Type_2_Selected)
                        {
                            ToolStripItem subItemF = new ToolStripMenuItem(sDM.Description);
                            subItemF.Click += new EventHandler(tsmEtalonSelect_Click);
                            tsmEtalon.Items.Add(subItemF);
                            tsi.index = tsmEtalon.Items.IndexOf(subItemF);
                            CD.lTSI_S_ID.Add(tsi);
                        }
                    }
                    if (type == options.Type3Text)
                    {
                        if (sDM.Type_3_Selected)
                        {
                            ToolStripItem subItemF = new ToolStripMenuItem(sDM.Description);
                            subItemF.Click += new EventHandler(tsmEtalonSelect_Click);
                            tsmEtalon.Items.Add(subItemF);
                            tsi.index = tsmEtalon.Items.IndexOf(subItemF);
                            CD.lTSI_S_ID.Add(tsi);
                        }
                    }
                }
            }
        }
    

        void WriteEtalonDataGridView(DataGridView DGV, ClassRowsDiagnostic Param)
        {
            DGV.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DGV.Rows.Clear();
            DGV.Columns[0].Visible = false;
            foreach (ClassRowsDiagnostic.sDiagRow sDR in Param.lsRowsDiagnosticActive)
            {
                int n = DGV.Rows.Add();
                DGV.Rows[n].Cells[0].Value = sDR.S_id;
                DGV.Rows[n].Cells[1].Value = sDR.D_id;
                DGV.Rows[n].Cells[2].Value = sDR.Description;
                // DGV.Rows[n].Cells[3].Value = sDR.ErrorValue;
                //       DGV.Rows[n].Cells[3].Value = IntToStringVisibleEtalonErrorValue(2);
                DGV.Rows[n].Cells[3].Value = IntToStringVisibleEtalonErrorValue(sDR.Visible);
                DGV.Rows[n].Cells[4].Value = sDR.LoginName;
                DGV.Rows[n].Cells[5].Value = sDR.ModifyDate;
            }
        }

        private string IntToStringVisibleEtalonErrorValue(int index)
        {
            string ret = "Nem figyel";
            switch (index)
            {
                case scEtalonNemFigyel:
                    ret = "Nem figyel";
                    break;
                case scEtalonMegfelelo:
                    ret = "Megfelelő";
                    break;
                case scEtalonHibas:
                    ret = "Hibás";
                    break;
                default:
                    ret = "Nem figyel";
                    break;
            }
            return ret;
        }

        private int StringToIntVisibleErrorValue(string str)
        {


            int ret = 1;
            switch (str)
            {
                case "Nem figyel":
                    ret = scEtalonNemFigyel;
                    break;

                case "Megfelelő":
                    ret = scEtalonMegfelelo;
                    break;
                case "Hibás":
                    ret = scEtalonHibas;
                    break;
            }
            return ret;
        }



        private void btnKCounterClear_Click(object sender, EventArgs e)
        {
            ClassTech.CounterClear();
        }

        private void btnOpMachinParamSave_Click(object sender, EventArgs e)
        {
            ParamSave(dGVOpMachineParam, ProductType_Global, true); // menti a dgv-ből az aktuálisan beállított type hoz tartozó értékeket

            ProductType_Global.SelectSensorPropertiesActiveType();//   ProductType_Global.SelectSensorPropertiesActive(); // letölti az sql-ből a legfrissebb type beállításokat
            PLCPropertyDataMove(ProductType_Global);
            WriteMeasurementDataGridViewFull = true;     // a méréshez trartozó szálnak adunk egy triggert hogy frissitse a megjelent mérést
            WriteDataGridView(dGVOpMachineParam, ProductType_Global); // megjelenő dgv frissitése         

        }

        private void btnOpMachinParamCancel_Click(object sender, EventArgs e)
        {
            WriteDataGridView(dGVOpMachineParam, ProductType_Global);
        }

        private void btnOpEtalonParamSave_Click(object sender, EventArgs e)
        {

            MeasuremenParamSave(dGVOpEtalonParam, ClassRowsDiagnosticActive_S_id);
            CD.menuRefresh();
            WriteEtalonDataGridView(dGVOpEtalonParam, CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id));
        }

        private void btnOpEtalonParamCancel_Click(object sender, EventArgs e)
        {
            WriteEtalonDataGridView(dGVOpEtalonParam, CD.GetClassRowsDiagnostic(ClassRowsDiagnosticActive_S_id));
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

        private void btn_Out_Interface_15_Click(object sender, EventArgs e)
        {
            btnServiceInterfaceOut(technologia.cbtn_Out_Interface_15);
        }

        private void tsmOpMeasurement_Type1_Click(object sender, EventArgs e)
        {
            if (OPParamSelect == ParamNormalType_1 || OPParamSelect == ParamNormalType_2 || OPParamSelect == ParamNormalType_3)
                OPMeasurementParamSelect(scOpMeasurement_NormalType_1);
            else
                OPMeasurementParamSelect(scOpMeasurement_EtalonType_1);
        }

        private void tsmOpMeasurement_Type2_Click(object sender, EventArgs e)
        {
            if (OPParamSelect == ParamNormalType_1 || OPParamSelect == ParamNormalType_2 || OPParamSelect == ParamNormalType_3)
                OPMeasurementParamSelect(scOpMeasurement_NormalType_2);
            else
                OPMeasurementParamSelect(scOpMeasurement_EtalonType_2);
        }

        private void tsmOpMeasurement_Type3_Click(object sender, EventArgs e)
        {
            if (OPParamSelect == ParamNormalType_1 || OPParamSelect == ParamNormalType_2 || OPParamSelect == ParamNormalType_3)
                OPMeasurementParamSelect(scOpMeasurement_NormalType_3);
            else
                OPMeasurementParamSelect(scOpMeasurement_EtalonType_3);
        }


        private void cbEtalonTypeSelectStartValue()
        {

            cbEtalonTypeSelect.Items.Clear();
            cbEtalonTypeSelect.Items.Add(options.Type1Text);
            cbEtalonTypeSelect.Items.Add(options.Type2Text);
            cbEtalonTypeSelect.Items.Add(options.Type3Text);
            cbEtalonTypeSelect.Text = options.Type1Text;
        }

        private void cbEtalonTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClassMenuDiagnostic.bEtalonMenu_update = true;
        }

        private void tsmRestoration_Click(object sender, EventArgs e)
        {
            ScreenVisible(scSearch);
        }

        private void btnFirstStartRecalibration_Click(object sender, EventArgs e)
        {
            if (!technologia.cbtnFirstStartRecalibration.value)
            {
                technologia.cbtnFirstStartRecalibration.SetPls = true;
                options.FirstStartRecalibration = true;
            }
            else
            {
                technologia.cbtnFirstStartRecalibration.ResetPls = true;
                options.FirstStartRecalibration = false;
            }
            options.SaveOptions();
        }

        bool Search = false;
        private void btnSearchFiler_Click(object sender, EventArgs e)
        {
            if (technologia.cbtnSearchFiler.Enabled)
            {
                DateTime tempSaveStart = dTPSearchStartDate.Value;
                DateTime tempSaveFinish = dTPSearchFinishDate.Value;

                if (tempSaveStart > tempSaveFinish)
                {
                    System.Windows.Forms.MessageBox.Show("Dátum nem megfelelő!");
                }
                else
                {

                    technologia.cbtnSearchFiler.Enabled = false;
                    technologia.cbtnSearchExport.Enabled = false;
                    options.SaveStart = tempSaveStart;
                    options.SaveFinish = tempSaveFinish;
                    options.SearchSerialNumber = tbSearchSerialNumber.Text;
                    dgvSearchMeasurementData.Rows.Clear();
                    dgvSearchEEPROM.Rows.Clear();
                    options.SaveOptions();
                    Search = true;
                }
            }
        }

        private void SearchDGVHead()
        {
            int i = 0;

            foreach (string head in sMeasurement.ToListHead())
            {
                dgvSearchMeasurement.Columns[i].HeaderText = head;
                dgvSearchMeasurement.Columns[i].ReadOnly = true;
                i++;
            }
            i = 0;
            dgvSearchMeasurementData.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            foreach (string head in sMeasurementData.ToListHead())
            {
                dgvSearchMeasurementData.Columns[i].HeaderText = head;
                dgvSearchMeasurementData.Columns[i].ReadOnly = true;
                i++;
            }
            i = 0;
            dgvSearchEEPROM.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            foreach (string head in sEEPROMRow.ToListHead())
            {
                dgvSearchEEPROM.Columns[i].HeaderText = head;
                dgvSearchEEPROM.Columns[i].ReadOnly = true;
                i++;
            }
        }

        private void dgvSearchMeasurementData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearchMeasurement.CurrentCell != null)
                if (dgvSearchMeasurement.CurrentCell.RowIndex >= 0)
                    SearchMeasurementDataAndEEPROM(Convert.ToInt32(dgvSearchMeasurement.Rows[dgvSearchMeasurement.CurrentCell.RowIndex].Cells[0].Value));
        }
        private void SearchMeasurementDataAndEEPROM(int S_id)
        {
            //    btnSearchFiler.Text = dgvSearchMeasurement.Rows[dgvSearchMeasurement.CurrentCell.RowIndex].Cells[0].Value.ToString();
            int k;
            int i = 0;

            dgvSearchMeasurementData.Rows.Clear();
            dgvSearchEEPROM.Rows.Clear();
            if (lsME != null)
            {
                foreach (cMeasurementExcell cME in lsME)
                {
                    if (cME.sM.S_id == S_id)
                    {
                        foreach (List<string> sCM in cME.lClassMeasurementData)
                        {
                            k = dgvSearchMeasurementData.Rows.Add();
                            i = 0;
                            foreach (string cell in sCM)
                            {

                                dgvSearchMeasurementData.Rows[k].Cells[i].Value = cell;
                                i++;
                            }

                        }
                        foreach (List<string> sE in cME.lEEPROMData)
                        {
                            k = dgvSearchEEPROM.Rows.Add();
                            i = 0;
                            foreach (string cell in sE)
                            {
                                dgvSearchEEPROM.Rows[k].Cells[i].Value = cell;
                                i += 2;
                            }
                        }
                        k = 0;
                        i = 1;
                        if (cME.lEEPROMData.Count != 0)
                        {
                            foreach (string item in sEEPROMData.ToListDescription())
                            {
                                dgvSearchEEPROM.Rows[k].Cells[i].Value = item;
                                k++;
                            }
                        }
                        break;
                    }
                }
            }
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
            lbOpEtalonParamType_1.Text = options.Type1Text;
            lbOpEtalonParamType_2.Text = options.Type2Text;
            lbOpEtalonParamType_3.Text = options.Type3Text;
            tsmOpMeasurement_Type1.Text = options.Type1Text;
            tsmOpMeasurement_Type2.Text = options.Type2Text;
            tsmOpMeasurement_Type3.Text = options.Type3Text;
            cbEtalonTypeSelectStartValue();
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
        public bool FirstStartRecalibration;
        public int TypeIndex;

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
            FirstStartRecalibration =Convert.ToBoolean( Settings.Default["FirstStartRecalibration"]);
            TypeIndex = Convert.ToInt32(Settings.Default["TypeIndex"]);
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
            SaveSettings("FirstStartRecalibration", FirstStartRecalibration);
            SaveSettings("TypeIndex", TypeIndex);
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
