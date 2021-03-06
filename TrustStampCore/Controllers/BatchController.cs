﻿using System;
using System.Web.Http;
using System.Web.Http.Results;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Controllers
{

    public class BatchController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetAllBatchs()
        {
            try
            {
                using (var db = TrustStampDatabase.Open())
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
                using (var db = TrustStampDatabase.Open())
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

