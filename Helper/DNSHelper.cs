using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDNS.Client.Helper
{
    public class DNSHelper
    {
        /// <summary>
        /// 将IP，DNS设置为自动获取
        /// </summary>
        public static void SetDHCP()
        {
            string _doscmd = "netsh interface ip set address 本地连接 DHCP";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
                p.StandardInput.WriteLine(_doscmd.ToString());
                _doscmd = "netsh interface ip set address name=\"无线网络连接\" source=dhcp";
                p.StandardInput.WriteLine(_doscmd.ToString());
                p.Refresh();
                p.StandardInput.WriteLine("exit");
            }
        }

        /// <summary>
        /// 设置IP地址，掩码，网关等
        /// </summary>
        public static void SetIPAddress(string ipaddress, string submask, string gateway, string dns1, string dns2)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            string _doscmd = "netsh interface ip set address name=\"无线网络连接\" source= static addr="
    + ipaddress + " mask=" + submask + " gateway=" + gateway;
            p.StandardInput.WriteLine(_doscmd.ToString());

            // DNS
            if (string.IsNullOrEmpty(dns1) && string.IsNullOrEmpty(dns2))
            {
                // TODO : 
            }

            // 首选DNS服务器
            if (!string.IsNullOrEmpty(dns1))
            {
                _doscmd = "netsh interface ip add dnsservers name=\"无线网络连接\" addr=202.96.134.53 index=2";
                p.StandardInput.WriteLine(_doscmd.ToString());
            }

            if (!string.IsNullOrEmpty(dns2))
            {
                // 备用DNS服务器
                _doscmd = "netsh interface ip add dnsservers name=\"无线网络连接\" addr=202.96.134.53 index=2";
                p.StandardInput.WriteLine(_doscmd.ToString());
            }

            p.StandardInput.WriteLine("exit");
        }
    }
}
