using System;
using System.Text;
using System.IO;

namespace MatchingServer
{
    class Logger
    {
        public static void Debug(string message)
        {
            Print("Debug", message);
        }

        public static void Debug(string message, string ipAddress)
        {
            Print("Debug", message, ipAddress);
        }

        public static void Info(string message)
        {
            Print("Info", message);
        }

        public static void Info(string message, string ipAddress)
        {
            Print("Info", message, ipAddress);
        }

        public static void Warning(string message)
        {
            Print("Warning", message);
        }

        public static void Warning(string message, string ipAddress)
        {
            Print("Warning", message);
        }

        public static void Error(string message)
        {
            Print("Error", message);
        }

        public static void Error(string message, string ipAddress)
        {
            Print("Error", message);
        }

        static void Print(string status, string message, string ipAddress)
        {
            var decorateMessage = Decorate(status, message, ipAddress);
            Console.WriteLine(decorateMessage);
            FileWrite(decorateMessage);
        }

        static void Print(string status, string message)
        {
            var decorateMessage = Decorate(status, message, "-");
            Console.WriteLine(decorateMessage);
            FileWrite(decorateMessage);
        }

        static void FileWrite(string message)
        {
            using (FileStream stream = CreateFileStream())
            {
              using (StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding(Application.Config.Encode)))
              {
                  writer.Write(message + "\n");
                  writer.Close();
              }
            }
        }

        static FileStream CreateFileStream()
        {
            return new FileStream(
                Application.Config.LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite
            );
        }

        static string Decorate(string status, string message, string ipAddress)
        {
            return string.Format("{0}\t[{1}]\t[{2}]\t{3}", ipAddress, DateTime.Now, status, message);
        }
    }
}
