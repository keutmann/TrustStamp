using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class TimeMe : IDisposable
    {
        public Stopwatch Watch { get; set; }
        public string Message { get; set; }

        public TimeMe(string message)
        {
            Message = message;
            Watch = new Stopwatch();
            Watch.Start();
        }

        public void Dispose()
        {
            Watch.Stop();
            Print();
        }

        public virtual void Print()
        {
            Console.WriteLine(Message+ " - Elapsed milliseconds: " + Watch.ElapsedMilliseconds + " ");
        }
    }
}
