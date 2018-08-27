using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using UART;
using NameSpace_BinProcess;
using System.Collections.Generic;
using NS_Config;
using System.Runtime.InteropServices;

namespace ProgaramerOrderProcess
{

    public enum eErrNumber
    {
        Succesful = 0x00,
        FrameErr,
        NotDef,
        FlashAddressOF,
        FrameLenghtOF,
        DataLenghtOF,
        BaudrateOF,
        Encrypted,
        ReadFail,
        MassEraseFail,
        PageEraseFail,
        TimeOut,
        URLErr,
        FileFormatErr,
        LoadSuccessful,
        VerifyFail,
        WriteFail,
        SerialPortErr,
        MCUInitFail,
        PLCConnectFail,
    }

    public class Class_ProgaramerOrderProcess1
    {
        
        public Class_ProgaramerOrderProcess1()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt  = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
       // bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00,9600, 14400, 19200, 38400, 57600, 115200, 128000,256000 };
        [DllImport("kernel32.dll",
            CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count,SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if(ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if(CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ,UInt32 RAMCodeAdd,string COM,int Baudrate,UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if(SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            
            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];
            
            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            System.Diagnostics.Debug.WriteLine(string.Format("1:{0}", reconnectcount));
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if(SerialPort_ObJ.BytesToRead>0)
            {
                //byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                ReceiveData = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if(reconnectcount>0&& stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }
            
            if (stats!= (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum,0 };
            
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand,0, send,0,1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);
            
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal),0, bin.BinData, bin.BinData.Length-4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = {0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
            //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
            SerialPort_ObJ.Open();
            }
            catch
            {
            // message += e.Message+"1";
            //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
            //SerialPort_ObJ.Open();
              return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data,int offset,int lenght)
        {
            byte checksum = 0;
            for(int i = offset;i<offset+lenght;i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11-2);
            Frame.RemoveRange(12, Frame.Count -12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ,byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11-2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ,byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length+8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length>>8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length+12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if(count>0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData,UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght)>>8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ,byte[] Address, int lenght,ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8) );
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if(SerialPort_ObJ.BytesToRead>0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b,0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;
            
            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if(regetcount>0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
//                return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ,byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ,byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ,ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ,ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//1
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port1]"+DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port1]"+DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port1]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port1]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port1]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port1]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port1]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port1]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port1]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port1]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port1]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port1]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port1]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port1]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess2
    {

        public Class_ProgaramerOrderProcess2()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            System.Diagnostics.Debug.WriteLine(string.Format("2:{0}", reconnectcount));
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)///2
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port2]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port2]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port2]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port2]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port2]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port2]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port2]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port2]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port2]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port2]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port2]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port2]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port2]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port2]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess3
    {

        public Class_ProgaramerOrderProcess3()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//3
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port3]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port3]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port3]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port3]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port3]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port3]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port3]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port3]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port3]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port3]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port3]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port3]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port3]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port3]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess4
    {

        public Class_ProgaramerOrderProcess4()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                // byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                ReceiveData = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//4
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port4]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port4]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port4]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port4]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port4]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port4]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port4]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port4]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port4]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port4]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port4]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port4]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port4]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port4]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess5
    {

        public Class_ProgaramerOrderProcess5()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//5
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port5]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port5]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port5]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port5]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port5]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port5]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port5]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port5]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port5]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port5]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port5]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port5]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port5]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port5]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess6
    {

        public Class_ProgaramerOrderProcess6()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//6
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port6]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port6]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port6]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port6]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port6]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port6]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port6]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port6]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port6]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port6]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port6]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port6]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port6]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port6]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess7
    {

        public Class_ProgaramerOrderProcess7()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                    //reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//7
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port7]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port7]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port7]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port7]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port7]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port7]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port7]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port7]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port7]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port7]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port7]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port7]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port7]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port7]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }
    public class Class_ProgaramerOrderProcess8
    {

        public Class_ProgaramerOrderProcess8()
        {
            XMLProcess_obj.GetText(ref DisplayText_obj);
            XMLProcess_obj.GetSystemConfig(ref SystemConfig_obj);
            //SerialPort_ObJ.DataReceived += DataReceivedServer;
            tt.Elapsed += TimeOutTick;
            //t1.Elapsed += t1Tick;
            //SerialPort_ObJ.BaudRate = 9600;
            //SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.StopBits = StopBits.One;
            //SerialPort_ObJ.PortName = "COM5";
            //SerialPort_ObJ.DataBits = 8;
        }
        public uint RetryCnt = 5;
        public uint ConnectTimeOut = 1000;
        public bool serialstop = false;
        public DisplayText DisplayText_obj = new DisplayText();
        public SystemConfig SystemConfig_obj = new SystemConfig();
        public XMLProcess XMLProcess_obj = new XMLProcess();
        List<byte> Frame = new List<byte> { 0x49, 0x53, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        System.Timers.Timer tt = new System.Timers.Timer();
        System.Timers.Timer t1 = new System.Timers.Timer();
        //SerialPort SerialPort_ObJ = new SerialPort();
        bool TimeOutFlag = false;
        //bool DelayFlag = false;
        public bool ReceiveFlag = false;
        public bool ConnectFlag = false;
        public byte[] ReceiveData = new byte[1];
        int[] BaudrateArr = { 0x00, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        [DllImport("kernel32.dll",
    CallingConvention = CallingConvention.Winapi)]
        extern static int GetTickCount();
        /*
        private void DataReceivedServer(object sender, EventArgs e)
        {
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            if(SerialPort_ObJ.IsOpen == false)
            {
                SerialPort_ObJ.Open();
            }
            receivedBytesThreshold = SerialPort_ObJ.ReceivedBytesThreshold;
            ReceiveData = new byte[receivedBytesThreshold];
            while (SerialPort_ObJ.BytesToRead < receivedBytesThreshold) ;
            readdata = SerialPort_ObJ.Read(ReceiveData,0, SerialPort_ObJ.ReceivedBytesThreshold);
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_ObJ.Read(ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            ReceiveFlag = true;
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeOutTick(object sender, EventArgs e)
        {
            TimeOutFlag = true;
            tt.Stop();
            tt.Enabled = false;
        }

        //void t1Tick(object sender, EventArgs e)
        //{
        //    DelayFlag = true;
        //    t1.Stop();
        //    t1.Enabled = false;
        //}


        void Delay(UInt32 Count)
        {
            //t1.Interval = Count;         
            //t1.Enabled = true;
            //t1.Start();
            //while (true)
            //{
            //    if(DelayFlag == true)
            //    {
            //        t1.Stop();
            //        t1.Enabled = false;
            //        DelayFlag = false;
            //        return;
            //    }
            //}
            int start = GetTickCount();
            while (GetTickCount() - start < Count)
            {
                Application.DoEvents();
            }
        }
        private byte TimeOut(UInt32 Count, SerialPort s)
        {
            //tt.Interval = Count;         
            //tt.Enabled = true;
            //tt.Start();
            int start = GetTickCount();

            UInt32 DelayCnt = 0;
            while (ReceiveFlag != true)
            {
                try
                {
                    ReceiveData = new byte[s.ReceivedBytesThreshold];
                    if (s.BytesToRead >= s.ReceivedBytesThreshold)
                    {
                        Delay(10);
                        s.Read(ReceiveData, 0, s.ReceivedBytesThreshold);
                        ReceiveFlag = true;

                        break;
                    }
                }
                catch //(Exception e)
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
                if (GetTickCount() - start > Count)
                {
                    TimeOutFlag = true;
                }
                if (TimeOutFlag == true)
                {
                    TimeOutFlag = false;
                    //tt.Stop();
                    //tt.Enabled = false;
                    return (byte)eErrNumber.TimeOut;

                }
            }
            TimeOutFlag = false;
            //tt.Stop();
            //tt.Enabled = false;
            return (byte)eErrNumber.Succesful;
        }

        public bool SetHexFileURL(string URL)
        {
            //HexFileURL = URL;
            try
            {
                StreamReader FileSR = new StreamReader(URL);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte DisConnectProcess(SerialPort SerialPort_ObJ)
        {
            try
            {
                SerialPort_ObJ.Close();
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte ConnectACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x01)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="RAMCodeAdd"></param>
        /// <returns></returns>
        public byte ConnectProcess(SerialPort SerialPort_ObJ, UInt32 RAMCodeAdd, string COM, int Baudrate, UInt32 Crystal)
        {
            byte[] RAMCodeAdd1 = BitConverter.GetBytes(RAMCodeAdd);
            //Array.Reverse(RAMCodeAdd1);
            int reconnectcount = 16;
            if (SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Close();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }

            SerialPort_ObJ.BaudRate = Baudrate;
            SerialPort_ObJ.PortName = COM;
            SerialPort_ObJ.DataBits = 8;
            SerialPort_ObJ.StopBits = StopBits.One;
            SerialPort_ObJ.Parity = Parity.None;
            //SerialPort_ObJ.ReadBufferSize = 512 * 256;
            //SerialPort_ObJ.
            byte[] ResetCommand1 = new byte[1];

            try
            {
                if (!SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Open();
                }
            }
            catch //(Exception e)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                //MessageBox.Show("xinxi",e.Message);
                return (byte)eErrNumber.SerialPortErr;
            }
            Reconnect:
            //Send reset command
            ResetCommand1[0] = (byte)(0x18);
            byte[] DownLoadCommand = { 0x00 };
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            try
            {
                SerialPort_ObJ.Write(ResetCommand1, 0x00, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(ConnectTimeOut, SerialPort_ObJ);
            if (reconnectcount > 0 && stats != (byte)eErrNumber.Succesful)
            {
                reconnectcount--;
                goto Reconnect;
            }

            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x11)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                if (reconnectcount > 0)
                {
                   // reconnectcount--;
                    goto Reconnect;
                }
                return (byte)eErrNumber.MCUInitFail;
            }

            //Send download command
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath + "\\Config\\" + SystemConfig_obj.BinFile[0]);
            byte checksum = 0;//CheckSum(bin.BinData, 0, bin.BinData.Length);
            checksum += DownLoadCommand[0];
            //Array.Reverse(RAMCodeAdd1);
            checksum += RAMCodeAdd1[0];
            checksum += RAMCodeAdd1[1];
            checksum += RAMCodeAdd1[2];
            checksum += RAMCodeAdd1[3];
            //byte[] Lenght = { (byte)(bin.BinData.Length & 0xFF), (byte)((bin.BinData.Length>>8) & 0xFF), (byte)((bin.BinData.Length>>16) & 0xFF), (byte)((bin.BinData.Length>>24) & 0xFF) };
            byte[] Lenght = BitConverter.GetBytes(bin.BinData.Length);
            //Array.Reverse(Lenght);
            checksum += Lenght[0];
            checksum += Lenght[1];
            checksum += Lenght[2];
            checksum += Lenght[3];
            byte[] CheckSumData = { checksum, 0 };

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            byte[] send = new byte[10];
            Array.Copy(DownLoadCommand, 0, send, 0, 1);
            Array.Copy(RAMCodeAdd1, 0, send, 1, 4);
            Array.Copy(Lenght, 0, send, 5, 4);
            Array.Copy(CheckSumData, 0, send, 9, 1);
            //SerialPort_ObJ.Write(DownLoadCommand, 0x00, 1);
            //SerialPort_ObJ.Write(RAMCodeAdd1, 0x00, 4);
            //SerialPort_ObJ.Write(Lenght, 0x00, 4);
            //SerialPort_ObJ.Write(CheckSumData, 0x00, 1);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(send, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //Array.Reverse(bin.BinData);

            SerialPort_ObJ.ReceivedBytesThreshold = 1;
            Array.Copy(BitConverter.GetBytes(Crystal), 0, bin.BinData, bin.BinData.Length - 4, 4);
            try
            {
                SerialPort_ObJ.Write(bin.BinData, 0x00, bin.BinData.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            //SerialPort_ObJ.Write(BitConverter.GetBytes(Crystal), 0x00, 4);
            CheckSumData[1] = CheckSum(bin.BinData, 0, bin.BinData.Length);
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(CheckSumData, 0x01, 1);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            if (ReceiveData[0] == 0x01)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return (byte)eErrNumber.MCUInitFail;
            }
            //DelayFlag = false;
            //Delay(1000);
            //while(DelayFlag)
            byte[] ExecuteCommand = { 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0 };
            //SerialPort_ObJ.ReceivedBytesThreshold = 1;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(ExecuteCommand, 0x00, 10);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            ReceiveFlag = false;
            stats = TimeOut(10000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            /*
            ReceiveFlag = false;
            stats = TimeOut(5000);
            if (stats != (byte)eErrNumber.Succesful)
            {
                return stats;
            }
            if (ReceiveData[0] != 0xC2)
            {

            }
            else
            {
                return 5;
            }
            */
            /*
            for(int i =0;i<10000;i++)
            {
                for (int j = 0; j < 1000; j++)
                {

                }
            }
            */



            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }

            Frame[2] = 0x00;
            Frame[3] = 0x09;
            Frame[4] = 0x01;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x01;
            Frame.Insert(11, 0x07);

            try
            {
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x08];
                SerialPort_ObJ.Open();
            }
            catch
            {
                // message += e.Message+"1";
                //SerialPort_ObJ.BaudRate = BaudrateArr[0x06];
                //SerialPort_ObJ.Open();
                return (byte)eErrNumber.SerialPortErr;
            }

            /*删除close open操作*/
            //if (SerialPort_ObJ.IsOpen)
            //{
            //    SerialPort_ObJ.Close();
            //}
            //SerialPort_ObJ.BaudRate = Baudrate;

            //try
            //{
            //   SerialPort_ObJ.Open();
            //}
            //catch
            //{
            //    return (byte)eErrNumber.SerialPortErr;
            //}

            Frame[12] = CheckSum(Frame.ToArray(), 2, 10);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            if (SerialPort_ObJ.BytesToRead > 0)
            {
                byte[] t = new byte[SerialPort_ObJ.BytesToRead];
                SerialPort_ObJ.Read(ReceiveData, 0, SerialPort_ObJ.BytesToRead);
            }
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 13);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            stats = ConnectACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                if (SerialPort_ObJ.IsOpen)
                {
                    //SerialPort_ObJ.Close();
                }
                return stats;
            }
            SerialPort_ObJ.BaudRate = BaudrateArr[Frame[11]];
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        byte CheckSum(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public UInt16 CheckSumUInt16(byte[] data, int offset, int lenght)
        {
            UInt16 checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        UInt16 CheckSum_uint16(byte[] data, int offset, int lenght)
        {
            byte checksum = 0;
            for (int i = offset; i < offset + lenght; i++)
            {
                checksum += data[i];
            }
            return checksum;
        }

        byte EraseFlashACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x02)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public byte EraseFlash(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                try
                {
                    SerialPort_ObJ.Open();
                }
                catch
                {
                    return (byte)eErrNumber.SerialPortErr;
                }
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x02;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    try
                    {
                        SerialPort_ObJ.Close();
                    }
                    catch
                    {
                        return (byte)eErrNumber.SerialPortErr;
                    }
                }
                */
                return stats;
            }
            stats = EraseFlashACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {

                /*
                if (SerialPort_ObJ.IsOpen)
                {

                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte PaseEraseACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x03)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <returns></returns>
        public byte ErasePage(SerialPort SerialPort_ObJ, byte[] Address)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x03;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = PaseEraseACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte WriteACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x04)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Write(SerialPort SerialPort_ObJ, byte[] Address, byte[] Data)
        {
            int count = 0x20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((Data.Length + 8) >> 8);
            Frame[3] = (byte)((Data.Length + 8));
            Frame[4] = 0x04;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(Data.Length >> 8);
            Frame[10] = (byte)(Data.Length);
            Frame.InsertRange(11, Data);
            Frame[11 + Data.Length] = CheckSum(Frame.ToArray(), 2, 11 - 2 + Data.Length);
            Frame.RemoveRange(Data.Length + 12, Frame.Count - 12 - Data.Length);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            rewrite:
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12 + Data.Length);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                serialstop = false;
                return stats;
            }
            stats = WriteACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {

            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                if (count > 0)
                {
                    count--;
                    goto rewrite;
                }
                serialstop = false;
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte ReadACKCheck(byte[] ReceiveData, UInt32 Lenght)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != (byte)((0x07 + Lenght) >> 8) || ReceiveData[3] != (byte)(0x07 + Lenght) ||
               ReceiveData[4] != 0x05)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, (int)(8 + Lenght)) != ReceiveData[10 + Lenght])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Read(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            int regetcount = 20;
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = (byte)((8) >> 8);
            Frame[3] = (byte)((8));
            Frame[4] = 0x05;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(lenght >> 8);
            Frame[10] = (byte)(lenght);
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);

            //port.AckFlag = false;
            Reget:
            try
            {
                if (SerialPort_ObJ.BytesToRead > 0)
                {
                    byte[] b = new byte[SerialPort_ObJ.BytesToRead];
                    SerialPort_ObJ.Read(b, 0, SerialPort_ObJ.BytesToRead);
                }
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + lenght;
            ReceiveFlag = false;

            byte stats = TimeOut(8000, SerialPort_ObJ);
            serialstop = true;
            if (stats != (byte)eErrNumber.Succesful)
            {
                serialstop = false;
                return stats;
            }
            stats = ReadACKCheck(ReceiveData, (UInt32)lenght);
            if (stats == (byte)eErrNumber.Succesful)
            {
                /*
                if(CheckSum(ReceiveData,2, lenght+8) != ReceiveData[lenght+11-1])
                {
                    if (regetcount > 0)
                    {
                        regetcount--;
                        goto Reget;
                    }
                    return (byte)eErrNumber.ReadFail;
                }
                */

                for (int i = 0; i < lenght; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                if (regetcount > 0)
                {
                    regetcount--;
                    goto Reget;
                }
                serialstop = false;
                return (byte)eErrNumber.ReadFail;
                //return ReceiveData[5];
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                //SerialPort_ObJ.Close();
            }
            */
            serialstop = false;
            return (byte)eErrNumber.Succesful;
        }

        byte VerifyACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x09 ||
               ReceiveData[4] != 0x06)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 10) != ReceiveData[12])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte Verify(SerialPort SerialPort_ObJ, byte[] Address, int lenght, ref byte[] Data)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x06;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 11 + 2;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = VerifyACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                for (int i = 0; i < 2; i++)
                {
                    Data[i] = ReceiveData[i + 10];
                }
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckBlankACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x07)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckBlank(SerialPort SerialPort_ObJ, byte[] Address, UInt32 lenght, ref byte Result, ref UInt32 ErrAdd)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Array.Reverse(Address);
            Frame[2] = 0x00;
            Frame[3] = 0x0C;
            Frame[4] = 0x07;
            Frame[5] = Address[0];
            Frame[6] = Address[1];
            Frame[7] = Address[2];
            Frame[8] = Address[3];
            Frame[9] = (byte)(4 >> 8);
            Frame[10] = (byte)(4);
            Frame.Insert(11, (byte)(lenght >> 24));
            Frame.Insert(12, (byte)(lenght >> 16));
            Frame.Insert(13, (byte)(lenght >> 8));
            Frame.Insert(14, (byte)(lenght >> 0));
            /*
            Frame[11] = (byte)(lenght >> 24);
            Frame[12] = (byte)(lenght >> 16);
            Frame[13] = (byte)(lenght >> 8);
            Frame[14] = (byte)(lenght);
            */
            //Frame.InsertRange(11, Data);
            Frame[15] = CheckSum(Frame.ToArray(), 2, 15 - 2);
            Frame.RemoveRange(16, Frame.Count - 16);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 16);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckBlankACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
                ErrAdd = (UInt32)((ReceiveData[6] << 24) + (ReceiveData[7] << 16) + (ReceiveData[8] << 8) + (ReceiveData[9] << 0));
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            /*
            if (SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Close();
            }
            */
            return (byte)eErrNumber.Succesful;
        }

        byte CheckEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x08 ||
               ReceiveData[4] != 0x08)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 9) != ReceiveData[11])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Address"></param>
        /// <param name="lenght"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte CheckEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x08;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 12;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = CheckEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[10];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte SetEncryptACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x09)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte SetEncrypt(SerialPort SerialPort_ObJ, ref byte Result)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x09;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = SetEncryptACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }

        byte MCUResetACKCheck(byte[] ReceiveData)
        {
            if (ReceiveData[0] != 0x49 || ReceiveData[1] != 0x53 ||
               ReceiveData[2] != 0x00 || ReceiveData[3] != 0x07 ||
               ReceiveData[4] != 0x0A)
            {
                return (byte)eErrNumber.FrameErr;
            }
            if (CheckSum(ReceiveData, 2, 8) != ReceiveData[10])
            {
                return (byte)eErrNumber.FrameErr;
            }
            return ReceiveData[5];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SerialPort_ObJ"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public byte MCUReset(SerialPort SerialPort_ObJ)
        {
            /*
            if (!SerialPort_ObJ.IsOpen)
            {
                SerialPort_ObJ.Open();
            }
            */
            Frame[2] = 0x00;
            Frame[3] = 0x08;
            Frame[4] = 0x0A;
            Frame[5] = 0x00;
            Frame[6] = 0x00;
            Frame[7] = 0x00;
            Frame[8] = 0x00;
            Frame[9] = 0x00;
            Frame[10] = 0x00;
            //Frame.InsertRange(11, Data);
            Frame[11] = CheckSum(Frame.ToArray(), 2, 11 - 2);
            Frame.RemoveRange(12, Frame.Count - 12);
            SerialPort_ObJ.ReceivedBytesThreshold = 11;
            //port.AckFlag = false;
            try
            {
                SerialPort_ObJ.Write(Frame.ToArray(), 0x00, 12);
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }

            ReceiveFlag = false;
            byte stats = TimeOut(5000, SerialPort_ObJ);
            if (stats != (byte)eErrNumber.Succesful)
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            stats = MCUResetACKCheck(ReceiveData);
            if (stats == (byte)eErrNumber.Succesful)
            {
                //Result = ReceiveData[9];
            }
            else
            {
                /*
                if (SerialPort_ObJ.IsOpen)
                {
                    SerialPort_ObJ.Close();
                }
                */
                return stats;
            }
            return (byte)eErrNumber.Succesful;
        }


        public byte serialportclose(SerialPort SerialPort_ObJ)
        {
            try
            {
                if (SerialPort_ObJ.IsOpen == true)
                {
                    SerialPort_ObJ.Close();
                }
            }
            catch
            {
                return (byte)eErrNumber.SerialPortErr;
            }
            return (byte)eErrNumber.Succesful;
        }

        public void ErrProcess(byte ErrNumber)//8
        {
            switch (ErrNumber)
            {
                case (byte)eErrNumber.Succesful:
                    MessageBox.Show("[Port8]" + DisplayText_obj.ConnectSuccessful, DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                    break;
                case (byte)eErrNumber.FrameErr:
                    //Refresh();
                    MessageBox.Show("[Port8]" + DisplayText_obj.FrameErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.NotDef:
                    MessageBox.Show("[Port8]" + DisplayText_obj.FrameNotDef, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.Encrypted:
                    MessageBox.Show("[Port8]" + DisplayText_obj.Encrypted, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.ReadFail:
                    MessageBox.Show("[Port8]" + DisplayText_obj.ReadFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MassEraseFail:
                    MessageBox.Show("[Port8]" + DisplayText_obj.MassEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.PageEraseFail:
                    MessageBox.Show("[Port8]" + DisplayText_obj.PageEraseFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.TimeOut:
                    //MessageBox.Show("[Port8]" + DisplayText_obj.TimeOut, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                    //                            MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.URLErr:
                    MessageBox.Show("[Port8]" + DisplayText_obj.URLError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.FileFormatErr:
                    MessageBox.Show("[Port8]" + DisplayText_obj.FileFormatError, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.LoadSuccessful:
                    //MessageBox.Show(DisplayText_obj., DisplayText_obj.HintText);
                    break;
                case (byte)eErrNumber.VerifyFail:
                    MessageBox.Show("[Port8]" + DisplayText_obj.VerifyFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.WriteFail:
                    MessageBox.Show("[Port8]" + DisplayText_obj.WriteFail, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.SerialPortErr:
                    MessageBox.Show("[Port8]" + DisplayText_obj.SerialPortErr, DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
                case (byte)eErrNumber.MCUInitFail:
                    MessageBox.Show("[Port8]" + "MCU初始化失败。", DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                    break;
            }
        }

    }

}
