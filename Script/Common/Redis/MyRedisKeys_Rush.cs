using longid = System.Int64;

namespace Script
{
    public static class RushKey
    {
        public static string Profile() => "rush:info";

        public static string TakeLockControl() => "rush:takeLockControl";
        public static string LockedHash() => "rush:lockedHash";
        public static string LockPrefix() => "lock:rush";
        public static class LockKey
        {
            public static string Rush() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }

        public static class Player
        {
            public static string Info(longid playerId) => "rush:" + "player:" + playerId + ":info";
        }
        public static string RankingList(int serverId) => $"s{serverId}:rush:rankingList";
    }
}