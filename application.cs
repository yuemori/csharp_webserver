using System;

namespace MatchingServer
{
    public class Application
    {
        static readonly string CONFIG_FILE = "config.json";
        static Config config;

        static void Main (string[] args)
        {
            config = new Config(CONFIG_FILE);
            ConfigCheck();

            Server server = new Server(IsDebugMode(args));
            server.Start();
        }

        public static Config Config
        {
            get { return config; }
        }

        static void ConfigCheck()
        {
            if (config.IsValid())
                return;

            Console.WriteLine("Config file error");
            throw new Exception();
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
