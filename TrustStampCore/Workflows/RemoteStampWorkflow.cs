using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class RemoteStampWorkflow : WorkflowBatch
    {


        public override void Execute()
        {
            var remoteEndpoint = App.Config["remoteendpoint"].ToStringValue().Trim();
            if (!VerifyEndpoint(remoteEndpoint)) // No WIF key, then try to stamp remotely
            {
                WriteLog("Invalid remote endpoint"); // No comment!
                Push(new FailedWorkflow());
                return;
            }


            Update();
        }

        private bool VerifyEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return false;

            return true;
        }

    }
}
