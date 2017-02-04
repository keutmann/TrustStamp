﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Workflows
{
    public class WorkflowEngine
    {
        public static Dictionary<string, Type> WorkflowTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public Stack<WorkflowBatch> Workflows = new Stack<WorkflowBatch>();

        static WorkflowEngine()
        {
            AddWorkflowType(typeof(FailedWorkflow));
            AddWorkflowType(typeof(MerkleWorkflow));
            AddWorkflowType(typeof(NewWorkflow));
            AddWorkflowType(typeof(SuccessWorkflow));
            AddWorkflowType(typeof(TimeStampWorkflow));
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
                // Set to New state if empty!
                var state = batch["state"].Contains("state") ? (string)batch["state"]["state"] : typeof(NewWorkflow).Name;
                //if (batch["state"].Contains("state"))
                //    state = NewWorkflow.Name;
                
                if (!WorkflowTypes.ContainsKey(state))
                    continue; // Handle this as an error!!!

                var workflowType = WorkflowTypes[state];

                var wf = (WorkflowBatch)Activator.CreateInstance(workflowType);
                wf.CurrentBatch = batch;
                wf.Workflows = Workflows;

                Workflows.Push(wf);
            }
        }

        public void Execute()
        {
            while(Workflows.Count > 0) // Possiblility for parallel execution!?
            {
                var wf = Workflows.Pop(); 
                wf.Execute();
            }
        }

        public static WorkflowBatch CreateAndSetState(string name, JObject batch, Stack<WorkflowBatch> workflows)
        {
            if (!WorkflowTypes.ContainsKey(name))
                return null; // Handle this as an error!!!

            var workflowType = WorkflowTypes[name];

            var wf = (WorkflowBatch)Activator.CreateInstance(workflowType);
            wf.CurrentBatch = batch;
            wf.Workflows = workflows;
            wf.SetState();
            return wf;
        }
    }
}
