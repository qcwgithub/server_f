using longid = System.Int64;
using Data;

namespace Script
{
    public static class MatchGameKey
    {
        public static string Profile() => "matchGame:info";

        public static string TakeLockControl() => "matchGame:takeLockControl";
        public static string LockedHash() => "matchGame:lockedHash";
        public static string LockPrefix() => "lock:matchGame";
        public static class LockKey
        {
            public static string MatchGame() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }
        
        public static class Player
        {
            public static string Info(longid playerId) => "matchGame:" + "player:" + playerId + ":info";
        }

        public static string RankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:matchGame:rankingList";
    }
    public static class GroupMatchGameKey
    {
        public static string Info() => "groupMatchGame:info";

        public static string TakeLockControl() => "groupMatchGame:takeLockControl";
        public static string LockedHash() => "groupMatchGame:lockedHash";
        public static string LockPrefix() => "lock:groupMatchGame";
        public static class LockKey
        {
            public static string GroupMatchGame() => LockPrefix();
        }
    }
}