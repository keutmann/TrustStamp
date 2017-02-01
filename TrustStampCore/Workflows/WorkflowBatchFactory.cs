using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Workflows
{
    //public abstract class WorkflowBatchFactory
    //{
    //    public Type WorkflowType { get; }

    //    public WorkflowBatchFactory(Type workflowType)
    //    {
    //        WorkflowType = workflowType;
    //    }



    //    public virtual WorkflowBatch Create(JObject batch, Stack<WorkflowBatch> workflows)
    //    {
    //        var wf = (WorkflowBatch)Activator.CreateInstance(WorkflowType);
    //        wf.CurrentBatch = batch;
    //        wf.Workflows = workflows;
    //        return wf;
    //    }


    //    //public static WorkflowBatchFactory CreateAndSetState(JObject batch)
    //    //{
    //    //    var wf = new T();
    //    //    wf.CurrentBatch = batch;
    //    //    wf.SetState();
    //    //    return wf;
    //    //}


    //    public virtual bool Match(JObject batch)
    //    {
    //        return false;
    //    }

        
    //}
}
