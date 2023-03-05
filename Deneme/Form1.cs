using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Deneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;
            string ipadress = txtIpAdress.Text;
            string port = txtPort.Text;
            string password = txtPass.Text;

            var newDevice = new
            {
                ipAddress = txtIpAdress.Text,
                port = txtPort.Text,
                username = txtUser.Text,
                password = txtPass.Text
            };
            MessageBox.Show($"{username}-{ipadress}-{port}-{password}");
            string json = File.ReadAllText("./db/Config.json");
            var devices = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            int lastestDeviceNum = 0;
            foreach (var deviceName in devices.Keys)
            {
                if (deviceName.StartsWith("device"))
                {
                    int deviceNum = int.Parse(deviceName.Substring("device".Length));
                    if (deviceNum > lastestDeviceNum)
                    {
                        lastestDeviceNum = deviceNum;
                    }
                    {

                    }
                }
            }
            string newDeviceName = "device" + (lastestDeviceNum + 1);
            devices.Add(newDeviceName,newDevice);

            string updatedJson= JsonConvert.SerializeObject(devices, Formatting.Indented);

            File.WriteAllText("./db/Config.json", updatedJson);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
