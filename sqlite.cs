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
        readonly string tableDefinition = @"
            CREATE TABLE matching(id INTEGER PRYMARY KEY, ip_address_1, ip_address_2)
        ";

        public delegate void CallbackQuery(SqliteCommand command);

        public SqliteHandling(string filename)
        {
            this.filename = filename;
        }

        public void CreateTableOnlyOnce()
        {
            ExecuteQuery((command) => {
                command.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";

                // テーブルがあるかチェックしてなければ作成
                if (!IsTableExist(command))
                {
                    Console.WriteLine("Table created.");
                    command.CommandText = tableDefinition;
                    command.ExecuteNonQuery();
                }
            });
        }

        public void ExecuteQuery(CallbackQuery callback)
        {
            using (var conn = new SqliteConnection("Data Source=" + filename))
            {
                conn.Open();
                using (SqliteTransaction transaction = conn.BeginTransaction())
                {
                    using (SqliteCommand command = conn.CreateCommand())
                    {
                        callback(command);
                    }
                    transaction.Commit();
                }
                conn.Close();
            }
        }

        private bool IsTableExist(SqliteCommand command)
        {
            List<string> tList = new List<string>();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tList.Add(reader["tbl_name"].ToString());
                }
            }

            return tList.Count != 0;
        }
    }
}
