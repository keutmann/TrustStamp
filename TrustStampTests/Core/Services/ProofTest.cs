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
        public void TestMethod2()
        {
            // TODO: Add your test code here
            Assert.Pass("Your second passing test");
        }

    }
}

