using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TrustStampCore.Extensions;
using TrustStampCore.Repository;
using TrustStampCore.Service;

namespace TrustStampServer
{
    public class TrustStampService
    {
        private IDisposable _webApp;
        private Timer timer;
        private int timeInMs = 1000*60; // 1 minute
        public Settings Config { get; set; }

        public void Start(Settings settings)
        {
            var test = EncoderExtensions.ConvertFromHex("AA"); // Just to make Api Controller able to find the BatchController in library assembly, temporary solution

            Config = settings;
            var url = "http://" + settings.EndPoint.ToString();
            _webApp = WebApp.Start<StartOwin>(url);

            using (var db = TimeStampDatabase.Open())
            {
                db.CreateIfNotExist();
            }

            RunTimer(ProcessTimeStamps);
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

        private void RunTimer(Action method)
        {
            method(); // Start now!

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
            Console.WriteLine(DateTime.Now.ToLocalTime() + " : Processing...");
            using (var manager = Batch.OpenWithDatabase())
            {
                manager.Process();
            }
        }

        public void Pause()
        {
            //process = false;
        }

        public void Continue()
        {
            //process = true;
        }

        public void Stop()
        {
            _webApp.Dispose();
        }
    }
}
