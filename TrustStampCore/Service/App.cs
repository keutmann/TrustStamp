﻿using Newtonsoft.Json.Linq;
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

        public static void SaveConfigFile(string filename)
        {
            var json = App.Config.ToString();
            File.WriteAllText(filename, json);
        }


        private static void SetupConfig()
        {
            Config["endpoint"] = IPAddress.Loopback.ToString();
            Config["port"] = 12700;
            Config["eventlog"] = !Environment.UserInteractive; // Activate event logger if no console is active.
            Config["test"] = false; // General test, no real data is stored, run in memory database!
            Config["partition"] = "yyyyMMddhhmm00"; // Create a new batch every hour.
            Config["processinterval"] = 1000 * 30; // 30 sec

            // Bitcoin
            Config["network"] = "TestNet";  // Bitcoin Private key in wif format
            Config["btcwif"] = "cMcGZkth7ufvQC59NSTSCTpepSxXbig9JfhCYJtn9RppU4DXx4cy"; // Test net key
            Config["btcfee"] = "0.0001";
            Config["remoteendpoint"] = ""; // Connect to a remote TrustStamp server
            Config["remoteport"] = 12700; // Connect to a remote TrustStamp server port


            Config["dbconnectionstring"] = "";  // Connection or dbfilename
            Config["dbfilename"] = "TrustStamp.db";
            Config["database"] = new JObject();
            Config["database"]["pooling"] = true;
            Config["database"]["cache"] = "shared";
            Config["database"]["syncmode"] = 0;
            Config["database"]["journalmode"] = -1;

        }

    }
}
