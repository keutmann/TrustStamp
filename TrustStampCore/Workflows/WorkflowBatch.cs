using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{


    public abstract class WorkflowBatch
    {
        public JObject CurrentBatch { get; set; }
        public Stack<WorkflowBatch> Workflows { get; set; }

        public abstract string StateName { get; }

        public virtual void Execute()
        {
        }

        public virtual void UpdateState(JObject state, TimeStampDatabase db)
        {
            CurrentBatch["state"] = state;
            db.Batch.Update(CurrentBatch);
        }

        public virtual void WriteLog(string message, TimeStampDatabase db)
        {
            this.WriteLog(StateName, message, db);
        }

        public virtual void WriteLog(string source, string message, TimeStampDatabase db)
        {
            var log = (JArray)CurrentBatch["log"];
            log.Add(new JObject(
                new JProperty("Time", DateTime.Now),
                new JProperty("Source", source),
                new JProperty("Message", message)
                ));
        }

        public virtual void SetState()
        {
            CurrentBatch["state"] = new JObject(new JProperty("state", StateName));
        }

        public virtual void Push(string name)
        {
            Workflows.Push(WorkflowEngine.CreateAndSetState(name, CurrentBatch, Workflows));
        }

    }
}
