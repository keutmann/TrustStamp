using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class Batch
    {
        //public string _PartitionKey = null;
        //public string PartitionKey
        //{
        //    get
        //    {
        //        return _PartitionKey ?? string.Format("{0}0000", DateTime.Now.ToString("yyyyMMddHH"));
        //    }
        //    set
        //    {
        //        _PartitionKey = value;
        //    }
        //}


        public static Func<string> PartitionMedthod = DefaultPartition;

        public static string DefaultPartition()
        {
            return string.Format("{0}0000", DateTime.Now.ToString("yyyyMMddHH"));
        } 

        public static string GetCurrentPartition()
        {
            return PartitionMedthod();
        }
    }
}
