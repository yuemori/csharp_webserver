namespace MatchingServer
{
    public class Matching
    {
        private string hostAddress;

        public enum STATUS
        {
            WAITING,
            MATCHING
        }

        public Matching(string hostAddress)
        {
            this.hostAddress = hostAddress;
        }

        public string GetResponseValue()
        {
            var status = MatchingStatus();
            switch(status)
            {
                case Matching.STATUS.WAITING:
                    return "waiting";
                case Matching.STATUS.MATCHING:
                    return GetEnemyAddress();
            }
            return "";
        }

        STATUS MatchingStatus()
        {
            return STATUS.WAITING;
        }

        string GetEnemyAddress()
        {
            return "localhost";
        }
    }
}
