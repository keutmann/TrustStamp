using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using System;

namespace TrustStampCore.Workflows
{
    public class RemotePayWorkflow : BlockchainWorkflowBatch
    {
        public override void Execute()
        {

            using (var db = TrustStampDatabase.Open())
            {
                if (!EnsureDependencies(db))
                    return;

                WriteLog("Waiting for payment on Batch root", db);


                var rootKey = BitcoinManager.GetKey(Root);
                var rootAddress = rootKey.PubKey.GetAddress(BlockchainFactory.GetBitcoinNetwork());
                var info = BlockchainRepository.GetAddressInfo(rootAddress.ToWif());


                if(info != null && info["totalreceived"] != null || info["totalreceived"].ToInteger() > 0)
                {
                    // payment has been made!
                    WriteLog("Payment has been made on Batch root", db);
                    Push(new SuccessWorkflow());
                    return;
                }

                // Wait some time to see if someone pays for the Batch root!
                PushRetry(3, 180); // 3 retries x 3 hours 

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
