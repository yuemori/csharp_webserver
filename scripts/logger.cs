using System;
using System.Text;
using System.IO;

namespace MatchingServer
{
    class Logger
    {
        enum Level {
            Debug, Info, Warning, Error
        }

        public static void Debug(string message)
        {
            Print(Level.Debug, message);
        }

        public static void Debug(string message, string ipAddress)
        {
            Print(Level.Debug, message, ipAddress);
        }

        public static void Info(string message)
        {
            Print(Level.Info, message);
        }

        public static void Info(string message, string ipAddress)
        {
            Print(Level.Info, message, ipAddress);
        }

        public static void Warning(string message)
        {
            Print(Level.Warning, message);
        }

        public static void Warning(string message, string ipAddress)
        {
            Print(Level.Warning, message);
        }

        public static void Error(string message)
        {
            Print(Level.Error, message);
        }

        public static void Error(string message, string ipAddress)
        {
            Print(Level.Error, message);
        }

        static void Print(Level level, string message, string ipAddress)
        {
            var decorateMessage = Decorate(level.ToString(), message, ipAddress);
            Console.WriteLine(decorateMessage);
            FileWrite(decorateMessage);
        }

        static void Print(Level level, string message)
        {
            var decorateMessage = Decorate(level.ToString(), message, "-");
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
