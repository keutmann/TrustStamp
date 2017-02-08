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
    public class Proof : BusinessService
    {

        public static Proof OpenWithDatabase()
        {
            var p = new Proof(TrustStampDatabase.Open());
            p.LocalDB = true;
            return p;
        }

        public Proof(TrustStampDatabase db) : base(db)
        {
        }

        public JObject Add(string id)
        {
            var idc =  IDContainer.Parse(id);
            
            var item = DB.ProofTable.GetByHash(idc.Hash);
            if (item != null)
                return item;

            item = DB.ProofTable.NewItem(idc.Hash, null, Batch.GetCurrentPartition(), DateTime.Now);
            DB.ProofTable.Add(item);

            return item;
        }

        public JObject Get(string id)
        {
            var idc = IDContainer.Parse(id);
            
            var item = DB.ProofTable.GetByHash(idc.Hash);
            if (item != null)
                return item;

            return null;
        }

        //public JArray UnprocessedPartitions()
        //{
        //    return DB.Proof.GetUnprocessed();
        //}



    }
}
