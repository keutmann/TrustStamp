using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class EventLoggerTextWriter : TextWriter
    {
        // Note: The actual limit is higher than this, but different Microsoft operating systems actually have
        //       different limits. So just use 30,000 to be safe.
        private const int MaxEventLogEntryLength = 30000;

        public TextWriter DefaultOut { get; set; }
        public string Source { get; }
        private bool eventLoggerActive = true;

        public EventLoggerTextWriter(TextWriter defaultOut)
        {
            DefaultOut = defaultOut;
            Source = GetSource();
            try
            {
                if (!EventLog.SourceExists(Source))
                    EventLog.CreateEventSource(Source, "Application");
            }
            catch
            {
                eventLoggerActive = false;
            }
        }

        public override Encoding Encoding
        {
            get
            {
                return this.Encoding;
            }
        }

        public override void WriteLine(string value)
        {
            // Write event log here!
            WriteEntry(value, EventLogEntryType.Information);

            if (Environment.UserInteractive)
                DefaultOut.WriteLine(value);
        }

        protected void WriteEntry(string message, EventLogEntryType entryType)
        {
            if(eventLoggerActive)
                EventLog.WriteEntry(Source, EnsureLogMessageLimit(message), entryType);
        }


        private string GetSource()
        {
            // If the caller has explicitly set a source value, just use it.
            if (!string.IsNullOrWhiteSpace(Source)) { return Source; }

            try
            {
                var assembly = Assembly.GetEntryAssembly();

                // GetEntryAssembly() can return null when called in the context of a unit test project.
                // That can also happen when called from an app hosted in IIS, or even a windows service.

                if (assembly == null)
                {
                    assembly = Assembly.GetExecutingAssembly();
                }


                if (assembly == null)
                {
                    // From http://stackoverflow.com/a/14165787/279516:
                    assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                }

                if (assembly == null) { return "Unknown"; }

                return assembly.GetName().Name;
            }
            catch
            {
                return "Unknown";
            }
        }

        // Ensures that the log message entry text length does not exceed the event log viewer maximum length of 32766 characters.
        private string EnsureLogMessageLimit(string logMessage)
        {
            if (logMessage.Length > MaxEventLogEntryLength)
            {
                string truncateWarningText = string.Format(CultureInfo.CurrentCulture, "... | Log Message Truncated [ Limit: {0} ]", MaxEventLogEntryLength);

                // Set the message to the max minus enough room to add the truncate warning.
                logMessage = logMessage.Substring(0, MaxEventLogEntryLength - truncateWarningText.Length);

                logMessage = string.Format(CultureInfo.CurrentCulture, "{0}{1}", logMessage, truncateWarningText);
            }

            return logMessage;
        }
    }
}
