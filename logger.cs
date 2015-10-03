using System;

namespace MatchingServer
{
    class Logger
    {
        public static void Debug(string message)
        {
            Print("[Debug]\t" + message);
        }

        public static void Info(string message)
        {
            Print("[Info]\t" + message);
        }

        public static void Warning(string message)
        {
            Print("[Warning]\t" + message);
        }

        public static void Error(string message)
        {
            Print("[Error]\t" + message);
        }

        static void Print(string message)
        {
            Console.WriteLine(DecorateWithTimeStamp(message));
        }

        static string DecorateWithTimeStamp(string message)
        {
            return DateTime.Now + "\t" + message;
        }
    }
}
