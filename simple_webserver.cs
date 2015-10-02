using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ExampleSimpleWebserver
{
   class Program
   {
      static void Main (string[] args)
      {
        Console.WriteLine("Running sync server.");
        new SyncServer();
      }
   }

   public class SyncServer
   {
      public SyncServer()
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
