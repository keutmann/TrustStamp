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

    public class ProofController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Add([FromUri]string id)
        {
            try
            {
                var proof = new Proof();
                var result = proof.Add(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }



        // GET api/
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var proof = new Proof();
                var result = proof.Get(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }
    }
}

