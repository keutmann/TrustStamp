using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class BatchTest : StampTest
    {
        //public override void Init()
        //{
        //    Console.WriteLine("Run init on BatchTest");

        //}

        //public override void Dispose()
        //{
        //    Console.WriteLine("Run Dispose on BatchTest");
        //}

        [Test]
        public void TestEnsure()
        {
            var partition = Batch.GetCurrentPartition();
            using (var db = TimeStampDatabase.Open())
            {
                var batchItem = db.Batch.Ensure(partition);
                var loadItem = db.Batch.GetByPartition(partition);
                Assert.AreEqual(partition, loadItem["partition"].ToString());
            }
        }

        //[Test]
        //public void TestProcessBuild()
        //{
        //    // Setup
        //    using (var proof = Proof.OpenWithDatabase())
        //    {
        //        int numOfPartitions = 9;
        //        var partitionDate = DateTime.Now;
        //        ProofTest.BuildUnprocessed(proof, numOfPartitions, partitionDate);

        //        // Move partition forward!
        //        Batch.PartitionMethod = () => string.Format("{0}000", partitionDate.AddDays(1).ToString("yyyyMMddHH"));

        //        var batch = new Batch(proof.DB);

        //        // Reference batchItem
        //        var batchItemToProcess = batch.GetNextBatchToProcess();
        //        var partition = batchItemToProcess["partition"].ToString();

        //        batch.Process();
        //        var db = batch.DB;

        //        var proofProcessed = db.Proof.GetByPartition(partition);

        //        batchItemToProcess = db.Batch.GetByPartition(partition);
        //        Console.WriteLine("Bitcoin tx: " + batchItemToProcess["tx"].ToString());

        //        foreach (var item in proofProcessed)
        //        {
        //            Assert.IsNotEmpty(item["path"] + "");
        //            Console.WriteLine("Proof path: " + item["path"]);
        //        }
        //    }
        //}

    }
}
