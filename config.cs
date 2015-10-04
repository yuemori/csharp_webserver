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
        Dictionary<string, object> config;

        public Config(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Config file not exist.");
                throw new ArgumentException();
            }
            this.filePath = configFilePath;
            config = new Dictionary<string, object>();
            LoadConfig();
        }

        public string DatabaseFile
        {
            get { return config["database_file"].ToString(); }
        }

        public string LogFile
        {
            get { return config["log_file"].ToString(); }
        }

        public string Encode
        {
            get { return config["encoding"].ToString(); }
        }

        public List<string> Prefixes
        {
            get {
              List<string> prefixes = new List<string>();
              foreach (object prefix in (List<object>)config["prefixes"])
              {
                  prefixes.Add((string)prefix);
              }
              return prefixes;
            }
        }

        public bool IsValid()
        {
            try
            {
                FileValidation("database_file", DatabaseFile);
                FileValidation("log_file", LogFile);
                EncodingValidation(Encode);
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
                config = Json.Deserialize(jsonString) as Dictionary<string, object>;
            }
            Console.WriteLine("Load Config.");
        }

        void FileValidation(string key, string filePath)
        {
            if (File.Exists(filePath))
                return;

            Console.WriteLine(string.Format("File not exist: {0}, {1}", key, filePath));
            throw new Exception();
        }

        void EncodingValidation(string encodeType)
        {
            try
            {
                Encoding.GetEncoding(encodeType);
            }
            catch
            {
                Console.WriteLine("Illegal Encoding Type: " + encodeType);
                throw new Exception();
            }
        }
    }
}
