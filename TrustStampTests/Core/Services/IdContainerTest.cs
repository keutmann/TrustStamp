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
        public void TestHexParse()
        {
            var id = Crypto.GetRandomHash().ConvertToHex();
            var container = IDContainer.Parse(id);
            Assert.AreEqual(id, container.Hash.ConvertToHex());
        }

        [Test]
        public void TestBase64Parse()
        {
            var id = Convert.ToBase64String(Crypto.GetRandomHash());
            var container = IDContainer.Parse(id);
            Assert.AreEqual(id, Convert.ToBase64String(container.Hash));
        }

    }
}

