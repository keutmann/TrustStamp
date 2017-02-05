using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampCore.Controllers
{
    public class InfoController : ApiController
    {

        public IHttpActionResult GetStatus()
        {
            try
            {
                using (var db = TimeStampDatabase.Open())
                {
                    var status = new JObject();
                    status["Proof"] = db.ProofTable.Count();
                    status["Batch"] = db.BatchTable.Count();

                    var btc = new BitcoinManager();
                    status["blockchain"] = new JObject();
                    status["blockchain"]["btc"] = new JObject();
                    status["blockchain"]["btc"]["network"] = btc.CurrentNetwork.Name;
                    status["blockchain"]["btc"]["address"] = btc.Adr32.ToWif();

                    return Ok(status);
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }


    }
}
