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
        public const string Name = "Faild";

        public override string StateName
        {
            get
            {
                return Name;
            }
        }

        static FailedWorkflow()
        {
            WorkflowEngine.WorkflowTypes.Add(Name, typeof(FailedWorkflow));
        }

        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Workflow has stopped", db);

                CurrentBatch["active"] = 0;
                db.Batch.Update(CurrentBatch);
            }
        }
    }
}
