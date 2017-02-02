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
        public const string Name = "Success";

        static SuccessWorkflow()
        {
            WorkflowEngine.WorkflowTypes.Add(Name, typeof(SuccessWorkflow));
        }

        public override string StateName
        {
            get
            {
                return Name;
            }
        }

        public override void Execute()
        {
            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("The workflow has finished", db);

                CurrentBatch["active"] = 0;
                db.BatchTable.Update(CurrentBatch);
            }
        }
    }
}
