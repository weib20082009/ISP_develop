using System;
using System.Xml;
using ProgaramerOrderProcess;

namespace NS_Config
{

    public partial class DisplayText
    {
        public bool LanguageSelect { get; set; }
        public string MenuFileText { get; set; }
        public string MenuSaveText { get; set; }
        public string MenuLanguageText { get; set; }
        public string MenuChineseText { get; set; }
        public string MenuEnglishText { get; set; }
        public string MenuExitText { get; set; }
        public string MenuHelpText { get; set; }
        public string MenuSubHelpText { get; set; }
        public string GroupMCUSettingText { get; set; }
        public string TargetMCUText { get; set; }
        public string CrystalFrequencyText { get; set; }
        public string HexFileText { get; set; }
        public string COMSettingText { get; set; }
        public string ConnectText { get; set; }
        public string DisconnectText { get; set; }
        public string MCUFlashInfoText { get; set; }
        public string StartAddressText { get; set; }
        public string PageSizeText { get; set; }
        public string PageCountText { get; set; }
        public string ResultsText { get; set; }
        public string PassCountText { get; set; }
        public string FailCountText { get; set; }
        public string TotalCountText { get; set; }
        public string ClearText { get; set; }
        public string OperationText { get; set; }
        public string ChipEraseText { get; set; }
        public string PageEraseText { get; set; }
        public string EraseText { get; set; }
        public string BlankCheckText { get; set; }
        public string ProgramText { get; set; }
        public string VerifyText { get; set; }
        public string CheckBlankCheckText { get; set; }
        public string CHeckVerifyText { get; set; }
        public string CheckProgramText { get; set; }
        public string CheckEncryptText { get; set; }
        public string ExecuteText { get; set; }
        public string UploadText { get; set; }
        public string ChecksumText { get; set; }
        public string FrequencyText { get; set; }
        public string AutoNumber { get; set; }
        public string NumberAddress { get; set; }
        public string StartNumber { get; set; }
        public string NumberLength { get; set; }
        public string NumberInterval { get; set; }
        public string CurrentNumber { get; set; }
        public string HistoryNumber { get; set; }
        public string AutoNumberOpen { get; set; }
        public string AutoNumberClose { get; set; }
        public byte Items { get; set; }

        public string[] Item;
        public string Version { get; set; }
        public string EraseCheck { get; set; }


        public string ConnectSuccessful { get; set; }
        public string WarningInformationText { get; set; }
        public string DisconnectSuccessful { get; set; }
        public string OperationBusy { get; set; }
        public string ChipEraseSuccessful { get; set; }
        public string PageEraseSuccessful { get; set; }
        public string BlankCheckSuccessful { get; set; }
        public string ResultBlank { get; set; }
        public string ResultNotBlank { get; set; }
        public string ProgramSuccessful { get; set; }
        public string VerifySuccessful { get; set; }
        public string VerifyMatch { get; set; }
        public string VerifyNotMatch { get; set; }
        public string StartChipErase { get; set; }
        public string StartPageErase { get; set; }
        public string StartBlankCheck { get; set; }
        public string BlankCheckFail { get; set; }
        public string StartProgram { get; set; }
        public string ProgramFail { get; set; }
        public string StartVerify { get; set; }
        public string StartUpload { get; set; }
        public string UploadSuccessful { get; set; }
        public string UploadFail { get; set; }
        public string StartEncrypt { get; set; }
        public string EncryptFail { get; set; }
        public string EncryptedSuccessful { get; set; }
        public string AutoNumberFinsh { get; set; }
        public string OverHexData { get; set; }

        public string ErrorInformationText { get; set; }
        public string SelectTargetMCUEmpty { get; set; }
        public string SelectFrequencyEmpty { get; set; }
        public string SelectHexFileEmpty { get; set; }
        public string URLError { get; set; }
        public string FileFormatError { get; set; }
        public string SelectHexFileCOM { get; set; }
        public string FrameErr { get; set; }
        public string FrameNotDef { get; set; }
        public string ReadFail { get; set; }
        public string MassEraseFail { get; set; }
        public string PageEraseFail { get; set; }
        public string TimeOut { get; set; }
        public string VerifyFail { get; set; }
        public string WriteFail { get; set; }
        public string Encrypted { get; set; }
        public string FlashAddressOF { get; set; }
        public string FrameLenghtOF { get; set; }
        public string DataLenghtOF { get; set; }
        public string BaudrateOF { get; set; }
        public string SerialPortErr { get; set; }
        public string EmptyFill { get; set; }
        public string FormatErr { get; set; }
        public string AddressInvalid { get; set; }
        public string OpenAutoNumber { get; set; }
        public string ComPortConflict { get; set; }


        public string MCUSelected { get; set; }
        public string CrystalSelected { get; set; }
        public string HexFileURL { get; set; }
        public string COMSelect1 { get; set; }
        public string COMSelect2 { get; set; }
        public string COMSelect3 { get; set; }
        public string COMSelect4 { get; set; }
        public string COMSelect5 { get; set; }
        public string COMSelect6 { get; set; }
        public string COMSelect7 { get; set; }
        public string COMSelect8 { get; set; }
        public string EraseMode { get; set; }
        public string EraseEnb { get; set; }
        public string BankEnb { get; set; }
        public string ProEnb { get; set; }
        public string VerifyEnb { get; set; }
        public string EncryptEnb { get; set; }
        public string ModBustEnb { get; set; }
        public string IPAddr { get; set; }
        public string Port { get; set; }
    }

    /// <summary>
    /// Store system config
    /// </summary>
    public partial class SystemConfig
    {
        public UInt32 MCUNumber { get; set; }
        public string[] MCUName;
        public UInt32[] FlashAddress;
        public UInt32[] PageCount;
        public UInt32[] PageSize;
        public string[] BinFile;
        public UInt32[] RAMCodeAdd;
        public UInt32[] MainCR;
        public UInt32[] CRTrimmingAdd;
        public UInt32[] CRTrimmingLength;
        public UInt32[] UIDAdd;
        public byte[] UIDLength;
        public string[] UID;
        public string[] UserArea;
    }

    public partial class XMLProcess
    {

        public bool ChineseLanguage = true;

        public byte SaveAutoNumber(string Number)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/SystemConfig.XML");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }

            try
            {
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/HDM16L_PGM/StoryNumberValue");
                mcunodelist.InnerText = Number;
                xml.Save("config/SystemConfig.XML");
            }
            catch
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        public byte SaveUserArea(string Userdata,int index)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/MCU Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }

            try
            {
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + index.ToString() + "/UserArea");
                mcunodelist.InnerText = Userdata;
                xml.Save("config/MCU Config.XML");
            }
            catch
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        public byte SaveUID(string Userdata, int index)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/MCU Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }

            try
            {
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + index.ToString() + "/UID");
                mcunodelist.InnerText = Userdata;
                xml.Save("config/MCU Config.XML");
            }
            catch
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        public byte SaveLanguage(string Number)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/Language Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }

            try
            {
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/LanguageSelect");
                mcunodelist.InnerText = Number;
                xml.Save("config/Language Config.xml");
            }
            catch
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        public byte SaveConfig(DisplayText myDisplayText)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/Language Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }

            try
            {
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/MCUSelected");
                mcunodelist.InnerText = myDisplayText.MCUSelected;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/CrystalSelected");
                mcunodelist.InnerText = myDisplayText.CrystalSelected;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/HexFileURL");
                mcunodelist.InnerText = myDisplayText.HexFileURL;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect1");
                mcunodelist.InnerText = myDisplayText.COMSelect1;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect2");
                mcunodelist.InnerText = myDisplayText.COMSelect2;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect3");
                mcunodelist.InnerText = myDisplayText.COMSelect3;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect4");
                mcunodelist.InnerText = myDisplayText.COMSelect4;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect5");
                mcunodelist.InnerText = myDisplayText.COMSelect5;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect6");
                mcunodelist.InnerText = myDisplayText.COMSelect6;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect7");
                mcunodelist.InnerText = myDisplayText.COMSelect7;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/COMSelect8");
                mcunodelist.InnerText = myDisplayText.COMSelect8;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/EraseMode");
                mcunodelist.InnerText = myDisplayText.EraseMode;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/EraseEnb");
                mcunodelist.InnerText = myDisplayText.EraseEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/BankEnb");
                mcunodelist.InnerText = myDisplayText.BankEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/ProEnb");
                mcunodelist.InnerText = myDisplayText.ProEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/VerifyEnb");
                mcunodelist.InnerText = myDisplayText.VerifyEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/EncryptEnb");
                mcunodelist.InnerText = myDisplayText.EncryptEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/ModBusEnb");
                mcunodelist.InnerText = myDisplayText.ModBustEnb;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/IPAddr");
                mcunodelist.InnerText = myDisplayText.IPAddr;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + "Chinese" + "/ConfigSave/Port");
                mcunodelist.InnerText = myDisplayText.Port;

                xml.Save("config/Language Config.xml");
            }
            catch
            {

            }
            return (byte)eErrNumber.Succesful;
        }

        /// <summary>
        /// Get text from XML file
        /// </summary>
        /// <param name="languageenglish">Language type,true is English, false is Chinese</param>
        /// <param name="myDisplayText">Store text</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public byte GetText(ref DisplayText myDisplayText)
        {
            bool languageenglish = false;
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/Language Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }
            try
            {

                //get Text content
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/LanguageSelect");
                if (mcunodelist.InnerText == "0")
                {
                    languageenglish = false;
                    myDisplayText.LanguageSelect = true;
                }
                else if (mcunodelist.InnerText == "1")
                {
                    languageenglish = true;
                    myDisplayText.LanguageSelect = false;

                }
                else
                {
                    languageenglish = false;
                    myDisplayText.LanguageSelect = true;
                }

            }
            catch
            {
                return (byte)eErrNumber.FileFormatErr;

            }


            string str = "";
            if (true == languageenglish)
            {
                str = "English";
                ChineseLanguage = false;
            }
            else
            {
                str = "Chinese";
                ChineseLanguage = true;
            }

            try
            {
                //get Text content

                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuFileText");
                myDisplayText.MenuFileText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuSaveText");
                myDisplayText.MenuSaveText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuLanguageText");
                myDisplayText.MenuLanguageText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuChineseText");
                myDisplayText.MenuChineseText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuEnglishText");
                myDisplayText.MenuEnglishText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuExitText");
                myDisplayText.MenuExitText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuHelpText");
                myDisplayText.MenuHelpText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MenuSubHelpText");
                myDisplayText.MenuSubHelpText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/GroupMCUSettingText");
                myDisplayText.GroupMCUSettingText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/TargetMCUText");
                myDisplayText.TargetMCUText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CrystalFrequencyText");
                myDisplayText.CrystalFrequencyText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/HexFileText");
                myDisplayText.HexFileText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/COMSettingText");
                myDisplayText.COMSettingText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ConnectText");
                myDisplayText.ConnectText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/DisconnectText");
                myDisplayText.DisconnectText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/MCUFlashInfoText");
                myDisplayText.MCUFlashInfoText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/StartAddressText");
                myDisplayText.StartAddressText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/PageSizeText");
                myDisplayText.PageSizeText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/PageCountText");
                myDisplayText.PageCountText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ResultsText");
                myDisplayText.ResultsText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/PassCountText");
                myDisplayText.PassCountText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/FailCountText");
                myDisplayText.FailCountText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/TotalCountText");
                myDisplayText.TotalCountText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ClearText");
                myDisplayText.ClearText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/OperationText");
                myDisplayText.OperationText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ChipEraseText");
                myDisplayText.ChipEraseText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/PageEraseText");
                myDisplayText.PageEraseText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/EraseText");
                myDisplayText.EraseText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/BlankCheckText");
                myDisplayText.BlankCheckText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ProgramText");
                myDisplayText.ProgramText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/VerifyText");
                myDisplayText.VerifyText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CheckBlankCheckText");
                myDisplayText.CheckBlankCheckText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CHeckVerifyText");
                myDisplayText.CHeckVerifyText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CheckProgramText");
                myDisplayText.CheckProgramText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CheckEncryptText");
                myDisplayText.CheckEncryptText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ExecuteText");
                myDisplayText.ExecuteText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/UploadText");
                myDisplayText.UploadText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/ChecksumText");
                myDisplayText.ChecksumText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/Frequency/FrequencyText");
                myDisplayText.FrequencyText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/AutoNumber");
                myDisplayText.AutoNumber = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/NumberAddress");
                myDisplayText.NumberAddress = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/StartNumber");
                myDisplayText.StartNumber = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/NumberLength");
                myDisplayText.NumberLength = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/NumberInterval");
                myDisplayText.NumberInterval = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/CurrentNumber");
                myDisplayText.CurrentNumber = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/HistoryNumber");
                myDisplayText.HistoryNumber = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/AutoNumberOpen");
                myDisplayText.AutoNumberOpen = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/AutoNumberClose");
                myDisplayText.AutoNumberClose = mcunodelist.InnerText;
                

                               mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/Frequency/Items");
                myDisplayText.Items = Convert.ToByte(mcunodelist.InnerText, 10);
                myDisplayText.Item = new string[myDisplayText.Items];
                for (int i = 0; i < myDisplayText.Items; i++)
                {
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/Frequency/Item" + i.ToString());
                    myDisplayText.Item[i] = mcunodelist.InnerText;
                }
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/Version");
                myDisplayText.Version = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/Text/EraseCheck");
                myDisplayText.EraseCheck = mcunodelist.InnerText;
                


                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ConnectSuccessful");
                myDisplayText.ConnectSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/WarningInformationText");
                myDisplayText.WarningInformationText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/DisconnectSuccessful");
                myDisplayText.DisconnectSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/OperationBusy");
                myDisplayText.OperationBusy = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ChipEraseSuccessful");
                myDisplayText.ChipEraseSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/PageEraseSuccessful");
                myDisplayText.PageEraseSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/BlankCheckSuccessful");
                myDisplayText.BlankCheckSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ResultBlank");
                myDisplayText.ResultBlank = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ResultNotBlank");
                myDisplayText.ResultNotBlank = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ProgramSuccessful");
                myDisplayText.ProgramSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/VerifySuccessful");
                myDisplayText.VerifySuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/VerifyMatch");
                myDisplayText.VerifyMatch = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/VerifyNotMatch");
                myDisplayText.VerifyNotMatch = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartChipErase");
                myDisplayText.StartChipErase = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartPageErase");
                myDisplayText.StartPageErase = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartBlankCheck");
                myDisplayText.StartBlankCheck = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/BlankCheckFail");
                myDisplayText.BlankCheckFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartProgram");
                myDisplayText.StartProgram = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ProgramFail");
                myDisplayText.ProgramFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartVerify");
                myDisplayText.StartVerify = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartUpload");
                myDisplayText.StartUpload = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/UploadSuccessful");
                myDisplayText.UploadSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/UploadFail");
                myDisplayText.UploadFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/StartEncrypt");
                myDisplayText.StartEncrypt = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/EncryptFail");
                myDisplayText.EncryptFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/EncryptedSuccessful");
                myDisplayText.EncryptedSuccessful = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/OverHexData");
                myDisplayText.OverHexData = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/AutoNumberFinsh");
                myDisplayText.AutoNumberFinsh = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/WarningInformation/ComPortConflict");
                myDisplayText.ComPortConflict = mcunodelist.InnerText;



                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/ErrorInformationText");
                myDisplayText.ErrorInformationText = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/SelectTargetMCUEmpty");
                myDisplayText.SelectTargetMCUEmpty = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/SelectFrequencyEmpty");
                myDisplayText.SelectFrequencyEmpty = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/SelectHexFileEmpty");
                myDisplayText.SelectHexFileEmpty = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/URLError");
                myDisplayText.URLError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FileFormatError");
                myDisplayText.FileFormatError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/SelectHexFileCOM");
                myDisplayText.SelectHexFileCOM = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FrameErr");
                myDisplayText.FrameErr = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FrameNotDef");
                myDisplayText.FrameNotDef = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/ReadFail");
                myDisplayText.ReadFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/MassEraseFail");
                myDisplayText.MassEraseFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/PageEraseFail");
                myDisplayText.PageEraseFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/TimeOut");
                myDisplayText.TimeOut = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/VerifyFail");
                myDisplayText.VerifyFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/WriteFail");
                myDisplayText.WriteFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/Encrypted");
                myDisplayText.Encrypted = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FlashAddressOF");
                myDisplayText.FlashAddressOF = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FrameLenghtOF");
                myDisplayText.FrameLenghtOF = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/DataLenghtOF");
                myDisplayText.DataLenghtOF = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/BaudrateOF");
                myDisplayText.BaudrateOF = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/Encrypted");
                myDisplayText.Encrypted = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/SerialPortErr");
                myDisplayText.SerialPortErr = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/EmptyFill");
                myDisplayText.EmptyFill = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/FormatErr");
                myDisplayText.FormatErr = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/AddressInvalid");
                myDisplayText.AddressInvalid = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ErrorInformation/OpenAutoNumber");
                myDisplayText.OpenAutoNumber = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/MCUSelected");
                myDisplayText.MCUSelected = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/CrystalSelected");
                myDisplayText.CrystalSelected = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/HexFileURL");
                myDisplayText.HexFileURL = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect1");
                myDisplayText.COMSelect1 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect2");
                myDisplayText.COMSelect2 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect3");
                myDisplayText.COMSelect3 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect4");
                myDisplayText.COMSelect4 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect5");
                myDisplayText.COMSelect5 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect6");
                myDisplayText.COMSelect6 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect7");
                myDisplayText.COMSelect7 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/COMSelect8");
                myDisplayText.COMSelect8 = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/EraseMode");
                myDisplayText.EraseMode = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/EraseEnb");
                myDisplayText.EraseEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/BankEnb");
                myDisplayText.BankEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/ProEnb");
                myDisplayText.ProEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/VerifyEnb");
                myDisplayText.VerifyEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/EncryptEnb");
                myDisplayText.EncryptEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/ModBusEnb");
                myDisplayText.ModBustEnb = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/IPAddr");
                myDisplayText.IPAddr = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/Language/" + str + "/ConfigSave/Port");
                myDisplayText.Port = mcunodelist.InnerText;
                //myDisplayText.OK = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/OK").InnerText;
                //myDisplayText.OK = mcunodelist;


                return (byte)eErrNumber.LoadSuccessful;
            }
            catch
            {
                return (byte)eErrNumber.FileFormatErr;
            }
        }

        /// <summary>
        /// Get system config
        /// </summary>
        /// <param name="mySystemConfig">Store system config</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public byte GetSystemConfig(ref SystemConfig mySystemConfig)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/MCU Config.xml");
            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }
            try
            {
                //get Text content
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCUNumber");
                mySystemConfig.MCUNumber = Convert.ToUInt32(mcunodelist.InnerText, 10);
                mySystemConfig.MCUName = new string[mySystemConfig.MCUNumber];
                mySystemConfig.FlashAddress = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.PageCount = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.PageSize = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.BinFile = new string[mySystemConfig.MCUNumber];
                mySystemConfig.RAMCodeAdd = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.MainCR = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.CRTrimmingAdd = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.CRTrimmingLength = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.UIDAdd = new UInt32[mySystemConfig.MCUNumber];
                mySystemConfig.UID = new string[mySystemConfig.MCUNumber];
                mySystemConfig.UIDLength = new byte[mySystemConfig.MCUNumber];
                mySystemConfig.UserArea = new string[mySystemConfig.MCUNumber];

                for (int i = 0; i < mySystemConfig.MCUNumber; i++)
                {
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/MCUName");
                    mySystemConfig.MCUName[i] = mcunodelist.InnerText;
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/FlashAddress");
                    mySystemConfig.FlashAddress[i] = Convert.ToUInt32(mcunodelist.InnerText, 16);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/PageCount");
                    mySystemConfig.PageCount[i] = Convert.ToUInt32(mcunodelist.InnerText, 10);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/PageSize");
                    mySystemConfig.PageSize[i] = Convert.ToUInt32(mcunodelist.InnerText, 10);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/BinFile");
                    mySystemConfig.BinFile[i] = mcunodelist.InnerText;
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/RAMCodeAdd");
                    mySystemConfig.RAMCodeAdd[i] = Convert.ToUInt32(mcunodelist.InnerText, 16);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/MainCR");
                    mySystemConfig.MainCR[i] = Convert.ToUInt32(mcunodelist.InnerText, 10);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/CRTrimmingAdd");
                    mySystemConfig.CRTrimmingAdd[i] = Convert.ToUInt32(mcunodelist.InnerText, 16);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/CRTrimmingLength");
                    mySystemConfig.CRTrimmingLength[i] = Convert.ToUInt32(mcunodelist.InnerText, 10);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/UIDAdd");
                    mySystemConfig.UIDAdd[i] = Convert.ToUInt32(mcunodelist.InnerText, 16);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/UID");
                    mySystemConfig.UID[i] = mcunodelist.InnerText;
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/UIDLength");
                    mySystemConfig.UIDLength[i] = Convert.ToByte(mcunodelist.InnerText, 16);
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/System/MCU/MCU" + i.ToString() + "/UserArea");
                    mySystemConfig.UserArea[i] = mcunodelist.InnerText;
                }
                return (byte)eErrNumber.LoadSuccessful;
            }
            catch
            {
                return (byte)eErrNumber.FileFormatErr;
            }
        }

        /// <summary>
        /// Save the setting to XML file
        /// </summary>
        /// <param name="fileurl">XML file url</param>
        /// <param name="configname">Need to setting department</param>
        /// <param name="configvalue">Save value</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public byte SaveConfig(string fileurl, string configname, string configvalue)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(fileurl);

            }
            catch
            {
                return (byte)eErrNumber.URLErr;
            }
            try
            {
                //Set content to xml
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode(configname);
                mcunodelist.InnerText = configvalue;
                xml.Save(fileurl);
                return (byte)eErrNumber.LoadSuccessful;
            }
            catch
            {
                return (byte)eErrNumber.FileFormatErr;
            }
        }
    }
}

