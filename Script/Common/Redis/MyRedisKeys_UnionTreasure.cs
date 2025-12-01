using longid = System.Int64;

namespace Script
{
    public static class UnionTreasureKey
    {
        public static string Profile() => "unionTreasure:info";

        public static string TakeLockControl() => "unionTreasure:takeLockControl";
        public static string LockedHash() => "unionTreasure:lockedHash";
        public static string LockPrefix() => "lock:unionTreasure";
        public static class LockKey
        {
            public static string UnionTreasure() => LockPrefix();
        }
    }
    public static class GroupUnionTreasureKey
    {
        public static string Info() => "groupUnionTreasure:info";

        public static string TakeLockControl() => "groupUnionTreasure:takeLockControl";
        public static string LockedHash() => "groupUnionTreasure:lockedHash";
        public static string LockPrefix() => "lock:groupUnionTreasure";
        public static class LockKey
        {
            public static string GroupUnionTreasure() => LockPrefix();
        }
    }
}