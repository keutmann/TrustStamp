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

        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual void Execute()
        {
        }

        public virtual void UpdateState(JObject state, TrustStampDatabase db)
        {
            CurrentBatch["state"] = state;
            db.BatchTable.Update(CurrentBatch);
        }

        public virtual void WriteLog(string message, TrustStampDatabase db)
        {
            WriteLog(Name, message, db);
        }

        public virtual void WriteLog(string source, string message, TrustStampDatabase db)
        {
            var log = (JArray)CurrentBatch["log"];
            log.Add(new JObject(
                new JProperty("Time", DateTime.Now),
                new JProperty("Source", source),
                new JProperty("Message", message)
                ));

            Console.WriteLine(DateTime.Now.ToShortTimeString()+ ": "+ source + ": " + message);
        }


        public virtual void SetState()
        {
            CurrentBatch["state"] = new JObject(new JProperty("state", Name));
        }

        public virtual void Push(string name)
        {
            Push((WorkflowBatch)Activator.CreateInstance(WorkflowEngine.WorkflowTypes[name]));
        }

        public virtual void Push(WorkflowBatch wf)
        {
            wf.CurrentBatch = CurrentBatch;
            wf.SetState();
            wf.Workflows = Workflows;
            Workflows.Push(wf);
        }

    }
}
