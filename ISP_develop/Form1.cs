using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProgaramerOrderProcess;

namespace ISP_develop
{
    public partial class Form_Checksum : Form
    {
        public Form_Checksum()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Class_ProgaramerOrderProcess1 ProgaramerOrderProcess1_obj = new Class_ProgaramerOrderProcess1();
            byte[] data = new byte[MainForm.HexData.Length];
            UInt16 checksun = 0x00;
            byte filldata = 0x00;
            if(textBox1.Text == "")
            {
                return;
            }
            try
            {
                filldata = Convert.ToByte(textBox1.Text,16);
            }
            catch
            {
                return;
            }
            UInt32 i = 0;
            for (i = 0; i< MainForm.HexData.Length;i++)
            {
                data[i] = filldata;
            }
            //UInt32 i = 0;
            for(i = 0;i< MainForm.HexFileRecord.Length;i++)
            {
                if (MainForm.HexFileRecord[i].DataType == "00" ||
                    MainForm.HexFileRecord[i].DataType == "S1" ||
                    MainForm.HexFileRecord[i].DataType == "S2" ||
                    MainForm.HexFileRecord[i].DataType == "S3" )
                {
                    for (int j = 0; j < MainForm.HexFileRecord[i].DataLenght; j++)
                    {
                        data[MainForm.HexFileRecord[i].RealAddress + j] = MainForm.HexFileRecord[i].Data[j];
                    }
                }
            }
            checksun = ProgaramerOrderProcess1_obj.CheckSumUInt16(data, 0, data.Length);
            /*
            for (int i = 0; i < MainForm.HexData.Length; i++)
            {
                checksun += data[i];
            }
            */
            textBox2.Text = Convert.ToString(checksun, 16).ToUpper().PadLeft(4,'0');

        }

        private void Form_Checksum_Load(object sender, EventArgs e)
        {

        }
    }
}
