using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using MiniJSON;

namespace MatchingServer
{
    public class Config
    {
        string filePath;
        Dictionary<string, string> config;

        public Config(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Config file not exist.");
                throw new ArgumentException();
            }
            this.filePath = configFilePath;
            config = new Dictionary<string, string>();
            LoadConfig();
        }

        public string DatabaseFile
        {
            get { return config["database_file"]; }
        }

        public string LogFile
        {
            get { return config["log_file"]; }
        }

        public string Encode
        {
            get { return config["encoding"]; }
        }

        public bool IsValid()
        {
            try
            {
                FileValidation("database_file");
                FileValidation("log_file");
                EncodingValidation(config["encoding"]);
            }
            catch
            {
                return false;
            }
            return true;
        }

        void LoadConfig()
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
            {
                string jsonString = reader.ReadToEnd();
                reader.Close();
                var jsonDictionary = Json.Deserialize(jsonString) as Dictionary<string, object>;
                foreach(KeyValuePair<string, object> pair in jsonDictionary)
                {
                    config[pair.Key] = pair.Value.ToString();
                }
            }
            Console.WriteLine("Load Config.");
        }

        void FileValidation(string key)
        {
            if (File.Exists(config[key]))
                return;

            Console.WriteLine(string.Format("File not exist: {0}, {1}", key, config[key]));
            throw new Exception();
        }

        void EncodingValidation(string encoding)
        {
            try
            {
                Encoding.GetEncoding(encoding);
            }
            catch
            {
                Console.WriteLine("Illegal Encoding Type: " + encoding);
                throw new Exception();
            }
        }
    }
}
