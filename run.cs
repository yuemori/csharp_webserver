using System;

namespace MatchingServer
{
    class Run
    {
        static void Main (string[] args)
        {
            Console.WriteLine("Running sync server.");
            new Server();
        }
    }
}
