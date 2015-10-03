using System;
using System.Collections.Generic;
using Mono.Data.Sqlite; // Mac or Linux
// using System.Data.Sqlite // Windows

namespace MatchingServer
{
    public class Matching
    {
        private string hostAddress;
        private string enemyAddress;
        private STATUS result;

        public enum STATUS
        {
            WAITING,
            MATCHING,
            ERROR
        }

        public Matching(string hostAddress)
        {
            this.hostAddress = hostAddress;
            MatchStart();
        }

        public string GetResponseValue()
        {
            switch(result)
            {
                case Matching.STATUS.WAITING:
                    return "waiting";
                case Matching.STATUS.MATCHING:
                    return enemyAddress;
                case Matching.STATUS.ERROR:
                    return "error";
                default:
                    Logger.Debug("Found illegal STATUS", hostAddress);
                    throw new Exception();
            }
        }

        MatchingTable CreateMatchingTable(SqliteDataReader reader)
        {
            return new MatchingTable(
                reader["id"].ToString(),
                reader["host"].ToString(),
                reader["guest"].ToString()
            );
        }

        List<MatchingTable> GetMatchingList(SqliteCommand command, string sql)
        {
            command.CommandText = sql;
            List<MatchingTable> matchingList = new List<MatchingTable>();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    matchingList.Add(CreateMatchingTable(reader));
                }
            }
            return matchingList;
        }

        STATUS RegisterHost(SqliteCommand command)
        {
            List<MatchingTable> matchingList = GetMatchingList(command, "SELECT * FROM matching WHERE guest = null");
            switch (matchingList.Count)
            {
                case 0:
                    RegisterAddressToHost(command, hostAddress);
                    return STATUS.WAITING;
                case 1:
                    RegisterAddressToGuest(command, matchingList[0].id, hostAddress);
                    enemyAddress = matchingList[0].host;
                    return STATUS.MATCHING;
                default:
                    Logger.Error("MatchingList found over 2 number", hostAddress);
                    return STATUS.ERROR;
            }
        }

        void RegisterAddressToHost(SqliteCommand command, string address)
        {
            Logger.Info("Register to host", hostAddress);
            command.CommandText = string.Format(
                "INSERT INTO matching (host, guest) VALUES('{0}', null)",
                address
            );
            command.ExecuteNonQuery();
        }

        void RegisterAddressToGuest(SqliteCommand command, string id, string address)
        {
            Logger.Info("Register to guest", hostAddress);
            command.CommandText = string.Format(
                "UPDATE matching SET guest = '{0}' WHERE id = {1}",
                address,
                id
            );
            command.ExecuteNonQuery();
        }

        STATUS MatchingCheck(MatchingTable matching)
        {
            if (matching.IsWaiting())
            {
                Logger.Info("Status is waiting", hostAddress);
                return STATUS.WAITING;
            }

            Logger.Info("Status is matching", hostAddress);
            enemyAddress = matching.GetEnemyAddress(hostAddress);
            return STATUS.MATCHING;
        }

        STATUS Match(SqliteCommand command)
        {
            List<MatchingTable> matchList = GetMatchingList(
                command,
                string.Format( "SELECT * FROM matching WHERE host = '{0}' OR guest = '{0}'", hostAddress)
            );
            switch (matchList.Count)
            {
                case 0:
                    Logger.Info("HostAddress not registerd", hostAddress);
                    return RegisterHost(command);
                case 1:
                    Logger.Info("HostAddress registerd", hostAddress);
                    return MatchingCheck(matchList[0]);
                default:
                    Logger.Error("Matching over 2 number", hostAddress);
                    return STATUS.ERROR;
            }
        }

        void MatchStart()
        {
            SqliteHandler sqlite = SqliteHandler.GetInstance();
            Logger.Info("Request recieved", hostAddress);
            sqlite.ExecuteQuery((command) => {
                Logger.Info("Matching Start", hostAddress);
                result = Match(command);
            });
            Logger.Info("Matching End", hostAddress);
        }
    }
}
