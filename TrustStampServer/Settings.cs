using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrustStampServer
{
    public class Settings
    {
        public int Port
        {
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
}
