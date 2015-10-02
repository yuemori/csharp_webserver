using System;
using System.Collections.Generic;
using Mono.Data.Sqlite; // Mac or Linux
//using System.Data.SQlite // Windows
//
namespace Webserver
{
    public class SqliteHandling
    {
        string filename;

        public SqliteHandling(string filename)
        {
            this.filename = filename;
        }

        public void CreateTableOnlyOnce()
        {
            List<string> tList = new List<string>();
            using (var conn = new SqliteConnection("Data Source=" + filename))
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

}
