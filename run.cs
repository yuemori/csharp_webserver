using System;

namespace MatchingServer
{
    class Run
    {
        static void Main (string[] args)
        {
            bool debug;
            if (args.Length >= 1 && args[0] == "debug")
            {
                Logger.Info("Running server(debug mode).");
                debug = true;
            }
            else
            {
                Logger.Info("Running server.");
                debug = false;
            }
            new Server(debug);
        }
    }
}
