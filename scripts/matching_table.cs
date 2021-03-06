using System;

namespace MatchingServer
{
    public class MatchingTable
    {
        public string id;
        public string host;
        public string guest;

        public MatchingTable(string id, string host, string guest)
        {
            this.id = id;
            this.host = host;
            this.guest = guest;
        }

        public bool IsWaiting()
        {
            return host == "" || guest == "";
        }

        public bool IsHost(string address)
        {
            return host == address;
        }

        public bool IsGuest(string address)
        {
            return guest == address;
        }


        public string GetEnemyAddress(string address)
        {
            if (IsHost(address))
                return guest;
            if (IsGuest(address))
                return host;
            Logger.Error("Illegal address", address);
            throw new Exception();
        }
    }
}
