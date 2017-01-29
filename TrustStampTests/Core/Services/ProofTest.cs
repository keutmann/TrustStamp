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
        public override void Init()
        {
            Console.WriteLine("Run init on ProofTest");

        }

        public override void Dispose()
        {
            Console.WriteLine("Run Dispose on ProofTest");
        }

        [Test]
        public void TestAdd()
        {
            var id = ID.RandomSHA256Hex().ToLower(); // ToLower for making it a "not" safe id
            var p = new Proof();
            var item = p.Add(id);

            Assert.AreEqual(id.ToUpper(), item["hash"].ToString());
        }

        [Test]
        public void TestGetUnprocessed()
        {
            int numOfPartitions = 9;
            // Build Test
            var proof = new Proof();
            for (int par = 1; par <= numOfPartitions; par++)
            {
                Batch.PartitionMedthod = () => string.Format("{0}{1}00", DateTime.Now.ToString("yyyyMMddHH"), par);
                for (int i = 0; i < 10; i++)
                {
                    var id = ID.RandomSHA256Hex();
                    var item = proof.Add(id);
                }
            }

            // Read
            var partitions = proof.UnprocessedPartitions();

            // Test
            Assert.AreEqual(numOfPartitions, partitions.Count);
        }

    }
}

