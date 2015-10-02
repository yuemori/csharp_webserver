using System;
using System.Net;
using System.Text;

namespace Webserver
{
    public class Server
    {
        public Server()
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
            var bytes = Encoding.UTF8.GetBytes("waiting");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        void ErrorHandling(Exception e)
        {
            Console.WriteLine("Raise Exception.");
            Console.WriteLine(e.ToString());
        }
    }
}
