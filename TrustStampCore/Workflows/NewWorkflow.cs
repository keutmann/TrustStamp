using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{
    public class NewWorkflow : WorkflowBatch
    {
        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Workflow stated", db);

                Push(new MerkleWorkflow());

                db.BatchTable.Update(CurrentBatch);
            }
        }
    }
}
