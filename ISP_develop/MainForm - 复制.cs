using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NameSpace_HexProcess;
using NameSpace_BinProcess;

namespace ISP_develop
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HexProcess test = new HexProcess();
            FrameFormatStr[] Data = new FrameFormatStr[test.GetRows("D:\\work\\training.hex") - 1];
            byte[] FlahRAM = new byte[0xf00000];
            UInt32 RecordCount = 0;
            UInt32 FlashBaseAddress = 0x0000;
            WriteDataBuff[] modifydata = new WriteDataBuff[5];
            BinProcess bin = new BinProcess();
            bin.ReadBinFile(Application.StartupPath+"\\training.bin");
            bin.WriteBinFile(bin.BinData,0,bin.BinData.Length,"D:\\work\\training1.bin");
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
            test.DecodeProcess("D:\\work\\training.hex", ref Data, ref FlahRAM, ref RecordCount, FlashBaseAddress);
            //test.TransferHexType("Intel", "S-Records", Data, "D:\\work\\training1.srec");
            test.HexFileModify(modifydata, Data, "Intel", "D:\\work\\training2.hex");
        }
        
    }
}
