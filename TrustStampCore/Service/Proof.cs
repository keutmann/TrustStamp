using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrustStampCore.Extensions;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public class Proof : BusinessDB
    {

        public Proof(TimeStampDatabase db = null) : base(db)
        {
        }

        public JObject Add(string id)
        {
            var idc =  IDContainer.Parse(id);
            
            var item = DB.Proof.GetByHash(idc.Hash);
            if (item != null)
                return item;

            item = DB.Proof.NewItem(idc.Hash, null, Batch.GetCurrentPartition(), DateTime.Now);
            DB.Proof.Add(item);

            return item;
        }

        public JObject Get(string id)
        {
            var idc = IDContainer.Parse(id);
            
            var item = DB.Proof.GetByHash(idc.Hash);
            if (item != null)
                return item;

            return null;
        }

        public JArray UnprocessedPartitions()
        {
            return DB.Proof.GetUnprocessed();
        }



    }
}
