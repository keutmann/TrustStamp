using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class ProofTest : StampTest
    {
        [Test]
        public void TestAdd()
        {
            var id = ID.RandomHash().ToHex();
            var p = new Proof();
            var item = p.Add(id);
            var resultId = ((byte[])item["hash"]).ToHex();
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

        public static void BuildUnprocessed(Proof proof, int numOfPartitions, DateTime partitionDate)
        {
            for (int par = 1; par <= numOfPartitions; par++)
            {
                Batch.PartitionMethod = () => string.Format("{0}{1}00", partitionDate.ToString("yyyyMMddHH"), par);
                for (int i = 0; i < 10; i++)
                {
                    var id = ID.RandomHash().ToHex();
                    var item = proof.Add(id);
                }
            }
        }
    }
}

