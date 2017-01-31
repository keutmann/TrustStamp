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
    public class Proof
    {
        public JObject Add(string id)
        {
            var idContainer = new ID(id);
            
            using (var db = TimeStampDatabase.Open())
            {
                var hash = idContainer.GetSafeHashFromHex();

                var item = db.Proof.GetByHash(hash);
                if (item != null)
                    return item;

                item = db.Proof.NewItem(hash, null, Batch.GetCurrentPartition(), DateTime.Now);
                db.Proof.Add(item);

                return item;
            }
        }

        public JObject Get(string id)
        {
            var idContainer = new ID(id);
            
            using (var db = TimeStampDatabase.Open())
            {
                var table = new ProofTable(db.Connection);

                var item = table.GetByHash(idContainer.GetSafeHashFromHex());
                if (item != null)
                    return item;

                return null;
            }
        }

        public JArray UnprocessedPartitions()
        {
            using (var db = TimeStampDatabase.Open())
            {
                var table = new ProofTable(db.Connection);
                return table.GetUnprocessed();
            }
        }



    }
}
