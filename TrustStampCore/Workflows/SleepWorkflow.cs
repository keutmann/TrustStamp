using TrustStampCore.Repository;
using TrustStampCore.Extensions;
using System;
using Newtonsoft.Json.Linq;

namespace TrustStampCore.Workflows
{
    public class SleepWorkflow : WorkflowBatch
    {

        protected DateTime DateTimeOfInstance;
        protected DateTime TimeoutDate;
        protected string NextWorkflow;

        public SleepWorkflow()
        {
            DateTimeOfInstance = DateTime.Now;
            TimeoutDate = DateTimeOfInstance;
        }

        public SleepWorkflow(DateTime timeoutDate, string nextWorkflow) : this()
        {
            TimeoutDate = timeoutDate;
            NextWorkflow = nextWorkflow;
        }

        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                var timeOutDate = CurrentBatch["state"]["timeout"].ToDateTime(DateTimeOfInstance);
                if (DateTimeOfInstance == timeOutDate)
                    WriteLog("Workflow sleeping, reactivate on "+timeOutDate.ToString(), db); // Will on be called once!

                if (DateTimeOfInstance < timeOutDate)
                    return; // Not ready yet!

                var nextWorkflowName = CurrentBatch["state"]["nextworkflow"].ToStringValue("");
                if (String.IsNullOrEmpty(nextWorkflowName))
                    return; // No workflow specified for handling of the next step.

                Push(nextWorkflowName);

                db.BatchTable.Update(CurrentBatch);
            }
        }

        public override void SetState()
        {
            CurrentBatch["state"] = new JObject(
                new JProperty("state", Name),
                new JProperty("timeout", TimeoutDate),
                new JProperty("nextworkflow", NextWorkflow)
                );

        }
    }
}

