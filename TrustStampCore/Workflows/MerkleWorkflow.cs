using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampCore.Workflows
{
    public class MerkleWorkflow : WorkflowBatch
    {
        public override void Execute()
        {
            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Started", db);
                db.BatchTable.Update(CurrentBatch);

                // This may take some time.
                var proofCount = BuildMerkle(db);

                WriteLog(string.Format("Finished building {0} proofs.", proofCount), db);

                if (proofCount > 0)
                    Push(new BitcoinWorkflow());
                else
                    Push(new FailedWorkflow());

                db.BatchTable.Update(CurrentBatch);
            }
        }

        private int BuildMerkle(TimeStampDatabase db)
        {
            var proofs = db.ProofTable.GetByPartition(CurrentBatch["partition"].ToString());
            if (proofs.Count == 0)
                return 0;

            var leafNodes = (from p in proofs
                            select new Models.MerkleNode((JObject)p)).ToList();

            var merkleTree = new MerkleTree(leafNodes);
            var rootNode = merkleTree.Build();
            CurrentBatch["root"] = rootNode.Hash;

            // Update the path back to proof entities
            foreach (var node in leafNodes)
                db.ProofTable.UpdatePath(node.Hash, node.Path);

            return proofs.Count;
        }

    }
}
