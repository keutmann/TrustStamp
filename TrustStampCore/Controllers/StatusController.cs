using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TrustStampCore.Repository;

namespace TrustStampCore.Controllers
{
    public class StatusController : ApiController
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
