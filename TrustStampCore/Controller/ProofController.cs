using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TrustStampCore.Repository;

namespace TrustStampCore.Controller
{

    public class ProofController : ApiController
    {

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            using (var db = TimeStampDatabase.Open())
            {
                var proof = ProofTable.Get(db.Connection);

                //var unprocessed = batch.GetUnprocessed();

                return Ok();
            }

        }
    }
}

