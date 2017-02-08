using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using System;

namespace TrustStampCore.Workflows
{
    public class BlockchainWorkflowBatch : WorkflowBatch
    {

        public IBlockchainRepository BlockchainRepository { get; set; }
        public byte[] Root { get; set; }

        public bool EnsureRepository(TrustStampDatabase db)
        {
            var blockchainRepositoryName = App.Config["blockchainprovider"].ToStringValue("blockr");
            var BlockchainRepository = BlockchainFactory.GetRepository(blockchainRepositoryName, BlockchainFactory.GetBitcoinNetwork());
            if (BlockchainRepository == null)
            {
                WriteLog("No blockchain provider found", db); // No comment!
                return false;
            }
            return true;
        }

        public bool EnsureRoot(TrustStampDatabase db)
        {
            var hash = (byte[])CurrentBatch["root"];
            if (hash.Length == 0)
            {
                WriteLog("No root to timestamp!", db);
                return false;
            }
            return true;
        }

        public bool EnsureDependencies(TrustStampDatabase db)
        {
            if (!EnsureRoot(db))
                return false;

            if (!EnsureRepository(db))
                return false;

            return true;
        }


        public void PushRetry(int retries, int sleepMinutes)
        {
            CurrentBatch["state"]["retry"] = CurrentBatch["state"]["retry"].ToInteger() + 1;

            if (CurrentBatch["state"]["retry"].ToInteger() == retries)
                Push(new FailedWorkflow("Failed "+ retries +" times."));
            else
                Push(new SleepWorkflow(DateTime.Now.AddMinutes(sleepMinutes), CurrentBatch)); // Sleep to 2 hours and retry this workflow
        }

    }
}
