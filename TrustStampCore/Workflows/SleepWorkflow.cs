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
        protected JToken NextWorkflowState;

        public SleepWorkflow()
        {
            DateTimeOfInstance = DateTime.Now;
            TimeoutDate = DateTimeOfInstance;
        }

        public SleepWorkflow(DateTime timeoutDate, JToken nextWorkflowstate) : this()
        {
            TimeoutDate = timeoutDate;
            NextWorkflowState = nextWorkflowstate;
        }

        public override void Execute()
        {

            using (var db = TrustStampDatabase.Open())
            {
                var timeOutDate = CurrentBatch["state"]["timeout"].ToDateTime(DateTimeOfInstance);
                if (DateTimeOfInstance == timeOutDate)
                    WriteLog("Workflow sleeping, reactivate on "+timeOutDate.ToString(), db); // Will on be called once!

                if (DateTimeOfInstance < timeOutDate)
                    return; // Not ready yet!

                var nextWorkflowState = (JObject)CurrentBatch["state"]["nextworkflowstate"];
                var nextWorkflowName = (string)nextWorkflowState["state"];

                var wf = WorkflowEngine.CreateInstance(nextWorkflowName, CurrentBatch, Workflows);
                Push(wf);

                db.BatchTable.Update(CurrentBatch);
            }
        }

        public override void SetState()
        {
            CurrentBatch["state"] = new JObject(
                new JProperty("state", Name),
                new JProperty("timeout", TimeoutDate),
                new JProperty("nextworkflowstate", NextWorkflowState)
                );

        }
    }
}


