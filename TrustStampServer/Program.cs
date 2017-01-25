using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampServer
{
    class Program
    {
        public static string dbfileName = "test.db";
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            StarService();
            Console.WriteLine("Add data to db via http!");
            Console.WriteLine("Press c for stop!");
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            while (key.KeyChar != 'c')
            {
                Task.Factory.StartNew(() => key = Console.ReadKey()).Wait(TimeSpan.FromMinutes(60.0));
            }

            using (var db = new TimeStampDatabase(dbfileName))
            {
                db.Open();
                var batch = new BatchTable(db.Connection);
                batch.CreateIfNotExist();

                var unprocessed = batch.GetUnprocessed();

                Console.WriteLine(unprocessed.ToString());
            }
            Console.WriteLine("Done!");
            Console.ReadKey();

        }

        public static void StarService()
        {
            System.Net.IPEndPoint listenpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 10000);
            Console.WriteLine("Listening at: {0}", listenpoint);
            var listener = new HttpService(listenpoint);

            var batchService = new BatchService(dbfileName);

            listener.RequestReceived += new EventHandler<RequestParameters>(batchService.RequestReceived);

            listener.Start();

        }
    }
}
