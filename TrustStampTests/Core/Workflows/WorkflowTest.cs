using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            JObject batchItem = DBBatchTable.NewItem(Batch.GetCurrentPartition());

            var wf = new NewWorkflow();
            wf.Context = new WorkflowContext();
            wf.CurrentBatch = batchItem;

            Assert.IsTrue(wf.Initialize());

            wf.Execute();

            Console.WriteLine("Log: "+wf.CurrentBatch["log"]);

            Assert.AreEqual(1, wf.Context.Workflows.Count);
            Assert.AreEqual(typeof(MerkleWorkflow).Name, wf.Context.Workflows.Peek().Name);
        }

        [Test]
        public void TestMerkleWorkflow()
        {
            JObject batchItem = DBBatchTable.NewItem(Batch.GetCurrentPartition());

            var wf = new MerkleWorkflow();
            wf.Context = new WorkflowContext();
            wf.CurrentBatch = batchItem;

            Assert.IsTrue(wf.Initialize());

            //var partition = Batch.GetCurrentPartition();
            //JObject batchItem = null;
            //using (var db = TrustStampDatabase.Open())
            //{
            //    batchItem = db.BatchTable.AddDefault(partition);
            //}

            //var wf = WorkflowContext.CreateAndSetState(typeof(MerkleWorkflow).Name, batchItem, new Stack<WorkflowBatch>());
            wf.Execute();

            Console.WriteLine("Log: " + wf.CurrentBatch["log"]);

            Assert.AreEqual(1, wf.Context.Workflows.Count);
        }

        [Test]
        public void TestWorkflowContext()
        {
            JArray batchs = null;
            using (var db = TrustStampDatabase.Open())
            {
                DBProofTableTest.BuildUnprocessed(db.ProofTable, 1, DateTime.Now);
                var batchManager = new Batch(db);
                batchManager.EnsureNewBatchs();
                batchs = db.BatchTable.GetActive();
            }

            var engine = new WorkflowContext(batchs);
            engine.Execute();

            JObject batch = null;
            using (var manager = Batch.OpenWithDatabase())
            {
                batch = manager.DB.BatchTable.GetByPartition((string)batchs[0]["partition"]);
            }

            Assert.AreEqual(20, ((byte[])batch["root"]).Length);

            Console.WriteLine(batchs[0].ToString()); //.CustomRender());
            
            Assert.AreEqual(1, batchs.Count);
        }

        [Test]
        public void KeyValueTable()
        {
            var context = new WorkflowContext();

            context.KeyValueTable["test"] = "test";
            Assert.IsNotNull(context.KeyValueTable["test"]);
            Assert.IsFalse(context.KeyValueTable.ContainsKey("noentry"));

        }
    }
}

