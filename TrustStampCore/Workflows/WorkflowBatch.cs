using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{


    public abstract class WorkflowBatch : IDisposable
    {
        public JObject CurrentBatch { get; set; }
        public WorkflowContext Context { get; set; }

        TrustStampDatabase _dataBase = null;
        public TrustStampDatabase DataBase {
            get
            {
                return _dataBase ?? (_dataBase = TrustStampDatabase.Open());
            }
        }

        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual bool Initialize()
        {
            SetStateName();
            return true;
        }

        public virtual void Execute()
        {
        }

        public virtual void WriteLog(string message)
        {
            WriteLog(Name, message);
        }

        public virtual void WriteLog(string source, string message)
        {
            var log = (JArray)CurrentBatch["log"];
            log.Add(new JObject(
                new JProperty("Time", DateTime.Now),
                new JProperty("Source", source),
                new JProperty("Message", message)
                ));

            Console.WriteLine(DateTime.Now.ToShortTimeString()+ ": "+ source + ": " + message);
        }


        public void SetStateName()
        {
            CurrentBatch.EnsureObject("state").SetProperty("name", Name);
        }

        public virtual void Push(string name)
        {
            var wf = (WorkflowBatch)Activator.CreateInstance(WorkflowContext.WorkflowTypes[name]);
            Push(wf);
        }

        public virtual void Push(JObject batch)
        {

            var wf = Context.CreateInstance(batch);
            Push(wf);
        }

        public virtual void Push(WorkflowBatch wf)
        {
            Context.Push(wf, CurrentBatch);
        }

        public virtual void Update()
        {
            DataBase.BatchTable.Update(CurrentBatch);
        }

        public void Dispose()
        {
            Update();

            if (_dataBase != null)
                _dataBase.Dispose();
        }
    }
}
