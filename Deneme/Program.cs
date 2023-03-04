using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;

namespace Deneme
{
    public partial class Program
    {
        public int m_iUserID = -1;
        public int m_UserID = -1;
        private string CsTemp = null;
        private int m_lLogNum = 0;
        private string MinorType = null;
        private string MajorType = null;
        public int m_lGetAcsEventHandle = -1;
        Thread m_pDisplayListThread = null;
        static void Main(string[] args)
        {
            int m_iUserID = -1;
            int m_UserID = -1;
            string CsTemp = null;
            int m_lLogNum = 0;
            string MinorType = null;
            string MajorType = null;
            int m_lGetAcsEventHandle = -1;
            Thread m_pDisplayListThread = null;

            if (m_UserID == -1)
            {
                string jsonString = File.ReadAllText("./db/Config.json");
                JObject devicesJson = JObject.Parse(jsonString);
                foreach (JProperty item in devicesJson.Properties())
                {

                    string deviceName = item.Name;
                    JObject deviceData = item.Value as JObject;
                    string ipAddress = deviceData["ipAddress"].ToString();
                    int port = (int)deviceData["port"];
                    string username = deviceData["username"].ToString();
                    string password = deviceData["password"].ToString();
                 /* int year = (int)deviceData["year"];
                    int month = (int)deviceData["month"];
                    int day = (int)deviceData["day"];
                    int hour = (int)deviceData["hour"];
                    int minute = (int)deviceData["minute"];
                    int second = (int)deviceData["second"]; */
                    Console.WriteLine($"{deviceName}: {ipAddress} , {port} , {username} , {password} ");
                    CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
                    CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
                    struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

                    struLoginInfo.sDeviceAddress = ipAddress;
                    struLoginInfo.sUserName = username;
                    struLoginInfo.sPassword = password;
                    ushort.TryParse(port.ToString(), out struLoginInfo.wPort);

                    int lUserID = -1;
                    lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
                    if (lUserID >= 0)
                    {
                        m_UserID = lUserID;
                        m_iUserID = lUserID;
                        Console.WriteLine("Login Successful");
                    }
                    else
                    {
                        uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                        if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                        {
                            Console.WriteLine("Cihaza bağlanılamadı Hata: Girdiğiniz bilgiler yanlış.");
                            if (1 == struDeviceInfoV40.bySupportLock)
                            {
                                string strTemp1 = string.Format("{0} Hakkınız kaldı.", struDeviceInfoV40.byRetryLoginTime);
                                Console.WriteLine(strTemp1);
                            }
                        }
                        else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                        {
                            if (1 == struDeviceInfoV40.bySupportLock)
                            {
                                string strTemp1 = string.Format("Kullanıcı kilitlendi. Maksimum yapılabilecek hata sayısı : {0}", struDeviceInfoV40.dwSurplusLockTime);
                                Console.WriteLine(strTemp1);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{deviceName} Cihazında Hata: İnternet hatası veya DVR meşgul.");

                        }
                    }
                }
            }
        }
    }
}
