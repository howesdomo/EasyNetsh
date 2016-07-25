using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Howe.Model
{
    public class EasyNetshModel : IComparable<EasyNetshModel>
    {
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
                        !string.IsNullOrEmpty(this.IP)
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
                    if (tmp.IP == this.IP
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
    }
}
