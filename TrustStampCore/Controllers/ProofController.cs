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
        public const string Path = "/api/proof/";

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
        public IHttpActionResult Get([FromUri]string id)
        {
            try
            {
                var proof = new Proof();
                var result = proof.Get(id);

                if (result == null)
                    return Ok("ID Not found!");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }
    }
}

