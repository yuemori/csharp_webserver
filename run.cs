using System;

namespace MatchingServer
{
    class Run
    {
        static void Main (string[] args)
        {
            SqliteHandling sqlite = new SqliteHandling("dataset.db");
            sqlite.CreateTableOnlyOnce();

            Console.WriteLine("Running sync server.");
            new Server();
        }
    }
}
