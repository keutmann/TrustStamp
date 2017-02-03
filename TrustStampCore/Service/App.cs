using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace TrustStampCore.Service
{
    public class App
    {
        public static JObject Config = new JObject();

        static App()
        {
            SetupConfig();
        }

        public static void EnableEventLogger()
        {
            Console.SetOut(new EventLoggerTextWriter(Console.Out));
            Console.SetError(new ErrorEventLoggerTextWriter(Console.Error));
        }

        public static void LoadConfigFile(string filename)
        {
            if (File.Exists(filename))
            {
                var text = File.ReadAllText(filename);
                Config.Merge(JObject.Parse(text));
            }
        }

        private static void SetupConfig()
        {
            Config["endpoint"] = IPAddress.Loopback.ToString();
            Config["port"] = 12700;
        }

    }
}
