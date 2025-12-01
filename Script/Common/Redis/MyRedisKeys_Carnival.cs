using longid = System.Int64;
using Data;

namespace Script
{
    public static class CarnivalKey
    {
        public static string Profile() => "carnival:info";

        public static string TakeLockControl() => "carnival:takeLockControl";
        public static string LockedHash() => "carnival:lockedHash";
        public static string LockPrefix() => "lock:carnival";
        public static class LockKey
        {
            public static string Carnival() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;
        }
        
        public static class Player
        {
            public static string Info(longid playerId) => "carnival:" + "player:" + playerId + ":info";
        }

        public static string RankingList(stParentServerId parentServerId) => $"s{parentServerId.value}:carnival:rankingList";
    }
    public static class GroupCarnivalKey
    {
        public static string Info() => "groupCarnival:info";

        public static string TakeLockControl() => "groupCarnival:takeLockControl";
        public static string LockedHash() => "groupCarnival:lockedHash";
        public static string LockPrefix() => "lock:groupCarnival";
        public static class LockKey
        {
            public static string GroupCarnival() => LockPrefix();
        }
    }
}