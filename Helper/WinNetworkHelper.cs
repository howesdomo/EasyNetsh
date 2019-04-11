using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Howe.Helper
{

    /// <summary>
    /// Author : Howe
    /// Date   : 2016-8-7 13:12:12
    /// Description : 获取windows系统网卡信息
    /// 
    /// NetworkInterface 知识总结
    /// 区分 PnpInstanceID     
    /// 如果前面有 PCI 就是本机的真实网卡    
    /// MediaSubType 为 01 则是常见网卡，02为无线网卡。 
    /// </summary>
    public class WinNetworkHelper
    {
        /// <summary>
        /// 获取系统所有可用网卡
        /// </summary>
        /// <returns></returns>
        public static List<NetworkInterfaceAdv> GetAll()
        {
            List<NetworkInterfaceAdv> result = new List<NetworkInterfaceAdv>();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                string fCardType = "未知网卡";
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID     
                    // 如果前面有 PCI 就是本机的真实网卡    
                    // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                    string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                    {
                        fCardType = "物理网卡";
                        if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            fCardType = "无线网卡";
                        }
                    }
                    else if (fMediaSubType == 1)
                    {
                        fCardType = "虚拟网卡";
                    }
                    else if (fMediaSubType == 2)
                    {
                        fCardType = "无线网卡";
                    }
                }

                NetworkInterfaceAdv toAdd = new NetworkInterfaceAdv(adapter, fCardType);
                result.Add(toAdd);
            }
            return result;
        }

        /// <summary>
        /// 获取所有物理网卡&无线网卡列表, 按 无线网卡 > 有线网卡 排序
        /// </summary>
        /// <returns></returns>
        public static List<NetworkInterfaceAdv> GetAllDevice()
        {
            List<NetworkInterfaceAdv> all = WinNetworkHelper.GetAll();
            List<NetworkInterfaceAdv> result = new List<NetworkInterfaceAdv>();
            result.AddRange(all.Where(i => i.CardType == NetworkDeviceType.WirelessDevice)); // 无线网卡
            result.AddRange(all.Where(i => i.CardType == NetworkDeviceType.PhysicalDevice)); // 有线网卡
            result.Add(new NetworkInterfaceAdv(null, "未知网卡")); // 刷新
            return result;
        }
    }


    public class NetworkInterfaceAdv
    {
        public const string RefleshInfo = "刷新...";

        public NetworkInterfaceAdv(NetworkInterface adapter, string typeCode)
        {
            if (adapter == null)
            {
                this.Name = RefleshInfo;
                return;
            }

            this.Id = adapter.Id; // 获取网络适配器的标识符    
            this.Name = adapter.Name; // 获取网络适配器的名称    
            this.Description = adapter.Description; // 获取接口的描述    
            this.NetworkInterfaceType = adapter.NetworkInterfaceType; // 获取接口类型    
            this.IsReceiveOnly = adapter.IsReceiveOnly; // 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。    
            this.SupportsMulticast = adapter.SupportsMulticast; // 获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。    
            this.Speed = adapter.Speed; // 网络接口的速度    
            this.PhysicalAddress = adapter.GetPhysicalAddress(); // MAC 地址    

            IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
            UnicastIPAddressInformationCollection collection = fIPInterfaceProperties.UnicastAddresses;
            foreach (UnicastIPAddressInformation UnicastIPAddressInformation in collection)
            {
                if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.IPAddress = UnicastIPAddressInformation.Address; // Ip 地址
                }
            }

            this.CardType = this.GetCardType(typeCode);
        }

        private NetworkDeviceType GetCardType(string type)
        {
            switch (type)
            {
                case "物理网卡": return NetworkDeviceType.PhysicalDevice;
                case "无线网卡": return NetworkDeviceType.WirelessDevice;
                case "虚拟网卡": return NetworkDeviceType.VirtualDevice;
                case "未知网卡": return NetworkDeviceType.Unknow;
                default: return NetworkDeviceType.Unknow;
            }
        }

        /// <summary>
        /// 网卡类型
        /// </summary>
        public NetworkDeviceType CardType { get; private set; }

        /// <summary>
        /// 控制台输出网卡基本信息
        /// </summary>
        public static void ShowNetworkInterfaceMessage()
        {
            NetworkInterface[] fNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in fNetworkInterfaces)
            {
                #region " 网卡类型 "
                string fCardType = "未知网卡";
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    // 区分 PnpInstanceID     
                    // 如果前面有 PCI 就是本机的真实网卡    
                    // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                    string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                    if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                    {
                        fCardType = "物理网卡";
                        if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                        {
                            fCardType = "无线网卡";
                        }
                    }
                    else if (fMediaSubType == 1)
                    {
                        fCardType = "虚拟网卡";
                    }
                    else if (fMediaSubType == 2)
                    {
                        fCardType = "无线网卡";
                    }
                }
                #endregion
                #region " 网卡信息 "
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("-- " + fCardType);
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine("Id .................. : {0}", adapter.Id); // 获取网络适配器的标识符    
                Console.WriteLine("Name ................ : {0}", adapter.Name); // 获取网络适配器的名称    
                Console.WriteLine("Description ......... : {0}", adapter.Description); // 获取接口的描述    
                Console.WriteLine("Interface type ...... : {0}", adapter.NetworkInterfaceType); // 获取接口类型    
                Console.WriteLine("Is receive only...... : {0}", adapter.IsReceiveOnly); // 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。    
                Console.WriteLine("Multicast............ : {0}", adapter.SupportsMulticast); // 获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。    
                Console.WriteLine("Speed ............... : {0}", adapter.Speed); // 网络接口的速度    
                Console.WriteLine("Physical Address .... : {0}", adapter.GetPhysicalAddress().ToString()); // MAC 地址    
                IPInterfaceProperties fIPInterfaceProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = fIPInterfaceProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        Console.WriteLine("Ip Address .......... : {0}", UnicastIPAddressInformation.Address); // Ip 地址    
                }
                Console.WriteLine();
                #endregion
            }
        }

        #region Properties

        /// <summary>
        /// 获取网络适配器的标识符
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 获取网络适配器的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取接口的描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 获取接口类型
        /// </summary>
        public NetworkInterfaceType NetworkInterfaceType { get; private set; }

        /// <summary>
        /// 获取 Boolean 值，该值指示网络接口是否设置为仅接收数据包。    
        /// </summary>
        public bool IsReceiveOnly { get; private set; }

        /// <summary>
        ///  获取 Boolean 值，该值指示是否启用网络接口以接收多路广播数据包。 
        /// </summary>
        public bool SupportsMulticast { get; private set; }

        /// <summary>
        /// 网络接口的速度
        /// </summary>
        public long Speed { get; private set; }

        /// <summary>
        /// MAC 地址 
        /// </summary>
        public PhysicalAddress PhysicalAddress { get; private set; }

        /// <summary>
        /// IP 地址
        /// </summary>
        public IPAddress IPAddress { get; private set; }
        #endregion

        #region UI

        public string DisplayName
        {
            get
            {
                string result = string.Empty;
                try
                {
                    if (this.Name == RefleshInfo)
                    {
                        result = this.Name;
                    }
                    else
                    {
                        result = string.Format("{0}( {1} )", this.Name, this.IPAddress);
                    }
                }
                catch (Exception) { }
                return result;
            }
        }

        #endregion
    }

    /// <summary>
    /// 网卡类型
    /// </summary>
    public enum NetworkDeviceType
    {
        /// <summary>
        /// 物理网卡
        /// </summary>
        PhysicalDevice = 0,
        /// <summary>
        /// 无线网卡
        /// </summary>
        WirelessDevice,
        /// <summary>
        /// 虚拟网卡
        /// </summary>
        VirtualDevice,
        /// <summary>
        /// 未知类型
        /// </summary>
        Unknow
    }
}
