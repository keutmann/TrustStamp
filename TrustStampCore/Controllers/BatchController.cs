using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IHttpActionResult GetAllBatchs()
        {
            try
            {
                using (var db = TimeStampDatabase.Open())
                {
                    var items = db.BatchTable.Select(100);
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }

        [HttpGet]
        public IHttpActionResult GetBatch([FromUri]string id)
        {
            try
            {
                using (var batch = Batch.OpenWithDatabase())
                {
                    var item = batch.Get(id);

                    //var s = new JsonSerializer();

                    //var sb = new StringBuilder();
                    //var sw = new StringWriter(sb);

                    //s.Converters.Add(new BytesToHexConverter());
                    //s.Serialize(sw, item);
                    //var json = sb.ToString();

                    //return Ok(json);
                    return Ok(item);
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }

        public IHttpActionResult Count()
        {
            try
            {
                using (var db = TimeStampDatabase.Open())
                {
                    return Ok(db.BatchTable.Count());
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex, this);
            }
        }
    }
}

