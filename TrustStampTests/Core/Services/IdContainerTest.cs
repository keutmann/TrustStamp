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
    public class IdContainerTest : StampTest
    {
        [Test]
        public void TestAdd()
        {
            var id = ID.RandomHash().ToHex();
            var container = new ID(id);
            var hash = container.GetSafeHashFromHex();
            var resultId = hash.ToHex();
            Assert.AreEqual(id, resultId);
        }
    }
}

