using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampCore.Workflows
{
    public class MerkleWorkflow : WorkflowBatch
    {
        public const string Name = "Merkle build";
        public override string StateName
        {
            get
            {
                return Name;
            }
        }

        static MerkleWorkflow()
        {
            WorkflowEngine.WorkflowTypes.Add(Name, typeof(MerkleWorkflow));
        }

        public override void Execute()
        {
            using (var db = TimeStampDatabase.Open())
            {
                WriteLog("Started", db);
                var proofCount = BuildMerkle(db);
                WriteLog(string.Format("Finished building {0} proofs", proofCount), db);

                Push(TimeStampWorkflow.Name);

                db.BatchTable.Update(CurrentBatch);
            }
        }

        private int BuildMerkle(TimeStampDatabase db)
        {
            var proofs = db.ProofTable.GetByPartition(CurrentBatch["partition"].ToString());

            var leafNodes = from p in proofs
                            select new Models.MerkleNode((JObject)p);

            var merkleTree = new MerkleTree(leafNodes);
            var rootNode = merkleTree.Build();

            // Update the path back to proof entities
            foreach (var node in merkleTree.LeafNodes)
                db.ProofTable.UpdatePath(node.Hash, node.Path);

            return proofs.Count;
        }

    }
}
