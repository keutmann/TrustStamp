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
        private volatile bool process = true;

        public void Start()
        {
            var url = "http://" + App.Config["endpoint"] + ":" + App.Config["port"]+ "/";
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
                    if (process) // Run the job
                        method();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
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
}
