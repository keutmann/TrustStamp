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
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Topshelf;
using TrustStampCore.Extensions;
using System.Collections.Specialized;
using System.Net;

namespace TrustStampServer
{

    public class Program
    {
        public static string dbfileName = "test.db";

        public static int Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            var settings = new Settings(ConfigurationManager.AppSettings);

            return (int)HostFactory.Run(configurator =>
            {
                configurator.AddCommandLineDefinition("port", f => { settings.NameValue["port"] = f; });
                configurator.ApplyCommandLine();

                configurator.Service<TrustStampService>(s =>
                {
                    s.ConstructUsing(() => new TrustStampService());
                    s.WhenStarted(service => service.Start(settings));
                    s.WhenPaused(service => service.Pause());
                    s.WhenContinued(service => service.Continue());
                    s.WhenStopped(service => service.Stop());
                });
            });

        }
    }

    public class Settings 
    {
        public int Port {
            get
            {
                int result;
                if (int.TryParse(NameValue["port"], out result))
                    return result;
                return 9000;
            }
            set
            {
                NameValue["port"] = value.ToString();
            }
        }

        public IPEndPoint EndPoint;

        public NameValueCollection NameValue;

        public Settings(NameValueCollection settings)
        {
            NameValue = settings;
            EndPoint = new IPEndPoint(IPAddress.Loopback, 9000);
        }
    }

    public class TrustStampService
    {
        private IDisposable _webApp;
        private bool process = true;
        private Timer timer;
        private int timeInMs = 1000;
        public Settings Config { get; set; }

        public void Start(Settings settings)
        {
            var test = EncoderExtensions.ConvertFromHex("AA"); // Just to make Api Controller able to find the BatchController in library assembly, temporary solution

            Config = settings;
            var url = "http://"+settings.EndPoint.ToString();
            _webApp = WebApp.Start<StartOwin>(url);
            RunTimer(ProcessTimeStamps);
        }

        private void RunTimer(Action method)
        {


            timer = new Timer((o) =>
            {
                try
                {
                    method();
                }
                catch (Exception)
                {
                    // handle
                }
                finally
                {
                    // only set the initial time, do not set the recurring time
                    timer.Change(timeInMs, Timeout.Infinite);
                }
            });

            // only set the initial time, do not set the recurring time
            timer.Change(timeInMs, Timeout.Infinite);
        }

        public void ProcessTimeStamps()
        {
            Console.WriteLine("Processing: " + DateTime.Now.ToLocalTime());
            Thread.Sleep(2000);
            Console.WriteLine("Done working!");
        }

        public void Pause()
        {
            process = false;
        }

        public void Continue()
        {
            process = true;
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
