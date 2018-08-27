using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Xml;


namespace XMLNamespace
{
    /// <summary>
    /// Store display text
    /// </summary>
    public partial class DisplayText
    {
        public string LanguageDefault { get; set; }
        public string ApplicationName { get; set; }
        public string File { get; set; }
        public string Connect { get; set; }
        public string Discommect { get; set; }
        public string CancelPrint { get; set; }
        public string Exit { get; set; }
        public string Tool { get; set; }
        public string Setup { get; set; }
        public string Help { get; set; }
        public string About { get; set; }
        public string HelpSub { get; set; }
        public string Language { get; set; }

        public string Print { get; set; }
        public string FontPrint { get; set; }
        public string Clear { get; set; }
        public string PrintSub { get; set; }
        public string ImagePrint { get; set; }
        public string Browse { get; set; }
        public string BarcodePrint { get; set; }
        public string Build { get; set; }
        public string InsettoFont { get; set; }
        public string BarCode { get; set; }
        public string BarCode2D { get; set; }

        public string PrinterControl { get; set; }
        public string StatusDisplay { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public string Setting { get; set; }
        public string FontStyle { get; set; }
        public string Bold { get; set; }
        public string Italic { get; set; }
        public string Underline { get; set; }
        public string Strikeout { get; set; }
        public string DoubleWidth { get; set; }
        public string DoubleHeight { get; set; }
        public string PrintFormat { get; set; }
        public string PrintWidth { get; set; }
        public string PrintDensity { get; set; }
        public string Set { get; set; }
        public string FeedPaper { get; set; }
        public string TestPrint { get; set; }

        public string UARTSet { get; set; }
        public string PortName { get; set; }
        public string BaudRate { get; set; }
        public string DataBit { get; set; }
        public string StopBit { get; set; }
        public string Parity { get; set; }
        public string Cancel { get; set; }
        public string OK { get; set; }

        public string InformationString { get; set; }
        public string ConfigUART { get; set; }
        public string ConnectPrinter { get; set; }
        public string Connected { get; set; }
        public string ConnectFail { get; set; }
        public string ExitInformation { get; set; }

        public string ErrorString { get; set; }
        public string HWError { get; set; }
        public string ImageError { get; set; }
        public string ImageOutSize { get; set; }
        public string SerialError { get; set; }
        public string BarcodeError { get; set; }
        public string BarcodeOutSize { get; set; }
        public string BufferOverFlow { get; set; }
        public string FontLength { get; set; }
        public string FontFormat { get; set; }
        public string ParameterError { get; set; }

        public string[] StatusString { get; set; }
        public string StatusFalse { get; set; }
        public string StatusTrue { get; set; }

        public string Version { get; set; }
        public string VersionNum { get; set; }
        public string UpdateDate { get; set; }
        public string CompanyInformation { get; set; }
        public string Copyright { get; set; }
        public string Close { get; set; }

        public string ConnectStatus { get; set; }
        public string StatusConnected { get; set; }
        public string StatusDisconnect { get; set; }
        public string PrinterStatus { get; set; }
        public string PrinterPrinting { get; set; }
        public string PrinterIDLE { get; set; }
        public string PrinterNoPaper { get; set; }
        public string PrinterLVD { get; set; }
        public string PrinterOverHeat { get; set; }
        public string PrinterOVD { get; set; }
        public string BufferInsuffcient { get; set; }
        public string FrameError { get; set; }
    }

    /// <summary>
    /// Store system config
    /// </summary>
    public partial class SystemConfig
    {
        public string GetStatusAmount { get; set; }
        public string[] StatusString { get; set; }
        public string Phase { get; set; }
        public string ReceiveFrameHead { get; set; }
        public string ReceiveDeviceID { get; set; }
        public string TransmitFrameHead { get; set; }
        public string TransmitDeviceID { get; set; }
        public string TestPrint { get; set; }
        public string FeedBackStatus { get; set; }
        public string FeedPaper { get; set; }
        public string ESCPOS { get; set; }
        public string ACKResend { get; set; }
        public string MaxDataLength { get; set; }
        public string DefaultPort { get; set; }
        public string BaudRate { get; set; }
        public string DataBits { get; set; }
        public string DiscardNull { get; set; }
        public string DtrEnable { get; set; }
        public string Handshake { get; set; }
        public string Parity { get; set; }
        public string ParityReplace { get; set; }
        public string ReadBufferSize { get; set; }
        public string ReadTimeout { get; set; }
        public string ReceivedBytesThreshold { get; set; }
        public string RtsEnable { get; set; }
        public string StopBits { get; set; }
        public string WriteBufferSize { get; set; }
        public string WriteTimeout { get; set; }
        public string OutPutType { get; set; }
        public string FontWindowWidth { get; set; }
        public string FontWindowLength { get; set; }
        public string ImageWindowWidth { get; set; }
        public string ImageWindowLength { get; set; }
        public string BarcodeWindowWidth { get; set; }
        public string BarcodeWindowLength { get; set; }
        public string PlanarVersion { get; set; }
        public string Cell { get; set; }
        public string BarCodeVersion { get; set; }
    }

    public partial class XMLProcess
    {
        /// <summary>
        /// Get system default language
        /// </summary>
        /// <returns>The default language</returns>
        public string GetLanguageDefault()
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config/SystemConfig.XML");

                //get Text content
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/Language");
                return mcunodelist.InnerText;
            }
            catch
            {
                return "true";
 
            }
        }


        /// <summary>
        /// Get text from XML file
        /// </summary>
        /// <param name="languageenglish">Language type,true is English, false is Chinese</param>
        /// <param name="myDisplayText">Store text</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool GetText(bool languageenglish, ref DisplayText myDisplayText)
        {
            XmlDocument xml = new XmlDocument();
            string str = "";
            if (true == languageenglish)
            {
                str = "English";
            }
            else
            {
                str = "Chinese";
            }
            try
            {
                xml.Load("config/SystemConfig.XML");

                //get Text content
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/ApplicationName");
                myDisplayText.ApplicationName = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/File");
                myDisplayText.File = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Connect");
                myDisplayText.Connect = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Discommect");
                myDisplayText.Discommect = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/CancelPrint");
                myDisplayText.CancelPrint = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Exit");
                myDisplayText.Exit = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Tool");
                myDisplayText.Tool = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Setup");
                myDisplayText.Setup = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Help");
                myDisplayText.Help = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/About");
                myDisplayText.About = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/HelpSub");
                myDisplayText.HelpSub = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/MenuBar/Language");
                myDisplayText.Language = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/Print");
                myDisplayText.Print = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/FontPrint");
                myDisplayText.FontPrint = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/Clear");
                myDisplayText.Clear = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/PrintSub");
                myDisplayText.PrintSub = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/ImagePrint");
                myDisplayText.ImagePrint = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/Browse");
                myDisplayText.Browse = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/BarcodePrint");
                myDisplayText.BarcodePrint = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/Build");
                myDisplayText.Build = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/InsettoFont");
                myDisplayText.InsettoFont = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/BarCode");
                myDisplayText.BarCode = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/Print/BarCode2D");
                myDisplayText.BarCode2D = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/PrinterControl");
                myDisplayText.PrinterControl = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/StatusDisplay");
                myDisplayText.StatusDisplay = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/ParameterName");
                myDisplayText.ParameterName = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Value");
                myDisplayText.Value = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Setting");
                myDisplayText.Setting = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/FontStyle");
                myDisplayText.FontStyle = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Bold");
                myDisplayText.Bold = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Italic");
                myDisplayText.Italic = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Underline");
                myDisplayText.Underline = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Strikeout");
                myDisplayText.Strikeout = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/DoubleWidth");
                myDisplayText.DoubleWidth = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/DoubleHeight");
                myDisplayText.DoubleHeight = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/PrintFormat");
                myDisplayText.PrintFormat = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/PrintWidth");
                myDisplayText.PrintWidth = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/PrintDensity");
                myDisplayText.PrintDensity = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/Set");
                myDisplayText.Set = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/FeedPaper");
                myDisplayText.FeedPaper = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PrinterControl/TestPrint");
                myDisplayText.TestPrint = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/UARTSet");
                myDisplayText.UARTSet = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/PortName");
                myDisplayText.PortName = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/BaudRate");
                myDisplayText.BaudRate = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/DataBit");
                myDisplayText.DataBit = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/StopBit");
                myDisplayText.StopBit = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/Parity");
                myDisplayText.Parity = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/Cancel");
                myDisplayText.Cancel = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/OK");
                myDisplayText.OK = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/Version");
                myDisplayText.Version = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/VersionNum");
                myDisplayText.VersionNum = mcunodelist.InnerText;
                
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/UpdateDate");
                myDisplayText.UpdateDate = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/CompanyInformation");
                myDisplayText.CompanyInformation = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/Copyright");
                myDisplayText.Copyright = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/AboutForm/Close");
                myDisplayText.Close = mcunodelist.InnerText;

                myDisplayText.StatusString = new string[7];
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status0String");
                myDisplayText.StatusString[0] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status1String");
                myDisplayText.StatusString[1] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status2String");
                myDisplayText.StatusString[2] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status3String");
                myDisplayText.StatusString[3] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status4String");
                myDisplayText.StatusString[4] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status5String");
                myDisplayText.StatusString[5] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/Status6String");
                myDisplayText.StatusString[6] = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/StatusTrue");
                myDisplayText.StatusTrue = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/GetFromPrinter/StatusFalse");
                myDisplayText.StatusFalse = mcunodelist.InnerText;


                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/InformationString");
                myDisplayText.InformationString = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/ConfigUART");
                myDisplayText.ConfigUART = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/ConnectPrinter");
                myDisplayText.ConnectPrinter = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/Connected");
                myDisplayText.Connected = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/ConnectFail");
                myDisplayText.ConnectFail = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Information/Exit");
                myDisplayText.ExitInformation = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/ErrorString");
                myDisplayText.ErrorString = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/HWError");
                myDisplayText.HWError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/ImageError");
                myDisplayText.ImageError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/ImageOutSize");
                myDisplayText.ImageOutSize = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/SerialError");
                myDisplayText.SerialError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/BarcodeError");
                myDisplayText.BarcodeError = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/BarcodeOutSize");
                myDisplayText.BarcodeOutSize = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/BufferOverFlow");
                myDisplayText.BufferOverFlow = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/FontLength");
                myDisplayText.FontLength = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/FontFormat");
                myDisplayText.FontFormat = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/PopupWindow/Error/ParameterError");
                myDisplayText.ParameterError = mcunodelist.InnerText;
                
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/ConnectStatus");
                myDisplayText.ConnectStatus = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/StatusConnected");
                myDisplayText.StatusConnected = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/StatusDisconnect");
                myDisplayText.StatusDisconnect = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterStatus");
                myDisplayText.PrinterStatus = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterPrinting");
                myDisplayText.PrinterPrinting = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterIDLE");
                myDisplayText.PrinterIDLE = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterNoPaper");
                myDisplayText.PrinterNoPaper = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterLVD");
                myDisplayText.PrinterLVD = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterOverHeat");
                myDisplayText.PrinterOverHeat = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/PrinterOVD");
                myDisplayText.PrinterOVD = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/BufferInsuffcient");
                myDisplayText.BufferInsuffcient = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/StatusBar/FrameError");
                myDisplayText.FrameError = mcunodelist.InnerText;

               //myDisplayText.OK = xml.DocumentElement.SelectSingleNode("/PrinterSetting/Text/" + str + "/UARTSet/OK").InnerText;
                //myDisplayText.OK = mcunodelist;


                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get system config
        /// </summary>
        /// <param name="mySystemConfig">Store system config</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool GetSystemConfig(ref SystemConfig mySystemConfig)
        {
            XmlDocument xml = new XmlDocument();
            int i;
            try
            {
                xml.Load("config/SystemConfig.XML");
                
                //get Text content
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/PrinterInformation/GetFromPrinter/GetStatusAmount");
                mySystemConfig.GetStatusAmount = mcunodelist.InnerText;
                mySystemConfig.StatusString = new string[int.Parse(mySystemConfig.GetStatusAmount)];
                for (i = 0; i < Convert.ToInt32(mySystemConfig.GetStatusAmount, 10); i++)
                {
                    mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/PrinterInformation/GetFromPrinter/Status" + i.ToString() + "String");
                    mySystemConfig.StatusString[i] = mcunodelist.InnerText;
                }
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/PrinterInformation/SettoPrinter/Phase");
                mySystemConfig.Phase = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Receive/FrameHead");
                mySystemConfig.ReceiveFrameHead = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Receive/DeviceID");
                mySystemConfig.ReceiveDeviceID = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/FrameHead");
                mySystemConfig.TransmitFrameHead = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/DeviceID");
                mySystemConfig.TransmitDeviceID = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/Command/TestPrint");
                mySystemConfig.TestPrint = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/Command/FeedBackStatus");
                mySystemConfig.FeedBackStatus = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/Command/FeedPaper");
                mySystemConfig.FeedPaper = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/Command/ESCPOS");
                mySystemConfig.ESCPOS = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/Command/ACKResend");
                mySystemConfig.ACKResend = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/Protocol/Transmit/MaxDataLength");
                mySystemConfig.MaxDataLength = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/DefaultPort");
                mySystemConfig.DefaultPort = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/BaudRate");
                mySystemConfig.BaudRate = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/DataBits");
                mySystemConfig.DataBits = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/DiscardNull");
                mySystemConfig.DiscardNull = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/DtrEnable");
                mySystemConfig.DtrEnable = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/Handshake");
                mySystemConfig.Handshake = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/Parity");
                mySystemConfig.Parity = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/ParityReplace");
                mySystemConfig.ParityReplace = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/ReadBufferSize");
                mySystemConfig.ReadBufferSize = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/ReadTimeout");
                mySystemConfig.ReadTimeout = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/ReceivedBytesThreshold");
                mySystemConfig.ReceivedBytesThreshold = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/RtsEnable");
                mySystemConfig.RtsEnable = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/StopBits");
                mySystemConfig.StopBits = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/WriteBufferSize");
                mySystemConfig.WriteBufferSize = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/Communication/UARTSetting/WriteTimeout");
                mySystemConfig.WriteTimeout = mcunodelist.InnerText;

                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/OutPutType");
                mySystemConfig.OutPutType = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/Font/WindowWidth");
                mySystemConfig.FontWindowWidth = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/Font/WindowLength");
                mySystemConfig.FontWindowLength = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/Image/WindowWidth");
                mySystemConfig.ImageWindowWidth = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/Image/WindowLength");
                mySystemConfig.ImageWindowLength = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/BarCode/WindowWidth");
                mySystemConfig.BarcodeWindowWidth = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/BarCode/WindowLength");
                mySystemConfig.BarcodeWindowLength = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/BarCode/PlanarVersion");
                mySystemConfig.PlanarVersion = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/BarCode/Cell");
                mySystemConfig.Cell = mcunodelist.InnerText;
                mcunodelist = xml.DocumentElement.SelectSingleNode("/PrinterSetting/SystemConfig/ImageProcess/BarCode/BarCodeVersion");
                mySystemConfig.BarCodeVersion = mcunodelist.InnerText;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save the setting to XML file
        /// </summary>
        /// <param name="fileurl">XML file url</param>
        /// <param name="configname">Need to setting department</param>
        /// <param name="configvalue">Save value</param>
        /// <returns>If the operation correct, return true, else return false</returns>
        public bool SaveConfig(string fileurl,string configname,string configvalue)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(fileurl);

                //Set content to xml
                XmlNode mcunodelist = xml.DocumentElement.SelectSingleNode(configname);
                mcunodelist.InnerText = configvalue;
                xml.Save(fileurl);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}