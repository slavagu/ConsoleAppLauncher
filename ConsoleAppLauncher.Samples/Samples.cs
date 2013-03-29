using System;
using System.Windows.Forms;

namespace SlavaGu.ConsoleAppLauncher.Samples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonVer_Click(object sender, EventArgs e)
        {
            labelVer.Text = SysInfo.GetWindowsVersion();
        }

        private void buttonGetIpAddress_Click(object sender, EventArgs e)
        {
            labelIpAddress.Text = SysInfo.GetIpAddress();
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            SysInfo.PingUrl("google.com", reply => BeginInvoke((MethodInvoker)delegate { labelPing.Text = reply; }));
        }

        private void buttonSkype_Click(object sender, EventArgs e)
        {
            labelSkype.Text = SysInfo.GetFirewallRule("Skype");
        }

    }
}
