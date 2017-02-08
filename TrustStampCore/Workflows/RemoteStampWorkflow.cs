using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class RemoteStampWorkflow : WorkflowBatch
    {
        public override void Execute()
        {

            using (var db = TimeStampDatabase.Open())
            {
                var remoteEndpoint = App.Config["remoteEndpoint"].ToStringValue().Trim();
                if (!VerifyEndpoint(remoteEndpoint)) // No WIF key, then try to stamp remotely
                {
                    WriteLog("Invalid remoteEndpoint", db); // No comment!
                    Push(new FailedWorkflow());
                    return;
                }


                // Reg hash to remote
                //Push(new MerkleWorkflow());

                db.BatchTable.Update(CurrentBatch);
            }
        }

        private bool VerifyEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return false;

            return true;
        }

    }
}
