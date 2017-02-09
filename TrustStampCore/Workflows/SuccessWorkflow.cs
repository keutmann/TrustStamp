using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{
    public class SuccessWorkflow : WorkflowBatch
    {
        public override void Execute()
        {
            WriteLog("The workflow has finished");

            CurrentBatch["active"] = 0;
            Update();
        }
    }
}
