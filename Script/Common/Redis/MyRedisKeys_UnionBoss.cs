using longid = System.Int64;

namespace Script
{
    public static class UnionBossKey
    {
        public static string Profile() => "unionBoss:info";

        public static string TakeLockControl() => "unionBoss:takeLockControl";
        public static string LockedHash() => "unionBoss:lockedHash";
        public static string LockPrefix() => "lock:unionBoss";
        public static class LockKey
        {
            public static string UnionBoss() => LockPrefix();
        }
    }
    public static class GroupUnionBossKey
    {
        public static string Info() => "groupUnionBoss:info";

        public static string TakeLockControl() => "groupUnionBoss:takeLockControl";
        public static string LockedHash() => "groupUnionBoss:lockedHash";
        public static string LockPrefix() => "lock:groupUnionBoss";
        public static class LockKey
        {
            public static string GroupUnionBoss() => LockPrefix();
        }
    }
}