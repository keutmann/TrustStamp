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
        public const string Name = "New";
        public override string StateName
        {
            get
            {
                return Name;
            }
        }

        static NewWorkflow()
        {
            WorkflowEngine.WorkflowTypes.Add(Name, typeof(NewWorkflow));
        }

        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Workflow stated", db);

                Push(MerkleWorkflow.Name);

                db.Batch.Update(CurrentBatch);
            }
        }
    }
}
