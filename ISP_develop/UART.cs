using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;


namespace UART
{
    public partial class UARTProcess : SerialPort
    {
        public struct UARTAttribute
        {
            public int BaudRate;
            public int DataBits;
            public bool DiscardNull;
            public bool DtrEnable;
            public Handshake Handshake;
            public Parity Parity;
            public byte ParityReplace;
            public string PortName;
            public int ReadBufferSize;
            public int ReadTimeout;
            public int ReceivedBytesThreshold;
            public bool RtsEnable;
            public StopBits StopBits;
            public int WriteBufferSize;
            public int WriteTimeout;
        }

        public byte[] ReceiveData{get;set;}
        public bool AckFlag { get; set; }
        /// <summary>
        /// Initialize serial port
        /// </summary>
        /// <param name="uartattribute">Serial port attribute</param>
        /// <param name="myserialport">Target serial port</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool InitializeUART(UARTAttribute uartattribute)
        {
            try
            {
                BaudRate = uartattribute.BaudRate;
                DataBits = uartattribute.DataBits;
                DiscardNull = uartattribute.DiscardNull;
                DtrEnable = uartattribute.DtrEnable;
                Handshake = uartattribute.Handshake;
                Parity = uartattribute.Parity;
                ParityReplace = uartattribute.ParityReplace;
                PortName = uartattribute.PortName;
                ReadBufferSize = uartattribute.ReadBufferSize;
                ReadTimeout = uartattribute.ReadTimeout;
                ReceivedBytesThreshold = uartattribute.ReceivedBytesThreshold;
                RtsEnable = uartattribute.RtsEnable;
                StopBits = uartattribute.StopBits;
                WriteBufferSize = uartattribute.WriteBufferSize;
                WriteTimeout = uartattribute.WriteTimeout;
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        /// <summary>
        /// Get serial port names, and add to combobox
        /// </summary>
        /// <param name="mycombobox">Target combobox, use to display serial port name list</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool GetPortNames(ComboBox mycombobox)
        {
            int i;
            try
            {
                //get serial port name list
                string[] portlist = SerialPort.GetPortNames();

                //clear old com items
                for (i = 0; i < mycombobox.Items.Count; i++)
                {
                    mycombobox.Items.Remove(i);                         
                }

                //add new com items
                for (i = 0; i < portlist.Length; i++)
                {
                    mycombobox.Items.Add(portlist[i]);                         
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get serial port information
        /// </summary>
        /// <param name="serialport">Target port name, get this port information</param>
        /// <param name="uartattribute">Store port information</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool GetPortInformation(out UARTAttribute uartattribute)
        {
            try
            {
                uartattribute.BaudRate = BaudRate;
                uartattribute.DataBits = DataBits;
                uartattribute.DiscardNull = DiscardNull;
                uartattribute.DtrEnable = DtrEnable;
                uartattribute.Handshake = Handshake;
                uartattribute.Parity = Parity;
                uartattribute.ParityReplace = ParityReplace;
                uartattribute.PortName = PortName;
                uartattribute.ReadBufferSize = ReadBufferSize;
                uartattribute.ReadTimeout = ReadTimeout;
                uartattribute.ReceivedBytesThreshold = ReceivedBytesThreshold;
                uartattribute.RtsEnable = RtsEnable;
                uartattribute.StopBits = StopBits;
                uartattribute.WriteBufferSize = WriteBufferSize;
                uartattribute.WriteTimeout = WriteTimeout;
                //serialport.Close();
                return true;
            }
            catch
            {
                uartattribute.BaudRate = 115200;
                uartattribute.DataBits = 8;
                uartattribute.DiscardNull = false;
                uartattribute.DtrEnable = false;
                uartattribute.Handshake = Handshake.None;
                uartattribute.Parity = Parity.None;
                uartattribute.ParityReplace = 63;
                uartattribute.PortName = "COM1";
                uartattribute.ReadBufferSize = 4096;
                uartattribute.ReadTimeout = -1;
                uartattribute.ReceivedBytesThreshold = 1;
                uartattribute.RtsEnable = false;
                uartattribute.StopBits = StopBits.One;
                uartattribute.WriteBufferSize = 2048;
                uartattribute.WriteTimeout = -1;
                return false;
            }
        }
        private static void SerialDataReceivedEventHandler(Object sender,
                        SerialDataReceivedEventArgs e)
        {

        }

    }
}