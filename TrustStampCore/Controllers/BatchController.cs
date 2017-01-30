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

    public class BatchController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get([FromUri]string partition)
        {
            try
            {
                var batch = new Batch();
                var item = batch.Get(partition);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }
    }
}

