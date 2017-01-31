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
    public class ProofTest : StampTest
    {
        [Test]
        public void TestAdd()
        {
            var id = Crypto.GetRandomHash().ConvertToHex();
            var p = new Proof();
            var item = p.Add(id);
            var resultId = ((byte[])item["hash"]).ConvertToHex();
            Assert.AreEqual(id, resultId);
        }

        [Test]
        public void TestGetUnprocessed()
        {
            var proof = new Proof();
            int numOfPartitions = 9;
            // Build Test
            BuildUnprocessed(proof, numOfPartitions, DateTime.Now);

            // Read
            var partitions = proof.UnprocessedPartitions();

            // Test
            Assert.AreEqual(numOfPartitions, partitions.Count);
        }

        [Test]
        public void TestGetHash()
        {
            int count = 10;
            var proof = new Proof();

            // Build Test
            var list = new List<byte[]>();
            using (var db = TimeStampDatabase.Open())
            {
                for (int i = 0; i < count; i++)
                {
                    var id = Crypto.GetRandomHash();
                    list.Add(id);
                    var item = db.Proof.NewItem(id, null, Batch.GetCurrentPartition(), DateTime.Now);
                    db.Proof.Add(item);
                }

                using (TimeMe time = new TimeMe("Seek hash"))
                {
                    foreach (var id in list)
                    {
                        var item = db.Proof.GetByHash(id);
                    }
                }
            }
            Assert.IsTrue(true);
        }


        public static void BuildUnprocessed(Proof proof, int numOfPartitions, DateTime partitionDate)
        {
            for (int par = 1; par <= numOfPartitions; par++)
            {
                Batch.PartitionMethod = () => string.Format("{0}{1}00", partitionDate.ToString("yyyyMMddHH"), par);
                for (int i = 0; i < 10; i++)
                {
                    var id = Crypto.GetRandomHash().ConvertToHex();
                    var item = proof.Add(id);
                }
            }
        }
    }
}

