using longid = System.Int64;

namespace Script
{
    public static class ExperienceKey
    {
        public static string Profile() => "experience:info";

        public static string TakeLockControl() => "experience:takeLockControl";
        public static string LockedHash() => "experience:lockedHash";
        public static string LockPrefix() => "lock:experience";
        public static class LockKey
        {
            public static string Experience() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }
        public static class Player
        {
            public static string Info(longid playerId) => "experience:" + "player:" + playerId + ":info";
        }
        public static string RankingList(int serverId) => $"s{serverId}:experience:rankingList";
    }
}