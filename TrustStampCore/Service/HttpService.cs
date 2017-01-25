using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace TrustStampCore.Service
{
    public class HttpService 
    {
        private string prefix;
        private System.Net.HttpListener listener;

        public event EventHandler<RequestParameters> RequestReceived;


        /// <summary>
        /// True if the listener is waiting for incoming connections
        /// </summary>
        public bool Running
        {
            get { return listener != null; }
        }

        public HttpService(IPAddress address, int port)
            : this(string.Format("http://{0}:{1}/", address, port))
        {

        }

        public HttpService(IPEndPoint endpoint)
            : this(endpoint.Address, endpoint.Port)
        {

        }

        public HttpService(string httpPrefix)
        {
            if (string.IsNullOrEmpty(httpPrefix))
                throw new ArgumentNullException("httpPrefix");

            prefix = httpPrefix;
        }


        /// <summary>
        /// Starts listening for incoming connections
        /// </summary>
        public void Start()
        {
            if (Running)
                return;

            listener = new System.Net.HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            listener.BeginGetContext(EndGetRequest, listener);
        }

        /// <summary>
        /// Stops listening for incoming connections
        /// </summary>
        public void Stop()
        {
            if (!Running)
                return;

            IDisposable d = (IDisposable)listener;
            listener = null;
            d.Dispose();
        }

        private void EndGetRequest(IAsyncResult result)
        {
            HttpListenerContext context = null;
            System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;

            try
            {
                context = listener.EndGetContext(result);
                using (context.Response)
                    HandleRequest(context);
            }
            catch (Exception ex)
            {
                Console.Write("Exception in listener: {0}{1}", Environment.NewLine, ex);
            }
            finally
            {
                try
                {
                    if (listener.IsListening)
                        listener.BeginGetContext(EndGetRequest, listener);
                }
                catch
                {
                    Stop();
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.StatusCode = 200;

            var parameters = new RequestParameters(context);

            if (RequestReceived != null)
                RequestReceived.Invoke(this, parameters);

            //byte[] response = Encoding.UTF8.GetBytes("Hello world!"); //responseData.Encode();
            //context.Response.ContentLength64 = response.LongLength;
            //context.Response.OutputStream.Write(response, 0, response.Length);
        }

        private NameValueCollection ParseQuery(string url)
        {
            // The '?' symbol will be there if we received the entire URL as opposed to
            // just the query string - we accept both therfore trim out the excess if we have the entire URL
            if (url.IndexOf('?') != -1)
                url = url.Substring(url.IndexOf('?') + 1);

            string[] parts = url.Split('&', '=');
            NameValueCollection c = new NameValueCollection(1 + parts.Length / 2);
            for (int i = 0; i < parts.Length; i += 2)
                if (parts.Length > i + 1)
                    c.Add(parts[i], parts[i + 1]);

            return c;
        }

    }
}
