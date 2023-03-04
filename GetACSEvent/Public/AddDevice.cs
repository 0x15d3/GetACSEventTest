using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using GetACSEvent.Language;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GetACSEvent
{
    public partial class AddDevice : Form
    {
        public int m_iUserID = -1;
        public AddDevice()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ///////

            ///////
            if (textBoxDeviceAddress.Text.Length <= 0 || textBoxDeviceAddress.Text.Length >128)
            {
                MessageBox.Show(Properties.Resources.deviceAddressTips);
                return;
            }

            int port;
            int.TryParse(textBoxPort.Text, out port);
            if (textBoxPort.Text.Length > 5 || port <= 0)
            {
                MessageBox.Show(Properties.Resources.portTips);
                return;
            }

            if (textBoxUserName.Text.Length > 32 || textBoxPassword.Text.Length > 16)
            {
                MessageBox.Show(Properties.Resources.usernameAndPasswordTips);
                return;
            }

            Login();
            if (m_iUserID >= 0)
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Login()
        {
            StreamReader Read = new StreamReader("./db/Config.txt");
            string text = Read.ReadToEnd();
            Read.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);


            XmlNode DataRoot = doc.SelectSingleNode("/Setings");
            XmlNode InputNodeListDB = DataRoot.SelectSingleNode("ReadMethod");


            string Port = InputNodeListDB.Attributes.GetNamedItem("Port").Value;
            string DeviceAdress = InputNodeListDB.Attributes.GetNamedItem("DeviceAdress").Value;
            string RootName = InputNodeListDB.Attributes.GetNamedItem("RootName").Value;
            string Password = InputNodeListDB.Attributes.GetNamedItem("Password").Value;

            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.sDeviceAddress = DeviceAdress;
            struLoginInfo.sUserName = RootName;
            struLoginInfo.sPassword = Password;
            ushort.TryParse(Port, out struLoginInfo.wPort);

            int lUserID = -1;
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (lUserID >= 0)
            {
                m_iUserID = lUserID;
                MessageBox.Show("Login Successful");
            }
            else
            {
                uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    MessageBox.Show("user name or password error!");
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
                        MessageBox.Show(strTemp1);
                    }
                }
                else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
                        MessageBox.Show(strTemp1);
                    }
                }
                else
                {
                    MessageBox.Show("net error or dvr is busy!");
                }
            }
        }

        private void AddDevice_Load(object sender, EventArgs e)
        {
            StreamReader Read = new StreamReader("./db/Config.txt");
            string text = Read.ReadToEnd();
            Read.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);


            XmlNode DataRoot = doc.SelectSingleNode("/Setings");
            XmlNode InputNodeListDB = DataRoot.SelectSingleNode("ReadMethod");


            string Port = InputNodeListDB.Attributes.GetNamedItem("Port").Value;
            string DeviceAdress = InputNodeListDB.Attributes.GetNamedItem("DeviceAdress").Value;
            string RootName = InputNodeListDB.Attributes.GetNamedItem("RootName").Value;
            string Password = InputNodeListDB.Attributes.GetNamedItem("Password").Value;
            textBoxPort.Text = Port;
            textBoxDeviceAddress.Text= DeviceAdress;
            textBoxUserName.Text = RootName;
            textBoxPassword.Text = Password;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
