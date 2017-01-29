using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TrustStampCore.Repository;

namespace TrustStampCore.Controllers
{

    public class BatchController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            using (var db = TimeStampDatabase.Open())
            {
                var batch = BatchTable.Get(db.Connection);

                var unprocessed = batch.GetUnprocessed();

                return Ok(unprocessed);
            }

        }
    }
}

