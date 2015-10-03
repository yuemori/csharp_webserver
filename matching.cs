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
            MATCHING
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
            }
            return "";
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
                    Console.WriteLine("Waiting over 2 number");
                    throw new Exception();
            }
        }

        void RegisterAddressToHost(SqliteCommand command, string address)
        {
            Console.WriteLine("Register to host");
            command.CommandText = string.Format(
                "INSERT INTO matching (host, guest) VALUES('{0}', null)",
                address
            );
            command.ExecuteNonQuery();
        }

        void RegisterAddressToGuest(SqliteCommand command, string id, string address)
        {
            Console.WriteLine("Register to guest");
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
                Console.WriteLine("Status is waiting");
                return STATUS.WAITING;
            }

            Console.WriteLine("Status is matching");
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
                    Console.WriteLine("HostAddress not registerd");
                    return RegisterHost(command);
                case 1:
                    Console.WriteLine("HostAddress registerd");
                    return MatchingCheck(matchList[0]);
                default:
                    Console.WriteLine("Matching over 2 number");
                    throw new Exception();
            }
        }

        void MatchStart()
        {
            Console.WriteLine("Matching Start");
            SqliteHandler sqlite = SqliteHandler.GetInstance();
            sqlite.ExecuteQuery((command) => {
                Console.WriteLine("Matching search start: " + hostAddress);
                result = Match(command);
            });
            Console.WriteLine("Matching End\n");
        }
    }
}
