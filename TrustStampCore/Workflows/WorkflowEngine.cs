using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class WorkflowContext
    {
        public static Dictionary<string, Type> WorkflowTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public Stack<WorkflowBatch> Workflows = new Stack<WorkflowBatch>();

        static WorkflowContext()
        {
            AddWorkflowType(typeof(BitcoinWorkflow));
            AddWorkflowType(typeof(FailedWorkflow));
            AddWorkflowType(typeof(MerkleWorkflow));
            AddWorkflowType(typeof(NewWorkflow));
            AddWorkflowType(typeof(RemotePayWorkflow));
            AddWorkflowType(typeof(RemoteStampWorkflow));
            AddWorkflowType(typeof(SleepWorkflow));
            AddWorkflowType(typeof(SuccessWorkflow));
        }

        private static void AddWorkflowType(Type wfType)
        {
            if (!WorkflowTypes.ContainsKey(wfType.Name))
                WorkflowTypes.Add(wfType.Name, wfType);
        }

        public WorkflowContext(JArray batchs) : this()
        {
            foreach (JObject batch in batchs)
            {
                Push(batch);
            }
        }

        public WorkflowContext()
        {
        }



        public void Execute()
        {
            while(Workflows.Count > 0) // Possiblility for parallel execution!?
            {
                using (var wf = Workflows.Pop())
                {
                    try
                    {
                        if (wf.Initialize()) // Initialize and make sure that dependencies are ready
                            wf.Execute();
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void Push(JObject batch)
        {
            var wf = CreateInstance(batch);
            Push(wf, batch);
        }


        public void Push(WorkflowBatch wf, JObject currentBatch)
        {
            wf.CurrentBatch = currentBatch;
            wf.Context = this;
            Workflows.Push(wf);
        }

        public WorkflowBatch CreateInstance(JObject batch)
        {
            // Set to New state if empty!
            var name = batch["state"].EnsureProperty("name", typeof(NewWorkflow).Name);

            return CreateInstance((string)name.Value, batch);
        }

        public WorkflowBatch CreateInstance(string name, JObject batch)
        {
            if (!WorkflowTypes.ContainsKey(name))
                return null; // Handle this as an error!!!

            var workflowType = WorkflowTypes[name];

            var wf = (WorkflowBatch)Activator.CreateInstance(workflowType);
            wf.CurrentBatch = batch;
            wf.Context = this;
            return wf;
        }

        public WorkflowBatch CreateAndSetState(string name, JObject batch)
        {
            var wf = CreateInstance(name, batch);
            return wf;
        }
    }
}
