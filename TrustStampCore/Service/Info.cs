using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public class Info: BusinessService
    {
        public Info(TimeStampDatabase db) : base(db)
        {
        }


        public static Info OpenWithDatabase()
        {
            var p = new Info(TimeStampDatabase.Open());
            p.LocalDB = true;
            return p;
        }

        public JObject Status()
        {
            var btc = new BitcoinManager();

            var obj = new JObject(
                new JProperty("proof", DB.ProofTable.Count()),
                new JProperty("batch", DB.BatchTable.Count()),
                new JProperty("blockchain", new JObject(
                    new JProperty("btc", new JObject(
                        new JProperty("network", btc.CurrentNetwork.Name),
                        new JProperty("address", btc.Adr32.ToWif())
                        ))
                    )));

            return obj;
        }

    }
}
