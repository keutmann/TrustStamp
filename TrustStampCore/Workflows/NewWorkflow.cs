using TrustStampCore.Repository;

namespace TrustStampCore.Workflows
{
    public class NewWorkflow : WorkflowBatch
    {
        public override void Execute()
        {
            WriteLog("Workflow started");

            Push(new MerkleWorkflow());
            Update();
        }

    }
}
