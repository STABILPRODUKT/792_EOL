using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EOL_tesztelo
{

    public struct sRead
    {
        public bool HutesVentilatorMotorvedoOK;
        public bool SzervizAljzatMotorvedoOK;
        public bool PC_Monitor_Nyomtato_MotorvedoOK;
        public bool AsztalVilagitasKismegszakitoOK;
        public bool EllenorzoKamerak24VDCTapKismegszakitoOK;
        public bool DigitalisEsAnalogbemenetek24VDCTapKismegszakitoOK;
        public bool BiztonsagiRelevelLevDigitalisKimenetekMotorvedoOK;
        public bool BiztonsagiRele_hiba;
        public bool VesznyomogombAsztalonBenyomva;
        public bool TeszteloFeszultsegKismegszakitoRendben;
        public bool TeszteloFeszultsegElektromosRendben;
        public bool TeszteloFeszultsegElektromosAramvedelemRendben;
        public bool TeszteloFeszultsegElektromosAramvedelemKelencvenSzazalek;
        public bool SzekrenyHomersegletHiba;
        public bool ZaroKupakPozicio;
        public bool SelesjtTaroloDarabszemlalo;
        public bool MerotuskePozicio_1;
        public bool MerotuskePozicio_2;
        public bool GripperNyitva;
        public bool GripperMegfogta;
        public bool GripperZarva;
        public bool LevegoelokeszitoNyomasKapcsolo;
        public bool Spare_1;
        public bool ZaroKupakElenorzesErentosGomb;
        public bool KabelkorbacsHosszusagErzekelo_1;
        public bool KabelkorbacsHosszusagErzekelo_2;
        public bool KabelkorbacsHosszusagErzekelo_3;
        public bool Spare_2;
        public bool Spare_3;
        public bool Spare_4;
        public bool Probatest_Alap;
        public bool Probatest_FelhLenyom;
        public bool Probatest_vezerelheto;
        public bool Probatest_Lent;
        public bool Spare_5;
        public bool Spare_6;
        public bool Spare_7;
        public bool Machine_On;
        public bool Live;
        public bool MeresLogRun;
        public bool CameraMenetrogzitoCheck_1;
        public bool CameraMenetrogzitoCheck_2;
        public bool CameraMenetrogzitoCheck_3;
        public bool CameraZsugorcsoCheck_1;
        public bool CameraZsugorcsoCheck_2;
        public bool CameraFinish_1;
        public bool CameraFinish_2;
        public bool Spare_8;
        public bool Spare_9;
        public bool Spare_10;
        public bool Spare_16;
        public bool Spare_17;
        public bool TermekLeurult;
        public bool Spare_11;
        public bool TeszteloFeszultsegSzakadas_failure;
        public bool Cooling_motor_protection_failure;
        public bool Service_motor_protection_failure;
        public bool Interface_system_motor_protection_failure;
        public bool DigitalOutMotorProtection_failure;
        public bool Safety_motor_protection_failure;
        public bool E_stop_failure;
        public bool Cabinet_temperature_failure;
        public bool Air_pressure_failure;
        public bool TermekFeltoltes_failure;
        public bool Connection_failure;
        public bool MeroTuskaVegalasNyit_failure;
        public bool MeroTuskaVegalasZar_failure;
        public bool ProbatestVegalasNyit_failure;
        public bool ProbatestVegalasZar_failure;
        public bool GripperNyit_failure;
        public bool GrippeZar_failure;
        public bool Camera_also_failure;
        public bool Camera_felso_failure;
        public bool Spare_19;
        public bool TeszteloFeszultsegKismegszakito_failure;
        public bool TeszteloFeszultsegElektromos_failure;
        public bool TeszteloFeszultsegElektromosAramvedelem_failure;
        public bool LevegoElokeszitoVezereltLeeresztoSzelep_Enabled;
        public bool MeroTuskeMozgatoSzelepNyit_Enabled;
        public bool MeroTuskeMozgatoSzelepZar_Enabled;
        public bool ProbatestMozgatoSzelepFel_Enabled;
        public bool ProbatestMozgatoSzelepLe_Enabled;
        public bool Spare_20_Enabled;
        public bool GripperZarSzelep_Enabled;
        public bool GripperNyitSzelep_Enabled;
        public bool TermekFeltoltoSzelep_Enabled;
        public bool TeszteloFeszultsegElektromosAramvedelemVezerles_Enabled; //ez már automatikusan vezérelt
        public bool MeresLog_Enabled;
        public bool CameraAlso_Enabled;
        public bool CameraFelso_Enabled;
        public bool Spare_21_Enabled;
        public bool Spare_22_Enabled;
        public bool Spare_23_Enabled;
        public bool Spare_24_Enabled;
        public bool Spare_25_Enabled;
        public bool Spare_26_Enabled;
        public int ProbatestMozgatoPozicio;
        public ushort ProbatestNyomasTavado;
        public ushort TermekFeltoltveNyomasErtek;
        public ushort TermekLeurultNyomasErtek;

    }
    [Serializable] public struct sReadFogyasztas
    {
        public ushort AramValue;
        public ushort FeszValue;
        public ushort YEAR;
        public byte MONTH;
        public byte DAY;
        public byte WEEKDAY;
        public byte HOUR;
        public byte MINUTE;
        public byte SECOND;
        public uint NANOSECOND;
     /*   public ushort AramValue;
        public ushort FeszValue;
        public short YEAR;
        public byte MONTH;
        public byte DAY;
        public byte WEEKDAY;
        public byte HOUR;
        public byte MINUTE;
        public byte SECOND;
        public int NANOSECOND;*/


    }
    public struct sWrite
    {
        public bool Spare_1;
        public bool LevegoElokeszitoVezereltLeeresztoSzelep;
        public bool MeroTuskeMozgatoSzelepNyit;
        public bool MeroTuskeMozgatoSzelepZar;
        public bool ProbatestMozgatoSzelepLe;
        public bool ProbatestMozgatoSzelepFel;
        public bool ACK;
        public bool GripperZar;
        public bool GripperNyit;
        public bool Spare_2;
        public bool Spare_3;
        public bool TermekFeltoltes;
        public bool AramMeresTesztelesInditasa;
        public bool Spare_4;
        public bool Spare_5;
        public bool Spare_6;
        public bool Spare_7;
        public bool Spare_8;
        public bool Spare_9;
        public bool Spare_10;
        public bool Spare_11;
        public bool Spare_12;
        public bool Spare_13;
        public bool Spare_14;
        public bool Live;
        public bool Machine_on;
        public bool Service;
        public bool Spare_15;
        public bool Spare_16_1;
        public bool Spare_16;
        public bool Spare_17;
        public bool Spare_18;
        public bool Spare_19;
        public bool CameraCheck_Also;
        public bool CameraCheck_Felso;
        public bool Spare_20;
        public bool Spare_21;
        public bool Spare_25;
        public bool Spare_22;
        public bool Spare_23;
        public bool Spare_24;
        public ushort ProbatestPozicio_Alap;       //global
        public ushort ProbatestVezKezdoPozicio;    //global
        public ushort ProbatestVezVegPozicio;    //global
        public ushort ProbatestPozicioKompenzalas;//global
        public ushort KivantInduloNyomas;

        public ushort KivantNyomasMax;
        public ushort KivantNyomasMin;

        public UInt32 ProbatesLenyomvaDelay;
        public UInt32 ProbatesLeszoritvaDelay;
        public UInt32 ProbatestCloseFailureDelay;
        public UInt32 GripperOpenFailureDelay;
        public UInt32 GripperCloseFailureDelay;
        public UInt32 GripperMegfogDelay;
        public UInt32 MeroTuskeOpenFailureDelay;

        public UInt32 MeroTuskeCloseFailureDelay;
        public UInt32 TermekFeltoltesFailureDelay;
        public UInt32 NyomasFigyelesKesleltetes;
        public UInt32 NyugalmiFogyasztasFigyelesKesleltetes;


    }

     public   class ClassPLC
    {
        public sRead read;
        public sReadFogyasztas read2;
        public sWrite write;
        public string IP;
        public bool ACK;
        Plc plc;

        private int RDB_id = 101;
        private int RDB_AramMeres_id = 102;
        private int WDB_id = 100;

        public static bool ConnectionLife = false;
        public void Con()
        {
            plc = new Plc(CpuType.S71200, this.IP, 0, 1);
            if (plc.IsAvailable)
            {
                plc.Open();
            }
        }
        public void Discon()
        {
            if (plc.IsAvailable)
            {
                plc.Close();
            }
        }

        public void Read()
        {
            try
            {
                /*       using (var plc = new Plc(CpuType.S71200, this.IP, 0, 1))
                       {
                           if (plc.IsAvailable)
                           {
                               plc.Open();*/
               
                read = (sRead)plc.ReadStruct(typeof(sRead), RDB_id);
                ConnectionLife = true;
                /*       plc.Close();
                 }
                  else
                      ConnectionLife = false;
            }*/
            }
            catch
            {
                ConnectionLife = false;
            }
        }
        int StartBytAdressDefault = 22;
        int StartBytAdressStep = 16;
        int StartBytAdress = 22;

        sReadFogyasztas ByteArrayToNewStuff(byte[] bytes)
        {
            sReadFogyasztas stuff;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                stuff = (sReadFogyasztas)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(sReadFogyasztas));
            }
            finally
            {
                handle.Free();
            }
            return stuff;
        }

        public static byte[] Serialize<T>(T data)
    where T : struct
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }
        public static T Deserialize<T>(byte[] array)
            where T : struct
        {
            var stream = new MemoryStream(array);
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }
        public static sReadFogyasztas[] FromArray(byte[] bytes)
        {
            var reader = new BinaryReader(new MemoryStream(bytes));

            sReadFogyasztas[] s = new sReadFogyasztas[Form1.technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes /10];

            for (int i = 0; i < Form1.technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes /10; i++)
            {
                s[i].AramValue = ReverseBytes(reader.ReadUInt16());
                s[i].FeszValue = ReverseBytes(reader.ReadUInt16());
                s[i].YEAR = ReverseBytes(reader.ReadUInt16());
                s[i].MONTH = reader.ReadByte();
                s[i].DAY = reader.ReadByte();
                s[i].WEEKDAY = reader.ReadByte();
                s[i].HOUR = reader.ReadByte();
                s[i].MINUTE = reader.ReadByte();
                s[i].SECOND = reader.ReadByte();
                s[i].NANOSECOND = ReverseBytes(reader.ReadUInt32());  

            }   


            return s;
        }


        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public sReadFogyasztas[] ReadAramMeres(bool reset)
        {
            //     try
            {
                if (reset)
                    StartBytAdress = StartBytAdressDefault;
                byte[] data = plc.ReadBytes(DataType.DataBlock, RDB_AramMeres_id, StartBytAdress,Convert.ToInt32( Form1.technologia.plc.write.NyugalmiFogyasztasFigyelesKesleltetes /10*16));
                sReadFogyasztas[] data2 = FromArray(data);



                //   read2 = (sReadFogyasztas)plc.ReadStruct(typeof(sReadFogyasztas), RDB_AramMeres_id, StartBytAdress);
                StartBytAdress += StartBytAdressStep*5;
                ConnectionLife = true;
                return data2;
            }
        /*    catch
            {
                ConnectionLife = false;
            }*/
        }
        public void Write()
        {
            try
            {
                /*  using (var plc = new Plc(CpuType.S71200, this.IP, 0, 1))
                  {
                      if (plc.IsAvailable)
                      {
                          plc.Open();*/
                plc.WriteStruct(write, WDB_id);
                ConnectionLife = true;
                /*   plc.Close();
               }
               else
                   ConnectionLife = false;
             }*/
            }
            catch
            {
                ConnectionLife = false;
            }
        }
        public void PingHost()
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(this.IP);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            ConnectionLife = pingable;
        }
    }
    public enum ErrorCode
    {
        NoError = 0,
        WrongCPU_Type = 1,
        ConnectionError = 2,
        IPAddressNotAvailable,
        WrongVarFormat = 10,
        WrongNumberReceivedBytes = 11,
        SendData = 20,
        ReadData = 30,
        WriteData = 50
    }
}
