using System;
using System.IO;
using System.Resources;

namespace NameSpace_HexProcess
{
    public struct FrameFormatStr
    {
        public string DataType;
        public byte DataLenght;
        public uint Address;
        public byte[] Data;
    }

    public class HexProcess
    {
        

        public HexProcess()
        {

        }

        public bool DecodeProcess(string FileName, out FrameFormatStr[] Data, out byte[] FlahRAM)
        {
            string ReadBuff;
            char[] Buff = new char[20];
            for (int i = 0; i < FlahRAM.Length; i++)
            {
                FlahRAM[i] = 0xFF;
            }
            if (!File.Exists(FileName))
            {
                return false;
            }
            StreamReader FileSR = new StreamReader(FileName);
            FileSR.BaseStream.Seek(0, SeekOrigin.Begin);
            FileSR.BaseStream.Position = 0;
            ReadBuff = FileSR.ReadLine();
            if (ReadBuff == null)
            {
                return false;
            }
            Buff = ReadBuff.ToCharArray(0, 2);
            if (Buff[0] == ':')
            {
                return IntelHexFormatDecode(FileName, out Data, out FlahRAM);
            }
            else if (Buff[0] == 'S')
            {
                return MotorolaHexFormatDecode(FileName, out Data, out FlahRAM);
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Intel hex file decode
        /// </summary>
        /// <param name="FileName">hex file path</param>
        /// <param name="Data">data for hex format</param>
        /// <param name="FlahRAM">data for RAM format</param>
        /// <returns>decode result</returns>
        public bool IntelHexFormatDecode(string FileName, out FrameFormatStr[] Data, out byte[] FlahRAM)
        {
            string ReadBuff;
            byte DataLenght = 0;
            char[] Buff = new char[20];
            for (int i = 0; i < FlahRAM.Length; i++)
            {
                FlahRAM[i] = 0xFF;
            }
            if (!File.Exists(FileName))
            {
                return false;
            }
            StreamReader FileSR = new StreamReader(FileName);
            FileSR.BaseStream.Seek(0, SeekOrigin.Begin);
            FileSR.BaseStream.Position = 0;
            ReadBuff = FileSR.ReadLine();
            if (ReadBuff == null)
            {
                return false;
            }
            Buff = ReadBuff.ToCharArray(0, 1);
            if (Buff[0] != ':')
            {
                return false;
            }
            if (!IntelHexFormatCheckSum(ReadBuff))
            {
                return false;
            }
            //Buff = ReadBuff.ToCharArray(1, 8);
            Data[0].DataLenght = Convert.ToByte(ReadBuff.Substring(1, 2), 16);
        }

        /// <summary>
        /// Intel Hex format checksum verify
        /// </summary>
        /// <param name="CheckData">check data</param>
        /// <returns>checksum correct or incorrect</returns>
        public bool IntelHexFormatCheckSum(string CheckData)
        {
            byte checksum = 0;
            if(CheckData.Length<3)
            {
                return false;
            }
            //char[] buff = CheckData.ToCharArray(1, CheckData.Length-3);
            for(int i =0;i<(CheckData.Length-3)/2;i++)
            {
                checksum += Convert.ToByte(CheckData.Substring(1 + 2 * i, 2), 16);
            }
            checksum = 0x100 - checksum;

            if (Convert.ToByte(CheckData.Substring(CheckData.Length - 2, 2), 16) == checksum)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Motorola hex file decode
        /// </summary>
        /// <param name="FileName">hex file path</param>
        /// <param name="Data">data for hex format</param>
        /// <param name="FlahRAM">data for RAM format</param>
        /// <returns>decode result</returns>
        public bool MotorolaHexFormatDecode(string FileName, out byte[][] Data, out byte[] FlahRAM)
        {
            string ReadBuff;
            char[] Buff = new char[20];
            for (int i = 0; i < FlahRAM.Length; i++)
            {
                FlahRAM[i] = 0xFF;
            }
            if (!File.Exists(FileName))
            {
                return false;
            }
            StreamReader FileSR = new StreamReader(FileName);
            FileSR.BaseStream.Seek(0, SeekOrigin.Begin);
            FileSR.BaseStream.Position = 0;
            ReadBuff = FileSR.ReadLine();
            if (ReadBuff == null)
            {
                return false;
            }
            Buff = ReadBuff.ToCharArray(0, 2);
            if (Buff[0] != 'S')
            {
                return false;
            }
        }

        /// <summary>
        /// Motorola Hex format checksum verify
        /// </summary>
        /// <param name="CheckData">check data</param>
        /// <returns>checksum correct or incorrect</returns>
        public bool MotorolaHexFormatCheckSum(string CheckData)
        {
            byte checksum = 0;
            if (CheckData.Length < 4)
            {
                return false;
            }
            //char[] buff = CheckData.ToCharArray(2, CheckData.Length - 4);
            for (int i = 0; i < (CheckData.Length - 4)/2; i++)
            {
                checksum += Convert.ToByte(CheckData.Substring(2+2*i,2), 16) ;
            }
            checksum = 0xFF - checksum;

            if (Convert.ToByte(CheckData.Substring(CheckData.Length - 2,2), 16) == checksum)
            {
                return true;
            }
            return false;
        }

    }
}
