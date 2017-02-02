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
using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;

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
}
