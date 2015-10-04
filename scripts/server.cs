using System;
using System.Net;
using System.Text;
using System.Threading;

namespace MatchingServer
{
    public class Server
    {
        private bool debug = false;

        public Server()
        {
        }

        public Server(bool debug)
        {
            this.debug = debug;
        }

        public void Start()
        {
            HttpListener listener = GetHttpListener();
            try
            {
                listener.Start();
            }
            catch (Exception e)
            {
                ErrorHandling(e);
            }
            StartListenLoop(listener);
        }

        HttpListener GetHttpListener()
        {
            var listener = new HttpListener();

            foreach (string prefix in Application.Config.Prefixes)
            {
                Logger.Debug("Server Binding URI: " + prefix);
                listener.Prefixes.Add(prefix);
            }
            return listener;
        }

        void StartListenLoop(HttpListener listener)
        {
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
            }
        }

        void HandleRequest(HttpListenerContext context)
        {
            Logger.Debug("Create Thread");
            AddResponseHeader(context);
            AddResponseOutput(context);
            context.Response.Close();
        }

        void AddResponseHeader(HttpListenerContext context)
        {
            context.Response.StatusCode = 200;
            context.Response.SendChunked = true;
        }

        void AddResponseOutput(HttpListenerContext context)
        {
            Matching matching;
            if (debug)
            {
                var param = context.Request.QueryString["name"];
                matching = new Matching((param == "") ? "debug" : param);
            }
            else
            {
                matching = new Matching(context.Request.UserHostAddress);
            }
            string result = matching.GetResponseValue();
            byte[] bytes = Encoding.GetEncoding(Application.Config.Encode).GetBytes(result);

            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        void ErrorHandling(Exception e)
        {
            Logger.Error("Raise Exception.");
            Logger.Error(e.ToString());
        }
    }
}
