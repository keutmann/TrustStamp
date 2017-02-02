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

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class DBProofTableTest : StampTest
    {
        [Test]
        public void TestNewItem()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                var item = db.ProofTable.NewItem(id, new byte[0], partition, timeStamp);
                Assert.AreEqual(id, (byte[])item["hash"]);
                Assert.AreEqual(new byte[0], (byte[])item["path"]);
                Assert.AreEqual(partition, (string)item["partition"]);
                Assert.AreEqual(timeStamp, (DateTime)item["timestamp"]);
            }
        }


        [Test]
        public void TestAdd()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                var item = db.ProofTable.NewItem(id, null, partition, timeStamp);
                Assert.AreEqual(1, db.ProofTable.Add(item));
            }
        }

        [Test]
        public void TestGetByHash()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                var item = db.ProofTable.NewItem(id, null, partition, timeStamp);
                Assert.AreEqual(1, db.ProofTable.Add(item));

                var load = db.ProofTable.GetByHash(id);
                Assert.AreEqual(id, (byte[])load["hash"]);
            }
        }

        [Test]
        public void TestGetByPartition()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                var item = db.ProofTable.NewItem(id, null, partition, timeStamp);
                Assert.AreEqual(1, db.ProofTable.Add(item));

                var load = db.ProofTable.GetByPartition(partition);
                Assert.AreEqual(id, (byte[])load[0]["hash"]);
            }
        }
        

        [Test]
        public void TestUpdatePath()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                var item = db.ProofTable.NewItem(id, null, partition, timeStamp);
                Assert.AreEqual(1, db.ProofTable.Add(item));

                var path = Crypto.GetRandomHash();
                Assert.AreEqual(1, db.ProofTable.UpdatePath(id, path));

                var load = db.ProofTable.GetByHash(id);

                Assert.AreEqual(path, (byte[])load["path"]);
            }
        }

        [Test]
        public void TestGetUnprocessedPartitions()
        {
            var id = Crypto.GetRandomHash();
            var partition = Batch.GetCurrentPartition();
            var timeStamp = DateTime.Now;
            using (var db = TimeStampDatabase.Open())
            {
                int numOfPartitions = 9;
                var partitionDate = DateTime.Now;
                BuildUnprocessed(db.ProofTable, numOfPartitions, partitionDate);

                var partitions = db.ProofTable.GetUnprocessedPartitions(Batch.GetPartition(partitionDate.AddDays(1)));

                Assert.AreEqual(numOfPartitions-1, partitions.Count);
            }
        }


        public static void BuildUnprocessed(DBProofTable proof, int numOfPartitions, DateTime partitionDate)
        {
            for (int par = 1; par <= numOfPartitions; par++)
            {
                for (int i = 0; i < 10; i++)
                {
                    var id = Crypto.GetRandomHash();
                    var item = proof.NewItem(id, null, Batch.GetPartition(partitionDate.AddDays(par)), DateTime.Now);
                    proof.Add(item);
                }
            }
        }
    }
}

