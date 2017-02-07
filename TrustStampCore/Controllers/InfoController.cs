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
                    var info = new Info(db);
                    return Ok(info.Status());
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }


    }
}
