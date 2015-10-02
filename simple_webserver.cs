using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Mono.Data.Sqlite; // Mac or Linux
//using System.Data.SQlite // Windows

namespace ExampleSimpleWebserver
{
    class Program
    {
        static void Main (string[] args)
        {
            List<string> tList = new List<string>();

            Console.WriteLine("Running sync server.");
            new SyncServer();
        }
    }

    class SqliteHandling
    {
        string filename;

        public SqliteHandling(string filename)
        {
            this.filename = filename;
            CreateTableOnlyOnce();
        }

        public void CreateTableOnlyOnce()
        {
            using (var conn = new SqliteConnection("Data Source=dataset.db"))
            {
                conn.Open();
                using (SqliteTransaction transaction = conn.BeginTransaction())
                {
                    using (SqliteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tList.Add(reader["tbl_name"].ToString());
                            }
                        }

                        if (tList.Count == 0)
                        {
                            Console.WriteLine("Table created.");

                            command.CommandText = "CREATE TABLE matching(id INTEGER PRYMARY KEY, ip_address_1, ip_address_2)";
                            command.ExecuteNonQuery();
                        } else {
                            Console.WriteLine("Table exist.");

                            foreach (string tableName in tList)
                            {
                                Console.WriteLine(tableName);
                            }
                        }
                    }
                    transaction.Commit();
                }
                conn.Close();
            }
        }
    }

    public class SyncServer
    {
        public SyncServer()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8081/");
            listener.Prefixes.Add("http://127.0.0.1:8081/");

            listener.Start();

            while (true)
            {
                try
                {
                    var context = listener.GetContext(); //Block until a connection comes in
                    context.Response.StatusCode = 200;
                    context.Response.SendChunked = true;

                    var bytes = Encoding.UTF8.GetBytes("waiting");
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    context.Response.Close();
                }
                catch (Exception)
                {
                    // Client disconnected or some other error - ignored for this example
                }
            }
        }
    }
}
