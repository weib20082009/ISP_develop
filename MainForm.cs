using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Hex_Srec_process_N;
using NameSpace_BinProcess;
using System.Threading;
using ProgaramerOrderProcess;
using UART;
using NS_Config;
using System.IO.Ports;

namespace ISP_develop
{
    public partial class MainForm : Form
    {
        Class_ProgaramerOrderProcess ProgaramerOrderProcess_Obj = new Class_ProgaramerOrderProcess();
        UInt32 FailCount = 0;
        UInt32 PassCount = 0;
        UInt32 TotalCount = 0;
        //bool ConnectFlag = false;
        bool[] ConnectFlagSub = new bool[9];
        //bool BusyFlag = false;
        bool[] BusyFlagSub = new bool[9];
        int TargetMCUSlecet = -1;
        int FrequencySlecet = 0x00;
        //int COMSlecet = 0x00;
        public static int[] COMSlecet = new int[9];//0 as flag for all;1-8 for all ports
        HexSrec_Process HexProcess_obj = new HexSrec_Process();
        public static byte[] HexData = new byte[1];
        UInt32 RecordCount = 0x00;
        public static FrameFormatStr[] HexFileRecord = new FrameFormatStr[1];
        bool ChipEraseFlag = true;
        System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();
        string hexformat = "";
        public MainForm()
        {
            InitializeComponent();
        }
        Class_ProgaramerOrderProcess Class_ProgaramerOrderProcess_obj = new Class_ProgaramerOrderProcess();

        private void ConnectedConfigSet()
        {
            if(Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode) == 0)
            {
                RadioButton_ChipErase.Checked = true;
                RadioButton_PageErase.Checked = false;
            }
            else
            {
                RadioButton_ChipErase.Checked = false;
                RadioButton_PageErase.Checked = true;
            }
            if (Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.EraseEnb) == 0)
            {
                CheckBox_Erase.Checked = false;
            }
            else
            {
                CheckBox_Erase.Checked = true;
            }
            if (Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.BankEnb) == 0)
            {
                CheckBox_BlankCheck.Checked = false;
            }
            else
            {
                CheckBox_BlankCheck.Checked = true;
            }
            if (Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.ProEnb) == 0)
            {
                CheckBox_Program.Checked = false;
            }
            else
            {
                CheckBox_Program.Checked = true;
            }
            if (Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyEnb) == 0)
            {
                CheckBox_Verify.Checked = false;
            }
            else
            {
                CheckBox_Verify.Checked = true;
            }
            if (Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptEnb) == 0)
            {
                CheckBox_Encrypt.Checked = false;
            }
            else
            {
                CheckBox_Encrypt.Checked = true;
            }
        }

        private void ConnectedConfigClear()
        {
            RadioButton_PageErase.Checked = false;
            RadioButton_ChipErase.Checked = false;
            CheckBox_Erase.Checked = false;
            CheckBox_BlankCheck.Checked = false;
            CheckBox_Program.Checked = false;
            CheckBox_Verify.Checked = false;
            CheckBox_Encrypt.Checked = false;

        }

        private void MainConfigSet()
        {
            ComboBox_TargetMCU.SelectedIndex = Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.MCUSelected);
            ComboBox_Frequency.SelectedIndex = Convert.ToInt32(ProgaramerOrderProcess_Obj.DisplayText_obj.CrystalSelected);
            TextBox_HexFile.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.HexFileURL;
            string[] ss = SerialPort.GetPortNames();
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i] == ProgaramerOrderProcess_Obj.DisplayText_obj.COMSelect)
                {
                    ComboBox_COM.SelectedIndex = i;
                }
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            ControlsText();
            MainConfigSet();
            //SerialPort_Communiction.Open();
            HexSrec_Process test = new HexSrec_Process();
            //FrameFormatStr[] Data = new FrameFormatStr[test.GetRows("D:\\work\\training.hex") - 1];
            byte[] FlahRAM = new byte[0xf00000];
            UInt32 RecordCount = 0;
            UInt32 FlashBaseAddress = 0x0000;
            WriteDataBuff[] modifydata = new WriteDataBuff[5];
            BinProcess bin = new BinProcess();
            //bin.ReadBinFile(Application.StartupPath + "\\training.bin");
            //bin.WriteBinFile(bin.BinData, 0, bin.BinData.Length, "D:\\work\\training1.bin");
            modifydata[0].Address = 0x11;
            modifydata[0].Data = 0x55;
            modifydata[1].Address = 0x10600;
            modifydata[1].Data = 0x55;
            modifydata[2].Address = 0x2222;
            modifydata[2].Data = 0x55;
            modifydata[3].Address = 0x3333;
            modifydata[3].Data = 0x55;
            modifydata[4].Address = 0x4444;
            modifydata[4].Data = 0x55;
            ChineseToolStripMenuItem.Checked = true;
            //test.DecodeProcess("D:\\work\\training.hex", ref Data, ref FlahRAM, ref RecordCount, FlashBaseAddress);
            //test.TransferHexType("Intel", "S-Records", Data, "D:\\work\\training1.srec");
            //test.HexFileModify(modifydata, Data, "Intel", "D:\\work\\training2.hex");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            byte stats = GetHexFile();
            if (stats != (byte)eErrNumber.Succesful)
            {
                BusyFlagSub[5] = false;
                return;
            }
            if (ConnectFlagSub[5] == false)
                return;
            if (BusyFlagSub[5] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //BusyFlag[5] = true;
            }
            //Form_Checksum form = new Form_Checksum();
            //form.Show();
            Form form = Application.OpenForms["Form_Checksum"];  //查找是否打开过Form1窗体  
            if (form == null)  //没打开过  
            {
                Form_Checksum fa = new Form_Checksum();
                fa.ShowDialog();   //重新new一个Show出来  
            }
            else
            {
                form.Focus();   //打开过就让其获得焦点  
            }
            //Thread t = new Thread(new ThreadStart(PageErase_thread));
            //t.Start();
            //Control.CheckForIllegalCrossThreadCalls = false;
            //Button_Erase.Enabled = false;
        }

        byte GetHexFile()
        {
            hexformat = TextBox_HexFile.Text.Substring(TextBox_HexFile.Text.LastIndexOf(".") + 1, (TextBox_HexFile.Text.Length - TextBox_HexFile.Text.LastIndexOf(".") - 1));
            hexformat.ToLower();
            if (hexformat == "hex")
            {
                if (!HexProcess_obj.IntelHexFormatDecode(TextBox_HexFile.Text, ref HexFileRecord, ref HexData, ref RecordCount,
                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex],
                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex] *
                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex]))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FileFormatError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[0] = false;
                    BusyFlagSub[1] = false;
                    BusyFlagSub[2] = false;
                    BusyFlagSub[3] = false;
                    BusyFlagSub[4] = false;
                    BusyFlagSub[5] = false;
                    BusyFlagSub[6] = false;
                    BusyFlagSub[7] = false;
                    BusyFlagSub[8] = false;
                    return (byte)eErrNumber.FileFormatErr;
                }
            }
            else if (hexformat == "srec")
            {
                if (!HexProcess_obj.MotorolaHexFormatDecode(TextBox_HexFile.Text, ref HexFileRecord, ref HexData, ref RecordCount,
                                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex],
                                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex] *
                                                                         ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex]))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FileFormatError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[0] = false;
                    BusyFlagSub[1] = false;
                    BusyFlagSub[2] = false;
                    BusyFlagSub[3] = false;
                    BusyFlagSub[4] = false;
                    BusyFlagSub[5] = false;
                    BusyFlagSub[6] = false;
                    BusyFlagSub[7] = false;
                    BusyFlagSub[8] = false;
                    return (byte)eErrNumber.FileFormatErr; 
                }
            }
            else if (hexformat == "bin")
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        UInt32 Crystal = 0x00;
        int frequnecy = 0x00;
        //private void Button_Connect_Click(object sender, EventArgs e)
        //{

        //    //if (ProgaramerOrderProcess_Obj.ConnectFlag == false)
        //    //return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
        //    FrequencySlecet = ComboBox_Frequency.SelectedIndex;
        //    COMSlecet[0] = ComboBox_COM.SelectedIndex;

        //    if (ComboBox_TargetMCU.SelectedIndex < 0)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (ComboBox_Frequency.SelectedIndex < 0)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (ComboBox_COM.SelectedIndex < 0)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (Button_Start1.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
        //    {
        //        if (TextBox_HexFile.Text == "")
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
        //    if (ConnectFlag == false)
        //    {
        //        if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
        //    RecordCount = 0;
        //    /*
        //    if (hexformat == "hex")
        //    {
        //        if (!HexProcess_obj.IntelHexFormatDecode(TextBox_HexFile.Text, ref HexFileRecord, ref HexData, ref RecordCount,
        //                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex],
        //                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex] *
        //                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex]))
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FileFormatError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
        //    else if (hexformat == "srec")
        //    {
        //        if (!HexProcess_obj.MotorolaHexFormatDecode(TextBox_HexFile.Text, ref HexFileRecord, ref HexData, ref RecordCount,
        //                                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex],
        //                                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex] *
        //                                                                 ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex]))
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FileFormatError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
        //    else if (hexformat == "bin")
        //    {

        //    }
        //    */
        //    if (Button_Start1.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
        //    {
        //        byte stats = GetHexFile();
        //        if (stats != (byte)eErrNumber.Succesful)
        //        {
        //            return;
        //        }
        //    }

        //    switch (ComboBox_Frequency.SelectedIndex)
        //    {
        //        case 0:
        //            frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
        //            //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
        //            Crystal = 0;
        //            break;
                    
        //        case 1:
        //            frequnecy = (int)(9600 * 1);
        //            Crystal = 4000000;
        //            break;
        //        case 2:
        //            frequnecy = (int)(9600 * 1.5);
        //            Crystal = 6000000;
        //            break;
                    
        //        case 3:
        //            frequnecy = (int)(9600 * 2);
        //            Crystal = 8000000;
        //            break;
        //        case 4:
        //            frequnecy = (int)(9600 * 2.5);
        //            Crystal = 10000000;
        //            break;
        //        case 5:
        //            frequnecy = (int)(9600 * 3);
        //            Crystal = 12000000;
        //            break;
        //        case 6:
        //            frequnecy = (int)(9600 * 4);
        //            Crystal = 16000000;
        //            break;
        //        case 7:
        //            frequnecy = (int)(9600 * 4.5);
        //            Crystal = 18000000;
        //            break;
        //        case 8:
        //            frequnecy = (int)(9600 * 5);
        //            Crystal = 20000000;
        //            break;
        //        case 9:
        //            frequnecy = (int)(9600 * 6);
        //            Crystal = 24000000;
        //            break;
        //        case 10:
        //            frequnecy = (int)(9600 * 8);
        //            Crystal = 32000000;
        //            break;
        //    }
        //    if (SerialPort_Communiction.IsOpen == true)
        //    {
        //        try
        //        {
        //            SerialPort_Communiction.Close();
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    SerialPort_Communiction.BaudRate = frequnecy;
        //    SerialPort_Communiction.PortName = ComboBox_COM.Text;
        //    if (SerialPort_Communiction.IsOpen == false)
        //    {
        //        try
        //        {
        //            SerialPort_Communiction.Open();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Thread t = new Thread(new ThreadStart(Connect_thread));
        //    t.Start();
        //    Button_Start1.Enabled = false;

        //}
        private void Button_Start1_Click(object sender, EventArgs e)
        {

            //if (ProgaramerOrderProcess_Obj.ConnectFlag == false)
            //return;
            if (BusyFlagSub[1] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[1] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[1] = ComboBox_COM.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            if (ComboBox_COM.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            if (Button_Start1.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[1] = false;
                    return;
                }
            }
            if (ConnectFlagSub[1] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[1] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start1.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction1.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction1.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction1.BaudRate = frequnecy;
            SerialPort_Communiction1.PortName = ComboBox_COM.Text;
            if (SerialPort_Communiction1.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction1.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread1));
            t.Start();
            Button_Start1.Enabled = false;
            Button_Start2.Enabled = false;
            Button_Start3.Enabled = false;
            Button_Start4.Enabled = false;
            Button_Start5.Enabled = false;
            Button_Start6.Enabled = false;
            Button_Start7.Enabled = false;
            Button_Start8.Enabled = false;
        }
        private void Connect_thread1()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[1] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction1, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM.Text, frequnecy, Crystal);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction1);
                        ConnectControlDisplay(ConnectFlagSub[1]);
                        //Button_Start1.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        ConnectFlagSub[1] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {
                    
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction1);
                    ConnectControlDisplay(ConnectFlagSub[1]);
                    //Button_Start1.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    ConnectFlagSub[1] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;
                
                ConnectFlagSub[1] = true;
                ConnectControlDisplay(ConnectFlagSub[1]);
                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start1.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[1] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                            //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction1);
               // Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[1] = false;
                ConnectControlDisplay(ConnectFlagSub[1]);
                //Button_Start1.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[1] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun1();
            StartControlDisplay(true);
        }
        private void Connect_thread2()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[2] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction2, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM2.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                       // MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                      //                              MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction2);
                        ConnectControlDisplay(ConnectFlagSub[2]);
                        //Button_Start2.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[2] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction2);
                    ConnectControlDisplay(ConnectFlagSub[2]);
                    //Button_Start2.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[2] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[2] = true;
                ConnectControlDisplay(ConnectFlagSub[2]);
               // Button_Start2.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start2.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[2] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction2);
                //Button_Start2.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[2] = false;
                ConnectControlDisplay(ConnectFlagSub[2]);
                //Button_Start2.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[2] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun2();
            StartControlDisplay(true);
        }
        private void Connect_thread3()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[3] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction3, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM3.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction1);
                        ConnectControlDisplay(ConnectFlagSub[3]);
                        Button_Start3.Enabled = true;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[3] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction3);
                    ConnectControlDisplay(ConnectFlagSub[3]);
                    //Button_Start3.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[3] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[3] = true;
                ConnectControlDisplay(ConnectFlagSub[3]);
                //Button_Start3.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start3.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[3] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction3);
               // Button_Start3.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[3] = false;
                ConnectControlDisplay(ConnectFlagSub[3]);
               // Button_Start3.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[3] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun3();
            StartControlDisplay(true);
        }
        private void Connect_thread4()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[4] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction4, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM4.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction4);
                        ConnectControlDisplay(ConnectFlagSub[4]);
                        //Button_Start4.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[4] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction4);
                    ConnectControlDisplay(ConnectFlagSub[4]);
                    //Button_Start4.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[4] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[4] = true;
                ConnectControlDisplay(ConnectFlagSub[4]);
                //Button_Start4.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start4.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[4] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction4);
                //Button_Start4.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[4] = false;
                ConnectControlDisplay(ConnectFlagSub[4]);
                //Button_Start4.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[4] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun4();
            StartControlDisplay(true);
        }
        private void Connect_thread5()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[5] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction5, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM5.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction5);
                        ConnectControlDisplay(ConnectFlagSub[5]);
                        //Button_Start5.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[5] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction5);
                    ConnectControlDisplay(ConnectFlagSub[5]);
                    //Button_Start5.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[5] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[5] = true;
                ConnectControlDisplay(ConnectFlagSub[5]);
                //Button_Start5.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start5.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[5] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction5);
                //Button_Start5.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[5] = false;
                ConnectControlDisplay(ConnectFlagSub[5]);
                //Button_Start5.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[5] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun5();
            StartControlDisplay(true);
        }
        private void Connect_thread6()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[6] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction6, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM6.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction6);
                        ConnectControlDisplay(ConnectFlagSub[6]);
                        //Button_Start6.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[6] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction6);
                    ConnectControlDisplay(ConnectFlagSub[6]);
                    //Button_Start6.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[6] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[6] = true;
                ConnectControlDisplay(ConnectFlagSub[6]);
                //Button_Start6.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start6.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[6] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction6);
                //Button_Start6.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[6] = false;
                ConnectControlDisplay(ConnectFlagSub[6]);
                //Button_Start6.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[6] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun6();
            StartControlDisplay(true);
        }
        private void Connect_thread7()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[7] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction7, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM7.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction7);
                        ConnectControlDisplay(ConnectFlagSub[7]);
                        //Button_Start7.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[7] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction7);
                    ConnectControlDisplay(ConnectFlagSub[7]);
                    //Button_Start7.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[7] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[7] = true;
                ConnectControlDisplay(ConnectFlagSub[7]);
                //Button_Start7.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start7.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[7] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction7);
                //Button_Start7.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[7] = false;
                ConnectControlDisplay(ConnectFlagSub[7]);
                //Button_Start7.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[7] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun7();
            StartControlDisplay(true);
        }
        private void Connect_thread8()
        {
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
            byte stats = 0x00;
            if (ConnectFlagSub[8] == false)
            {

                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                if (true)
                {
                    try
                    {
                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction8, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                   ComboBox_COM8.Text, frequnecy, Crystal);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction8);
                        ConnectControlDisplay(ConnectFlagSub[8]);
                        //Button_Start8.Enabled = true;
                        StartControlDisplay(true);
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[8] = false;
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        return;
                    }
                }
                if (stats == (byte)eErrNumber.Succesful)
                {

                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction8);
                    ConnectControlDisplay(ConnectFlagSub[8]);
                    //Button_Start8.Enabled = true;
                    StartControlDisplay(true);
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlagSub[8] = false;
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    return;
                }
                /*
                stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                string cr = "";
                if (stats == (byte)eErrNumber.Succesful)
                {
                    XMLProcess x = new XMLProcess();
                    
                    for (int i = 0; i < 12; i++)
                    {
                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                    }
                    //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                }
                else
                {
                    Button_Connect.Enabled = true;
                    //BusyFlag = false;
                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    //TextBox_Info.ScrollToCaret();
                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                    //Button_Execute.Focus();
                    Control.CheckForIllegalCrossThreadCalls = true;
                    BusyFlag = false;
                    return;
                }
                byte[] sentdata = new byte[16];
                for (int i = 0;i<16;i++)
                {
                    string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
                    sentdata[i] = Convert.ToByte(ss,16);
                }

                if(cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
                {
                    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
                                                        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        Button_Connect.Enabled = true;
                        //BusyFlag = false;
                        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        //TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlag = false;
                        return;
                    }
                }
                */
                ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
                ConnectedConfigSet();
                TextBox_Info.Text = "";
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgressBar_1.Value = 0;

                ConnectFlagSub[8] = true;
                ConnectControlDisplay(ConnectFlagSub[8]);
                //Button_Start8.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
                //Button_Start8.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[8] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            else
            {
                SaveConfig();
                ConnectedConfigClear();
                if (TextBox_Info.Text == "")
                {
                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                else
                {
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                }
                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction8);
               // Button_Start8.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                ConnectFlagSub[8] = false;
                ConnectControlDisplay(ConnectFlagSub[8]);
                //Button_Start8.Enabled = true;
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[8] = false;
                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                //MessageBoxIcon.Warning);
            }
            Button_Execute_fun8();
            StartControlDisplay(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectflag"></param>
        void ConnectControlDisplay(bool connectflag)
        {
            if (connectflag)
            {
                Button_BlankCheck.Enabled = true;
                Button_Checksum.Enabled = true;
                Button_Erase.Enabled = true;
                //Button_Execute.Enabled = true;
                Button_Program.Enabled = true;
                Button_Upload.Enabled = true;
                Button_Verify.Enabled = true;
                CheckBox_Erase.Enabled = true;
                CheckBox_BlankCheck.Enabled = true;
                CheckBox_Encrypt.Enabled = true;
                CheckBox_Program.Enabled = true;
                CheckBox_Verify.Enabled = true;
                RadioButton_ChipErase.Enabled = true;
                RadioButton_PageErase.Enabled = true;
                ComboBox_COM.Enabled = false;
                ComboBox_Frequency.Enabled = false;
                ComboBox_TargetMCU.Enabled = false;
                TextBox_HexFile.Enabled = true;
                PictureBox_hexFile.Enabled = true;
                //TextBox_NumberAddress.Enabled = true;
                //TextBox_StartNumber.Enabled = true;
                //TextBox_Length.Enabled = true;
                //TextBox_Interval.Enabled = true;
                CheckBox_AutoNumber.Enabled = true;
                //Button_OpenClose.Enabled = true;
            }
            else
            {
                Button_BlankCheck.Enabled = false;
                Button_Checksum.Enabled = false;
                Button_Erase.Enabled = false;
                //Button_Execute.Enabled = false;
                Button_Program.Enabled = false;
                Button_Upload.Enabled = false;
                Button_Verify.Enabled = false;
                CheckBox_Erase.Enabled = false;
                CheckBox_BlankCheck.Enabled = false;
                CheckBox_Encrypt.Enabled = false;
                CheckBox_Program.Enabled = false;
                CheckBox_Verify.Enabled = false;
                RadioButton_ChipErase.Enabled = false;
                RadioButton_PageErase.Enabled = false;
                ComboBox_COM.Enabled = true;
                ComboBox_Frequency.Enabled = true;
                ComboBox_TargetMCU.Enabled = true;
                TextBox_HexFile.Enabled = true;
                PictureBox_hexFile.Enabled = true;
                TextBox_NumberAddress.Enabled = false;
                TextBox_StartNumber.Enabled = false;
                TextBox_Length.Enabled = false;
                TextBox_Interval.Enabled = false;
                CheckBox_AutoNumber.Enabled = false;
                Button_OpenClose.Enabled = false;
            }
        }
        void StartControlDisplay(bool busyflag)
        {
            if (busyflag)
            {
                Button_Start1.Enabled = true;
                Button_Start2.Enabled = true;
                Button_Start3.Enabled = true;
                Button_Start4.Enabled = true;
                Button_Start5.Enabled = true;
                Button_Start6.Enabled = true;
                Button_Start7.Enabled = true;
                Button_Start8.Enabled = true;
            }
            else
            {
                Button_Start1.Enabled = false;
                Button_Start2.Enabled = false;
                Button_Start3.Enabled = false;
                Button_Start4.Enabled = false;
                Button_Start5.Enabled = false;
                Button_Start6.Enabled = false;
                Button_Start7.Enabled = false;
                Button_Start8.Enabled = false;
            }
        }
        private void ControlsText()
        {
            if(ProgaramerOrderProcess_Obj.DisplayText_obj.LanguageSelect==true)
            {
                /*
                Label_TargetMCU.Location = new Point(18, 24);
                Label_Frequency.Location = new Point(18, 58);
                Label_HexFile.Location = new Point(18, 94);
                Label_COM.Location = new Point(18, 130);
                Label_StartAddress.Location = new Point(9, 31);
                Label_PageSize.Location = new Point(9, 65);
                Label_PageCount.Location = new Point(9, 100);
                Label_PassCount.Location = new Point(7, 32);
                Label_FailCount.Location = new Point(7, 69);
                label_TotalCount.Location = new Point(7, 104);
                ChineseToolStripMenuItem.Checked = true;
                EnglishToolStripMenuItem.Checked = false;
                */
            }
            else
            {
                /*
                Label_TargetMCU.Location = new Point(35, 24);
                Label_Frequency.Location = new Point(23, 57);
                Label_HexFile.Location = new Point(47, 94);
                Label_COM.Location = new Point(29, 130);
                Label_StartAddress.Location = new Point(9, 31);
                Label_PageSize.Location = new Point(33, 65);
                Label_PageCount.Location = new Point(27, 100);
                Label_PassCount.Location = new Point(13, 32);
                Label_FailCount.Location = new Point(13, 66);
                label_TotalCount.Location = new Point(7, 104);
                ChineseToolStripMenuItem.Checked = false;
                EnglishToolStripMenuItem.Checked = true;
                */

            }
            FileToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuFileText;
            //SaveToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuSaveText;
            LanguageToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuLanguageText;
            ChineseToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuChineseText;
            //EnglishToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuEnglishText;
            ExitToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuExitText;
            HelpToolStripMenuItem.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuHelpText;
            SubHelpToolStripMenuItem1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MenuSubHelpText;
            GroupBox_MCUSetting.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.GroupMCUSettingText;
            Label_TargetMCU.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.TargetMCUText;
            //Label_TargetMCU.TextAlign = Alignment.Left;
            Label_Frequency.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CrystalFrequencyText;
            Label_HexFile.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.HexFileText;
            //Label_COM.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.COMSettingText;
////            if (ConnectFlag == true)
////            {
////               //Button_Connect.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
////-+            }
////            else
////            {
////                //Button_Connect.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
////            }
            GroupBox_FlashInf.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.MCUFlashInfoText;
            Label_StartAddress.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.StartAddressText;
            Label_PageSize.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.PageSizeText;
            Label_PageCount.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.PageCountText;
            Label_NumberAddress.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.NumberAddress;
            GroupBox_Results.Text = "";
            Label_Length.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.NumberLength;
            Label_StartNumber.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.StartNumber;
            Label_Interval.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.NumberInterval;
            Label_CurrentNumber.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CurrentNumber;
            Label_HistoryNumber.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.HistoryNumber;
            CheckBox_AutoNumber.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumber;
            if (AutoNumberEnable == true)
            {
                Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberClose;
            }
            else
            {
                Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
            }
            GroupBox_Operation.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.OperationText;
            RadioButton_ChipErase.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseText;
            RadioButton_PageErase.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseText;
            Button_BlankCheck.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckText;
            Button_Erase.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.EraseText;
            Button_Program.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramText;
            Button_Verify.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyText;
            //Button_Execute.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ExecuteText;
            CheckBox_BlankCheck.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CheckBlankCheckText;
            CheckBox_Program.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CheckProgramText;
            CheckBox_Verify.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CHeckVerifyText;
            CheckBox_Encrypt.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.CheckEncryptText;
            Button_Upload.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.UploadText;
            Button_Checksum.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ChecksumText;
            Label_Version.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.Version;
            CheckBox_Erase.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.EraseCheck;
            if (ComboBox_TargetMCU.Items.Count > 0)
            {
                TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            }
            ComboBox_TargetMCU.Items.Clear();
            for (int i = 0; i < ProgaramerOrderProcess_Obj.SystemConfig_obj.MCUNumber; i++)
            {
                ComboBox_TargetMCU.Items.Add(ProgaramerOrderProcess_Obj.SystemConfig_obj.MCUName[i]);
            }
            if(ComboBox_TargetMCU.Items.Count>0)
            {
                ComboBox_TargetMCU.SelectedIndex = TargetMCUSlecet;
            }

            if (ComboBox_Frequency.Items.Count > 0)
            {
                FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            }
            else
            {
                FrequencySlecet = 1;
            }
            ComboBox_Frequency.Items.Clear();
            ComboBox_Frequency.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.FrequencyText;
            for (int i = 0; i < ProgaramerOrderProcess_Obj.DisplayText_obj.Items; i++)
            {
                ComboBox_Frequency.Items.Add(ProgaramerOrderProcess_Obj.DisplayText_obj.Item[i]);
            }
            if(ComboBox_Frequency.Items.Count>0)
            {
                ComboBox_Frequency.SelectedIndex = FrequencySlecet;
            }
            UARTProcess UARTProcess_obj = new UARTProcess();
            if (ComboBox_COM.Items.Count > 0)
            {
                COMSlecet[1] = ComboBox_COM.SelectedIndex;
            }
            else
            {
                COMSlecet[1] = 0x00;
            }

            if (ComboBox_COM2.Items.Count > 0)
            {
                COMSlecet[2] = ComboBox_COM2.SelectedIndex;
            }
            else
            {
                COMSlecet[2] = 0x00;
            }
            if (ComboBox_COM3.Items.Count > 0)
            {
                COMSlecet[3] = ComboBox_COM3.SelectedIndex;
            }
            else
            {
                COMSlecet[3] = 0x00;
            }

            if (ComboBox_COM4.Items.Count > 0)
            {
                COMSlecet[4] = ComboBox_COM4.SelectedIndex;
            }
            else
            {
                COMSlecet[4] = 0x00;
            }
            if (ComboBox_COM5.Items.Count > 0)
            {
                COMSlecet[5] = ComboBox_COM5.SelectedIndex;
            }
            else
            {
                COMSlecet[5] = 0x00;
            }

            if (ComboBox_COM6.Items.Count > 0)
            {
                COMSlecet[6] = ComboBox_COM6.SelectedIndex;
            }
            else
            {
                COMSlecet[6] = 0x00;
            }
            if (ComboBox_COM7.Items.Count > 0)
            {
                COMSlecet[7] = ComboBox_COM7.SelectedIndex;
            }
            else
            {
                COMSlecet[7] = 0x00;
            }

            if (ComboBox_COM8.Items.Count > 0)
            {
                COMSlecet[8] = ComboBox_COM8.SelectedIndex;
            }
            else
            {
                COMSlecet[8] = 0x00;
            }



            ComboBox_COM.Items.Clear();
            ComboBox_COM2.Items.Clear();
            ComboBox_COM3.Items.Clear();
            ComboBox_COM4.Items.Clear();
            ComboBox_COM5.Items.Clear();
            ComboBox_COM6.Items.Clear();
            ComboBox_COM7.Items.Clear();
            ComboBox_COM8.Items.Clear();
            UARTProcess_obj.GetPortNames(ComboBox_COM);
            UARTProcess_obj.GetPortNames(ComboBox_COM2);
            UARTProcess_obj.GetPortNames(ComboBox_COM3);
            UARTProcess_obj.GetPortNames(ComboBox_COM4);
            UARTProcess_obj.GetPortNames(ComboBox_COM5);
            UARTProcess_obj.GetPortNames(ComboBox_COM6);
            UARTProcess_obj.GetPortNames(ComboBox_COM7);
            UARTProcess_obj.GetPortNames(ComboBox_COM8);
            if (ComboBox_COM.Items.Count > 0)
            {
                ComboBox_COM.SelectedIndex = COMSlecet[1];
            }
            if (ComboBox_COM2.Items.Count > 0)
            {
                ComboBox_COM2.SelectedIndex = COMSlecet[2];
            }
            if (ComboBox_COM3.Items.Count > 0)
            {
                ComboBox_COM3.SelectedIndex = COMSlecet[3];
            }
            if (ComboBox_COM4.Items.Count > 0)
            {
                ComboBox_COM4.SelectedIndex = COMSlecet[4];
            }
            if (ComboBox_COM5.Items.Count > 0)
            {
                ComboBox_COM5.SelectedIndex = COMSlecet[5];
            }
            if (ComboBox_COM6.Items.Count > 0)
            {
                ComboBox_COM6.SelectedIndex = COMSlecet[6];
            }
            if (ComboBox_COM7.Items.Count > 0)
            {
                ComboBox_COM7.SelectedIndex = COMSlecet[7];
            }
            if (ComboBox_COM8.Items.Count > 0)
            {
                ComboBox_COM8.SelectedIndex = COMSlecet[8];
            }
            //TextBox_Length.Text = FailCount.ToString();
            //TextBox_NumberAddress.Text = PassCount.ToString();
            //TextBox_StartNumber.Text = TotalCount.ToString();
            //if (ConnectFlag == false)
            //{
            //    //RadioButton_ChipErase.Checked = true;
            //    Button_BlankCheck.Enabled = false;
            //    Button_Checksum.Enabled = false;
            //    Button_Erase.Enabled = false;
            //    Button_Execute.Enabled = false;
            //    Button_Program.Enabled = false;
            //    Button_Upload.Enabled = false;
            //    Button_Verify.Enabled = false;
            //    CheckBox_Erase.Enabled = false;
            //    CheckBox_BlankCheck.Enabled = false;
            //    CheckBox_Encrypt.Enabled = false;
            //    CheckBox_Program.Enabled = false;
            //    CheckBox_Verify.Enabled = false;
            //    RadioButton_ChipErase.Enabled = false;
            //    RadioButton_PageErase.Enabled = false;
            //    TextBox_NumberAddress.Enabled = false;
            //    TextBox_StartNumber.Enabled = false;
            //    TextBox_Length.Enabled = false;
            //    TextBox_Interval.Enabled = false;
            //    CheckBox_AutoNumber.Enabled = false;
            //    Button_OpenClose.Enabled = false;
            //}
            //else
            //{
            //    //RadioButton_ChipErase.Checked = true;
            //    Button_BlankCheck.Enabled = true;
            //    Button_Checksum.Enabled = true;
            //    Button_Erase.Enabled = true;
            //    Button_Execute.Enabled = true;
            //    Button_Program.Enabled = true;
            //    Button_Upload.Enabled = true;
            //    Button_Verify.Enabled = true;
            //    CheckBox_Erase.Enabled = true;
            //    CheckBox_BlankCheck.Enabled = true;
            //    CheckBox_Encrypt.Enabled = true;
            //    CheckBox_Program.Enabled = true;
            //    CheckBox_Verify.Enabled = true;
            //    RadioButton_ChipErase.Enabled = true;
            //    RadioButton_PageErase.Enabled = true;
            //    //TextBox_NumberAddress.Enabled = true;
            //    //TextBox_StartNumber.Enabled = true;
            //    //TextBox_Length.Enabled = true;
            //    //TextBox_Interval.Enabled = true;
            //    CheckBox_AutoNumber.Enabled = true;
            //    //Button_OpenClose.Enabled = true;
            //}
            //ComboBox_Frequency.SelectedIndex = 0x01;
            
            //TextBox_StartAdess.Text =Convert.ToString(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[],16);
        }

        private void ComboBox_TargetMCU_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBox_StartAdess.Text = Convert.ToString(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex], 16).ToUpper().PadLeft(8, '0') + "H";
            TextBox_PageSize.Text = ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex].ToString();
            TextBox_PageCount.Text = ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex].ToString();
        }

        UInt32 NumberAdderss = 0x00;
        UInt32 StartNumber = 0x00;
        UInt32 CurrentNumber = 0x00;
        UInt32 HistoryNumber = 0x00;
        UInt32 NumberLength = 0x00;
        UInt32 NumberInterval = 0x00;
        bool AutoNumberEnable = false;

        private void Button_Clear_Click(object sender, EventArgs e)
        {
        //    //if (ConnectFlag == false)
        //    //    return;
        //    if (BusyFlagSub[1] == true || BusyFlagSub[2]|| BusyFlagSub[3] || BusyFlagSub[4] || BusyFlagSub[5] || BusyFlagSub[6] || BusyFlagSub[7] || BusyFlagSub[8])
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    if (Button_OpenClose.Text == ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen)
        //    {
        //        if (TextBox_NumberAddress.Text == "")
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.EmptyFill, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_NumberAddress.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (TextBox_StartNumber.Text == "")
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.EmptyFill, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_StartNumber.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (TextBox_Length.Text == "")
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.EmptyFill, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_Length.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (TextBox_Interval.Text == "")
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.EmptyFill, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_Interval.Focus();

        //            BusyFlag = false;
        //            return;
        //        }
        //        try
        //        {
        //            NumberAdderss = Convert.ToUInt32(TextBox_NumberAddress.Text, 16);
        //        }
        //        catch
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FormatErr, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_NumberAddress.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        try
        //        {

        //            StartNumber = Convert.ToUInt32(TextBox_StartNumber.Text, 10);
        //        }
        //        catch
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FormatErr, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_StartNumber.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        try
        //        {

        //            NumberLength = Convert.ToUInt32(TextBox_Length.Text, 10);
        //        }
        //        catch
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FormatErr, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_Length.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        try
        //        {
                    
        //            NumberInterval = Convert.ToUInt32(TextBox_Interval.Text, 10);
        //        }
        //        catch
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FormatErr, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_Interval.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        if ((ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] +
        //            ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[ComboBox_TargetMCU.SelectedIndex] *
        //            ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[ComboBox_TargetMCU.SelectedIndex]) < (NumberAdderss + 4))
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.FormatErr, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //            TextBox_StartNumber.Focus();
        //            BusyFlag = false;
        //            return;
        //        }
        //        CurrentNumber = StartNumber;
        //        Label_CurrentNumbers.Text = CurrentNumber.ToString();
        //        TextBox_NumberAddress.Enabled = false;
        //        TextBox_StartNumber.Enabled = false;
        //        TextBox_Interval.Enabled = false;
        //        TextBox_Length.Enabled = false;
        //        Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberClose;
        //        AutoNumberEnable = true;
        //    }
        //    else
        //    {
        //        TextBox_NumberAddress.Enabled = true;
        //        TextBox_StartNumber.Enabled = true;
        //        TextBox_Interval.Enabled = true;
        //        TextBox_Length.Enabled = true;
        //        AutoNumberEnable = false;
        //        Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
        //    }
        //    BusyFlag = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog_ISP.Filter = "Hex(*.hex)|*.hex|Srec(*.srec)|*.srec|Bin(*.bin)|*.bin";
            OpenFileDialog_ISP.FileName = "";
            DialogResult dr = OpenFileDialog_ISP.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TextBox_HexFile.Text = OpenFileDialog_ISP.FileName;
                //TextBox_OnlineProFile.TextAlign = HorizontalAlignment.Left;
                //TextBox_OnlineProFile.ForeColor = Color.Black;
                if(OpenFileDialog_ISP.FilterIndex == 0x01)
                {
                    hexformat = "hex";
                }
                else if (OpenFileDialog_ISP.FilterIndex == 0x02)
                {
                    hexformat = "srec";
                }
                else if (OpenFileDialog_ISP.FilterIndex == 0x03)
                {
                    hexformat = "bin";
                }
            }
        }

        //private void Button_Erase_Click(object sender, EventArgs e)
        //{
        //    if (ConnectFlag == false)
        //        return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    if (RadioButton_ChipErase.Checked == true)
        //    {
        //        Thread t = new Thread(new ThreadStart(ChipErase_thread));
        //        t.Start();
        //    }
        //    else if (RadioButton_PageErase.Checked == true)
        //    {
        //        Thread t = new Thread(new ThreadStart(PageErase_thread));
        //        t.Start();
        //    }

        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Button_Erase.Enabled = false;
        //    //t.Start();
        //}


        //void ChipErase_thread()
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
        //    byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->"+ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    //byte[] ReceiveData = new byte[100];
        //    if (SerialPort_Communiction.BytesToRead > 0)
        //    {
        //        byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //        byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //        SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //    }
        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    //byte[] CRadd = 
        //    string cr = "";
        //    /*
        //        byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
        //                                                    BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

        //        if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        XMLProcess x = new XMLProcess();
        //        cr = "";
        //        for (int i = 0; i < 16; i++)
        //        {
        //            cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
        //        }
        //        x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
        //    }
        //    else
        //    {
        //        Button_Erase.Enabled = true;
        //        //BusyFlag = false;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }*/
        //    /*
        //    stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
            
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        XMLProcess x = new XMLProcess();
        //        cr = "";
        //        for (int i = 0; i < 12; i++)
        //        {
        //            cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
        //        }
        //        x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
        //    }
        //    else
        //    {
        //        Button_Erase.Enabled = true;
        //        //BusyFlag = false;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Execute.Focus();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }*/

        //    byte Result = 0;
        //    byte stats = (byte)eErrNumber.Succesful;
        //    try
        //    {
        //        stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction, ref Result);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Erase.Enabled = true;
        //        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        //TextBox_Info.ScrollToCaret();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        /*if (Result == 0x01)
        //        {
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //            //MessageBoxIcon.Error);
        //            Button_Upload.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }*/
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Erase.Enabled = true;
        //        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        //TextBox_Info.ScrollToCaret();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }

        //    try
        //    {
        //        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction);
        //    }
        //    catch(Exception e) 
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Erase.Enabled = true;
        //        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        //TextBox_Info.ScrollToCaret();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        /*
        //        Button_Erase.Enabled = true;
        //        //BusyFlag = false;
        //        ChipEraseFlag = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgressBar_1.Value = 100;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        */
                
        //    }
        //    else
        //    {
        //        Button_Erase.Enabled = true;
        //        //BusyFlag = false;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
        //                                                //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
        //    //CRdata =new byte[]{ 0xf0,0x41,0x42,0x08,0x00,0x55,0x00,0x00,0x00,0x08,0xbd,0x00,0x00,0x00,0x00,0x00 };
        //    /*stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
        //    //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
        //                                                //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {

        //    }
        //    else
        //    {
        //        Button_Erase.Enabled = true;
        //        //BusyFlag = false;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    */

        //    if (Result == 0x01)
        //    {
        //        try
        //        {
        //            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction);
        //        }
        //        catch(Exception e)
        //        {
        //            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Button_Erase.Enabled = true;
        //            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            //TextBox_Info.ScrollToCaret();
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }

        //        if (stats == (byte)eErrNumber.Succesful)
        //        {

        //        }
        //        else
        //        {
        //            Button_Erase.Enabled = true;
        //            //BusyFlag = false;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }

        //        //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
        //        //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
        //        //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
        //        if (true)
        //        {
        //            try
        //            {
        //                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
        //                                                           ComboBox_COM.Text, frequnecy, Crystal);
        //            }
        //            catch (Exception e)
        //            {
        //                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                MessageBoxIcon.Error);
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                Button_Erase.Enabled = true;
        //                //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //                //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                //TextBox_Info.ScrollToCaret();
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                BusyFlag = false;
        //                return;
        //            }
        //        }
        //        if (stats == (byte)eErrNumber.Succesful)
        //        {

        //        }
        //        else
        //        {
        //            Button_Erase.Enabled = true;
        //            ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
        //            ConnectControlDisplay(ConnectFlag);
        //            //Button_Connect.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            return;
        //        }
        //    }
        //    /*
        //    stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
        //    cr = "";
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        XMLProcess x = new XMLProcess();

        //        for (int i = 0; i < 12; i++)
        //        {
        //            cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
        //        }
        //        //x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
        //    }
        //    else
        //    {
        //        Button_Connect.Enabled = true;
        //        //BusyFlag = false;
        //        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        //TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        //Button_Execute.Focus();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    byte[] sentdata = new byte[16];
        //    for (int i = 0; i < 16; i++)
        //    {
        //        string ss = ProgaramerOrderProcess_Obj.SystemConfig_obj.UserArea[ComboBox_TargetMCU.SelectedIndex].Substring(i * 2, 2);
        //        sentdata[i] = Convert.ToByte(ss, 16);
        //    }

        //    if (cr == ProgaramerOrderProcess_Obj.SystemConfig_obj.UID[ComboBox_TargetMCU.SelectedIndex])
        //    {
        //        //stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction,
        //        //BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);
        //        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex])), sentdata);
        //        if (stats == (byte)eErrNumber.Succesful)
        //        {

        //        }
        //        else
        //        {
        //            Button_Connect.Enabled = true;
        //            //BusyFlag = false;
        //            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            //TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            //Button_Execute.Focus();
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //    }*/
        //    //ConnectedConfigSet();
        //    /*
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectSuccessful;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    */
        //    //ProgressBar_1.Value = 0;
        //    //TextBox_Info.Text = "";
        //    //ConnectFlag = true;
        //    //ConnectControlDisplay(ConnectFlag);
        //    //Button_Connect.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText;
        //    //Button_Connect.Enabled = true;
        //    //Control.CheckForIllegalCrossThreadCalls = true;
        //    //BusyFlag = false;


        //    Button_Erase.Enabled = true;
        //    //BusyFlag = false;
        //    ChipEraseFlag = true;
        //    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
        //    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //    TextBox_Info.ScrollToCaret();
        //    ProgressBar_1.Value = 100;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    checkAutoNumber = false;
        //    Control.CheckForIllegalCrossThreadCalls = true;
        //    BusyFlag = false;
        //}

        //void PageErase_thread()
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->"+ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    float interval = (float)(100000/(HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex] );
        //    for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
        //    {
        //        byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
        //                                                   + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
        //        byte stats = (byte)eErrNumber.Succesful;
        //        try
        //        {
        //            stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction, pageaddress);
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            Button_Erase.Enabled = true;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (stats == (byte)eErrNumber.Succesful)
        //        {
        //            ChipEraseFlag = false;
                    
        //            ProgressBar_1.Value = (int)((i+1)* interval/1000);
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        }
        //        else
        //        {
                    
        //            Button_Erase.Enabled = true;
        //            string s = "";
        //            if (stats == (byte)eErrNumber.Encrypted)
        //            {
        //                s = ": "+ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //            }
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail +s;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
        //    //if(NumberAdd <= HexData.Length+ ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]-1)
        //    //{
        //        checkAutoNumber = false;
        //    //}
        //    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
        //    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //    TextBox_Info.ScrollToCaret();
        //    ProgressBar_1.Value = 100;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                    //MessageBoxIcon.Warning);
        //    Button_Erase.Enabled = true;
            
        //    Control.CheckForIllegalCrossThreadCalls = true;
        //    BusyFlag = false;
        //}


        //private void Button_BlankCheck_Click(object sender, EventArgs e)
        //{
        //    if (ConnectFlag == false)
        //        return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }

        //    Thread t = new Thread(new ThreadStart(BlankCHeck_thread));
        //    t.Start();
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Button_BlankCheck.Enabled = false;
        //}


        //void BlankCHeck_thread()
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    byte result = 0x00;
        //    UInt32 Lenght = 0x00;
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->"+ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    //byte[] ReceiveData = new byte[100];
        //    if (SerialPort_Communiction.BytesToRead > 0)
        //    {
        //        byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //        byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //        SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //    }
        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    if (ChipEraseFlag == true)
        //    {
        //        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
        //                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //    }
        //    else
        //    {
        //        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
        //                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //    }
        //    UInt32 ErrAdd = 0x00;
        //    byte stats = (byte)eErrNumber.Succesful;
        //    try
        //    {
        //        stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        Button_BlankCheck.Enabled = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {

        //    }
        //    else
        //    {
        //        string ss = "";
        //        if (stats == (byte)eErrNumber.Encrypted)
        //        {
        //            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //        }
        //        Button_BlankCheck.Enabled = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail +ss;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
            
        //    string s = "";
        //    if(result == 0x01)
        //    {
        //        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
        //    }
        //    else if(result == 0x00)
        //    {
        //        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank+"0x"+Convert.ToString(ErrAdd,16).ToUpper().PadLeft(8,'0');
        //    }
        //    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful+s ;
        //    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //    TextBox_Info.ScrollToCaret();
        //    ProgressBar_1.Value = 100;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful+s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                    //MessageBoxIcon.Warning);
            
        //    Button_BlankCheck.Enabled = true;
        //    Control.CheckForIllegalCrossThreadCalls = true;
        //    BusyFlag = false;
        //}

        //private void Button_Program_Click(object sender, EventArgs e)
        //{
        //    TotalCount++;
        //    //TextBox_StartNumber.Text = TotalCount.ToString();
        //    if (ConnectFlag == false)
        //        return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    byte stats = GetHexFile();
        //    if (stats != (byte)eErrNumber.Succesful)
        //    {
        //        BusyFlag = false;
        //        return;
        //    }
        //    Thread t = new Thread(new ThreadStart(Button_Program_thread));
        //    t.Start();
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Button_Program.Enabled = false;
        //    TextBox_HexFile.Enabled = false;
        //    PictureBox_hexFile.Enabled = false;
        //}

        bool checkAutoNumber = false;
        UInt32 NumberAdd = 0x00000000;
        //void Button_Program_thread()
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    byte stats = 0x00;
        //    byte[] sentdata = new byte[512];
        //    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->"+ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    //byte[] ReceiveData = new byte[100];
        //    if (SerialPort_Communiction.BytesToRead > 0)
        //    {
        //        byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //        byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //        SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //    }
        //    if (CheckBox_AutoNumber.Checked == true)
        //    {
        //        if(AutoNumberEnable == false)
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            TextBox_HexFile.Enabled = true;
        //            PictureBox_hexFile.Enabled = true;
        //            Button_Program.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //        else
        //        {
        //            if((NumberAdderss- ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])< HexData.Length)
        //            {
        //                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
        //                if(dr == DialogResult.No)
        //                {
        //                    TextBox_HexFile.Enabled = true;
        //                    PictureBox_hexFile.Enabled = true;
        //                    Button_Program.Enabled = true;
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                else if(dr == DialogResult.Yes)
        //                {
        //                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
        //                    for(int i = 0;i<4;i++)
        //                    {
        //                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
        //                        {
        //                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]+i] = CurrentNumData[i];
        //                        }
        //                    }
        //                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
        //                    {
        //                        byte[] senddata = new byte[1];
        //                        senddata[0] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);

        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Program.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {

        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Program.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
        //                    {
        //                        byte[] senddata = new byte[2];
        //                        senddata[0] = CurrentNumData[2];
        //                        senddata[1] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);

        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Program.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {

        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Program.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
        //                    {
        //                        byte[] senddata = new byte[3];
        //                        senddata[0] = CurrentNumData[1];
        //                        senddata[1] = CurrentNumData[2];
        //                        senddata[2] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Program.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {

        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Program.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
        //                }
        //                catch (Exception e)
        //                {
        //                    MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                    MessageBoxIcon.Error);
        //                    Button_Program.Enabled = true;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {

        //                }
        //                else
        //                {
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    //TextBox_Length.Text = FailCount.ToString();
        //                    TextBox_HexFile.Enabled = true;
        //                    PictureBox_hexFile.Enabled = true;
        //                    Button_Program.Enabled = true;
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //            }
        //            NumberAdd = NumberAdderss;
        //            checkAutoNumber = true;
        //        }
        //    }
        //    else
        //    {
        //        //checkAutoNumber = false;
        //    }

        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    float interval = (float)(100000/count);
        //    for (int i = 0; i < count; i++)
        //    {
        //        if (i == (count - 1))
        //        {
        //            sentdata = new byte[HexData.Length - (i * 512)];
        //            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
        //        }
        //        else
        //        {
        //            Array.Copy(HexData, i * 512, sentdata, 0, 512);
        //        }
        //        try
        //        {
        //            stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            Button_Program.Enabled = true;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (stats == (byte)eErrNumber.Succesful)
        //        {
        //            ProgressBar_1.Value = (int)((i + 1) * interval/1000);
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        }
        //        else
        //        {
        //            string s = "";
        //            if (stats == (byte)eErrNumber.Encrypted)
        //            {
        //                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //            }
        //            FailCount++;
        //            Button_Program.Enabled = true;

        //            TextBox_HexFile.Enabled = true;
        //            PictureBox_hexFile.Enabled = true;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail +s;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            //TextBox_Length.Text = FailCount.ToString();
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
                    
        //            return;
        //        }
        //    }
        //    PassCount++;
        //    Button_Program.Enabled = true;
        //    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful ;
        //    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //    TextBox_Info.ScrollToCaret();
        //    ProgressBar_1.Value = 100;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                    //MessageBoxIcon.Warning);
        //    //TextBox_NumberAddress.Text = PassCount.ToString();
        //    if (AutoNumberEnable == true&& CheckBox_AutoNumber.Checked == true)
        //    {
        //        HistoryNumber = CurrentNumber;
        //        CurrentNumber += NumberInterval;

        //        if (!(CurrentNumber < (StartNumber + (NumberLength)* NumberInterval)))
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            CheckBox_AutoNumber.Checked = false;
        //            AutoNumberEnable = false;
        //            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
        //            Button_OpenClose.Enabled = false;
        //        }
        //        else
        //        {
        //            Label_CurrentNumbers.Text = CurrentNumber.ToString();
        //        }
        //        Label_HistoryNumbers.Text = HistoryNumber.ToString();
        //    }

        //    TextBox_HexFile.Enabled = true;
        //    PictureBox_hexFile.Enabled = true;
        //    Control.CheckForIllegalCrossThreadCalls = true;
        //    BusyFlag = false;
        //}

        //private void Button_Verify_Click(object sender, EventArgs e)
        //{
        //    if (ConnectFlag == false)
        //        return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    byte stats = GetHexFile();
        //    if (stats != (byte)eErrNumber.Succesful)
        //    {
        //        BusyFlag = false;
        //        return;
        //    }
        //    Thread t = new Thread(new ThreadStart(Button_Verify_thread));
        //    t.Start();

        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Button_Verify.Enabled = false;
        //    TextBox_HexFile.Enabled = false;
        //    PictureBox_hexFile.Enabled = false;
        //}

        //void Button_Verify_thread()
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    byte[] checksumdata = new byte[2];
        //    UInt16 checksumdata1 = 0x00;
        //    byte stats = 0x00;
        //    //byte[] ReceiveData = new byte[100];
        //    if (SerialPort_Communiction.BytesToRead > 0)
        //    {
        //        byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //        byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //        SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //    }
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    if (checkAutoNumber == true)
        //    {
        //        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
        //        //if (AutoNumberEnable == false)
        //        if (false)
        //        {
        //            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            TextBox_HexFile.Enabled = true;
        //            PictureBox_hexFile.Enabled = true;
        //            Button_Verify.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //        else
        //        {
        //            if ((NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
        //            {
        //                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                if (dr == DialogResult.No)
        //                {
        //                    return;
        //                }
        //                else */
        //                if (true)
        //                {

        //                    for (int i = 0; i < 4; i++)
        //                    {
        //                        if ((NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
        //                        {
        //                            HexData[NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]+i] = CurrentNumData[i];
        //                        }
        //                    }
        //                    if (HexData.Length - (NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
        //                    {
        //                        byte[] senddata = new byte[1];
        //                        byte[] recdata = new byte[2];
        //                        senddata[0] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdd + 3)), recdata.Length, ref recdata);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Verify.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {
        //                            string s = "";
        //                            /*
        //                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->"+"自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            else
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            */
        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Verify.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                    else if (HexData.Length - (NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
        //                    {
        //                        byte[] senddata = new byte[2];
        //                        byte[] recdata = new byte[2];
        //                        senddata[0] = CurrentNumData[2];
        //                        senddata[1] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdd + 2)), recdata.Length, ref recdata);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Verify.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {
        //                            string s = "";
        //                            /*
        //                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            else
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            */
        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Verify.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                    else if (HexData.Length - (NumberAdd - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
        //                    {
        //                        byte[] senddata = new byte[3];
        //                        byte[] recdata = new byte[2];
        //                        senddata[0] = CurrentNumData[1];
        //                        senddata[1] = CurrentNumData[2];
        //                        senddata[2] = CurrentNumData[3];
        //                        try
        //                        {
        //                            stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdd + 1)), recdata.Length, ref recdata);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                            MessageBoxIcon.Error);
        //                            Button_Verify.Enabled = true;
        //                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail;
        //                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                            TextBox_Info.ScrollToCaret();
        //                            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {
        //                            string s = "";
        //                            /*
        //                            if (((recdata[0] << 8) + recdata[1]) == ((((HistoryNumber & 0xff0000) >> 16) + (HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            else
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            */
        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Verify.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                byte[] recdata = new byte[2];
        //                byte[] re = new byte[4];
        //                try
        //                {
        //                    stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdd)), re.Length, ref re);
        //                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdd)), recdata.Length, ref recdata);
        //                }
        //                catch (Exception e)
        //                {
        //                    MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                                    MessageBoxIcon.Error);
        //                    Button_Verify.Enabled = true;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {
        //                    string s = "";
        //                    /*
        //                    if (((recdata[0] << 8) + recdata[1]) == ((((HistoryNumber & 0xff000000) >> 24) + (((HistoryNumber & 0xff0000) >> 16) + (HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff)))
        //                    {
        //                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                        TextBox_Info.ScrollToCaret();
        //                    }
        //                    else
        //                    {
        //                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                        TextBox_Info.ScrollToCaret();
        //                    }
        //                    */
        //                }
        //                else
        //                {
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    //TextBox_Length.Text = FailCount.ToString();
        //                    TextBox_HexFile.Enabled = true;
        //                    PictureBox_hexFile.Enabled = true;
        //                    Button_Verify.Enabled = true;
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //            }
        //        }
        //    }

        //    checksumdata1 = ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length);
        //    try
        //    {
        //        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);

        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        Button_Verify.Enabled = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }


        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        string s = "";
        //        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
        //        {
        //            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful+s +"(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //        }
        //        else
        //        {
        //            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s ;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //        }
        //        //BusyFlag = false;
        //        ChipEraseFlag = true;
        //        ProgressBar_1.Value = 100;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s+"(0x"+ Convert.ToString(((checksumdata[0] << 8) + checksumdata[1] ),16).ToUpper().PadLeft(4,'0')+ ")", ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        //MessageBoxIcon.Warning);
        //        Button_Verify.Enabled = true;
        //    }
        //    else
        //    {
        //        string s = "";
        //        if (stats == (byte)eErrNumber.Encrypted)
        //        {
        //            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //        }
        //        //BusyFlag = false;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail+s ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Verify.Enabled = true;
        //    }
        //    TextBox_HexFile.Enabled = true;
        //    PictureBox_hexFile.Enabled = true;
        //    Control.CheckForIllegalCrossThreadCalls = true;
        //    BusyFlag = false;
        //}

        //private void Button_Upload_Click(object sender, EventArgs e)
        //{
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    if (ConnectFlag == false)
        //        return;
        //    if (BusyFlag == true)
        //    {
        //        MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        BusyFlag = true;
        //    }
        //    t1.Tick += t1_Tick;
        //    t1.Interval = 10;
        //    t1.Start();

        //    Thread t = new Thread(new ThreadStart(Button_Upload_thread));
        //    t.Start();
        //    Control.CheckForIllegalCrossThreadCalls = false;
        //    Button_Upload.Enabled = false;
        //}
        bool comp = false;
        byte[] recivedata = new byte[1];

        void t1_Tick(object sender, EventArgs e)
        {
            //HexSrec_Process HexProcess_obj = new HexSrec_Process();
            //FrameFormatStr[] Data = new FrameFormatStr[1];

            //if (comp == true)
            //{
            //    comp = false;
            //    t1.Stop();
            //    SaveFileDialog_ISP.Filter = "Hex(*.hex)|*.hex|Srec(*srec)|*srec";
            //    SaveFileDialog_ISP.FileName = "";
            //    //DialogResult dr = ;
            //    BinProcess BinProcessobj = new BinProcess();
            //    if (SaveFileDialog_ISP.ShowDialog() == DialogResult.OK)
            //    {
            //        HexProcess_obj.Bin2Hex(recivedata, ref Data);
            //        if (SaveFileDialog_ISP.FilterIndex == 0x01)
            //        {
            //            string xchar = SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 1, 1);
            //            string echar = SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 2, 1);
            //            string hchar = SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 3, 1);
            //            string dot = SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 4, 1);
            //            if ((xchar != "x" && xchar != "X") || (echar != "e" && echar != "E")|| (hchar != "h" && hchar != "H") || dot != ".")
            //            {
            //                SaveFileDialog_ISP.FileName += ".hex";
            //                HexProcess_obj.WriteIntelHex(Data, SaveFileDialog_ISP.FileName);
            //            }
            //            else
            //            {
            //                HexProcess_obj.WriteIntelHex(Data, SaveFileDialog_ISP.FileName);
            //            }
            //        }
            //        else if (SaveFileDialog_ISP.FilterIndex == 0x02)
            //        {
            //            HexProcess_obj.Intel2Motorola(ref Data);
            //            if ((SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 1, 1) != "c"
            //                && SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 1, 1) != "C")
            //                || (SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 2, 1) != "e"
            //                && SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 2, 1) != "E")
            //                || (SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 3, 1) != "r"
            //                && SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 3, 1) != "R")
            //                || (SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 4, 1) != "s"
            //                && SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 4, 1) != "S")
            //                || SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length - 5, 1) != ".")
            //            {
            //                SaveFileDialog_ISP.FileName += ".srec";
                            
            //                HexProcess_obj.WriteMotorolaHex(Data, SaveFileDialog_ISP.FileName);
            //            }
            //            else
            //            {
            //                HexProcess_obj.WriteMotorolaHex(Data, SaveFileDialog_ISP.FileName);
            //            }
            //        }
            //        /*
            //        if(SaveFileDialog_ISP.FileName.Substring(SaveFileDialog_ISP.FileName.Length-4,4) == "srec")
            //        {
            //            HexProcess_obj.WriteMotorolaHex(Data, SaveFileDialog_ISP.FileName);
            //        }
            //        else
            //        {
            //            HexProcess_obj.WriteIntelHex(Data, SaveFileDialog_ISP.FileName);
            //        }
            //        */
            //        //BinProcessobj.WriteBinFile(recivedata, 0, (int)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
            //                                   //ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]), SaveFileDialog_ISP.FileName);
            //    }

            //    ProgressBar_1.Value = 100;
            //    label2.Text = ProgressBar_1.Value.ToString() + "%";
            //    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
            //                                        //MessageBoxIcon.Warning);
            //    BusyFlag = false;
                
            //    Control.CheckForIllegalCrossThreadCalls = true;
            //}
        }

        //void Button_Upload_thread()
        //{
        //    //byte[] ReceiveData = new byte[100];
        //    if (SerialPort_Communiction.BytesToRead > 0)
        //    {
        //        byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //        byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //        SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //    }
        //    if (TextBox_Info.Text == "")
        //    {
        //        TextBox_Info.Text += "<--" + DateTime.Now + "-->"+ProgaramerOrderProcess_Obj.DisplayText_obj.StartUpload ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartUpload ;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //    }
        //    recivedata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
        //                                       ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]];
        //    byte[] recivedata1 = new byte[512];
        //    int lenght = 0x00;
        //    byte Result = 0x00;
        //    int count = (int)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex]);
        //    ProgressBar_1.Value = 0;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    byte stats = (byte)eErrNumber.Succesful;
        //    try
        //    {
        //        stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction, ref Result);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        Button_Upload.Enabled = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail;
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    if (stats == (byte)eErrNumber.Succesful)
        //    {
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        if (Result == 0x01)
        //        {
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail+ ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess((byte)eErrNumber.Encrypted);
        //            //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //            //MessageBoxIcon.Error);
        //            Button_Upload.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }

        //    }
        //    else
        //    {
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail ;
        //        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Button_Upload.Enabled = true;
        //        //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //        //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        //TextBox_Info.ScrollToCaret();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }
        //    ProgressBar_1.Value = 5;
        //    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //    int temp = ProgressBar_1.Value;
        //    double interval = (double)(90000/(count));
        //    for (int i = 0; i < count; i++)
        //    {
        //        //if (i == (count - 1))
        //        //{
        //            //lenght = HexData.Length - (i * 512);
        //            //recivedata1 = new byte[lenght];
        //        //}
        //        //else
        //        //{
        //        if(i == 111)
        //        {
        //            ;
        //        }
        //            lenght = 512;
        //        //}
        //        try
        //        {
        //            stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), lenght, ref recivedata1);

        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                            MessageBoxIcon.Error);
        //            Button_Upload.Enabled = true;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //        if (stats == (byte)eErrNumber.Succesful)
        //        {
        //            //BusyFlag = false;
        //            //ProgaramerOrderProcess_Obj.serialportclose();
        //            Array.Copy(recivedata1, 0, recivedata, i * 512, lenght);

        //            ProgressBar_1.Value = (int)((i+1)*interval/1000 + temp);
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        }
        //        else
        //        {
        //            //ProgaramerOrderProcess_Obj.serialportclose();
        //            //BusyFlag = false;
        //            label1.Text = i.ToString();
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //            Button_Upload.Enabled = true;
        //            Control.CheckForIllegalCrossThreadCalls = true;
        //            BusyFlag = false;
        //            return;
        //        }
        //    }
            
        //    Button_Upload.Enabled = true;
        //    //Control.CheckForIllegalCrossThreadCalls = true;
        //    comp = true;
        //    //BusyFlag = false;
        //}

        //private void SerialPort_Communiction_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        //{
        //    return;
        //    int readdata = 0x00;
        //    int receivedBytesThreshold = 0x00;
        //    int index = 0x00;
        //    //if (SerialPort_Communiction.IsOpen == false)
        //    //{
        //    //SerialPort_Communiction.Open();
        //    //}
        //    if(ProgaramerOrderProcess_Obj.serialstop)
        //    {
        //        return;
        //    }
        //    if(SerialPort_Communiction.IsOpen == false)
        //    {
        //        return;
        //    }
        //    int l = 0x00;
        //    receivedBytesThreshold = SerialPort_Communiction.ReceivedBytesThreshold;
        //    ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
        //    if (SerialPort_Communiction.BytesToRead < receivedBytesThreshold)
        //    {
        //        return;
        //        l = SerialPort_Communiction.BytesToRead;
        //    }
        //    try
        //    {


        //        readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction.ReceivedBytesThreshold);
        //    }
        //    catch (Exception e3)
        //    {
        //        MessageBox.Show("hint", e3.Message);
        //    }
        //    /*
        //    receivedBytesThreshold -= readdata;
        //    index = readdata;
        //    while (receivedBytesThreshold != 0)
        //    {
        //        readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
        //        index += readdata;

        //        receivedBytesThreshold -= readdata;
        //    }
        //    */
        //    ProgaramerOrderProcess_Obj.ReceiveFlag = true;
        //    SerialPort_Communiction.DiscardInBuffer();
        //}

        private void SerialPort_Communiction1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction1.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction1.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction1.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction1.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction1.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction1.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction1.DiscardInBuffer();
        }
        private void SerialPort_Communiction2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction2.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction2.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction2.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction2.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction2.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction2.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction2.DiscardInBuffer();
        }
        private void SerialPort_Communiction3_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction3.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction3.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction3.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction3.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction3.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction3.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction3.DiscardInBuffer();
        }
        private void SerialPort_Communiction4_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction4.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction4.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction4.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction4.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction4.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction4.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction4.DiscardInBuffer();
        }
        private void SerialPort_Communiction5_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction5.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction5.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction5.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction5.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction5.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction5.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction5.DiscardInBuffer();
        }
        private void SerialPort_Communiction6_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction6.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction6.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction6.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction6.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction6.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction6.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction6.DiscardInBuffer();
        }
        private void SerialPort_Communiction7_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction7.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction7.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction7.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction7.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction7.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction7.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction7.DiscardInBuffer();
        }
        private void SerialPort_Communiction8_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            return;
            int readdata = 0x00;
            int receivedBytesThreshold = 0x00;
            int index = 0x00;
            //if (SerialPort_Communiction.IsOpen == false)
            //{
            //SerialPort_Communiction.Open();
            //}
            if (ProgaramerOrderProcess_Obj.serialstop)
            {
                return;
            }
            if (SerialPort_Communiction8.IsOpen == false)
            {
                return;
            }
            int l = 0x00;
            receivedBytesThreshold = SerialPort_Communiction8.ReceivedBytesThreshold;
            ProgaramerOrderProcess_Obj.ReceiveData = new byte[receivedBytesThreshold];
            if (SerialPort_Communiction8.BytesToRead < receivedBytesThreshold)
            {
                return;
                l = SerialPort_Communiction8.BytesToRead;
            }
            try
            {


                readdata = SerialPort_Communiction8.Read(ProgaramerOrderProcess_Obj.ReceiveData, 0, SerialPort_Communiction8.ReceivedBytesThreshold);
            }
            catch (Exception e3)
            {
                MessageBox.Show("hint", e3.Message);
            }
            /*
            receivedBytesThreshold -= readdata;
            index = readdata;
            while (receivedBytesThreshold != 0)
            {
                readdata = SerialPort_Communiction.Read(ProgaramerOrderProcess_Obj.ReceiveData, index, receivedBytesThreshold);
                index += readdata;

                receivedBytesThreshold -= readdata;
            }
            */
            ProgaramerOrderProcess_Obj.ReceiveFlag = true;
            SerialPort_Communiction8.DiscardInBuffer();
        }


        private void Button_Execute_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[1] == true|| BusyFlagSub[2] == true || BusyFlagSub[3] == true || BusyFlagSub[4] == true || BusyFlagSub[5] == true || BusyFlagSub[6] == true || BusyFlagSub[7] == true || BusyFlagSub[8] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                //BusyFlagSub[1] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[1] = ComboBox_COM.SelectedIndex;
            COMSlecet[2] = ComboBox_COM2.SelectedIndex;
            COMSlecet[3] = ComboBox_COM3.SelectedIndex;
            COMSlecet[4] = ComboBox_COM4.SelectedIndex;
            COMSlecet[5] = ComboBox_COM5.SelectedIndex;
            COMSlecet[6] = ComboBox_COM6.SelectedIndex;
            COMSlecet[7] = ComboBox_COM7.SelectedIndex;
            COMSlecet[8] = ComboBox_COM8.SelectedIndex;

            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            if (TextBox_HexFile.Text == "")
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[1] = false;
                return;
            }
            RecordCount = 0;
            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction1.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction1.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction1.BaudRate = frequnecy;
            SerialPort_Communiction1.PortName = ComboBox_COM.Text;
            if (SerialPort_Communiction1.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction1.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;


            if (ComboBox_COM.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t = new Thread(new ThreadStart(Connect_thread1));
                t.Start();
            }
            if (ComboBox_COM2.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t2 = new Thread(new ThreadStart(Connect_thread2));
                t2.Start();
            }
            if (ComboBox_COM3.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t3 = new Thread(new ThreadStart(Connect_thread3));
                t3.Start();
            }
            if (ComboBox_COM4.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t4 = new Thread(new ThreadStart(Connect_thread4));
                t4.Start();

            }
            if (ComboBox_COM5.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t5 = new Thread(new ThreadStart(Connect_thread5));
                t5.Start();
            }
            if (ComboBox_COM6.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t6 = new Thread(new ThreadStart(Connect_thread6));
                t6.Start();
            }
            if (ComboBox_COM7.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t7 = new Thread(new ThreadStart(Connect_thread7));
                t7.Start();
            }
            if (ComboBox_COM8.SelectedIndex < 0)
            {

            }
            else
            {
                Thread t8 = new Thread(new ThreadStart(Connect_thread8));
                t8.Start();
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Button_Execute.Enabled = false;

            Button_Start1.Enabled = false;
            Button_Start2.Enabled = false;
            Button_Start3.Enabled = false;
            Button_Start4.Enabled = false;
            Button_Start5.Enabled = false;
            Button_Start6.Enabled = false;
            Button_Start7.Enabled = false;
            Button_Start8.Enabled = false;
        }


        //void Button_Execute_thread()
        //{
        //    try
        //    {
        //        if (SerialPort_Communiction.BytesToRead > 0)
        //        {
        //            byte[] ReceiveData = new byte[SerialPort_Communiction.BytesToRead];
        //            byte[] t = new byte[SerialPort_Communiction.BytesToRead];
        //            SerialPort_Communiction.Read(ReceiveData, 0, SerialPort_Communiction.BytesToRead);
        //        }
        //        //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
        //        //{
        //        //label4.Text = ii.ToString();
        //        Control.CheckForIllegalCrossThreadCalls = false;
        //        ProgressBar_1.Value = 0;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        if (CheckBox_Erase.Checked == true)
        //        {
        //            if (RadioButton_ChipErase.Checked == true)
        //            {
        //                byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
        //                byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
        //                if (TextBox_Info.Text == "")
        //                {
        //                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                }
        //                else
        //                {
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                }

        //                ProgressBar_1.Value = 2;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                //byte[] CRadd = 
        //                /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

        //                if (stats == (byte)eErrNumber.Succesful)
        //                {
        //                    XMLProcess x = new XMLProcess();
        //                    string cr = "";
        //                    for (int i = 0; i < 16; i++)
        //                    {
        //                        cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
        //                    }
        //                    x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
        //                }
        //                else
        //                {
        //                    Button_Execute.Enabled = true;
        //                    //BusyFlag = false;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Button_Execute.Focus();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }*/
        //                ProgressBar_1.Value = 4;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
        //                ProgressBar_1.Value = 8;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {
        //                    XMLProcess x = new XMLProcess();
        //                    string cr = "";
        //                    for (int i = 0; i < 12; i++)
        //                    {
        //                        cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
        //                    }
        //                    x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
        //                }
        //                else
        //                {
        //                    Button_Execute.Enabled = true;
        //                    //BusyFlag = false;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Button_Execute.Focus();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                */
        //                byte Result = 0;
        //                byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction, ref Result);
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {
        //                    //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                    /*if (Result == 0x01)
        //                    {
        //                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                        TextBox_Info.ScrollToCaret();
        //                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                        //MessageBoxIcon.Error);
        //                        Button_Upload.Enabled = true;
        //                        Control.CheckForIllegalCrossThreadCalls = true;
        //                        BusyFlag = false;
        //                        return;
        //                    }*/
        //                }
        //                else
        //                {
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                    //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Button_Execute.Enabled = true;
        //                    //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
        //                    //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    //TextBox_Info.ScrollToCaret();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }

        //                ProgressBar_1.Value = 6;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction);
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {
        //                    /*
        //                    Button_Erase.Enabled = true;
        //                    //BusyFlag = false;
        //                    ChipEraseFlag = true;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgressBar_1.Value = 100;
        //                    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                    */

        //                }
        //                else
        //                {
        //                    Button_Execute.Enabled = true;
        //                    //BusyFlag = false;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Button_Execute.Focus();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                ProgressBar_1.Value = 8;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                /*
        //                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {

        //                }
        //                else
        //                {
        //                    Button_Execute.Enabled = true;
        //                    //BusyFlag = false;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    Button_Execute.Focus();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                */
        //                if (Result == 0x01)
        //                {
        //                    stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction);
        //                    if (stats == (byte)eErrNumber.Succesful)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        Button_Execute.Enabled = true;
        //                        //BusyFlag = false;
        //                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
        //                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                        TextBox_Info.ScrollToCaret();
        //                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                        Control.CheckForIllegalCrossThreadCalls = true;
        //                        BusyFlag = false;
        //                        return;
        //                    }

        //                    //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
        //                    //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
        //                    //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
        //                    if (true)
        //                    {
        //                        stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
        //                                                                   ComboBox_COM.Text, frequnecy, Crystal);
        //                    }
        //                    if (stats == (byte)eErrNumber.Succesful)
        //                    {

        //                    }
        //                    else
        //                    {
        //                        Button_Execute.Enabled = true;
        //                        ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
        //                        ConnectControlDisplay(ConnectFlag);
        //                        //Button_Connect.Enabled = true;
        //                        Control.CheckForIllegalCrossThreadCalls = true;
        //                        BusyFlag = false;
        //                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                        return;
        //                    }
        //                }
        //                Button_Erase.Enabled = true;
        //                //BusyFlag = false;
        //                ChipEraseFlag = true;
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                //ProgressBar_1.Value = 100;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                checkAutoNumber = false;
        //                ProgressBar_1.Value = 10;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            }
        //            else if (RadioButton_PageErase.Checked == true)
        //            {
        //                if (TextBox_Info.Text == "")
        //                {
        //                    TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                }
        //                else
        //                {
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                }
        //                double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //                for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
        //                {
        //                    byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
        //                                                               + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
        //                    byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction, pageaddress);
        //                    if (stats == (byte)eErrNumber.Succesful)
        //                    {
        //                        ChipEraseFlag = false;

        //                    }
        //                    else
        //                    {
        //                        string s = "";
        //                        if (stats == (byte)eErrNumber.Encrypted)
        //                        {
        //                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //                        }
        //                        Button_Execute.Enabled = true;
        //                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
        //                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                        TextBox_Info.ScrollToCaret();
        //                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                        Button_Execute.Focus();
        //                        Control.CheckForIllegalCrossThreadCalls = true;
        //                        BusyFlag = false;
        //                        return;
        //                    }
        //                    ProgressBar_1.Value = (int)(i * tep);
        //                    label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                }
        //                checkAutoNumber = false;
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();

        //                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                //MessageBoxIcon.Warning);
        //            }
        //        }
        //        ProgressBar_1.Value = 10;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";

        //        if (CheckBox_BlankCheck.Checked == true)
        //        {
        //            if (TextBox_Info.Text == "")
        //            {
        //                TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            else
        //            {
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            ProgressBar_1.Value = 12;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            byte result = 0x00;
        //            UInt32 Lenght = 0x00;
        //            if (ChipEraseFlag == true)
        //            {
        //                Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
        //                             ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //            }
        //            else
        //            {
        //                Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
        //                             ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //            }
        //            ProgressBar_1.Value = 15;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            UInt32 ErrAdd = 0x00;
        //            byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
        //            if (stats == (byte)eErrNumber.Succesful)
        //            {

        //            }
        //            else
        //            {
        //                string ss = "";
        //                if (stats == (byte)eErrNumber.Encrypted)
        //                {
        //                    ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //                }
        //                Button_Execute.Enabled = true;
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                Button_Execute.Focus();
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                BusyFlag = false;
        //                return;
        //            }
        //            ProgressBar_1.Value = 17;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            string s = "";
        //            if (result == 0x01)
        //            {
        //                s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
        //            }
        //            else if (result == 0x00)
        //            {
        //                s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
        //            }
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //            //MessageBoxIcon.Warning);
        //        }
        //        ProgressBar_1.Value = 20;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        if (CheckBox_Program.Checked == true)
        //        {
        //            byte stats = GetHexFile();
        //            if (stats != (byte)eErrNumber.Succesful)
        //            {
        //                Button_Execute.Enabled = true;
        //                BusyFlag = false;
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                return;
        //            }
        //            PictureBox_hexFile.Enabled = false;
        //            TextBox_HexFile.Enabled = false;

        //            if (TextBox_Info.Text == "")
        //            {
        //                TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            else
        //            {
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            ProgressBar_1.Value = 20;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            if (CheckBox_AutoNumber.Checked == true)
        //            {
        //                if (AutoNumberEnable == false)
        //                {
        //                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    TextBox_HexFile.Enabled = true;
        //                    PictureBox_hexFile.Enabled = true;
        //                    Button_Execute.Enabled = true;
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                else
        //                {
        //                    if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
        //                    {
        //                        DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                        if (dr == DialogResult.No)
        //                        {
        //                            Button_Execute.Enabled = true;
        //                            Button_Execute.Focus();
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        else if (dr == DialogResult.Yes)
        //                        {
        //                            byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
        //                            for (int i = 0; i < 4; i++)
        //                            {
        //                                if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
        //                                {
        //                                    HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
        //                                }
        //                            }
        //                            if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
        //                            {
        //                                byte[] senddata = new byte[1];
        //                                senddata[0] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {

        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                            else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
        //                            {
        //                                byte[] senddata = new byte[2];
        //                                senddata[0] = CurrentNumData[2];
        //                                senddata[1] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {

        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                            else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
        //                            {
        //                                byte[] senddata = new byte[3];
        //                                senddata[0] = CurrentNumData[1];
        //                                senddata[1] = CurrentNumData[2];
        //                                senddata[2] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {

        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {

        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Execute.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                    checkAutoNumber = true;
        //                    NumberAdd = NumberAdderss;
        //                }
        //            }
        //            ProgressBar_1.Value = 25;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            TotalCount++;
        //            stats = 0x00;
        //            byte[] sentdata = new byte[512];
        //            int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
        //            int tep = 45000 / count;
        //            for (int i = 0; i < count; i++)
        //            {
        //                if (i == (count - 1))
        //                {
        //                    sentdata = new byte[HexData.Length - (i * 512)];
        //                    Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
        //                }
        //                else
        //                {
        //                    Array.Copy(HexData, i * 512, sentdata, 0, 512);
        //                }
        //                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
        //                if (stats == (byte)eErrNumber.Succesful)
        //                {

        //                }
        //                else
        //                {
        //                    string s = "";
        //                    if (stats == (byte)eErrNumber.Encrypted)
        //                    {
        //                        s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //                    }
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    FailCount++;
        //                    Button_Execute.Enabled = true;
        //                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                    //TextBox_Length.Text = FailCount.ToString();
        //                    PictureBox_hexFile.Enabled = true;
        //                    TextBox_HexFile.Enabled = true;
        //                    Button_Execute.Focus();
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            }
        //            if (AutoNumberEnable == true&& CheckBox_AutoNumber.Checked == true)
        //            {
        //                HistoryNumber = CurrentNumber;
        //                if (CheckBox_AutoNumber.Checked == true)
        //                {
        //                    CurrentNumber += NumberInterval;
        //                }

        //                if (!(CurrentNumber < (StartNumber + (NumberLength)* NumberInterval)))
        //                {
        //                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    CheckBox_AutoNumber.Checked = false;
        //                    AutoNumberEnable = false;
        //                    Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
        //                    Button_OpenClose.Enabled = false;
        //                }
        //                else
        //                {
        //                    Label_CurrentNumbers.Text = CurrentNumber.ToString();
        //                }
        //                Label_HistoryNumbers.Text = HistoryNumber.ToString();
        //            }
        //            PictureBox_hexFile.Enabled = true;
        //            TextBox_HexFile.Enabled = true;
        //            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
        //            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //            TextBox_Info.ScrollToCaret();
        //            //TextBox_NumberAddress.Text = PassCount.ToString();
        //            //TextBox_StartNumber.Text = TotalCount.ToString();
        //        }
        //        ProgressBar_1.Value = 70;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        if (CheckBox_Verify.Checked == true)
        //        {
        //            byte stats = GetHexFile();
        //            if (stats != (byte)eErrNumber.Succesful)
        //            {
        //                Button_Execute.Enabled = true;
        //                BusyFlag = false;
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                return;
        //            }
        //            PictureBox_hexFile.Enabled = false;
        //            TextBox_HexFile.Enabled = false;
        //            if (TextBox_Info.Text == "")
        //            {
        //                TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            else
        //            {
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            ProgressBar_1.Value = 75;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            if (checkAutoNumber == true)
        //            {
        //                byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
        //                //if (AutoNumberEnable == false)
        //                if (false)
        //                {
        //                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    TextBox_HexFile.Enabled = true;
        //                    PictureBox_hexFile.Enabled = true;
        //                    Button_Execute.Enabled = true;
        //                    Control.CheckForIllegalCrossThreadCalls = true;
        //                    BusyFlag = false;
        //                    return;
        //                }
        //                else
        //                {
        //                    if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
        //                    {
        //                        /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                        if (dr == DialogResult.No)
        //                        {
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Execute.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                        else */
        //                        if (true)
        //                        {

        //                            for (int i = 0; i < 4; i++)
        //                            {
        //                                if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
        //                                {
        //                                    HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]+i] = CurrentNumData[i];
        //                                }
        //                            }
        //                            if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
        //                            {
        //                                byte[] senddata = new byte[1];
        //                                byte[] recdata = new byte[2];
        //                                senddata[0] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {
        //                                    string s = "";
        //                                    /*
        //                                    if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    else
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    */
        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                            else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
        //                            {
        //                                byte[] senddata = new byte[2];
        //                                byte[] recdata = new byte[2];
        //                                senddata[0] = CurrentNumData[2];
        //                                senddata[1] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {
        //                                    string s = "";
        //                                    /*
        //                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    else
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    */
        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                            else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
        //                            {
        //                                byte[] senddata = new byte[3];
        //                                byte[] recdata = new byte[2];
        //                                senddata[0] = CurrentNumData[1];
        //                                senddata[1] = CurrentNumData[2];
        //                                senddata[2] = CurrentNumData[3];
        //                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
        //                                if (stats == (byte)eErrNumber.Succesful)
        //                                {
        //                                    string s = "";
        //                                    /*
        //                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    else
        //                                    {
        //                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                        TextBox_Info.ScrollToCaret();
        //                                    }
        //                                    */
        //                                }
        //                                else
        //                                {
        //                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                                    //TextBox_Length.Text = FailCount.ToString();
        //                                    TextBox_HexFile.Enabled = true;
        //                                    PictureBox_hexFile.Enabled = true;
        //                                    Button_Execute.Enabled = true;
        //                                    Control.CheckForIllegalCrossThreadCalls = true;
        //                                    BusyFlag = false;
        //                                    return;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        byte[] recdata = new byte[2];
        //                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
        //                        if (stats == (byte)eErrNumber.Succesful)
        //                        {
        //                            string s = "";
        //                            /*
        //                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            else
        //                            {
        //                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                                TextBox_Info.ScrollToCaret();
        //                            }
        //                            */
        //                        }
        //                        else
        //                        {
        //                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                            //TextBox_Length.Text = FailCount.ToString();
        //                            TextBox_HexFile.Enabled = true;
        //                            PictureBox_hexFile.Enabled = true;
        //                            Button_Execute.Enabled = true;
        //                            Control.CheckForIllegalCrossThreadCalls = true;
        //                            BusyFlag = false;
        //                            return;
        //                        }
        //                    }
        //                }
        //            }

        //            ProgressBar_1.Value = 80;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            byte[] checksumdata = new byte[2];
        //            stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
        //            if (stats == (byte)eErrNumber.Succesful)
        //            {
        //                string s = "";
        //                if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
        //                {
        //                    s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                }
        //                else
        //                {
        //                    s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
        //                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
        //                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                    TextBox_Info.ScrollToCaret();
        //                    //break;
        //                }
        //                //BusyFlag = false;
        //                ChipEraseFlag = true;
        //                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
        //                //MessageBoxIcon.Warning);

        //                //Button_Execute.Enabled = true;
        //            }
        //            else
        //            {
        //                string s = "";
        //                if (stats == (byte)eErrNumber.Encrypted)
        //                {
        //                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
        //                }
        //                //BusyFlag = false;
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                Button_Execute.Enabled = true;
        //                PictureBox_hexFile.Enabled = true;
        //                TextBox_HexFile.Enabled = true;
        //                Button_Execute.Focus();
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                BusyFlag = false;
        //                return;
        //            }
        //            PictureBox_hexFile.Enabled = true;
        //            TextBox_HexFile.Enabled = true;
        //        }
        //        ProgressBar_1.Value = 85;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        if (CheckBox_Encrypt.Checked == true)
        //        {
        //            if (TextBox_Info.Text == "")
        //            {
        //                TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            else
        //            {
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //            }
        //            ProgressBar_1.Value = 90;
        //            label2.Text = ProgressBar_1.Value.ToString() + "%";
        //            byte Result = 0x00;
        //            byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction, ref Result);
        //            if (stats == (byte)eErrNumber.Succesful)
        //            {
        //                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                //if (Result == 0x00)
        //                //{
        //                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                //MessageBoxIcon.Error);
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                Button_Execute.Enabled = true;
        //                ProgressBar_1.Value = 100;
        //                label2.Text = ProgressBar_1.Value.ToString() + "%";
        //                Button_Execute.Focus();
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                BusyFlag = false;
        //                return;
        //                //}

        //            }
        //            else
        //            {
        //                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
        //                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //                TextBox_Info.ScrollToCaret();
        //                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
        //                ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //                Button_Execute.Enabled = true;
        //                Button_Execute.Focus();
        //                Control.CheckForIllegalCrossThreadCalls = true;
        //                BusyFlag = false;
        //                return;
        //            }
        //        }
        //        //}
        //        ProgressBar_1.Value = 100;
        //        label2.Text = ProgressBar_1.Value.ToString() + "%";
        //        Button_Execute.Enabled = true;
        //        Button_Execute.Focus();
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
        //                                        MessageBoxIcon.Error);
        //        Button_Execute.Enabled = true;
        //        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
        //        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
        //        TextBox_Info.ScrollToCaret();
        //        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
        //        Control.CheckForIllegalCrossThreadCalls = true;
        //        BusyFlag = false;
        //        return;
        //    }

        //}
        void Button_Execute_fun1()
        {
            try
            {
                if (SerialPort_Communiction1.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction1.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction1.BytesToRead];
                    SerialPort_Communiction1.Read(ReceiveData, 0, SerialPort_Communiction1.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction1, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlag = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[1] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction1);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlag = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[1] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction1);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlag = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction1, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction1);
                                ConnectControlDisplay(ConnectFlagSub[1]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction1, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction1, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[1] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[1] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[1] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[1] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[1] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[1] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[1] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[1] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlag = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[1] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[1] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction1, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlag = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[1] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction1, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[1] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[1] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[1] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[1] = false;
                return;
            }

        }
        void Button_Execute_fun2()
        {
            try
            {
                if (SerialPort_Communiction2.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction2.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction2.BytesToRead];
                    SerialPort_Communiction2.Read(ReceiveData, 0, SerialPort_Communiction2.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction2, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction2, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction2, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction2);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction2);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlag = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction2);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[2] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction2);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlag = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[2] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction2);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlag = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[2] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction2, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM2.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction2);
                                ConnectControlDisplay(ConnectFlagSub[2]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[2] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction2, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[2] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction2, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[2] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[2] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[2] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[2] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[2] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[2] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[2] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[2] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlag = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[2] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[2] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction2, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlag = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[2] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction2, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction2);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[2] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction2);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[2] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[2] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[2] = false;
                return;
            }

        }
        void Button_Execute_fun3()
        {
            try
            {
                if (SerialPort_Communiction3.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction3.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction3.BytesToRead];
                    SerialPort_Communiction3.Read(ReceiveData, 0, SerialPort_Communiction3.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction3, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction3, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction3, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction3);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction3);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlag = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction3);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[3] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction3);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlag = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[3] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlag = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlag = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction3);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlag = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[3] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction3, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM3.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction3);
                                ConnectControlDisplay(ConnectFlagSub[3]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[3] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction3, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[3] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction3, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[3] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[3] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[3] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[3] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[3] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[3] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[3] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[3] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlag = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[3] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[3] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction3, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlag = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlag = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[3] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction3, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction3);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[3] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction3);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[3] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[3] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[3] = false;
                return;
            }

        }
        void Button_Execute_fun4()
        {
            try
            {
                if (SerialPort_Communiction4.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction4.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction4.BytesToRead];
                    SerialPort_Communiction4.Read(ReceiveData, 0, SerialPort_Communiction4.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction4, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[4] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction4, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[4] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction4, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction4);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction4);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[4] = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction4);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction4);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlagSub[4] = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[4] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[4] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction4);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlagSub[4] = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[4] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction4, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM4.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction4);
                                ConnectControlDisplay(ConnectFlagSub[4]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[4] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlagSub[4] = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction4, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[4] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction4, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[4] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[4] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[4] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[4] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[4] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[4] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[4] = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[4] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[4] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction4, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlagSub[4] = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlagSub[4] = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[4] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction4, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction4);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[4] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction4);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[4] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[4] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[4] = false;
                return;
            }

        }
        void Button_Execute_fun5()
        {
            try
            {
                if (SerialPort_Communiction5.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction5.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction5.BytesToRead];
                    SerialPort_Communiction5.Read(ReceiveData, 0, SerialPort_Communiction5.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction5, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[5] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction5, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[5] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction5, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction5);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction5);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[5] = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction5);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction5);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlagSub[5] = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[5] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[5] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction5);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlagSub[5] = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[5] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction5, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM5.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction5);
                                ConnectControlDisplay(ConnectFlagSub[5]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[5] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlagSub[5] = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction5, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[5] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction5, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[5] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[5] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[5] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[5] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[5] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[5] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[5] = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[5] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[5] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction5, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlagSub[5] = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlagSub[5] = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[5] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction5, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction5);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[5] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction5);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[5] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[5] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[5] = false;
                return;
            }

        }
        void Button_Execute_fun6()
        {
            try
            {
                if (SerialPort_Communiction6.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction6.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction6.BytesToRead];
                    SerialPort_Communiction6.Read(ReceiveData, 0, SerialPort_Communiction6.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction6, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[6] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction6, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[6] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction6, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction6);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction6);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[6] = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction6);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction6);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlagSub[6] = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[6] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[6] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction6);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlagSub[6] = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[6] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction6, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM6.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction6);
                                ConnectControlDisplay(ConnectFlagSub[6]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[6] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlagSub[6] = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction6, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[6] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction6, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[6] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[6] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[6] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[6] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[6] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[6] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[6] = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[6] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[6] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction6, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlagSub[6] = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlagSub[6] = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[6] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction6, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction6);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[6] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction6);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[6] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[6] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[6] = false;
                return;
            }

        }
        void Button_Execute_fun7()
        {
            try
            {
                if (SerialPort_Communiction7.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction7.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction7.BytesToRead];
                    SerialPort_Communiction7.Read(ReceiveData, 0, SerialPort_Communiction7.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction7, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[7] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction7, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[7] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction7, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction7);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction7);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[7] = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction7);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction7);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlagSub[7] = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[7] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[7] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction7);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlagSub[7] = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[7] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction7, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM7.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction7);
                                ConnectControlDisplay(ConnectFlagSub[7]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[7] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlagSub[7] = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction7, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[7] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction7, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[7] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[7] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[7] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[7] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[7] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[7] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[7] = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[7] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[7] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction7, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlagSub[7] = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlagSub[7] = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[7] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction7, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction7);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[7] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction7);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[7] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[7] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[7] = false;
                return;
            }

        }
        void Button_Execute_fun8()
        {
            try
            {
                if (SerialPort_Communiction8.BytesToRead > 0)
                {
                    byte[] ReceiveData = new byte[SerialPort_Communiction8.BytesToRead];
                    byte[] t = new byte[SerialPort_Communiction8.BytesToRead];
                    SerialPort_Communiction8.Read(ReceiveData, 0, SerialPort_Communiction8.BytesToRead);
                }
                //for (int ii = 0; ii < Convert.ToInt32(textBox1.Text,10); ii++)
                //{
                //label4.Text = ii.ToString();
                Control.CheckForIllegalCrossThreadCalls = false;
                ProgressBar_1.Value = 0;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Erase.Checked == true)
                {
                    if (RadioButton_ChipErase.Checked == true)
                    {
                        byte[] CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                        byte[] UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartChipErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }

                        ProgressBar_1.Value = 2;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        //byte[] CRadd = 
                        /*byte stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction8, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata.Length, ref CRdata);

                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 16; i++)
                            {
                                cr += Convert.ToString(CRdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUserArea(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[8] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }*/
                        ProgressBar_1.Value = 4;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*stats = ProgaramerOrderProcess_Obj.Read(SerialPort_Communiction8, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDAdd[ComboBox_TargetMCU.SelectedIndex]), UIDdata.Length, ref UIDdata);
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            XMLProcess x = new XMLProcess();
                            string cr = "";
                            for (int i = 0; i < 12; i++)
                            {
                                cr += Convert.ToString(UIDdata[i], 16).PadLeft(2, '0');
                            }
                            x.SaveUID(cr, ComboBox_TargetMCU.SelectedIndex);
                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[8] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        */
                        byte Result = 0;
                        byte stats = ProgaramerOrderProcess_Obj.CheckEncrypt(SerialPort_Communiction8, ref Result);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction8);
                            /*if (Result == 0x01)
                            {
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadFail + ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction8);
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                //MessageBoxIcon.Error);
                                Button_Upload.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[8] = false;
                                return;
                            }*/
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction8);
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Enabled = true;
                            //TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.UploadSuccessful ;
                            //TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            //TextBox_Info.ScrollToCaret();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }

                        ProgressBar_1.Value = 6;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        stats = ProgaramerOrderProcess_Obj.EraseFlash(SerialPort_Communiction8);
                        if (stats == (byte)eErrNumber.Succesful)
                        {
                            /*
                            Button_Erase.Enabled = true;
                            //BusyFlagSub[8] = false;
                            ChipEraseFlag = true;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgressBar_1.Value = 100;
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                            */

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[8] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        ProgressBar_1.Value = 8;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        /*
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingAdd[ComboBox_TargetMCU.SelectedIndex]), CRdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            Button_Execute.Enabled = true;
                            //BusyFlagSub[8] = false;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        */
                        if (Result == 0x01)
                        {
                            stats = ProgaramerOrderProcess_Obj.MCUReset(SerialPort_Communiction8);
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                //BusyFlagSub[8] = false;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.MassEraseFail;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[8] = false;
                                return;
                            }

                            //ProgaramerOrderProcess_Obj.XMLProcess_obj.GetSystemConfig(ref ProgaramerOrderProcess_Obj.SystemConfig_obj);
                            //CRdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.CRTrimmingLength[ComboBox_TargetMCU.SelectedIndex]];
                            //UIDdata = new byte[ProgaramerOrderProcess_Obj.SystemConfig_obj.UIDLength[ComboBox_TargetMCU.SelectedIndex]];
                            if (true)
                            {
                                stats = ProgaramerOrderProcess_Obj.ConnectProcess(SerialPort_Communiction8, ProgaramerOrderProcess_Obj.SystemConfig_obj.RAMCodeAdd[ComboBox_TargetMCU.SelectedIndex],
                                                                           ComboBox_COM8.Text, frequnecy, Crystal);
                            }
                            if (stats == (byte)eErrNumber.Succesful)
                            {

                            }
                            else
                            {
                                Button_Execute.Enabled = true;
                                ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction8);
                                ConnectControlDisplay(ConnectFlagSub[8]);
                                //Button_Connect.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[8] = false;
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                return;
                            }
                        }
                        Button_Erase.Enabled = true;
                        //BusyFlagSub[8] = false;
                        ChipEraseFlag = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ChipEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        checkAutoNumber = false;
                        ProgressBar_1.Value = 10;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    else if (RadioButton_PageErase.Checked == true)
                    {
                        if (TextBox_Info.Text == "")
                        {
                            TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartPageErase;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        double tep = 10 / ((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                        for (int i = 0; i < (HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]; i++)
                        {
                            byte[] pageaddress = BitConverter.GetBytes((UInt32)(i * ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]
                                                                       + ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]));
                            byte stats = ProgaramerOrderProcess_Obj.ErasePage(SerialPort_Communiction8, pageaddress);
                            if (stats == (byte)eErrNumber.Succesful)
                            {
                                ChipEraseFlag = false;

                            }
                            else
                            {
                                string s = "";
                                if (stats == (byte)eErrNumber.Encrypted)
                                {
                                    s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                                }
                                Button_Execute.Enabled = true;
                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseFail + s;
                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                TextBox_Info.ScrollToCaret();
                                ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                Button_Execute.Focus();
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[8] = false;
                                return;
                            }
                            ProgressBar_1.Value = (int)(i * tep);
                            label2.Text = ProgressBar_1.Value.ToString() + "%";
                        }
                        checkAutoNumber = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();

                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.PageEraseSuccessful, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);
                    }
                }
                ProgressBar_1.Value = 10;
                label2.Text = ProgressBar_1.Value.ToString() + "%";

                if (CheckBox_BlankCheck.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartBlankCheck;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 12;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte result = 0x00;
                    UInt32 Lenght = 0x00;
                    if (ChipEraseFlag == true)
                    {
                        Lenght = (UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.PageCount[this.ComboBox_TargetMCU.SelectedIndex] *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    else
                    {
                        Lenght = (UInt32)(((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]) *
                                     ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    }
                    ProgressBar_1.Value = 15;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    UInt32 ErrAdd = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.CheckBlank(SerialPort_Communiction8, BitConverter.GetBytes(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]), Lenght, ref result, ref ErrAdd);
                    if (stats == (byte)eErrNumber.Succesful)
                    {

                    }
                    else
                    {
                        string ss = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            ss = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        Button_Execute.Enabled = true;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckFail + ss;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[8] = false;
                        return;
                    }
                    ProgressBar_1.Value = 17;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    string s = "";
                    if (result == 0x01)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultBlank;
                    }
                    else if (result == 0x00)
                    {
                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.ResultNotBlank + "0x" + Convert.ToString(ErrAdd, 16).ToUpper().PadLeft(8, '0');
                    }
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.BlankCheckSuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                    //MessageBoxIcon.Warning);
                }
                ProgressBar_1.Value = 20;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Program.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[8] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;

                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartProgram;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 20;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (CheckBox_AutoNumber.Checked == true)
                    {
                        if (AutoNumberEnable == false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                DialogResult dr = MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OverHexData, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    Button_Execute.Enabled = true;
                                    Button_Execute.Focus();
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[8] = false;
                                    return;
                                }
                                else if (dr == DialogResult.Yes)
                                {
                                    byte[] CurrentNumData = BitConverter.GetBytes(CurrentNumber);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), senddata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {

                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss)), BitConverter.GetBytes((UInt32)(CurrentNumber)));
                                if (stats == (byte)eErrNumber.Succesful)
                                {

                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[8] = false;
                                    return;
                                }
                            }
                            checkAutoNumber = true;
                            NumberAdd = NumberAdderss;
                        }
                    }
                    ProgressBar_1.Value = 25;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    TotalCount++;
                    stats = 0x00;
                    byte[] sentdata = new byte[512];
                    int count = (int)((HexData.Length + 511) / ProgaramerOrderProcess_Obj.SystemConfig_obj.PageSize[this.ComboBox_TargetMCU.SelectedIndex]);
                    int tep = 45000 / count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == (count - 1))
                        {
                            sentdata = new byte[HexData.Length - (i * 512)];
                            Array.Copy(HexData, i * 512, sentdata, 0, HexData.Length - (i * 512));
                        }
                        else
                        {
                            Array.Copy(HexData, i * 512, sentdata, 0, 512);
                        }
                        stats = ProgaramerOrderProcess_Obj.Write(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i * 512)), sentdata);
                        if (stats == (byte)eErrNumber.Succesful)
                        {

                        }
                        else
                        {
                            string s = "";
                            if (stats == (byte)eErrNumber.Encrypted)
                            {
                                s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                            }
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramFail + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            FailCount++;
                            Button_Execute.Enabled = true;
                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                            //TextBox_Length.Text = FailCount.ToString();
                            PictureBox_hexFile.Enabled = true;
                            TextBox_HexFile.Enabled = true;
                            Button_Execute.Focus();
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        ProgressBar_1.Value = (int)(25 + ((i * tep) / 1000));
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                    }
                    if (AutoNumberEnable == true && CheckBox_AutoNumber.Checked == true)
                    {
                        HistoryNumber = CurrentNumber;
                        if (CheckBox_AutoNumber.Checked == true)
                        {
                            CurrentNumber += NumberInterval;
                        }

                        if (!(CurrentNumber < (StartNumber + (NumberLength) * NumberInterval)))
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberFinsh, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            CheckBox_AutoNumber.Checked = false;
                            AutoNumberEnable = false;
                            Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
                            Button_OpenClose.Enabled = false;
                        }
                        else
                        {
                            Label_CurrentNumbers.Text = CurrentNumber.ToString();
                        }
                        Label_HistoryNumbers.Text = HistoryNumber.ToString();
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                    TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.ProgramSuccessful;
                    TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                    TextBox_Info.ScrollToCaret();
                    //TextBox_NumberAddress.Text = PassCount.ToString();
                    //TextBox_StartNumber.Text = TotalCount.ToString();
                }
                ProgressBar_1.Value = 70;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Verify.Checked == true)
                {
                    byte stats = GetHexFile();
                    if (stats != (byte)eErrNumber.Succesful)
                    {
                        Button_Execute.Enabled = true;
                        BusyFlagSub[8] = false;
                        Control.CheckForIllegalCrossThreadCalls = true;
                        return;
                    }
                    PictureBox_hexFile.Enabled = false;
                    TextBox_HexFile.Enabled = false;
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartVerify;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 75;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    if (checkAutoNumber == true)
                    {
                        byte[] CurrentNumData = BitConverter.GetBytes((UInt32)HistoryNumber);
                        //if (AutoNumberEnable == false)
                        if (false)
                        {
                            MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OpenAutoNumber, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            TextBox_HexFile.Enabled = true;
                            PictureBox_hexFile.Enabled = true;
                            Button_Execute.Enabled = true;
                            Control.CheckForIllegalCrossThreadCalls = true;
                            BusyFlagSub[8] = false;
                            return;
                        }
                        else
                        {
                            if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) < HexData.Length)
                            {
                                /*DialogResult dr = MessageBox.Show("", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (dr == DialogResult.No)
                                {
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[8] = false;
                                    return;
                                }
                                else */
                                if (true)
                                {

                                    for (int i = 0; i < 4; i++)
                                    {
                                        if ((NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i) < HexData.Length)
                                        {
                                            HexData[NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex] + i] = CurrentNumData[i];
                                        }
                                    }
                                    if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 3)
                                    {
                                        byte[] senddata = new byte[1];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 3)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 2)
                                    {
                                        byte[] senddata = new byte[2];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[2];
                                        senddata[1] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 2)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                    else if (HexData.Length - (NumberAdderss - ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex]) == 1)
                                    {
                                        byte[] senddata = new byte[3];
                                        byte[] recdata = new byte[2];
                                        senddata[0] = CurrentNumData[1];
                                        senddata[1] = CurrentNumData[2];
                                        senddata[2] = CurrentNumData[3];
                                        stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss + 1)), recdata.Length, ref recdata);
                                        if (stats == (byte)eErrNumber.Succesful)
                                        {
                                            string s = "";
                                            /*
                                            if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            else
                                            {
                                                s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                                TextBox_Info.ScrollToCaret();
                                            }
                                            */
                                        }
                                        else
                                        {
                                            ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                            //TextBox_Length.Text = FailCount.ToString();
                                            TextBox_HexFile.Enabled = true;
                                            PictureBox_hexFile.Enabled = true;
                                            Button_Execute.Enabled = true;
                                            Control.CheckForIllegalCrossThreadCalls = true;
                                            BusyFlagSub[8] = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] recdata = new byte[2];
                                stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(NumberAdderss)), recdata.Length, ref recdata);
                                if (stats == (byte)eErrNumber.Succesful)
                                {
                                    string s = "";
                                    /*
                                    if (((recdata[0] << 8) + recdata[1]) == (((HistoryNumber & 0xff000000) >> 24) + ((HistoryNumber & 0xff0000) >> 16) + ((HistoryNumber & 0xff00) >> 8) + HistoryNumber & 0xff))
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((recdata[0] << 8) + recdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    else
                                    {
                                        s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "自动编号：" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                                        TextBox_Info.ScrollToCaret();
                                    }
                                    */
                                }
                                else
                                {
                                    ProgaramerOrderProcess_Obj.ErrProcess(stats);
                                    //TextBox_Length.Text = FailCount.ToString();
                                    TextBox_HexFile.Enabled = true;
                                    PictureBox_hexFile.Enabled = true;
                                    Button_Execute.Enabled = true;
                                    Control.CheckForIllegalCrossThreadCalls = true;
                                    BusyFlagSub[8] = false;
                                    return;
                                }
                            }
                        }
                    }

                    ProgressBar_1.Value = 80;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte[] checksumdata = new byte[2];
                    stats = ProgaramerOrderProcess_Obj.Verify(SerialPort_Communiction8, BitConverter.GetBytes((UInt32)(ProgaramerOrderProcess_Obj.SystemConfig_obj.FlashAddress[ComboBox_TargetMCU.SelectedIndex])), HexData.Length, ref checksumdata);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        string s = "";
                        if (((checksumdata[0] << 8) + checksumdata[1]) == ProgaramerOrderProcess_Obj.CheckSumUInt16(HexData, 0, HexData.Length))
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s + "(0x" + Convert.ToString(((checksumdata[0] << 8) + checksumdata[1]), 16).ToUpper().PadLeft(4, '0') + ")";
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                        }
                        else
                        {
                            s = ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyNotMatch;
                            TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s;
                            TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                            TextBox_Info.ScrollToCaret();
                            //break;
                        }
                        //BusyFlagSub[8] = false;
                        ChipEraseFlag = true;
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.VerifySuccessful + s, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Warning);

                        //Button_Execute.Enabled = true;
                    }
                    else
                    {
                        string s = "";
                        if (stats == (byte)eErrNumber.Encrypted)
                        {
                            s = ": " + ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted;
                        }
                        //BusyFlagSub[8] = false;
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyFail + s;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        PictureBox_hexFile.Enabled = true;
                        TextBox_HexFile.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[8] = false;
                        return;
                    }
                    PictureBox_hexFile.Enabled = true;
                    TextBox_HexFile.Enabled = true;
                }
                ProgressBar_1.Value = 85;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                if (CheckBox_Encrypt.Checked == true)
                {
                    if (TextBox_Info.Text == "")
                    {
                        TextBox_Info.Text += "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.StartEncrypt;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                    }
                    ProgressBar_1.Value = 90;
                    label2.Text = ProgressBar_1.Value.ToString() + "%";
                    byte Result = 0x00;
                    byte stats = ProgaramerOrderProcess_Obj.SetEncrypt(SerialPort_Communiction8, ref Result);
                    if (stats == (byte)eErrNumber.Succesful)
                    {
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction8);
                        //if (Result == 0x00)
                        //{
                        //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        //MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.Encrypted, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                        //MessageBoxIcon.Error);
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptedSuccessful;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        Button_Execute.Enabled = true;
                        ProgressBar_1.Value = 100;
                        label2.Text = ProgressBar_1.Value.ToString() + "%";
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[8] = false;
                        return;
                        //}

                    }
                    else
                    {
                        TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptFail;
                        TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                        TextBox_Info.ScrollToCaret();
                        //ProgaramerOrderProcess_Obj.serialportclose(SerialPort_Communiction8);
                        ProgaramerOrderProcess_Obj.ErrProcess(stats);
                        Button_Execute.Enabled = true;
                        Button_Execute.Focus();
                        Control.CheckForIllegalCrossThreadCalls = true;
                        BusyFlagSub[8] = false;
                        return;
                    }
                }
                //}
                ProgressBar_1.Value = 100;
                label2.Text = ProgressBar_1.Value.ToString() + "%";
                Button_Execute.Enabled = true;
                Button_Execute.Focus();
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[8] = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                Button_Execute.Enabled = true;
                TextBox_Info.Text += "\r\n" + "<--" + DateTime.Now + "-->" + "执行错误";
                TextBox_Info.SelectionStart = TextBox_Info.Text.Length;
                TextBox_Info.ScrollToCaret();
                //ProgaramerOrderProcess_Obj.ErrProcess(stats);
                Control.CheckForIllegalCrossThreadCalls = true;
                BusyFlagSub[8] = false;
                return;
            }

        }
        private void ChineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XMLProcess x = new XMLProcess();
            ChineseToolStripMenuItem.Checked = true;
            //EnglishToolStripMenuItem.Checked = false;
            x.SaveLanguage("0");
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
            ControlsText();
        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            XMLProcess x = new XMLProcess();
            ChineseToolStripMenuItem.Checked = false;
            EnglishToolStripMenuItem.Checked = true;
            x.SaveLanguage("1");
            ProgaramerOrderProcess_Obj.XMLProcess_obj.GetText(ref ProgaramerOrderProcess_Obj.DisplayText_obj);
            ControlsText();
            */
        }

        void SaveConfig()
        {
            ProgaramerOrderProcess_Obj.DisplayText_obj.MCUSelected = ComboBox_TargetMCU.SelectedIndex.ToString();
            ProgaramerOrderProcess_Obj.DisplayText_obj.CrystalSelected = ComboBox_Frequency.SelectedIndex.ToString();
            ProgaramerOrderProcess_Obj.DisplayText_obj.HexFileURL = TextBox_HexFile.Text;
            if (ComboBox_COM.Items.Count > 0)//////need to modify
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.COMSelect = ComboBox_COM.Items[ComboBox_COM.SelectedIndex].ToString();
            }
            else
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.COMSelect = "";
            }
            //if (RadioButton_ChipErase.Checked == true)
            //{
            //ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "0";
            //}
            //else
            //{
            //ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "1";
            //}
            if (ConnectFlagSub[0] == true)//nedd to modify
            {
                if (RadioButton_ChipErase.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "0";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "1";
                }
                if (CheckBox_Erase.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EraseEnb = "1";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EraseEnb = "0";
                }

                if (CheckBox_BlankCheck.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.BankEnb = "1";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.BankEnb = "0";
                }

                if (CheckBox_Program.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.ProEnb = "1";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.ProEnb = "0";
                }

                if (CheckBox_Verify.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyEnb = "1";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.VerifyEnb = "0";
                }

                if (CheckBox_Encrypt.Checked == true)
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptEnb = "1";
                }
                else
                {
                    ProgaramerOrderProcess_Obj.DisplayText_obj.EncryptEnb = "0";
                }
            }
            ProgaramerOrderProcess_Obj.XMLProcess_obj.SaveConfig(ProgaramerOrderProcess_Obj.DisplayText_obj);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SerialPort_Communiction1.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction1.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction2.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction2.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction3.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction3.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction4.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction4.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction5.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction5.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction6.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction6.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction7.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction7.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction8.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction8.Close();
                }
                catch
                {

                }
            }
            SaveConfig();
            Application.Exit();
            Application.ExitThread();
            this.Dispose();
            System.Environment.Exit(0);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SubHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath;
            path += "\\Config\\ProgramUM.pdf";
            System.Diagnostics.Process.Start(path);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SerialPort_Communiction1.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction1.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction2.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction2.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction3.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction3.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction4.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction4.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction5.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction5.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction6.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction6.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction7.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction7.Close();
                }
                catch
                {

                }
            }
            if (SerialPort_Communiction8.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction8.Close();
                }
                catch
                {

                }
            }
            SaveConfig();

            Application.Exit();
            Application.ExitThread();
            this.Dispose();
            System.Environment.Exit(0);
        }

        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE://
                            break;
                        case DBT_DEVICEARRIVAL://U盘插入
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                if (drive.DriveType == DriveType.Removable)
                                {
                                    //richTextBox1.AppendText("U盘已插入，盘符为:" + drive.Name.ToString() + "\r\n");
                                    break;
                                }
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            //MessageBox.Show("2");
                            break;
                        case DBT_CONFIGCHANGED:
                            //MessageBox.Show("3");
                            break;
                        case DBT_CUSTOMEVENT:
                            //MessageBox.Show("4");
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            //MessageBox.Show("5");
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            //MessageBox.Show("6");
                            break;
                        case DBT_DEVICEREMOVECOMPLETE: //U盘卸载
                            //richTextBox1.AppendText("U盘已卸载，盘符为:");
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            //MessageBox.Show("7");
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            //MessageBox.Show("8");
                            break;
                        case DBT_DEVNODES_CHANGED://可用，设备变化时

                            //MessageBox.Show("9");
                            /*
                            string ss = USBHID_Device.MatchDevice(0x10c4, 0x0100);
                            if (HDM16L_PGM_OperationProcess_obj.ConnectFlag == true)
                            {
                                //if (ss == "" || HDM16L_PGM_OperationProcess_obj.DeviceInformation != ss)
                                if (ss == "")
                                {
                                    HDM16L_PGM_OperationProcess_obj.ConnectFlag = false;
                                    CurrentGroup = 0x00;
                                    ConnectFlag = false;
                                    HDM16L_PGM_OperationProcess_obj.Disconnect();
                                    BusyFlag = false;
                                    Button_Connect.Text = HDM16L_PGM_OperationProcess_obj.DisplayText_obj.ButtonConnectText;
                                    RadioButton_OnlinePro.Enabled = false;
                                    RadioButton_OnlineReset.Enabled = false;
                                    RadioButton_OnlineReset.Checked = false;
                                    RadioButton_OnlineNotReset.Enabled = false;
                                    RadioButton_OnlineNotReset.Checked = false;
                                    ComboBox_AutoNumber.Enabled = false;
                                    TextBox_OnlineProFile.Enabled = false;
                                    Button_OnlineProFileSelect.Enabled = false;
                                    Button_OnlineProandVerify.Enabled = false;
                                    Button_Reset.Enabled = false;
                                    Button_Read.Enabled = false;
                                    Button_ClearInf.Enabled = false;

                                    RadioButton_Offline.Checked = false;
                                    RadioButton_OnlinePro.Checked = false;
                                    RadioButton_Password.Enabled = false;
                                    RadioButton_Password.Checked = false;

                                    Button_Password.Enabled = false;
                                    Button_Default.Enabled = false;
                                    Button_NumberStartStop.Enabled = false;
                                    Button_NumberStartStop.Text = HDM16L_PGM_OperationProcess_obj.DisplayText_obj.OpenNumberText;
                                    NumberEnable = false;
                                    Button_ReadUID.Enabled = false;
                                    RadioButton_Offline.Enabled = false;
                                    //RadioButton_OfflineReset.Enabled = false;
                                    // RadioButton_OfflineReset.Checked = false;
                                    //RadioButton_OfflineNotReset.Enabled = false;
                                    //RadioButton_OfflineNotReset.Checked = false;
                                    Button_OfflineProandVerify.Enabled = false;
                                    TextBox_OfflineProFile.Enabled = false;
                                    Button_OfflineProFileSelect.Enabled = false;
                                    TextBox_Interval.Enabled = false;
                                    TextBox_Interval.Text = "";
                                    TextBox_NumberAddress.Enabled = false;
                                    TextBox_NumberAddress.Text = "";
                                    TextBox_NumberLenght.Enabled = false;
                                    TextBox_NumberLenght.Text = "";
                                    TextBox_OfflineProFile.Enabled = false;
                                    TextBox_OnlineProFile.Enabled = false;
                                    TextBox_Password.Enabled = false;
                                    TextBox_Password.Text = "";
                                    TextBox_StartNumber.Enabled = false;
                                    TextBox_StartNumber.Text = "";
                                    TextBox_UID.Enabled = false;
                                    TextBox_ProInf.Enabled = false;
                                    TextBox_ProInf.Text = "";
                                    ProgressBar_Process.Value = 0;
                                    ProgressBar_Process.Enabled = false;

                                    Label_CurrentNumber.Text = "";
                                    Label_HistoryNumber.Text = "";
                                    TextBox_UID.Text = "";

                                    TextBox_OnlineProFile.Text = HDM16L_PGM_OperationProcess_obj.DisplayText_obj.HexfileHintText;
                                    TextBox_OnlineProFile.TextAlign = HorizontalAlignment.Center;
                                    TextBox_OnlineProFile.ForeColor = Color.Silver;

                                    TextBox_OfflineProFile.Text = HDM16L_PGM_OperationProcess_obj.DisplayText_obj.HexfileHintText;
                                    TextBox_OfflineProFile.TextAlign = HorizontalAlignment.Center;
                                    TextBox_OfflineProFile.ForeColor = Color.Silver;
                                    TextBox_DeviceInfo.Text = ss;
                                    ComboBox_AutoNumber.SelectedIndex = 0x01;
                                    NumberEnable = false;
                                    MessageBox.Show(HDM16L_PGM_OperationProcess_obj.DisplayText_obj.Disconnect, HDM16L_PGM_OperationProcess_obj.DisplayText_obj.HintText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);

                                }
                                else if (HDM16L_PGM_OperationProcess_obj.DeviceInformation != ss)
                                {

                                }
                                else
                                {
                                    TextBox_DeviceInfo.Text = ss;
                                }
                            }
                            else
                            {
                                TextBox_DeviceInfo.Text = ss;
                            }
                            */
                            UARTProcess UARTProcess_obj = new UARTProcess();
                            ComboBox_COM.Items.Clear();
                            

                            UARTProcess_obj.GetPortNames(ComboBox_COM);

                            if (ComboBox_COM.Items.Count > 0)
                            {
                                ComboBox_COM.SelectedIndex = 0;
                            }
                            if (ComboBox_COM2.Items.Count > 0)
                            {
                                ComboBox_COM2.SelectedIndex = 0;
                            }
                            if (ComboBox_COM3.Items.Count > 0)
                            {
                                ComboBox_COM3.SelectedIndex = 0;
                            }
                            if (ComboBox_COM4.Items.Count > 0)
                            {
                                ComboBox_COM4.SelectedIndex = 0;
                            }
                            if (ComboBox_COM5.Items.Count > 0)
                            {
                                ComboBox_COM5.SelectedIndex = 0;
                            }
                            if (ComboBox_COM6.Items.Count > 0)
                            {
                                ComboBox_COM6.SelectedIndex = 0;
                            }
                            if (ComboBox_COM7.Items.Count > 0)
                            {
                                ComboBox_COM7.SelectedIndex = 0;
                            }
                            string[] portnames = SerialPort.GetPortNames();
                            bool cmflag = false;
                            for(int i =0;i< portnames.Length; i++)
                            {
                                if(SerialPort_Communiction1.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction1.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction2.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction2.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                // ConnectControlDisplay(ConnectFlag);
                                StartControlDisplay(true);
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction3.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction3.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction4.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction4.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction5.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction5.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction6.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction6.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction7.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction7.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            cmflag = false;
                            for (int i = 0; i < portnames.Length; i++)
                            {
                                if (SerialPort_Communiction8.PortName == portnames[i])
                                {
                                    cmflag = true;
                                }
                            }
                            if (cmflag == false)
                            {
                                //ProgaramerOrderProcess_Obj.DisConnectProcess(SerialPort_Communiction);
                                SerialPort_Communiction8.Dispose();
                                //Button_Start1.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.ConnectText;
                                //ConnectFlag = false;
                                //ConnectControlDisplay(ConnectFlag);
                                //Button_Start1.Enabled = true;
                                Control.CheckForIllegalCrossThreadCalls = true;
                                BusyFlagSub[1] = false;
                            }
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            //MessageBox.Show("10");
                            break;
                        case DBT_USERDEFINED:
                            //MessageBox.Show("11");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, HDM16L_PGM_OperationProcess_obj.DisplayText_obj.EnterError, MessageBoxButtons.OK,
                                                //MessageBoxIcon.Error);
            }
            base.WndProc(ref m);
        }

        private void TextBox_HexFile_DragEnter(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            TextBox_HexFile.Text = path;
        }

        private void TextBox_HexFile_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void TextBox_HexFile_DragLeave(object sender, EventArgs e)
        {
            
        }

        private void TextBox_HexFile_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void CheckBox_Program_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Label_FailCount_Click(object sender, EventArgs e)
        {

        }

        private void GroupBox_Results_Enter(object sender, EventArgs e)
        {

        }

        private void CheckBox_AutoNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_AutoNumber.Checked == true)
            {
                TextBox_NumberAddress.Enabled = true;
                TextBox_StartNumber.Enabled = true;
                TextBox_Length.Enabled = true;
                TextBox_Interval.Enabled = true;
                //CheckBox_AutoNumber.Enabled = false;
                Button_OpenClose.Enabled = true;
            }
            else
            {
                TextBox_NumberAddress.Enabled = false;
                TextBox_StartNumber.Enabled = false;
                TextBox_Length.Enabled = false;
                TextBox_Interval.Enabled = false;
                //CheckBox_AutoNumber.Enabled = false;
                Button_OpenClose.Enabled = false;
                //AutoNumberEnable == false;
                Button_OpenClose.Text = ProgaramerOrderProcess_Obj.DisplayText_obj.AutoNumberOpen;
            }
        }

        private void RadioButton_ChipErase_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if(RadioButton_ChipErase.Checked == true)
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "0";
            }
            else
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "1";
            }
            */
        }

        private void RadioButton_PageErase_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (RadioButton_PageErase.Checked == true)
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "1";
            }
            else
            {
                ProgaramerOrderProcess_Obj.DisplayText_obj.EraseMode = "0";
            }
            */
        }

        private void Label_COM_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Button_Start2_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[2] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[2] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[2] = ComboBox_COM2.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[2] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[2] = false;
                return;
            }
            if (ComboBox_COM2.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[2] = false;
                return;
            }
            if (Button_Start2.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[2] = false;
                    return;
                }
            }
            if (ConnectFlagSub[2] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[2] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start2.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction2.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction2.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction2.BaudRate = frequnecy;
            SerialPort_Communiction2.PortName = ComboBox_COM2.Text;
            if (SerialPort_Communiction2.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction2.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread2));
            t.Start();
            StartControlDisplay(false);
        }

        private void Button_Start3_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[3] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[3] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[3] = ComboBox_COM3.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[3] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[3] = false;
                return;
            }
            if (ComboBox_COM3.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[3] = false;
                return;
            }
            if (Button_Start3.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[3] = false;
                    return;
                }
            }
            if (ConnectFlagSub[3] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[3] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start3.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction3.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction3.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction3.BaudRate = frequnecy;
            SerialPort_Communiction3.PortName = ComboBox_COM3.Text;
            if (SerialPort_Communiction3.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction3.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread3));
            t.Start();
            StartControlDisplay(false);
        }

        private void Button_Start4_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[4] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[4] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[4] = ComboBox_COM4.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[4] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[4] = false;
                return;
            }
            if (ComboBox_COM4.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[4] = false;
                return;
            }
            if (Button_Start4.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[4] = false;
                    return;
                }
            }
            if (ConnectFlagSub[4] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[4] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start4.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction4.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction4.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction4.BaudRate = frequnecy;
            SerialPort_Communiction4.PortName = ComboBox_COM4.Text;
            if (SerialPort_Communiction4.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction4.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread4));
            t.Start();
            StartControlDisplay(false);
        }

        private void Button_Start5_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[5] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[5] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[5] = ComboBox_COM5.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[5] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[5] = false;
                return;
            }
            if (ComboBox_COM5.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[5] = false;
                return;
            }
            if (Button_Start5.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[5] = false;
                    return;
                }
            }
            if (ConnectFlagSub[5] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[5] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start5.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction5.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction5.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction5.BaudRate = frequnecy;
            SerialPort_Communiction5.PortName = ComboBox_COM5.Text;
            if (SerialPort_Communiction5.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction5.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread5));
            t.Start();
            //Button_Start5.Enabled = false;
            StartControlDisplay(false);
        }

        private void Button_Start6_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[6] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[6] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[6] = ComboBox_COM6.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[6] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[6] = false;
                return;
            }
            if (ComboBox_COM6.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[6] = false;
                return;
            }
            if (Button_Start6.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[6] = false;
                    return;
                }
            }
            if (ConnectFlagSub[6] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[6] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start6.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction6.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction6.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction6.BaudRate = frequnecy;
            SerialPort_Communiction6.PortName = ComboBox_COM6.Text;
            if (SerialPort_Communiction6.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction6.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread6));
            t.Start();
            //Button_Start6.Enabled = false;
            StartControlDisplay(false);
        }
        private void Button_Start7_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[7] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[7] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[7] = ComboBox_COM7.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[7] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[7] = false;
                return;
            }
            if (ComboBox_COM7.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[7] = false;
                return;
            }
            if (Button_Start7.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[7] = false;
                    return;
                }
            }
            if (ConnectFlagSub[7] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[7] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start7.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction7.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction7.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction7.BaudRate = frequnecy;
            SerialPort_Communiction7.PortName = ComboBox_COM7.Text;
            if (SerialPort_Communiction7.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction7.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread7));
            t.Start();
            //Button_Start7.Enabled = false;
            StartControlDisplay(false);
        }

        private void Button_Start8_Click(object sender, EventArgs e)
        {
            if (BusyFlagSub[8] == true)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.OperationBusy, ProgaramerOrderProcess_Obj.DisplayText_obj.WarningInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                BusyFlagSub[8] = true;
            }
            TargetMCUSlecet = ComboBox_TargetMCU.SelectedIndex;
            FrequencySlecet = ComboBox_Frequency.SelectedIndex;
            COMSlecet[8] = ComboBox_COM8.SelectedIndex;
            if (ComboBox_TargetMCU.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectTargetMCUEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[8] = false;
                return;
            }
            if (ComboBox_Frequency.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectFrequencyEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[8] = false;
                return;
            }
            if (ComboBox_COM8.SelectedIndex < 0)
            {
                MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileCOM, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                MessageBoxIcon.Error);
                BusyFlagSub[8] = false;
                return;
            }
            if (Button_Start8.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                if (TextBox_HexFile.Text == "")
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.SelectHexFileEmpty, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[8] = false;
                    return;
                }
            }
            if (ConnectFlagSub[8] == false)
            {
                if (!ProgaramerOrderProcess_Obj.SetHexFileURL(TextBox_HexFile.Text))
                {
                    MessageBox.Show(ProgaramerOrderProcess_Obj.DisplayText_obj.URLError, ProgaramerOrderProcess_Obj.DisplayText_obj.ErrorInformationText, MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                    BusyFlagSub[8] = false;
                    return;
                }
            }
            RecordCount = 0;
            if (Button_Start8.Text != ProgaramerOrderProcess_Obj.DisplayText_obj.DisconnectText)
            {
                byte stats = GetHexFile();
                if (stats != (byte)eErrNumber.Succesful)
                {
                    return;
                }
            }

            switch (ComboBox_Frequency.SelectedIndex)
            {
                case 0:
                    frequnecy = (int)(9600 * (ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex] / 4000000));
                    //Crystal = ProgaramerOrderProcess_Obj.SystemConfig_obj.MainCR[ComboBox_TargetMCU.SelectedIndex];
                    Crystal = 0;
                    break;

                case 1:
                    frequnecy = (int)(9600 * 1);
                    Crystal = 4000000;
                    break;
                case 2:
                    frequnecy = (int)(9600 * 1.5);
                    Crystal = 6000000;
                    break;

                case 3:
                    frequnecy = (int)(9600 * 2);
                    Crystal = 8000000;
                    break;
                case 4:
                    frequnecy = (int)(9600 * 2.5);
                    Crystal = 10000000;
                    break;
                case 5:
                    frequnecy = (int)(9600 * 3);
                    Crystal = 12000000;
                    break;
                case 6:
                    frequnecy = (int)(9600 * 4);
                    Crystal = 16000000;
                    break;
                case 7:
                    frequnecy = (int)(9600 * 4.5);
                    Crystal = 18000000;
                    break;
                case 8:
                    frequnecy = (int)(9600 * 5);
                    Crystal = 20000000;
                    break;
                case 9:
                    frequnecy = (int)(9600 * 6);
                    Crystal = 24000000;
                    break;
                case 10:
                    frequnecy = (int)(9600 * 8);
                    Crystal = 32000000;
                    break;
            }
            if (SerialPort_Communiction8.IsOpen == true)
            {
                try
                {
                    SerialPort_Communiction8.Close();
                }
                catch
                {

                }
            }
            SerialPort_Communiction8.BaudRate = frequnecy;
            SerialPort_Communiction8.PortName = ComboBox_COM8.Text;
            if (SerialPort_Communiction8.IsOpen == false)
            {
                try
                {
                    SerialPort_Communiction8.Open();
                }
                catch (Exception ex)
                {

                }
            }
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread t = new Thread(new ThreadStart(Connect_thread8));
            t.Start();
            Button_Start8.Enabled = false;
        }

        private void Button_Start2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
