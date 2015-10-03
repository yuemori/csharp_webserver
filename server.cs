using System;
using System.Net;
using System.Text;

namespace MatchingServer
{
    public class Server
    {
        private bool debug = false;

        public Server()
        {
            StartServer();
        }

        public Server(bool debug)
        {
            this.debug = debug;
            StartServer();
        }

        void StartServer()
        {
            HttpListener listener = GetHttpListener();
            try
            {
                listener.Start();
            } catch (Exception e)
            {
                ErrorHandling(e);
            }
            StartListenLoop(listener);
        }

        HttpListener GetHttpListener()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8081/");
            listener.Prefixes.Add("http://127.0.0.1:8081/");
            return listener;
        }

        void StartListenLoop(HttpListener listener)
        {
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                AddResponseHeader(context);
                AddResponseOutput(context);
                context.Response.Close();
            }
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
            } else {
                matching = new Matching(context.Request.UserHostAddress);
            }
            string result = matching.GetResponseValue();
            byte[] bytes = Encoding.UTF8.GetBytes(result);

            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        void ErrorHandling(Exception e)
        {
            Logger.Error("Raise Exception.");
            Logger.Error(e.ToString());
        }
    }
}
