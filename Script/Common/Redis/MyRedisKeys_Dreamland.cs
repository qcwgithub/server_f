using longid = System.Int64;

namespace Script
{
    public static class DreamlandKey
    {
        public static string Profile() => "dreamland:info";

        public static string TakeLockControl() => "dreamland:takeLockControl";
        public static string LockedHash() => "dreamland:lockedHash";
        public static string LockPrefix() => "lock:dreamland";
        public static class LockKey
        {
            public static string Dreamland() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static class Player
        {
            public static string Info(longid playerId) => "dreamland:" + "player:" + playerId + ":info";
        }
        public static string RankingList(int serverId) => $"s{serverId}:dreamland:rankingList";
    }
}