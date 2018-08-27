using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace NameSpace_BinProcess
{
    public class BinProcess
    {
        //public List<byte> BinData = new List<byte>(0);
        public byte[] BinData;
        public BinProcess()
        {

        }
        /// <summary>
        /// read bin file
        /// </summary>
        /// <param name="path">the path of bin file</param>
        /// <returns></returns>
        public bool ReadBinFile(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                BinData = new byte[fs.Length];
                reader.Read(BinData, 0, (int)(fs.Length));
                reader.Close();
                fs.Close();
            }
            catch
            {
                return false;
            }
            //fs.Length
            return true;
        }

        /// <summary>
        /// save the data to bin file
        /// </summary>
        /// <param name="path">the path of bin file</param>
        /// <returns></returns>
        public bool WriteBinFile(byte[] data,int offset, int lenght,string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                
                BinaryWriter writer = new BinaryWriter(fs);
                //BinData = new byte[fs.Length];
                writer.Write(data, offset, lenght);
                writer.Close();
                fs.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
