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
            var base64id = VerifyAndGetBase64(id);

            using (var db = TimeStampDatabase.Open())
            {
                var table = ProofTable.Get(db.Connection);

                var item = table.GetByHash(base64id);
                if (item != null)
                    return item;

                item = table.NewItem(id, null, Batch.TimeStampSlice());
                table.Add(item);

                return item;
            }
        }

        public JObject Get(string id)
        {
            var base64id = VerifyAndGetBase64(id);

            using (var db = TimeStampDatabase.Open())
            {
                var table = ProofTable.Get(db.Connection);

                var item = table.GetByHash(base64id);
                if (item != null)
                    return item;

                return null;
            }
        }

        public string VerifyAndGetBase64(string id)
        {
            byte[] hash;
            if (string.IsNullOrEmpty(id))
                throw new ApplicationException("Value cannot be empty");

            if (id.Length == 64)
                hash = Hex.ToBytes(id);
            else
                hash = HttpServerUtility.UrlTokenDecode(id);

            if (hash.Length != 32)
                throw new ApplicationException("Invalid SHA256 hash format, byte length is " + hash.Length);

            var base64id = HttpServerUtility.UrlTokenEncode(hash);
            return base64id;
        }

    }
}
