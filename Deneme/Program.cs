using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using static CHCNetSDK.CHCNet;

namespace Deneme
{
    public partial class Program
    {
        static void Main(string[] args)
        {

            string jsonString = File.ReadAllText("./db/Config.json");
            JObject devicesJson = JObject.Parse(jsonString);


            foreach (JProperty deviceProp in devicesJson.Properties())
            {

                JObject device = (JObject)deviceProp.Value;
                string ipAddress = (string)device["ipAddress"];
                int port = (int)device["port"];
                string userName = (string)device["userName"];
                string password = (string)device["password"];
                HCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new HCNetSDK.NET_DVR_USER_LOGIN_INFO();
                HCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new HCNetSDK.NET_DVR_DEVICEINFO_V40();
                struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[HCNetSDK.SERIALNO_LEN];

                struLoginInfo.sDeviceAddress = ipAddress;
                struLoginInfo.sUserName = userName;
                struLoginInfo.sPassword = password;


                HCNetSDK sdk = new HCNetSDK();
                if (!HCNetSDK.NET_DVR_Init())
                {
                    Console.WriteLine("Failed to initialize SDK.");
                    return;
                }

                int userId = HCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
                if (userId < 0)
                {
                    Console.WriteLine($"Failed to login to device {deviceProp.Name}.");
                    return;
                }


                NET_DVR_XML_CONFIG_INPUT input = new NET_DVR_XML_CONFIG_INPUT
                {
                    dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(NET_DVR_XML_CONFIG_INPUT)),
                    lpRequestUrl = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("GET /ISAPI/Event/notification/alertStream HTTP/1.1\r\nHost: 127.0.0.1:80\r\nAuthorization: Basic Og==\r\n\r\n"),
                    dwRequestUrlLen = (uint)("GET /ISAPI/Event/notification/alertStream HTTP/1.1\r\nHost: 127.0.0.1:80\r\nAuthorization: Basic Og==\r\n\r\n".Length)
                };
                NET_DVR_XML_CONFIG_OUTPUT output = new NET_DVR_XML_CONFIG_OUTPUT
                {
                    dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(NET_DVR_XML_CONFIG_OUTPUT))
                };


                string outputFileName = $"{deviceProp.Name}.txt";
                byte[] outputBytes = new byte[output.dwOutBufferSize];
                System.Runtime.InteropServices.Marshal.Copy(output.lpOutBuffer, outputBytes, 0, (int)output.dwOutBufferSize);
                File.WriteAllText(outputFileName, Encoding.ASCII.GetString(outputBytes));
                Console.WriteLine($"{deviceProp.Name} Adli cihazin Eventleri {outputFileName} dosyasina yazdirilmistir.");


                HCNetSDK.NET_DVR_Cleanup();
            }
        }
    }
}