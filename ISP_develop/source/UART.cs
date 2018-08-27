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

        /// <summary>
        /// Initialize serial port
        /// </summary>
        /// <param name="uartattribute">Serial port attribute</param>
        /// <param name="myserialport">Target serial port</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool InitializeUART(UARTAttribute uartattribute, SerialPort myserialport)
        {
            try
            {
                myserialport.BaudRate = uartattribute.BaudRate;
                myserialport.DataBits = uartattribute.DataBits;
                myserialport.DiscardNull = uartattribute.DiscardNull;
                myserialport.DtrEnable = uartattribute.DtrEnable;
                myserialport.Handshake = uartattribute.Handshake;
                myserialport.Parity = uartattribute.Parity;
                myserialport.ParityReplace = uartattribute.ParityReplace;
                myserialport.PortName = uartattribute.PortName;
                myserialport.ReadBufferSize = uartattribute.ReadBufferSize;
                myserialport.ReadTimeout = uartattribute.ReadTimeout;
                myserialport.ReceivedBytesThreshold = uartattribute.ReceivedBytesThreshold;
                myserialport.RtsEnable = uartattribute.RtsEnable;
                myserialport.StopBits = uartattribute.StopBits;
                myserialport.WriteBufferSize = uartattribute.WriteBufferSize;
                myserialport.WriteTimeout = uartattribute.WriteTimeout;
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
        public bool GetPortInformation(SerialPort serialport, out UARTAttribute uartattribute)
        {
            try
            {
                uartattribute.BaudRate = serialport.BaudRate;
                uartattribute.DataBits = serialport.DataBits;
                uartattribute.DiscardNull = serialport.DiscardNull;
                uartattribute.DtrEnable = serialport.DtrEnable;
                uartattribute.Handshake = serialport.Handshake;
                uartattribute.Parity = serialport.Parity;
                uartattribute.ParityReplace = serialport.ParityReplace;
                uartattribute.PortName = serialport.PortName;
                uartattribute.ReadBufferSize = serialport.ReadBufferSize;
                uartattribute.ReadTimeout = serialport.ReadTimeout;
                uartattribute.ReceivedBytesThreshold = serialport.ReceivedBytesThreshold;
                uartattribute.RtsEnable = serialport.RtsEnable;
                uartattribute.StopBits = serialport.StopBits;
                uartattribute.WriteBufferSize = serialport.WriteBufferSize;
                uartattribute.WriteTimeout = serialport.WriteTimeout;
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

    }
}