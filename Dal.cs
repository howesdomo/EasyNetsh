using Howe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyNetsh
{
    class Dal
    {

        public List<EasyNetshModel> GetAll(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    List<EasyNetshModel> result = Howe.Helper.SerializationHelper.Load(typeof(List<EasyNetshModel>), path) as List<EasyNetshModel>;
                    return result;
                }
                else
                {
                    return new List<EasyNetshModel>();
                }
            }
            catch (Exception)
            {
                return new List<EasyNetshModel>();
            }
        }

        public void Add(List<EasyNetshModel> list, EasyNetshModel toAdd)
        {
            if (list.Exists(i => i.Name == toAdd.Name))
            {
                EasyNetshModel match = list.FirstOrDefault(i => i.Name == toAdd.Name);
                match.TargetDevice = toAdd.TargetDevice;
                match.IP = toAdd.IP;
                match.SubMask = toAdd.SubMask;
                match.GateWay = toAdd.GateWay;

                match.DNS_1 = toAdd.DNS_1;
                match.DNS_2 = toAdd.DNS_2;

                match.IsDHCP = toAdd.IsDHCP;
            }
            else
            {
                list.Add(toAdd);
            }
        }

        public void Save(List<EasyNetshModel> toSave, string path)
        {
            Howe.Helper.SerializationHelper.Save(toSave, path);
        }
    }
}
