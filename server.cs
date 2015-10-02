using System;
using System.Net;
using System.Text;

namespace Webserver
{
    public class Server
    {
        public Server()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8081/");
            listener.Prefixes.Add("http://127.0.0.1:8081/");

            listener.Start();

            while (true)
            {
                try
                {
                    var context = listener.GetContext(); //Block until a connection comes in
                    context.Response.StatusCode = 200;
                    context.Response.SendChunked = true;

                    var bytes = Encoding.UTF8.GetBytes("waiting");
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    context.Response.Close();
                }
                catch (Exception)
                {
                    // Client disconnected or some other error - ignored for this example
                }
            }
        }
    }
}
