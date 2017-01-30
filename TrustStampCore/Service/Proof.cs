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
                var safeId = idContainer.GetSafeSHA256ID();

                var item = db.Proof.GetByHash(safeId);
                if (item != null)
                    return item;

                item = db.Proof.NewItem(safeId, null, Batch.GetCurrentPartition(), DateTime.Now);
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

                var item = table.GetByHash(idContainer.GetSafeSHA256ID());
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
