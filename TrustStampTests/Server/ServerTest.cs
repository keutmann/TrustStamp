using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using TrustStampServer;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using TrustStampCore.Controllers;

namespace TrustStampTests.Core.Services
{
    [TestFixture]
    public class ServerTest : StampTest
    {
        public static TrustStampService service = null;

        public override void Init()
        {
            if (service == null)
            {
                var settings = new Settings(new NameValueCollection());
                service = new TrustStampService();
                service.Start(settings);
            }
        }

        public override void Dispose()
        {
            service.Stop();
        }



        //[Test]
        //public void ClientTest()
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        var id = Crypto.GetHash("Test").ConvertToHex();
        //        var url = "http://"+service.Config.EndPoint + ProofController.Path + id;

        //        // Add hash to database
        //        client.UploadString(url, id);

        //        // Get hash object from database
        //        var json = client.DownloadString(url);

        //        var obj = JObject.Parse(json);

        //        Assert.AreEqual(id, ((byte[])obj["hash"]).ConvertToHex());
        //    }
        //}

    }
}

