using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Service;
using TrustStampCore.Extensions;


namespace ClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 5;
            using (WebClient client = new WebClient())
            {
                for (var i = 0; i < count; i++)
                {
                    var id = Crypto.GetRandomHash().ConvertToHex();
                    var url = "http://127.0.0.1:12700/api/proof/" + id;
                    Console.WriteLine("Adding proof: " + id);
                    client.UploadString(url, id);
                }
                Console.WriteLine("Added " + count + " proofs!");
            }

            Console.WriteLine("Done - Press a key!");
            Console.ReadKey();
        }
    }
}
