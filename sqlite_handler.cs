using System;
using System.Collections.Generic;
using Mono.Data.Sqlite; // Mac or Linux
//using System.Data.SQlite // Windows
//
namespace MatchingServer
{
    public class SqliteHandler
    {
        private static SqliteHandler instance;
        private string filename;
        private readonly string tableDefinition = @"
            CREATE TABLE matching(id INTEGER PRIMARY KEY, host TEXT, guest TEXT)
        ";

        public delegate void CallbackQuery(SqliteCommand command);

        private SqliteHandler()
        {
            this.filename = Application.Config.DatabaseFile;
            CreateTableOnlyOnce();
        }

        public static SqliteHandler GetInstance()
        {
            if (instance == null)
            {
                instance = new SqliteHandler();
            }
            return instance;
        }

        void CreateTableOnlyOnce()
        {
            ExecuteQuery((command) => {
                // テーブルがあるかチェックしてなければ作成
                if (!IsTableExist(command))
                {
                    command.CommandText = tableDefinition;
                    command.ExecuteNonQuery();
                    Logger.Info("Table created");
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
            command.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";
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
