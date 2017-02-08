using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampTests.Core.Repository
{
    [TestFixture]
    public class DBBatchTableTest : StampTest
    {
        [Test]
        public void TestAddDefault()
        {
            var partition = Batch.GetCurrentPartition();
            using (var db = TrustStampDatabase.Open())
            {
                var item = db.BatchTable.AddDefault(partition);
                Assert.IsNotNull(item);

                var count = db.BatchTable.Count();

                Assert.AreEqual(1, count);
            }
        }

        [Test]
        public void TestGetByPartition()
        {
            var partition = Batch.GetCurrentPartition();
            using (var db = TrustStampDatabase.Open())
            {
                db.BatchTable.AddDefault(partition);
                var item = db.BatchTable.GetByPartition(partition);
                Assert.AreEqual(0, ((byte[])item["root"]).Length);

                Assert.AreEqual(partition, (string)item["partition"]);
            }
        }

        [Test]
        public void TestUpdate()
        {
            var partition = Batch.GetCurrentPartition();
            using (var db = TrustStampDatabase.Open())
            {
                db.BatchTable.AddDefault(partition);
                var item = db.BatchTable.GetByPartition(partition);
                Assert.AreEqual(1, (int)item["active"]);

                item["root"] = Crypto.GetRandomHash();
                Assert.AreEqual(20, ((byte[])item["root"]).Length);

                item["active"] = 0;
                Assert.AreEqual(1, db.BatchTable.Update(item)); 

                var updatedItem = db.BatchTable.GetByPartition(partition);

                Assert.AreEqual(20, ((byte[])updatedItem["root"]).Length);
                Assert.AreEqual(0, (int)updatedItem["active"]);
            }
        }


        [Test]
        public void TestEnsure()
        {
            var partition = Batch.GetCurrentPartition();
            using (var db = TrustStampDatabase.Open())
            {
                var batch = db.BatchTable.Ensure(partition);
                var item = db.BatchTable.GetByPartition(partition);

                Assert.AreEqual(partition, (string)item["partition"]);
                Assert.AreEqual("", item["state"]);
                Assert.AreEqual(1, (int)item["active"]);
            }
        }

        [Test]
        public void TestGetActive()
        {
            
            using (var db = TrustStampDatabase.Open())
            {
                var inactiveBatch = db.BatchTable.Ensure(Batch.GetPartition(DateTime.Now));
                inactiveBatch["active"] = 0;
                db.BatchTable.Update(inactiveBatch);

                db.BatchTable.Ensure(Batch.GetPartition(DateTime.Now.AddDays(1)));
                db.BatchTable.Ensure(Batch.GetPartition(DateTime.Now.AddDays(2)));
                db.BatchTable.Ensure(Batch.GetPartition(DateTime.Now.AddDays(3)));

                var active = db.BatchTable.GetActive();

                Assert.AreEqual(3, active.Count);
                Assert.AreEqual(Batch.GetPartition(DateTime.Now.AddDays(3)), (string)active[0]["partition"]);
                Assert.AreEqual(Batch.GetPartition(DateTime.Now.AddDays(2)), (string)active[1]["partition"]);
                Assert.AreEqual(Batch.GetPartition(DateTime.Now.AddDays(1)), (string)active[2]["partition"]);
            }
        }

    }
}
