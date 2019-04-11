using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Howe.Helper
{
    /// <summary>
    /// V 2
    /// Author : Howe
    /// Date   : 2019.04.11
    /// Description : 分离 IP 与 DNS 的设置
    /// 
    /// V 1
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
            string _doscmd_dns = string.Format("netsh interface ip set dns {0} DHCP", targetDevice);
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
        /// 将IP设置为自动获取
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        public static void Set_IP_DHCP(string targetDevice)
        {
            string _doscmd = string.Format("netsh interface ip set address {0} DHCP", targetDevice);
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
                p.Refresh();
                p.StandardInput.WriteLine("exit");
            }
        }

        /// <summary>
        /// 将DNS设置为自动获取
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        public static void Set_DNS_DHCP(string targetDevice)
        {
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
                p.StandardInput.WriteLine(_doscmd_dns);
                p.Refresh();
                p.StandardInput.WriteLine("exit");
            }
        }

        [Obsolete]
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
            // [已弃用] 由于没有返回执行结果被弃用, 但仍有其优点 - 运行速度极快.

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
        /// 设置IP地址，掩码，网关等
        /// 分步执行并会抛出执行时的错误信息
        /// 由于分步执行多次打开 Process，执行速度会很慢, 采用V3版本来执行速度(最后发现速度都差不多), 使用V2更佳严谨
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="submask">子网掩码</param>
        /// <param name="gateway">网关</param>
        /// <param name="dns1">首用DNS</param>
        /// <param name="dns2">备用DNS</param>
        public static void SetIPAddressV2(string targetDevice, string ipaddress, string submask, string gateway, string dns1, string dns2)
        {
            //        string _doscmd = "netsh interface ip set address name=\"无线网络连接\" source= static addr="
            //+ ipaddress + " mask=" + submask + " gateway=" + gateway;
            string cmdArguments = string.Format("netsh interface ip set address name=\"{0}\" source= static addr={1} mask={2} gateway={3}", targetDevice, ipaddress, submask, gateway);

            Util.ProcessResult r1 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
            NetshHelper.GetErrorMsg(cmdArguments, r1);

            if (!string.IsNullOrEmpty(dns1))
            {
                // 首选DNS服务器
                // netsh interface ip set dnsservers name="无线网络连接" static addr=202.96.134.33 primary
                //_doscmd = "netsh interface ip set dnsservers name=\"无线网络连接\" static addr=" + dns1 + " primary";
                cmdArguments = string.Format("netsh interface ip set dnsservers name=\"{0}\" static addr={1} primary", targetDevice, dns1);

                Util.ProcessResult r2 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
                NetshHelper.GetErrorMsg(cmdArguments, r2);
            }

            if (!string.IsNullOrEmpty(dns2))
            {
                // 备用DNS服务器
                // netsh interface ip add dnsservers name="无线网络连接" addr=202.96.128.86 index=2
                //_doscmd = "netsh interface ip add dnsservers name=\"无线网络连接\" addr=" + dns2 + " index=2";
                //_doscmd = "netsh interface ip add dnsservers name=\"WLAN\" addr=" + dns2 + " index=2";
                cmdArguments = string.Format("netsh interface ip add dnsservers name=\"{0}\" addr={1} index=2", targetDevice, dns2);
                Util.ProcessResult r3 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
                NetshHelper.GetErrorMsg(cmdArguments, r3);
            }
        }

        /// <summary>
        /// 精简自 SetIPAddressV2
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="submask">子网掩码</param>
        /// <param name="gateway">网关</param>
        /// <param name="dns1">首用DNS</param>
        /// <param name="dns2">备用DNS</param>
        public static void SetIP(string targetDevice, string ipaddress, string submask, string gateway)
        {
            string cmdArguments = string.Format("netsh interface ip set address name=\"{0}\" source= static addr={1} mask={2} gateway={3}", targetDevice, ipaddress, submask, gateway);
            Util.ProcessResult r1 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
            NetshHelper.GetErrorMsg(cmdArguments, r1);
        }

        /// <summary>
        /// 精简自 SetIPAddressV2
        /// </summary>
        /// <param name="targetDevice">目标网卡</param>
        /// <param name="dns1">首用DNS</param>
        /// <param name="dns2">备用DNS</param>
        public static void SetDNS(string targetDevice, string dns1, string dns2)
        {
            string cmdArguments = string.Empty;
            if (!string.IsNullOrEmpty(dns1))
            {
                cmdArguments = string.Format("netsh interface ip set dnsservers name=\"{0}\" static addr={1} primary", targetDevice, dns1);
                Util.ProcessResult r2 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
                NetshHelper.GetErrorMsg(cmdArguments, r2);
            }

            if (!string.IsNullOrEmpty(dns2))
            {
                cmdArguments = string.Format("netsh interface ip add dnsservers name=\"{0}\" addr={1} index=2", targetDevice, dns2);
                Util.ProcessResult r3 = Util.ProgressUtils.ExcuteBatCMD(cmdArguments);
                NetshHelper.GetErrorMsg(cmdArguments, r3);
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
        public static void SetIPAddressV3(string targetDevice, string ipaddress, string submask, string gateway, string dns1, string dns2)
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

                // TODO : 输出Error Msg信息, 而不是现在的 每次都是设置正确

                string cmdArguments = string.Format("netsh interface ip set address name=\"{0}\" source= static addr={1} mask={2} gateway={3}", targetDevice, ipaddress, submask, gateway);

                #region 跳过信息 版权信息
                // Microsoft Windows [版本 10.0.16299.431]
                // (c)2017 Microsoft Corporation。保留所有权利。
                string skipInputMsg = "skipVersionMsg";
                while (skipInputMsg.IsNullOrEmpty() == false)
                {
                    skipInputMsg = p.StandardOutput.ReadLine();
                }
                #endregion

                p.StandardInput.WriteLine(cmdArguments);
                // 跳过输入命令信息
                skipInputMsg = "skipInputMsg";
                while (skipInputMsg.Contains(cmdArguments) == false)
                {
                    skipInputMsg = p.StandardOutput.ReadLine();
                }

                string excuteOutput = p.StandardOutput.ReadLine();
                if (excuteOutput.IsNullOrEmpty() == false)
                {
                    NetshHelper.GetErrorMsg(cmdArguments, new Util.ProcessResult(1, excuteOutput, string.Empty));
                }

                if (!string.IsNullOrEmpty(dns1))
                {
                    // 首选DNS服务器
                    // netsh interface ip set dnsservers name="无线网络连接" static addr=202.96.134.33 primary
                    //_doscmd = "netsh interface ip set dnsservers name=\"无线网络连接\" static addr=" + dns1 + " primary";
                    cmdArguments = string.Format("netsh interface ip set dnsservers name=\"{0}\" static addr={1} primary", targetDevice, dns1);

                    p.StandardInput.WriteLine(cmdArguments);
                    // 跳过输入命令信息
                    skipInputMsg = "skipInputMsg";
                    while (skipInputMsg.Contains(cmdArguments) == false)
                    {
                        skipInputMsg = p.StandardOutput.ReadLine(); 
                    }

                    excuteOutput = p.StandardOutput.ReadLine();
                    if (excuteOutput.IsNullOrEmpty() == false)
                    {
                        NetshHelper.GetErrorMsg(cmdArguments, new Util.ProcessResult(1, excuteOutput, string.Empty));
                    }
                }

                if (!string.IsNullOrEmpty(dns2))
                {
                    // 备用DNS服务器
                    // netsh interface ip add dnsservers name="无线网络连接" addr=202.96.128.86 index=2
                    //_doscmd = "netsh interface ip add dnsservers name=\"无线网络连接\" addr=" + dns2 + " index=2";
                    //_doscmd = "netsh interface ip add dnsservers name=\"WLAN\" addr=" + dns2 + " index=2";
                    cmdArguments = string.Format("netsh interface ip add dnsservers name=\"{0}\" addr={1} index=2", targetDevice, dns2);

                    p.StandardInput.WriteLine(cmdArguments);
                    // 跳过输入命令信息
                    skipInputMsg = "skipInputMsg";
                    while (skipInputMsg.Contains(cmdArguments) == false)
                    {
                        skipInputMsg = p.StandardOutput.ReadLine();
                    }

                    excuteOutput = p.StandardOutput.ReadLine();
                    if (excuteOutput.IsNullOrEmpty() == false)
                    {
                        NetshHelper.GetErrorMsg(cmdArguments, new Util.ProcessResult(1, excuteOutput, string.Empty));
                    }
                }

                p.StandardInput.WriteLine("exit");
            }
        }

        public static void GetErrorMsg(string cmdArguments, Util.ProcessResult r1)
        {
            if (r1.OutputMsg.IsNullOrEmpty() == false || r1.ErrorMsg.IsNullOrEmpty() == false)
            {
                throw new Exception("执行命令发生错误。\r\n命令语句:{0}\r\nOutputMsg:{1}\r\nErrorMsg:{2}".FormatWith(cmdArguments, r1.OutputMsg, r1.ErrorMsg));
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
