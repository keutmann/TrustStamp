using Newtonsoft.Json.Linq;
using TrustStampCore.Repository;
using TrustStampCore.Extensions;

namespace TrustStampCore.Service
{
    public class Info: BusinessService
    {
        public Info(TrustStampDatabase db) : base(db)
        {
        }


        public static Info OpenWithDatabase()
        {
            var p = new Info(TrustStampDatabase.Open());
            p.LocalDB = true;
            return p;
        }

        public JObject Status()
        {

            var obj = new JObject(
                new JProperty("proof", DB.ProofTable.Count()),
                new JProperty("batch", DB.BatchTable.Count()));

            var wif = App.Config["btcwif"].ToStringValue();
            if (!string.IsNullOrEmpty(wif))
            {
                var btc = new Bitcoin(wif, null, BlockchainFactory.GetBitcoinNetwork());
                obj.Add(new JProperty("blockchain", new JObject(
                        new JProperty("btc", new JObject(
                            new JProperty("network", btc.Network.Name),
                            new JProperty("address", btc.SourceAddress.ToWif())
                            ))
                        )));
            }
            return obj;
        }

    }
}

