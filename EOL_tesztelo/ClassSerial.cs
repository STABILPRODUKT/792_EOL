using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace EOL_tesztelo
{
    class ClassSerial
    {
        private string portName;                //port nev
        private int baudRate;                   //atviteli sebesseg
        private SerialPort sPort;
        private Parity parity = Parity.None;
        private int dataBits = 8;
        private StopBits stopBits = StopBits.One;
        public bool ConnectionLife=false;
        public bool Tulindexeles = false;
        public ClassSerial()
        {
            this.baudRate = 19200;
        }
        public ClassSerial(string portName, int baudRate, SerialPort sPort)
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.sPort = sPort;
        }
        public string PortName
        {
            get { return this.portName; }
            set { this.portName = value; }
        }
        public int BaudRate
        {
            get { return this.baudRate; }
            set { this.baudRate = value; }
        }

        public bool connect()
        {
            try
            {
                if (this.portName != null)
                {
                   
                    sPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                    sPort.Open();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public int getData()
        {
            try
            {
                return sPort.ReadByte();
            }
            catch
            {
                return 0;
            }
        }
        public char getChar()
        {
            try
            {
                return Convert.ToChar(sPort.ReadChar());
            }
            catch
            {
                return 'e';
            }
        }
        public void DisConnet()
        {
            try
            {
                sPort.Close();
            }
            catch
            {

            }
        }
        public bool IsOpen()
        {
            if (sPort.IsOpen)
            {
                return true;
            }
            else
                return false;
        }
        public bool setData(string str)
        {
            try
            {
                sPort.Write(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

