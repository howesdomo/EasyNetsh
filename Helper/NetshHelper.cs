using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Howe.Helper
{
    /// <summary>
    /// Author : Howe
    /// Date   : 2015.11.01
    /// Description : Dos 命令 Netsh 工具集
    /// </summary>
    public class NetshHelper
    {
        /// <summary>
        /// 将IP，DNS设置为自动获取
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        public static void SetDHCP(string targetDevice)
        {
            //string _doscmd = "netsh interface ip set address 本地连接 DHCP";
            string _doscmd = string.Format("netsh interface ip set address {0} DHCP", targetDevice);
            string _doscmd_dns = string.Format("netsh interface ip set dns {0} dhcp", targetDevice);
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardInput.WriteLine(_doscmd);
                p.StandardInput.WriteLine(_doscmd_dns);
                p.Refresh();
                p.StandardInput.WriteLine("exit");
            }
        }

        /// <summary>
        /// 设置IP地址，掩码，网关等
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="submask">子网掩码</param>
        /// <param name="gateway">网关</param>
        /// <param name="dns1">首用DNS</param>
        /// <param name="dns2">备用DNS</param>
        public static void SetIPAddress(string targetDevice, string ipaddress, string submask, string gateway, string dns1, string dns2)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                //        string _doscmd = "netsh interface ip set address name=\"无线网络连接\" source= static addr="
                //+ ipaddress + " mask=" + submask + " gateway=" + gateway;

                string _doscmd = string.Format("netsh interface ip set address name=\"{0}\" source= static addr={1} mask={2} gateway={3}", targetDevice, ipaddress, submask, gateway);

                p.StandardInput.WriteLine(_doscmd.ToString());

                // DNS
                if (string.IsNullOrEmpty(dns1) && string.IsNullOrEmpty(dns2))
                {
                    // Do Nothing
                }

                if (!string.IsNullOrEmpty(dns1))
                {
                    // 首选DNS服务器
                    // netsh interface ip set dnsservers name="无线网络连接" static addr=202.96.134.33 primary
                    //_doscmd = "netsh interface ip set dnsservers name=\"无线网络连接\" static addr=" + dns1 + " primary";
                    _doscmd = string.Format("netsh interface ip set dnsservers name=\"{0}\" static addr={1} primary", targetDevice, dns1);

                    p.StandardInput.WriteLine(_doscmd.ToString());
                }

                if (!string.IsNullOrEmpty(dns2))
                {
                    // 备用DNS服务器
                    // netsh interface ip add dnsservers name="无线网络连接" addr=202.96.128.86 index=2
                    //_doscmd = "netsh interface ip add dnsservers name=\"无线网络连接\" addr=" + dns2 + " index=2";
                    //_doscmd = "netsh interface ip add dnsservers name=\"WLAN\" addr=" + dns2 + " index=2";
                    _doscmd = string.Format("netsh interface ip add dnsservers name=\"{0}\" addr={1} index=2", targetDevice, dns2);
                    p.StandardInput.WriteLine(_doscmd.ToString());
                }

                p.StandardInput.WriteLine("exit");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string WinsockReset()
        {
            int exitCode;
            System.Diagnostics.ProcessStartInfo processInfo;
            System.Diagnostics.Process process;

            processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c netsh winsock reset");
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = System.Diagnostics.Process.Start(processInfo);

            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            // Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            // Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            // Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            return error;

        }

        //public static string WinsockReset()
        //{
        //    int exitCode;
        //    using (Process p = new Process())
        //    {
        //        p.StartInfo.FileName = "cmd.exe";
        //        p.StartInfo.UseShellExecute = false;
        //        p.StartInfo.RedirectStandardInput = true;
        //        p.StartInfo.RedirectStandardOutput = true;
        //        p.StartInfo.RedirectStandardError = true;
        //        p.StartInfo.CreateNoWindow = true;
        //        p.Start();

        //        string _doscmd = "222netsh winsock reset";

        //        p.StandardInput.WriteLine(_doscmd.ToString());

        //        string output = p.StandardOutput.ReadToEnd();
        //        string error = p.StandardError.ReadToEnd();

        //        exitCode = p.ExitCode;

        //        Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
        //        Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
        //        Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
        //        p.Close();

        //        return error;
        //    }
        //}
    }
}
