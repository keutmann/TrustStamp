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
        protected JValue Timeout;
        protected JValue NextWorkflow;


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
            Sleep = CurrentBatch["state"].EnsureObject("sleep");
            Timeout = Sleep.EnsureProperty("timeout", TimeoutDate);
            NextWorkflow = Sleep.EnsureProperty("nextworkflow", NextWorkflowName);
            return true;
        }

        public override void Execute()
        {
            var timeOutDate = (DateTime)Timeout.Value; //.ToDateTime(DateTimeOfInstance);
            if (DateTimeOfInstance == timeOutDate)
                WriteLog("Workflow sleeping, reactivate on "+timeOutDate.ToString()); // Will on be called once!

            if (DateTimeOfInstance < timeOutDate)
                return; // Not ready yet!

             var wf = WorkflowEngine.CreateInstance((string)NextWorkflow.Value, CurrentBatch, Workflows);
            Push(wf);

            Update();
        }
    }
}


