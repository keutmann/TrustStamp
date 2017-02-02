using System;
using System.Web.Http;
using System.Web.Http.Results;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

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
                using (var proof = Proof.OpenWithDatabase())
                {
                    var result = proof.Add(id);
                    return Ok(result.CustomRender());
                }
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
                using (var proof = Proof.OpenWithDatabase())
                {
                    var result = proof.Get(id);

                    if (result == null)
                        return Ok("ID Not found!");

                    return Ok(result.CustomRender());
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }

        [HttpGet]
        public IHttpActionResult GetAllProofs()
        {
            try
            {
                using (var db = TimeStampDatabase.Open())
                {
                    var items = db.ProofTable.Select(100);
                    return Ok(items.CustomRender());
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }


        //[HttpGet]
        //public IHttpActionResult GetCount()
        //{
        //    try
        //    {
        //        using (var db = TimeStampDatabase.Open())
        //        {
        //            return Ok(db.BatchTable.Count());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ExceptionResult(ex, this);
        //    }
        //}
    }
}

