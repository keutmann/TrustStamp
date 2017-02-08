using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{
    public class SuccessWorkflow : WorkflowBatch
    {
        public override void Execute()
        {
            using (var db = TrustStampDatabase.Open())
            {
                WriteLog("The workflow has finished", db);

                CurrentBatch["active"] = 0;
                db.BatchTable.Update(CurrentBatch);
            }
        }
    }
}
