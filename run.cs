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
                Console.WriteLine("Running server(debug mode).");
                debug = true;
            }
            else
            {
                Console.WriteLine("Running server.");
                debug = false;
            }
            new Server(debug);
        }
    }
}
