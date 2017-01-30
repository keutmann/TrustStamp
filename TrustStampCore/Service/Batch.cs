using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Extensions;
using TrustStampCore.Models;
using NBitcoin;

namespace TrustStampCore.Service
{
    public class Batch
    {
        //public string _PartitionKey = null;
        //public string PartitionKey
        //{
        //    get
        //    {
        //        return _PartitionKey ?? string.Format("{0}0000", DateTime.Now.ToString("yyyyMMddHH"));
        //    }
        //    set
        //    {
        //        _PartitionKey = value;
        //    }
        //}


        public static Func<string> PartitionMethod = DefaultPartition;

        public static string DefaultPartition()
        {
            return string.Format("{0}0000", DateTime.Now.ToString("yyyyMMddHH"));
        } 

        public static string GetCurrentPartition()
        {
            return PartitionMethod();
        }



        public JObject Get(string partition)
        {
            using (var db = TimeStampDatabase.Open())
            {
                return db.Batch.GetByPartition(partition);
            }
        }

        public JObject GetNextBatchToProcess(TimeStampDatabase db)
        {
            var currentPartition = GetCurrentPartition();// current partition snapshot
            var partitions = db.Proof.GetUnprocessed(); // partitions are ordered!
            var partitionItem = partitions.FirstOrDefault(p => currentPartition.CompareTo(p["partition"].ToString()) > 0);// we what a partion before current partition;
            if (partitionItem == null)
                return null; // No partitions to be processed

            var partition = partitionItem["partition"].ToString();

            var batchItem = db.Batch.Ensure(partition); // Ensure that a batch is created in the db if missing

            return batchItem;
        }

        public bool Process(TimeStampDatabase db)
        {
            var batchItem = GetNextBatchToProcess(db);
            if (batchItem == null)
                return false;

            if (batchItem["state"].ToString().Equals(BatchState.New))
                BuildMerkle(batchItem, db);

            if (batchItem["state"].ToString().Equals(BatchState.BuildMerkleDone))
                TimeStampBatch(batchItem, db);

            return true;
        }

        private void TimeStampBatch(JObject batchItem, TimeStampDatabase db)
        {
            batchItem["state"] = BatchState.Timestamping;
            db.Batch.Update(batchItem);

            Transaction previousTx = null;
            var btc = new BitcoinManager();

            var hash = batchItem["root"].ToString().ToBytes();
            var result = btc.Send(hash, previousTx);
            if (result.status == "success")
            {
                // Not working
                //previousTx = result.Tx; // Save the current tx for later use in the next spent (support for unconfirmed spending!)
                var tx = (JArray)batchItem["tx"];
                tx.Add(new JObject(
                    new JProperty("protocol", "bitcoin"),
                    new JProperty("transaction", result.data)
                    ));

                batchItem["state"] = BatchState.TimeStampDone;
                db.Batch.Update(batchItem);
            }
            else
            {
                batchItem["state"] = BatchState.Failed;
                db.Batch.Update(batchItem);
            }
        }

        private void BuildMerkle(JObject batchItem, TimeStampDatabase db)
        {
            batchItem["state"] = BatchState.BuildMerkle;
            db.Batch.Update(batchItem);

            var proofs = db.Proof.GetByPartition(batchItem["partition"].ToString());

            var leafNodes = new List<TreeEntity>();
            // Build
            foreach (JObject proof in proofs)
            {
                Console.WriteLine("Hash: Partition {0} - HASH {1}", proof["partition"], proof["hash"]);
                leafNodes.Add(new TreeEntity(proof));
            }

            var merkleTree = new MerkleTree();
            var rootNode = merkleTree.BuildTree(leafNodes);
            merkleTree.ComputeMerkleTree(rootNode);

            // Update the path back to proof entities
            foreach (var node in leafNodes)
            {
                node.Entity["path"] = node.MerkleTree.ToHex();
                db.Proof.UpdatePath(node.Entity);
            }

            batchItem["root"] = rootNode.Hash.ToHex();
            batchItem["state"] = BatchState.BuildMerkleDone;

            db.Batch.Update(batchItem);

            Console.WriteLine(String.Format("Root: {0}", batchItem["root"]));
        }
    }
}
