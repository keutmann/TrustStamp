using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Topshelf;
using TrustStampCore.Extensions;
//using TrustStampCore.Repository;
//using TrustStampCore.Service;

namespace TrustStampServer
{
    public class Program
    {
        public static string dbfileName = "test.db";

        public static int Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            var test = Hex.ToBytes("AA"); // Just to make Api Controll able to find the BatchController in library assembly

            return (int)HostFactory.Run(x =>
            {
                x.Service<OwinService>(s =>
                {
                    s.ConstructUsing(() => new OwinService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
            });
        }



        //static void Main(string[] args)
        //{
        //    StarService();
        //    Console.WriteLine("Add data to db via http!");
        //    Console.WriteLine("Press c for stop!");
        //    ConsoleKeyInfo key = new ConsoleKeyInfo();
        //    while (key.KeyChar != 'c')
        //    {
        //        Task.Factory.StartNew(() => key = Console.ReadKey()).Wait(TimeSpan.FromMinutes(60.0));
        //    }

        //    using (var db = new TimeStampDatabase(dbfileName))
        //    {
        //        db.Open();
        //        var batch = new BatchTable(db.Connection);
        //        batch.CreateIfNotExist();

        //        var unprocessed = batch.GetUnprocessed();

        //        Console.WriteLine(unprocessed.ToString());
        //    }
        //    Console.WriteLine("Done!");
        //    Console.ReadKey();

        //}

        //public static void StarService()
        //{
        //    System.Net.IPEndPoint listenpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 10000);
        //    Console.WriteLine("Listening at: {0}", listenpoint);
        //    var listener = new HttpService(listenpoint);

        //    var batchService = new BatchService(dbfileName);

        //    listener.RequestReceived += new EventHandler<RequestParameters>(batchService.RequestReceived);

        //    listener.Start();

        //}
    }

    public class OwinService
    {
        private IDisposable _webApp;

        public void Start()
        {
            _webApp = WebApp.Start<StartOwin>("http://localhost:9000");
        }

        public void Stop()
        {
            _webApp.Dispose();
        }
    }

    public class StartOwin
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            appBuilder.UseWebApi(config);
        }
    }

    //public class HelloController : ApiController
    //{
    //    public IHttpActionResult Get()
    //    {
    //        return Ok("Hello, World! " + DateTime.Now.ToLocalTime());
    //    }
    //}
}
