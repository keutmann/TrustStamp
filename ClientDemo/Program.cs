using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;
using Newtonsoft.Json.Linq;

namespace ClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine(DateTime.Now.ToString("yyyyMMddhh0000"));
            //Console.WriteLine(DateTime.Now.ToString("yyyyMMddhhm000"));
            //Console.WriteLine(DateTime.Now.ToString("yyyyMMddhhmm00"));
            //Console.ReadKey();

            //return;
            var endpoint = "http://127.0.0.1:12700";
            List<string> proofIds = new List<string>();
            int count = 5;
            using (WebClient client = new WebClient())
            {
                for (var i = 0; i < count; i++)
                {
                    var id = Crypto.GetRandomHash().ConvertToHex();
                    proofIds.Add(id);
                    var url = endpoint +"/api/proof/" + id;
                    Console.WriteLine("Adding proof: " + id);
                    client.UploadString(url, id);
                }
                Console.WriteLine("Added " + count + " proofs!");


                Console.WriteLine("Waiting for batch to be build!");
                while (true)
                {
                    var id = proofIds[0];
                    var url = endpoint + "/api/proof/" + id;
                    var data = client.DownloadString(url);
                    var proof = JObject.Parse(data);
                    if(!String.IsNullOrEmpty((string)proof["path"]))
                    {
                        Console.WriteLine("Batch done!");
                        Console.WriteLine(proof);
                        break;
                    }
                    Console.Write(".");
                    System.Threading.Thread.Sleep(1000);
                }
            }



            Console.WriteLine("Done - Press a key!");
            Console.ReadKey();
        }
    }
}
