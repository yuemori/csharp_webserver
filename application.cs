using System;

namespace MatchingServer
{
    public class Application
    {
        static void Main (string[] args)
        {
            Server server = new Server(IsDebugMode(args));
            server.Start();
        }

        static bool IsDebugMode(string[] args)
        {
            if (args.Length >= 1 && args[0] == "debug")
            {
                Logger.Info("Running server(debug mode).");
                return true;
            }
            Logger.Info("Running server.");
            return false;
        }
    }
}
