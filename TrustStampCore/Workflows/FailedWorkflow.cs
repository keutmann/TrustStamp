using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{
    public class FailedWorkflow : WorkflowBatch
    {
        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Workflow has stopped", db);

                CurrentBatch["active"] = 0;
                db.BatchTable.Update(CurrentBatch);
            }
        }
    }
}
