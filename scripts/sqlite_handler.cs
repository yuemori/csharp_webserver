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
            CREATE TABLE IF NOT EXISTS matching(id INTEGER PRIMARY KEY, host TEXT, guest TEXT)
        ";

        public delegate void CallbackQuery(SqliteCommand command);

        private SqliteHandler()
        {
            this.filename = Application.Config.DatabaseFile;
            CreateTable();
        }

        public static SqliteHandler GetInstance()
        {
            if (instance == null)
            {
                instance = new SqliteHandler();
            }
            return instance;
        }

        void CreateTable()
        {
            ExecuteQuery((command) => {
                command.CommandText = tableDefinition;
                command.ExecuteNonQuery();
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
    }
}
