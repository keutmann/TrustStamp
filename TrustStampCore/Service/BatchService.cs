using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Service
{
    public class BatchService
    {
        public string DatabaseName { get; set; }

        public BatchService(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public void RequestReceived(object sender, RequestParameters e)
        {
            if (!e.Context.Request.RawUrl.Contains("addbatch"))
                return;


            using (var db = new TimeStampDatabase(DatabaseName))
            {
                db.Open();
                var batch = new BatchTable(db.Connection);
                batch.CreateIfNotExist();

                batch.Add(new JObject(
                    new JProperty("root", "abc"),
                    new JProperty("state", 1)));

                byte[] response = Encoding.UTF8.GetBytes("Batch added! - "+DateTime.Now.ToLongTimeString()); //responseData.Encode();
                e.Context.Response.OutputStream.Write(response, 0, response.Length);
            }

        }
    }
}
