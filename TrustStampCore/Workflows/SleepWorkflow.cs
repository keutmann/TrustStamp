using TrustStampCore.Repository;
using TrustStampCore.Extensions;
using System;
using Newtonsoft.Json.Linq;

namespace TrustStampCore.Workflows
{
    public class SleepWorkflow : WorkflowBatch
    {

        protected JObject Sleep;
        protected DateTime DateTimeOfInstance;
        protected DateTime TimeoutDate;
        protected string NextWorkflowName;
        protected JProperty Timeout;
        protected JProperty NextWorkflow;


        public SleepWorkflow()
        {
            DateTimeOfInstance = DateTime.Now;
            TimeoutDate = DateTimeOfInstance;
        }

        public SleepWorkflow(DateTime timeoutDate, string nextWorkflowname) : this()
        {
            TimeoutDate = timeoutDate;
            NextWorkflowName = nextWorkflowname;
        }

        public override bool Initialize()
        {
            if (!base.Initialize())
                return false;
            Sleep = CurrentBatch["state"]["sleep"].EnsureObject();
            Timeout = Sleep.EnsureProperty("timeout", TimeoutDate);
            NextWorkflow = Sleep.EnsureProperty("nextworkflow", NextWorkflowName);
            return true;
        }

        public override void Execute()
        {
            var timeOutDate = Timeout.Value.ToDateTime(DateTimeOfInstance);
            if (DateTimeOfInstance == timeOutDate)
                WriteLog("Workflow sleeping, reactivate on "+timeOutDate.ToString()); // Will on be called once!

            if (DateTimeOfInstance < timeOutDate)
                return; // Not ready yet!

             var wf = WorkflowEngine.CreateInstance(NextWorkflow.Value.ToStringValue(), CurrentBatch, Workflows);
            Push(wf);

            Update();
        }
    }
}


