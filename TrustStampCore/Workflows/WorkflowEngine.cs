using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class WorkflowEngine
    {
        public static Dictionary<string, Type> WorkflowTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public Stack<WorkflowBatch> Workflows = new Stack<WorkflowBatch>();

        static WorkflowEngine()
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


        public WorkflowEngine(JArray batchs)
        {
            foreach (JObject batch in batchs)
            {
                var wf = CreateInstance(batch, Workflows);

                Workflows.Push(wf);
            }
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

        public static WorkflowBatch CreateInstance(JObject batch, Stack<WorkflowBatch> workflows)
        {
            // Set to New state if empty!
            var name = batch["state"].EnsureProperty("name", typeof(NewWorkflow).Name);

            return CreateInstance(name.Value.ToStringValue(), batch, workflows);
        }

        public static WorkflowBatch CreateInstance(string name, JObject batch, Stack<WorkflowBatch> workflows)
        {
            if (!WorkflowTypes.ContainsKey(name))
                return null; // Handle this as an error!!!

            var workflowType = WorkflowTypes[name];

            var wf = (WorkflowBatch)Activator.CreateInstance(workflowType);
            wf.CurrentBatch = batch;
            wf.Workflows = workflows;
            return wf;
        }

        public static WorkflowBatch CreateAndSetState(string name, JObject batch, Stack<WorkflowBatch> workflows)
        {
            var wf = CreateInstance(name, batch, workflows);
            return wf;
        }
    }
}
