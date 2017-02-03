using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class ErrorEventLoggerTextWriter : EventLoggerTextWriter
    {
        public ErrorEventLoggerTextWriter(TextWriter defaultOut) : base(defaultOut)
        {
        }

        public override void WriteLine(string value)
        {
            // Write event log error here!
            WriteEntry(value, EventLogEntryType.Error);

            // Write to Console 
            if (Environment.UserInteractive)
                DefaultOut.WriteLine(value);
        }




    }
}
