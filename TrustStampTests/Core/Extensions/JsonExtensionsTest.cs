using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TrustStampCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampTests.Core.Extensions
{
    [TestFixture]
    public class JsonExtensionsTest
    {
        [Test]
        public void SetProperty()
        {
            var obj = new JObject();
            obj.SetProperty("test", true);
            Assert.IsTrue((bool)obj["test"] == true);

            JObject empty = null;
            empty.SetProperty("test", true);
            Assert.IsNull(empty);
        }

        [Test]
        public void EnsureObject()
        {
            JObject empty = null;
            empty = empty.EnsureObject("");
            Assert.IsNull(empty);


            var com = new JObject();
            com.EnsureObject("test");
            Assert.IsNotNull(com["test"]);
        }

        [Test]
        public void EnsureProperty()
        {
            var com = new JObject();
            com.EnsureProperty("test", true);
            Assert.IsTrue((bool)com["test"] == true);

            com.EnsureProperty("test", false);
            Assert.IsFalse((bool)com["test"] == false);

        }

        [Test]
        public void CustomRender()
        {
            var hello = Encoding.Unicode.GetBytes("Hello");
            var com = new JObject(new JProperty("test", hello));
            var result = com.CustomRender();

            Assert.IsTrue(result.Contains("test"));
            Assert.IsFalse(result.Contains("xmlns"));
        }

    }
}
