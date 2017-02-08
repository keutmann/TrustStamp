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
        public string Message { get; set; }

        public FailedWorkflow(string message = "")
        {
            Message = message;
        }


        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                var msg = "Workflow has stopped";
                if (!string.IsNullOrEmpty(Message))
                    msg = " : " + Message;

                WriteLog(msg, db);

                CurrentBatch["active"] = 0;
                db.BatchTable.Update(CurrentBatch);
            }
        }
    }
}
