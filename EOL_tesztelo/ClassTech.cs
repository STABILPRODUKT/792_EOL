using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using EOL_tesztelo.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace EOL_tesztelo
{
    public class ClassTech
    {
        public bool MachinOn;

        public bool updateStatus;
        public string machineStatus;
        public string userStaus;
        private int stepCounter;
        private int stepCounterLast = -1;
        public bool stepCounterReset = false;
        public static int Selejt;
        public static int Megfelelo;
        public bool Probatest_Alap = false;

        public bool MegfeleloFlag;


        public bool Hiba_NincsTermek = false;
        public bool Hiba_NincsAHelyenACsatlakozo = false;
        public bool TermekMagassagEllenorzesFlag = false;
        public bool MegfeleloMeresVegeFlag = false;
        public bool btnJoTermek;
        public bool btnRosszTermek;

        public bool TermekLeurultNyomasErtekFlag = false;
        public bool TermekFeltoltesHiba = false;
        public int TermekLeurultNyomasErtek;
        bool TermekLeurult = false;
        private bool MeresLogRun = false;
        public bool AramMeresEredmenyLekerdezesFlag = false;
        public bool AramErtekekEllenorzeseFlag = false;

        public bool ZarokupakEllenorzesFlag = false;

        public bool CameraAlsoFlag = false;
        public bool CameraFelsoFlag = false;

        public bool Meres = false;
        public bool MeresFinishFlag = false;
        public bool Megszakitva = false;

        public bool FeltoltesMeresFlag = false;
        public string ActKIONPartNumber;
        public string ActStabilPartNumber;
        public string ActTypeText;
        public string ActMeasurementType;

        public bool CimkePrintFlag = false;
        public bool PrintEnabled = false;

        public bool tZaroKupakElenorzesErentosGomb = false;
        public bool tempAutoSaveFlag = false;

        public bool AutoSaveFlag = false;
        public string AutoSaveS_id = "";
        List<string> MeasurementErrorList;
        public static List<CMeasurement> lsME;

        public ClassPLC plc = new ClassPLC();
        public ClassMeasurement cmEol;

        public ClassButton cbtnOVStart;
        public ClassButton cbtnOVStop;
        public ClassButton cbtnOVStepMode;
        public ClassButton cbtnOVAllTheWay;
        public ClassButton cbtnOVNext;
        public ClassButton cbtnOVMachinOn;
        public ClassButton cbtnOVMachinOff;
        public ClassButton cbtnOVNormal;
        public ClassButton cbtnOVEtalon;
        public ClassButton cbtnOVSzType_1;
        public ClassButton cbtnOVSzType_2;
        public ClassButton cbtnOVSzType_3;
        public ClassButton cbtnInterfaceService;
        public ClassButton cbtnSearchFiler;
        public ClassButton cbtnSearchExport;

        public ClassButton cbtn_Out_Interface_1;
        public ClassButton cbtn_Out_Interface_2;
        public ClassButton cbtn_Out_Interface_3;
        public ClassButton cbtn_Out_Interface_4;
        public ClassButton cbtn_Out_Interface_5;
        public ClassButton cbtn_Out_Interface_6;
        public ClassButton cbtn_Out_Interface_7;
        public ClassButton cbtn_Out_Interface_8;
        public ClassButton cbtn_Out_Interface_9;
        public ClassButton cbtn_Out_Interface_10;
        public ClassButton cbtn_Out_Interface_11;
        public ClassButton cbtn_Out_Interface_12;
        public ClassButton cbtn_Out_Interface_13;
        public ClassButton cbtn_Out_Interface_14;

        public ClassAramMeres cAramMeres;
        public ClassSerialNumber cSerialNumber;



        public const int machineStatusArraySize = 100;
        string[] machineStatusStepModeArray = new string[machineStatusArraySize];
        string[] machineStatusArray = new string[machineStatusArraySize];

        public const int userStatusArraySize = 100;
        string[] userStatusArray = new string[userStatusArraySize];

        public const int s_Hardver_Error = 0;
        public const int s_MachineOn_be = 1;
        public const int s_Service = 2;

        public const int s_AlapAllapot = 3;
        public const int s_AlapAllapotEllenorzes = 4;
        public const int s_Start = 5;
        public const int s_ProbaTest_Lenyom = 10;
        public const int s_ProbaTest_LenyomEnged = 11;
        public const int s_ProbaTest_Pozicioban = 12;
        public const int s_ProbaTest_Leszorit = 13;
        public const int s_ProbaTest_LeszoritasEllenorzes = 14;
        public const int s_TermekMagassagEllenorzes = 15;

        public const int s_InsertMeres = 16;
        public const int s_Gripper_Raszorit = 17;
        public const int s_Gripper_Ellenorzes = 18;
        public const int s_MeroTuske_Kinyom = 19;
        public const int s_MeroTuske_KinyomEllenorzes = 20;
        public const int s_CameraAlso = 25;
        public const int s_CameraAlso_Kiertekeles = 26;
        public const int s_CameraFelso = 27;
        public const int s_CameraFelso_Kiertekeles = 28;
        public const int s_TermekFeltoltes = 29;
        public const int s_TermekLeurules = 30;
        public const int s_TermekLeurules_Ellenorzes = 31;
        public const int s_AramMeresTesztelesInditasa = 32;
        public const int s_AramMeresEredmenyLekerdezes = 33;
        public const int s_AramMeresEredmenyEllenorzes = 34;
        public const int s_ProbaTest_Felenged = 35;
        public const int s_Cimkenyomtatas = 40;
        public const int s_Gombnyomas = 41;
        public const int s_MeroTuske_Behuz = 42;
        public const int s_Gripper_Elenged = 43;
        public const int s_ZaroKupakellenorzes = 44;
        public const int s_Megfelelo = 46;
        public const int s_SelejtTarolo = 47;

        public ClassTech()
        {
            //  cmEol = new ClassMeasurement();
            updateStatus = true;

            CounterRead();

            machineStatusArray[s_Hardver_Error] = "Hardver hiba";
            machineStatusArray[s_MachineOn_be] = "Beavatkozásra vár";
            machineStatusArray[s_Service] = "Beavatkozásra vár";
            machineStatusArray[s_Start] = "Beavatkozásra vár";
            machineStatusArray[s_AlapAllapot] = "Alap állapot";
            machineStatusArray[s_AlapAllapotEllenorzes] = "Alap állapot";
            machineStatusArray[s_ProbaTest_Lenyom] = "Beavatkozásra vár";
            machineStatusArray[s_ProbaTest_LenyomEnged] = "Beavatkozásra vár";
            machineStatusArray[s_ProbaTest_Pozicioban] = "Beavatkozásra vár";
            machineStatusArray[s_ProbaTest_Leszorit] = "Termék ellenőrzés";
            machineStatusArray[s_ProbaTest_LeszoritasEllenorzes] = "Termék ellenőrzés";
            machineStatusArray[s_TermekMagassagEllenorzes] = "Termék ellenőrzés";
            
            machineStatusArray[s_Gripper_Raszorit] = "Termék ellenőrzés";
            machineStatusArray[s_Gripper_Ellenorzes] = "Termék ellenőrzés";


            machineStatusArray[s_MeroTuske_Kinyom] = "Termék ellenőrzés";
            machineStatusArray[s_MeroTuske_KinyomEllenorzes] = "Termék ellenőrzés";
            machineStatusArray[s_CameraAlso] = "Termék ellenőrzés";
            machineStatusArray[s_CameraAlso_Kiertekeles] = "Termék ellenőrzés";
            machineStatusArray[s_CameraFelso] = "Termék ellenőrzés";
            machineStatusArray[s_CameraFelso_Kiertekeles] = "Termék ellenőrzés";
            machineStatusArray[s_TermekFeltoltes] = "Termék ellenőrzés";
            machineStatusArray[s_TermekLeurules] = "Termék ellenőrzés";
            machineStatusArray[s_TermekLeurules_Ellenorzes] = "Termék ellenőrzés";
            machineStatusArray[s_AramMeresTesztelesInditasa] = "Termék ellenőrzés";
            machineStatusArray[s_AramMeresEredmenyLekerdezes] = "Termék ellenőrzés";
            machineStatusArray[s_AramMeresEredmenyEllenorzes] = "Termék ellenőrzés";
            machineStatusArray[s_ProbaTest_Felenged] = "Termék ellenőrzés";
            machineStatusArray[s_Cimkenyomtatas] = "Termék ellenőrzés";
            machineStatusArray[s_Gombnyomas] = "Beavatkozásra vár";
            machineStatusArray[s_MeroTuske_Behuz] = "Termék ellenőrzés";
            machineStatusArray[s_Gripper_Elenged] = "Termék ellenőrzés";
            machineStatusArray[s_ZaroKupakellenorzes] = "Termék ellenőrzés";
            machineStatusArray[s_Megfelelo] = "Beavatkozásra vár";
            machineStatusArray[s_SelejtTarolo] = "Beavatkozásra vár";


            machineStatusStepModeArray[s_Hardver_Error] = "Hardver hiba";
            machineStatusStepModeArray[s_MachineOn_be] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_Service] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_Start] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_AlapAllapot] = "Alap állapot";
            machineStatusStepModeArray[s_AlapAllapotEllenorzes] = "Alap állapot ellenőrzése";
            machineStatusStepModeArray[s_ProbaTest_Lenyom] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_ProbaTest_LenyomEnged] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_ProbaTest_Pozicioban] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_ProbaTest_Leszorit] = "Termék rögzítve";
            machineStatusStepModeArray[s_ProbaTest_LeszoritasEllenorzes] = "Termék magasság ellenőrzése";
            machineStatusStepModeArray[s_TermekMagassagEllenorzes] = "Termék magasság ellenőrzése";
            
            machineStatusStepModeArray[s_Gripper_Raszorit] = "Csatlakozó rögzítése";
            machineStatusStepModeArray[s_Gripper_Ellenorzes] = "Csatlakozó rögzítés ellenőrzése";


            machineStatusStepModeArray[s_MeroTuske_Kinyom] = "Mérőtűske kinyomva";
            machineStatusStepModeArray[s_MeroTuske_KinyomEllenorzes] = "Mérőtűske elérte a végállást";
            machineStatusStepModeArray[s_CameraAlso] = "Kamera alsó fényképezés";
            machineStatusStepModeArray[s_CameraAlso_Kiertekeles] = "Kamera alsó kiértékelés";
            machineStatusStepModeArray[s_CameraFelso] = "Kamera felső fényképezés";
            machineStatusStepModeArray[s_CameraFelso_Kiertekeles] = "Kamera felső kiértékelés";
            machineStatusStepModeArray[s_TermekFeltoltes] = "Termék nyomásalá helyezve";
            machineStatusStepModeArray[s_TermekLeurules] = "Termék nyomásesés ellenőrzése";
            machineStatusStepModeArray[s_TermekLeurules_Ellenorzes] = "Termék nyomásesés ellenőrzve";
            machineStatusStepModeArray[s_AramMeresTesztelesInditasa] = "Árammérés vizsgálat elindítva";
            machineStatusStepModeArray[s_AramMeresEredmenyLekerdezes] = "Árammérés eredményének beolvasása";
            machineStatusStepModeArray[s_AramMeresEredmenyEllenorzes] = "Árammérés eredményének kiértékelése";
            machineStatusStepModeArray[s_ProbaTest_Felenged] = "Probatest felenged";
            machineStatusStepModeArray[s_Cimkenyomtatas] = "Cimke nyomtatás";
            machineStatusStepModeArray[s_Gombnyomas] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_MeroTuske_Behuz] = "Mérőtüske visszahúzva";
            machineStatusStepModeArray[s_Gripper_Elenged] = "Csatlakozó elengedve";
            machineStatusStepModeArray[s_ZaroKupakellenorzes] = "Zárókupak ellenőrizve";
            machineStatusStepModeArray[s_Megfelelo] = "Beavatkozásra vár";
            machineStatusStepModeArray[s_SelejtTarolo] = "Beavatkozásra vár";



            userStatusArray[s_Hardver_Error] = "Nyugtázza a hibákat!";
            userStatusArray[s_MachineOn_be] = "Nyomja meg a \"Bekapcsol\" gombot!";
            userStatusArray[s_Start] = "Nyomja meg az \"Indít\" gombot!";
            userStatusArray[s_Service] = "Karbantartás";
            userStatusArray[s_AlapAllapot] = "";
            userStatusArray[s_AlapAllapotEllenorzes] = "";
            userStatusArray[s_ProbaTest_Lenyom] = "Helyezzen terméket a gépbe és rögzítse!";
            userStatusArray[s_ProbaTest_LenyomEnged] = "Folytassa a rögzítést!";
            userStatusArray[s_ProbaTest_Pozicioban] = "Folytassa a rögzítést!";
            userStatusArray[s_ProbaTest_Leszorit] = "";
            userStatusArray[s_ProbaTest_LeszoritasEllenorzes] = "";
            userStatusArray[s_TermekMagassagEllenorzes] = "";
            
            userStatusArray[s_Gripper_Raszorit] = "";
            userStatusArray[s_Gripper_Ellenorzes] = "";
            userStatusArray[s_MeroTuske_Kinyom] = "";
            userStatusArray[s_MeroTuske_KinyomEllenorzes] = "";
            userStatusArray[s_CameraAlso] = "";
            userStatusArray[s_CameraAlso_Kiertekeles] = "";
            userStatusArray[s_CameraFelso] = "";
            userStatusArray[s_CameraFelso_Kiertekeles] = "";
            userStatusArray[s_TermekFeltoltes] = "";
            userStatusArray[s_TermekLeurules] = "";
            userStatusArray[s_TermekLeurules_Ellenorzes] = "";
            userStatusArray[s_AramMeresTesztelesInditasa] = "";
            userStatusArray[s_AramMeresEredmenyLekerdezes] = "";
            userStatusArray[s_AramMeresEredmenyEllenorzes] = "";
            userStatusArray[s_ProbaTest_Felenged] = "";
            userStatusArray[s_Cimkenyomtatas] = "";
            userStatusArray[s_Gombnyomas] = "Kupak felhelyezése után nyomja meg a gombot!";
   
            userStatusArray[s_MeroTuske_Behuz] = "";
            userStatusArray[s_Gripper_Elenged] = "";
            userStatusArray[s_ZaroKupakellenorzes] = "";
            userStatusArray[s_MeroTuske_Behuz] = "";
            userStatusArray[s_Megfelelo] = "Termék megfelelő";
            userStatusArray[s_SelejtTarolo] = "Helyezze a terméket a selejt tárolóba";
        }
        public static void CounterRead()
        {
            Selejt = Convert.ToInt32(Settings.Default["darabSzamlaloSelejt"]);
            Megfelelo = Convert.ToInt32(Settings.Default["darabSzamlaloMegfelelo"]);
        }
        public static void CounterClear()
        {
            SaveSettings("darabSzamlaloSelejt", 0);
            SaveSettings("darabSzamlaloMegfelelo", 0);
            CounterRead();
        }
        private static void SaveSettings(string setting, object variant)
        {
            object obj = Settings.Default[setting];
            if (obj.ToString() != variant.ToString())
            {
                ClassMessage.SetMessage(ClassUser.LoginUser.LoginName, "Módosult a(z) " + setting + ": " + Settings.Default[setting].ToString() + "  ->  " + variant.ToString());
                Settings.Default[setting] = variant;
            }
            Settings.Default.Save();
        }
        //   private bool MachinOnFirstFlag = false;

        public void Tech()
        {

            MachinOn = cbtnOVMachinOn.value;
            plc.write.Machine_on = MachinOn;
            if (plc.read.Interface_system_motor_protection_failure || plc.read.DigitalOutMotorProtection_failure || plc.read.Safety_motor_protection_failure
                || plc.read.E_stop_failure || plc.read.Cabinet_temperature_failure || plc.read.Air_pressure_failure || plc.read.Connection_failure || plc.read.MeroTuskaVegalasNyit_failure
                    || plc.read.MeroTuskaVegalasZar_failure || plc.read.ProbatestVegalasNyit_failure || plc.read.ProbatestVegalasZar_failure || plc.read.Camera_also_failure
                    || plc.read.Camera_felso_failure ||
                    plc.read.GripperNyit_failure || plc.read.GrippeZar_failure || plc.read.TeszteloFeszultsegKismegszakito_failure|| !ClassPLC.ConnectionLife)
            {
                setStatus(s_Hardver_Error);
                ControlOff();
                //    MachinOnFirstFlag = false;
                cbtnOVMachinOn.Enabled = false;
                cbtnOVMachinOff.SetPls = true;
                cbtnOVStart.Enabled = false;
                cbtnOVStop.Enabled = false;
                cbtnInterfaceService.Enabled = false;
                if (plc.write.Service == false)
                    plc.write.LevegoElokeszitoVezereltLeeresztoSzelep = false;
                btnJoTermek = false;
                btnRosszTermek = false;
            }
            else
            {
                cbtnOVMachinOn.Enabled = true;
            }


            //if (s_ProbaTest_Lenyom < stepCounter && stepCounter < s_ProbaTest_Felenged && (plc.read.Probatest_Lent || cbtnOVStop.value) || stepCounterReset)
            if (s_ProbaTest_Lenyom < stepCounter && stepCounter < s_ProbaTest_Felenged && ( cbtnOVStop.value) || stepCounterReset)
            {

                ControlOff();

                plc.write.MeroTuskeMozgatoSzelepZar = true;
                plc.write.ProbatestMozgatoSzelepFel = true;

                //  stepCounter = s_MeroTuske_Behuz;
                if (stepCounterReset)
                    Leptetoke(s_AlapAllapot);
                else
                {
                    Leptetoke(s_MeroTuske_Behuz);
                    Megszakitva = true;
                    MegfeleloFlag = false;
                }
                stepCounterReset = false;
            }

            if (!MachinOn && cbtnOVMachinOn.Enabled)
            {
                ControlOff();
                Leptetoke(s_MachineOn_be);

                //  MachinOnFirstFlag = true;
            }

            if (plc.write.Service == true && stepCounter > s_Service)
            {
                Leptetoke(s_Service);
            }

            if (s_Start < stepCounter && cbtnOVStop.value)
            {
                ControlOff();
                stepCounter = s_AlapAllapot;

            }

            //  cbtnOVMachinOn.value = MachinOn;

            switch (stepCounter)
            {

                case s_MachineOn_be:

                    if (MachinOn)
                    {
                        cmEol = new ClassMeasurement();

                        cbtnInterfaceService.Enabled = true;
                        cbtnOVStop.Enabled = true;
                        cbtnOVStop.SetPls = true;
                        //  MachinOnFirstFlag = false;
                        if (plc.write.Service == true)
                        {
                            Leptetoke(s_Service);
                            cbtnOVStart.Enabled = false;
                        }
                        else
                        {
                            cbtnOVStart.Enabled = true;
                            Leptetoke(s_AlapAllapot);
                            plc.write.LevegoElokeszitoVezereltLeeresztoSzelep = true;
                        }
                    }
                    break;

                case s_Service:
                    cbtnOVStart.Enabled = false;
                    if (plc.write.Service == false)
                    {
                        Leptetoke(s_AlapAllapot);
                    }
                    break;

                case s_AlapAllapot:

                    plc.write.Machine_on = true;
                    plc.write.LevegoElokeszitoVezereltLeeresztoSzelep = true;
                    plc.write.MeroTuskeMozgatoSzelepZar = true;
                    plc.write.MeroTuskeMozgatoSzelepNyit = false;
                    plc.write.ProbatestMozgatoSzelepFel = true;
                    plc.write.ProbatestMozgatoSzelepLe = false;
                    plc.write.GripperNyit = true;
                    plc.write.GripperZar = false;
                    plc.write.TermekFeltoltes = false;

                    plc.write.CameraCheck_Also = false;
                    plc.write.CameraCheck_Felso = false;
                    FeltoltesMeresFlag = false;


                    MeasurementErrorList = new List<string>();
                    Leptetoke(s_AlapAllapotEllenorzes);
                    Meres = false;
                    break;

                case s_AlapAllapotEllenorzes:
                    if (plc.read.MerotuskePozicio_1 && plc.read.Probatest_Alap&&plc.read.GripperNyitva)
                    {
                        plc.write.MeroTuskeMozgatoSzelepZar = false;
                        plc.write.ProbatestMozgatoSzelepFel = false;
                        if ((plc.read.KabelkorbacsHosszusagErzekelo_1 && cbtnOVSzType_1.value) ||
                                (plc.read.KabelkorbacsHosszusagErzekelo_2 && cbtnOVSzType_2.value) ||
                                (plc.read.KabelkorbacsHosszusagErzekelo_3 && cbtnOVSzType_3.value))
                        Leptetoke(s_Start);
                    }
                    break;

                case s_Start:
                    cbtnOVStart.Enabled = true;
                    if (cbtnOVStart.value)
                    {
                        Leptetoke(s_ProbaTest_Lenyom, false);
                    }
                    break;




                case s_ProbaTest_Lenyom:
                    //plc.write.ProbatestMozgatoSzelepFel = true;
                    if (Probatest_Alap && !plc.read.Probatest_Alap)
                    {

                        cmEol = new ClassMeasurement();

                        Form1.WriteMeasurementDataGridViewFull = true;
                        btnJoTermek = false;
                        btnRosszTermek = false;


                        CameraAlsoFlag = false;
                        CameraFelsoFlag = false;
                        CimkePrintFlag = false;
                        TermekMagassagEllenorzesFlag = false;
                        Meres = true;

                        MegfeleloMeresVegeFlag = false;

                        MegfeleloFlag = true;
                        Megszakitva = false;
                        tempAutoSaveFlag = false;
                        cAramMeres.WriteChartDefault(Form1.ActParamType);
                        Leptetoke(s_ProbaTest_LenyomEnged);
                    }
                    break;



                case s_ProbaTest_LenyomEnged:
                    plc.write.ProbatestMozgatoSzelepFel = false;
                    if (plc.read.Probatest_vezerelheto) //ha lenyomta a termekig
                    {
                        Leptetoke(s_ProbaTest_Pozicioban, false);
                    }



                    break;
                case s_ProbaTest_Pozicioban:

                    if (plc.read.ProbatestMozgatoSzelepLe_Enabled) //ha megfelelő ideig lent tartotta
                        Leptetoke(s_ProbaTest_Leszorit, false);

                    if (plc.read.Probatest_Alap) //ha vissza huzta akkor újrakezd 
                        Leptetoke(s_AlapAllapot);

                    if (plc.read.Probatest_Lent) // ha túltolta
                    {
                        Hiba_NincsTermek = true;
                        Leptetoke(s_AlapAllapot);
                    }
                    break;
                case s_ProbaTest_Leszorit:

                    plc.write.ProbatestMozgatoSzelepLe = true;
                    Leptetoke(s_ProbaTest_LeszoritasEllenorzes, false);


                    break;
                case s_ProbaTest_LeszoritasEllenorzes:
                    Thread.Sleep(500);
                    if (plc.read.Probatest_Lent) // ha túlmegy
                    {
                        Hiba_NincsTermek = true;
                        Leptetoke(s_AlapAllapot);
                    }
                    else
                    {
                        Leptetoke(s_Gripper_Raszorit, false);
                   
                    }
                    break;
  
                case s_Gripper_Raszorit:
                    if (plc.read.GripperZarSzelep_Enabled)
                    {
                        plc.write.GripperNyit = false;
                        plc.write.GripperZar = true;
                        Leptetoke(s_Gripper_Ellenorzes, false);
                    }
                    break;
                case s_Gripper_Ellenorzes:
                    if (plc.read.GripperMegfogta)
                    {
                        Leptetoke(s_MeroTuske_Kinyom, false);
                    }
                    if (plc.read.GripperZarva)
                    {
                        Hiba_NincsAHelyenACsatlakozo = true;
                        //nincs csatlakozó a gripperben
                        Leptetoke(s_AlapAllapot);
                    }

                    break;
                case s_MeroTuske_Kinyom:
                    if (plc.read.MeroTuskeMozgatoSzelepNyit_Enabled)
                    {
                        plc.write.MeroTuskeMozgatoSzelepNyit = true;
                        Leptetoke(s_MeroTuske_KinyomEllenorzes,false);
                    }

                    break;
                case s_MeroTuske_KinyomEllenorzes:
                    if (plc.read.MerotuskePozicio_2)
                    {
                        Leptetoke( s_InsertMeres,false);
                       // Leptetoke(s_TermekFeltoltes, false);           
                    }


                    break;

                case s_InsertMeres:
                    //nem tudom ezek hova kellenek
               
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cMeasurementState] = "Interrupted";
                    cSerialNumber.Update(ClassSerialNumber.SerialNumber + 1);
                    cSerialNumber.Select();

                    cmEol.sM.dtModify.Rows[0][CMeasurement.cSerialNumber] = ClassSerialNumber.SerialNumber.ToString();
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cKIONPartNumber] = ActKIONPartNumber;
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cStabilPartNumber] = ActStabilPartNumber;
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cType] = ActTypeText;
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cMeasurementType] = ActMeasurementType;
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cType_S_id] = Convert.ToInt32( Form1.ActParamType.dt.Rows[0][ClassSensorProperty.cS_id]);
                    cmEol.MeasurementSave();
                    Form1.options.SaveOptions();
                    Leptetoke(s_TermekMagassagEllenorzes);
                    TermekMagassagEllenorzesFlag = true;
                    break;

                case s_TermekMagassagEllenorzes:


                    if (TermekMagassagEllenorzesFlag == false)
                    {
                        if (MegfeleloFlag)
                            Leptetoke(s_CameraFelso, false);
                        else
                            Leptetoke(s_ProbaTest_Felenged, false);
                    }

                    break;

                case s_CameraFelso:

                    plc.write.CameraCheck_Felso = true;


                    if (plc.read.CameraFinish_2)
                    {
                        CameraFelsoFlag = true;
                        Leptetoke(s_CameraFelso_Kiertekeles, false);
                    }


                    break;
                case s_CameraFelso_Kiertekeles:


                    if (!CameraFelsoFlag)
                    {
                        plc.write.CameraCheck_Felso = false;

                        if (MegfeleloFlag || cbtnOVAllTheWay.value)
                            Leptetoke(s_CameraAlso, false);
                        else
                            Leptetoke(s_ProbaTest_Felenged, false);
                    }
                    break;
                case s_CameraAlso:
                    plc.write.CameraCheck_Also = true;
         

                    if (plc.read.CameraFinish_1 )
                    {
                        Leptetoke(s_CameraAlso_Kiertekeles, false);
                        CameraAlsoFlag = true;
                    }

                    break;
                case s_CameraAlso_Kiertekeles:

                    if (!CameraAlsoFlag)
                    {
                        plc.write.CameraCheck_Also = false;
        
                        if (MegfeleloFlag || cbtnOVAllTheWay.value)
                            Leptetoke(s_TermekFeltoltes, false);
                        else
                            Leptetoke(s_ProbaTest_Felenged, false);
                    }
                    break;
            

                 
                case s_TermekFeltoltes:
                    if (plc.read.TermekFeltoltoSzelep_Enabled)
                    {
                        plc.write.TermekFeltoltes = true;
                        TermekLeurult = plc.read.TermekLeurult;
                        Leptetoke(s_TermekLeurules, false);
                        FeltoltesMeresFlag = true;
                    }

                    break;
                case s_TermekLeurules:

                    if (plc.read.TermekLeurult)
                    {
                        FeltoltesMeresFlag = false;
                        plc.write.TermekFeltoltes = false;
                        //Kiertekel

                        TermekLeurultNyomasErtekFlag = true;
                        TermekLeurultNyomasErtek = plc.read.TermekLeurultNyomasErtek;
                        Leptetoke(s_TermekLeurules_Ellenorzes, false);
                    }
                    else
                        TermekLeurult = plc.read.TermekLeurult;
                    if (plc.read.TermekFeltoltes_failure)
                    {
                        plc.write.TermekFeltoltes = false;
                        TermekFeltoltesHiba = true;
                        if ( cbtnOVAllTheWay.value)
                            Leptetoke(s_TermekLeurules_Ellenorzes, false);
                        else
                            Leptetoke(s_AlapAllapot);
                    }

                    break;
                case s_TermekLeurules_Ellenorzes:
                    if (TermekLeurultNyomasErtekFlag == false)
                    {
                        if (MegfeleloFlag || cbtnOVAllTheWay.value)
                            Leptetoke(s_AramMeresTesztelesInditasa, false);
                        else
                            Leptetoke(s_ProbaTest_Felenged, false);
                    }

                    break;
                case s_AramMeresTesztelesInditasa:
                    plc.write.AramMeresTesztelesInditasa = true;
                    // ha véget ért a mérés akkor 
                    // mérés igény visszavonása
                    //Olvassuk ki 
                    if (plc.read.MeresLogRun == false && MeresLogRun == true)  ///lefuto él
                    {
                        plc.write.AramMeresTesztelesInditasa = false;
                        AramMeresEredmenyLekerdezesFlag = true;
                        Leptetoke(s_AramMeresEredmenyLekerdezes);
                    }

                    break;
                case s_AramMeresEredmenyLekerdezes:
                    //Ha véget ért a mérés akkor elleőrzés
                    if (AramMeresEredmenyLekerdezesFlag == false)
                    {
                        AramErtekekEllenorzeseFlag = true;
                        Leptetoke(s_AramMeresEredmenyEllenorzes);
                    }

                    break;
                case s_AramMeresEredmenyEllenorzes:
                    if (AramErtekekEllenorzeseFlag == false)
                    {
                        //ellenorzes ha megvan az eredmeny 
                        Leptetoke(s_ProbaTest_Felenged, false);
                    }
                    break;
                case s_ProbaTest_Felenged:
                    plc.write.ProbatestMozgatoSzelepLe = false;
                    tZaroKupakElenorzesErentosGomb = false;
                    if (plc.read.ProbatestMozgatoSzelepFel_Enabled)
                    {
                        plc.write.ProbatestMozgatoSzelepFel = true;
                        if (MegfeleloFlag || cbtnOVAllTheWay.value)
                            Leptetoke(s_Gombnyomas, false);
                        else
                            Leptetoke(s_MeroTuske_Behuz, false);
                    }
                    break;

                case s_Gombnyomas:
                   
                    if (plc.read.ZaroKupakElenorzesErentosGomb&& !tZaroKupakElenorzesErentosGomb)
                    {
                        if (MegfeleloFlag&&plc.read.ZaroKupakPozicio || cbtnOVAllTheWay.value)
                        {
                            Leptetoke(s_ZaroKupakellenorzes, false);
                            ZarokupakEllenorzesFlag = true;
                        }
                    }
                    tZaroKupakElenorzesErentosGomb = plc.read.ZaroKupakElenorzesErentosGomb;
                    break;
                case s_ZaroKupakellenorzes:
                    if (!ZarokupakEllenorzesFlag)
                    {
                        /*  if (plc.read.ZaroKupakPozicio)
                              MegfeleloFlag = true;
                          else
                              MegfeleloFlag = false;
                            if (MegfeleloFlag || cbtnOVAllTheWay.value)
                              Leptetoke(s_Megfelelo, false);
                          else
                              Leptetoke(s_SelejtTarolo, false);*/             
                        Leptetoke(s_MeroTuske_Behuz, false);
                    }
                    break;
                case s_MeroTuske_Behuz:
                    plc.write.MeroTuskeMozgatoSzelepNyit = false;
                    if (plc.read.MeroTuskeMozgatoSzelepZar_Enabled)
                    {
                        plc.write.MeroTuskeMozgatoSzelepZar = true;
                        Leptetoke(s_Gripper_Elenged, false);
                    }

                    break;
                case s_Gripper_Elenged:
                    plc.write.GripperZar = false;
                    plc.write.GripperNyit = true;
              /*      if (MegfeleloFlag || cbtnOVAllTheWay.value)
                    {
                        Leptetoke(s_Cimkenyomtatas, false); 
                    }
                    else*/
                        Leptetoke(s_Cimkenyomtatas, false);

                    break;
            

                case s_Cimkenyomtatas:

                    if (CimkePrintFlag == false)
                    {

                        CimkePrintFlag = true;
                        if (PrintEnabled)
                        {
                            Form1.CimkePrint(ActKIONPartNumber, ActStabilPartNumber, ClassSerialNumber.SerialNumber.ToString(), DateTime.Now, MegfeleloFlag);
                        }
                    }

                    if (MegfeleloFlag)
                        Leptetoke(s_Megfelelo);
                    else
                        Leptetoke(s_SelejtTarolo);

                    break;

                case s_Megfelelo:
                    btnJoTermek = true;
                    Megfelelo++;
                    SaveSettings("darabSzamlaloMegfelelo", Megfelelo);
                    Leptetoke(s_AlapAllapot);
                    cmEol.sM.dtModify.Rows[0][CMeasurement.cMeasurementState] = "Good";
                    cmEol.MeasurementUpdate();
                /*    if (Meres == true)
                        cmEol.MeasurementDataSave();*/
                    Meres = false;
                    MeresFinishFlag = true;

                    MegfeleloMeresVegeFlag = true;
                    if (!tempAutoSaveFlag)
                    {
                        tempAutoSaveFlag = true;
                        AutoSaveFlag = true;
                        AutoSaveS_id = cmEol.SensorProperty_S_id.ToString();
                    }

                    break;
                case s_SelejtTarolo:
                    if (plc.read.SelesjtTaroloDarabszemlalo || cbtnOVEtalon.value == true)
                    {
                        CimkePrintFlag = false;
                       /* if (Meres == true)
                            cmEol.MeasurementDataSave();*/
                        Meres = false;
                        MeresFinishFlag = true;
                        Selejt++;
                        SaveSettings("darabSzamlaloSelejt", Selejt);
                        if (!Megszakitva)
                        {
                            cmEol.sM.dtModify.Rows[0][CMeasurement.cMeasurementState] = "Wrong";
                            cmEol.MeasurementUpdate();
                        }
                        Leptetoke(s_AlapAllapot);
                        if (!tempAutoSaveFlag)
                        {
                            tempAutoSaveFlag = true;
                            AutoSaveFlag = true;
                            AutoSaveS_id = cmEol.SensorProperty_S_id.ToString();
                        }
                    }
                    break;


            }
            if (!MegfeleloFlag && stepCounter > s_Gripper_Elenged)
            {
                btnJoTermek = false;
                btnRosszTermek = true;
            }

            Probatest_Alap = plc.read.Probatest_Alap;
            MeresLogRun = plc.read.MeresLogRun;
        }


        private void ControlOff()
        {
            // sw.LevegoElokeszitoVezereltLeeresztoSzelep = false;
            plc.write.MeroTuskeMozgatoSzelepNyit = false;
            plc.write.MeroTuskeMozgatoSzelepZar = false;
            plc.write.ProbatestMozgatoSzelepLe = false;
            plc.write.ProbatestMozgatoSzelepFel = false;
            plc.write.GripperZar = false;
            plc.write.GripperNyit = false;
            plc.write.TermekFeltoltes = false;
            plc.write.AramMeresTesztelesInditasa = false;


            plc.write.CameraCheck_Also = false;
            plc.write.CameraCheck_Felso = false;
     

        }
        public bool RunEnabled = false;
        // public bool Next = false;
        //  public bool StepMode = false;
        // public bool Mehetne = false;
        public bool AllTheWay = false;
        private void Leptetoke(int stepCounter, bool auto = true)
        {
            if (cbtnOVStepMode.value == false)
            {
                this.stepCounter = stepCounter;
                setStatus(stepCounter);
            }
            if (cbtnOVStepMode.value)
            {
                cbtnOVNext.Enabled = true;
            }
            if ((cbtnOVStepMode.value && cbtnOVNext.value) || auto)
            {
                this.stepCounter = stepCounter;
                setStatus(stepCounter);
                cbtnOVNext.Enabled = false;
                cbtnOVNext.value = false;
                //   cbtnOVNext.ResetPls = true;
            }

        }

        private void setStatus(int stepCounter)
        {
            if (stepCounterLast != stepCounter)
            {
                updateStatus = true;
                if (!cbtnOVStepMode.value)
                {
                    machineStatus = machineStatusArray[stepCounter];
                //    machineStatus = machineStatusStepModeArray[stepCounter];
                }
                else
                    machineStatus = machineStatusStepModeArray[stepCounter];
                userStaus = userStatusArray[stepCounter];
                stepCounterLast = stepCounter;
            }
        }
    }
}
