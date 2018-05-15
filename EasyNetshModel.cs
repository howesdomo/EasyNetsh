using Howe.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Howe.Model
{
    public class EasyNetshModel : IComparable<EasyNetshModel>
    {
        public EasyNetshModel()
        {

        }

        public EasyNetshModel(string targetDevice, string ip, string submask, string gateway, string dns_1, string dns_2, string name)
        {
            ID = Guid.NewGuid().ToString();
            this.TargetDevice = StringHelper.StringToString(targetDevice).Trim();

            this.IP = StringHelper.StringToString(ip).Trim();
            this.SubMask = StringHelper.StringToString(submask).Trim();
            this.GateWay = StringHelper.StringToString(gateway).Trim();
            this.DNS_1 = StringHelper.StringToString(dns_1).Trim();
            this.DNS_2 = StringHelper.StringToString(dns_2).Trim();

            this.Name = StringHelper.StringToString(name).Trim();
            this.CreateDate = DateTime.Now;
            this.LastestUseDate = DateTime.Now;
        }

        public string TargetDevice { get; set; }

        public string ID { get; set; }

        public bool IsDHCP { get; set; }

        public string IP { get; set; }

        public string SubMask { get; set; }

        public string GateWay { get; set; }

        public string DNS_1 { get; set; }

        public string DNS_2 { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastestUseDate { get; set; }

        public bool SaveEnabled
        {
            get
            {
                if (
                           !string.IsNullOrEmpty(this.TargetDevice)
                        && !string.IsNullOrEmpty(this.IP)
                        && !string.IsNullOrEmpty(this.SubMask)
                        && !string.IsNullOrEmpty(this.GateWay)
                    //&& !string.IsNullOrEmpty(this.DNS_1)
                    //&& !string.IsNullOrEmpty(this.DNS_2)
                        )
                {
                    return true;
                }

                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is EasyNetshModel)
            {
                EasyNetshModel tmp = obj as EasyNetshModel;
                if (tmp.IsDHCP == this.IsDHCP == true)
                {
                    return true;
                }
                else
                {
                    if (
                           tmp.TargetDevice == this.TargetDevice
                        && tmp.IP == this.IP
                        && tmp.SubMask == this.SubMask
                        && tmp.GateWay == this.GateWay
                        && tmp.DNS_1 == this.DNS_1
                        && tmp.DNS_2 == this.DNS_2
                        )
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(EasyNetshModel other)
        {
            if (this.LastestUseDate.HasValue == other.LastestUseDate.HasValue == true)
            {
                return this.LastestUseDate.Value.CompareTo(other.LastestUseDate.Value);
            }
            else if (this.LastestUseDate.HasValue == other.LastestUseDate.HasValue == false)
            {
                return this.CreateDate.CompareTo(other.CreateDate);
            }
            else
            {
                return 0;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void SetIPAddressByNetshHelper()
        {
            if (this.IsDHCP)
            {
                Howe.Helper.NetshHelper.SetDHCP(this.TargetDevice);
            }
            else
            {
                // 由于没有执行返回信息已被弃用
                // Howe.Helper.NetshHelper.SetIPAddress(this.TargetDevice, this.IP, this.SubMask, this.GateWay, this.DNS_1, this.DNS_2);

                // 由于V2版本运行方式更为严谨所以采用V2
                Howe.Helper.NetshHelper.SetIPAddressV2(this.TargetDevice, this.IP, this.SubMask, this.GateWay, this.DNS_1, this.DNS_2);

                // Howe.Helper.NetshHelper.SetIPAddressV3(this.TargetDevice, this.IP, this.SubMask, this.GateWay, this.DNS_1, this.DNS_2);
            }
        }


        public string GetToolTipInfo()
        {
            return string.Format("设　　备：{0}\nＩ　　Ｐ：{1}\n子网掩码：{2}\n网　　关：{3}\nＤＮＳ１：{4}\nＤＮＳ２：{5}", this.TargetDevice, this.IP, this.SubMask, this.GateWay, this.DNS_1, this.DNS_2);
        }
    }
}
