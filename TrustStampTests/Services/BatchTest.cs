using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;

namespace TrustStampTests.Services
{
    [TestFixture]
    public class BatchTest : StampTest
    {
        public override void Init()
        {
            Console.WriteLine("Run init on BatchTest");

        }

        public override void Dispose()
        {
            Console.WriteLine("Run Dispose on BatchTest");
        }

        [Test]
        public void AddTest()
        {

        }

        [Test]
        public void TestMethod2()
        {
            // TODO: Add your test code here
            Assert.Pass("Your second passing test");
        }

    }
}
