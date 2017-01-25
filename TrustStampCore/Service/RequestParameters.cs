using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampCore.Service
{
    public class RequestParameters : EventArgs
    {
        public HttpListenerContext Context { get; }
        //public IPAddress RemoteAddress { get; }
        //public NameValueCollection Parameters { get; }

        ///public virtual bool IsValid { get; }


        public RequestParameters(HttpListenerContext context)
        {
            Context = context;
            //Parameters = parameters;
            //RemoteAddress = address;
        }
    }

}
