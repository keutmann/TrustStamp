using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using TrustStampCore.Repository;
using TrustStampCore.Workflows;
using Newtonsoft.Json.Linq;

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class WorkflowTest : StampTest
    {
        //public override void Init()
        //{
        //    WorkflowEngine.WorkflowTypes.Clear();
        //}

        //public override void Dispose()
        //{
        //    Console.WriteLine("Run Dispose on BatchTest");
        //}

        [Test]
        public void TestNewWorkflow()
        {
            var partition = Batch.GetCurrentPartition();
            JObject batchItem = null;
            using (var db = TimeStampDatabase.Open())
            {
                batchItem = db.BatchTable.AddDefault(partition);
            }


            var wf = WorkflowEngine.CreateAndSetState(NewWorkflow.Name, batchItem, new Stack<WorkflowBatch>());
            wf.Execute();

            Console.WriteLine("Log: "+wf.CurrentBatch["log"]);

            Assert.AreEqual(1, wf.Workflows.Count);
            Assert.AreEqual(MerkleWorkflow.Name, wf.Workflows.Peek().StateName);
        }

        [Test]
        public void TestMerkleWorkflow()
        {
            var partition = Batch.GetCurrentPartition();
            JObject batchItem = null;
            using (var db = TimeStampDatabase.Open())
            {
                batchItem = db.BatchTable.AddDefault(partition);
            }

            var wf = WorkflowEngine.CreateAndSetState(MerkleWorkflow.Name, batchItem, new Stack<WorkflowBatch>());
            wf.Execute();

            Console.WriteLine("Log: " + wf.CurrentBatch["log"]);

            Assert.AreEqual(1, wf.Workflows.Count);
            Assert.AreEqual(TimeStampWorkflow.Name, wf.Workflows.Peek().StateName);
        }

        [Test]
        public void TestWorkflowEngine()
        {
            var partition = Batch.GetCurrentPartition();
            JArray batchs = null;
            using (var db = TimeStampDatabase.Open())
            {
                DBProofTableTest.BuildUnprocessed(db.ProofTable, 1, DateTime.Now);
                var batchManager = new Batch(db);
                batchManager.EnsureNewBatchs();
                batchs = db.BatchTable.GetActive();
            }


            var engine = new WorkflowEngine(batchs);
            engine.Execute();

            Console.WriteLine(batchs[0]);
            //Console.WriteLine("Log: " + batchs[0]["log"]);
            //Console.WriteLine("Tx: " + batchs[0]["tx"]);

            Assert.AreEqual(1, batchs.Count);
        }
    }
}

