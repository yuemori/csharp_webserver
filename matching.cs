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
            MatchingStart();
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

        string GetMatchingSearchSql()
        {
            return string.Format(
                "SELECT * FROM matching WHERE host = '{0}' OR guest = '{0}'",
                hostAddress
            );
        }

        MatchingTable CreateMatchingTable(SqliteDataReader reader)
        {
            return new MatchingTable(
                reader["id"].ToString(),
                reader["host"].ToString(),
                reader["guest"].ToString()
            );
        }

        STATUS MatchingStart()
        {
            Console.WriteLine("Matching Start");
            SqliteHandler sqlite = SqliteHandler.GetInstance();
            sqlite.ExecuteQuery((command) => {
                Console.WriteLine("Matching search start: " + hostAddress);
                command.CommandText = GetMatchingSearchSql();
                List<MatchingTable> matchList = new List<MatchingTable>();
                // ホストかゲストで登録されてないか検索
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matchList.Add(CreateMatchingTable(reader));
                    }
                }
                switch (matchList.Count)
                {
                    // 0件なら登録されていないので登録する
                    case 0:
                        // マッチング待ちがいないか確認する
                        Console.WriteLine("Waiting search start");
                        command.CommandText = "SELECT * FROM matching WHERE guest = null";
                        List<MatchingTable> waitingList = new List<MatchingTable>();
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                waitingList.Add(CreateMatchingTable(reader));
                            }
                        }
                        switch (waitingList.Count)
                        {
                            // 0件ならマッチング待ちがいないのでhostとして登録する
                            case 0:
                                Console.WriteLine("Waiting not exist");
                                command.CommandText = string.Format(
                                    "INSERT INTO matching (host, guest) VALUES('{0}', null)",
                                    hostAddress
                                );
                                command.ExecuteNonQuery();
                                result = STATUS.WAITING;
                                break;
                            // 1件ならマッチング待ちがいるので登録し、ホストアドレスを登録する
                            case 1:
                                Console.WriteLine("Waiting exist");
                                command.CommandText = string.Format(
                                    "UPDATE matching SET guest = '{0}' WHERE id = {1}",
                                    hostAddress,
                                    waitingList[0].id.ToString()
                                );
                                command.ExecuteNonQuery();
                                enemyAddress = waitingList[0].host;
                                result = STATUS.MATCHING;
                                break;
                            // 2件以上の場合はエラー
                            default:
                                Console.WriteLine("Waiting over 2 number");
                                throw new Exception();
                        }
                        break;
                    // 1件なら登録済みなのでマッチング相手がいるか確認する
                    case 1:
                        Console.WriteLine("HostAddress registerd");
                        // host or guestが空文字ならマッチング相手がいないのでWAITINGを返す
                        if (matchList[0].IsWaiting())
                        {
                            Console.WriteLine("Status is waiting");
                            result = STATUS.WAITING;
                        // どちらもnullでなければマッチング済み
                        } else
                        {
                            Console.WriteLine("Status is matching");
                            if (matchList[0].host == hostAddress)
                            {
                                Console.WriteLine("Host");
                                Console.WriteLine(matchList[0].guest);
                                enemyAddress = matchList[0].guest;
                            } else
                            {
                                Console.WriteLine("Guest");
                                Console.WriteLine(matchList[0].host);
                                enemyAddress = matchList[0].host;
                            }
                            result = STATUS.MATCHING;
                        }
                        break;
                    // 2件以上の場合はエラー
                    default:
                        Console.WriteLine("Matching over 2 number");
                        throw new Exception();
                }
            });
            Console.WriteLine("Matching End\n");
            return result;
        }
    }
}
