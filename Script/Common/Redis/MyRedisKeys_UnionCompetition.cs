using longid = System.Int64;

namespace Script
{
    public static class GroupUnionCompetitionKey
    {
        public static string Info() => "groupUnionCompetition:info";

        public static string TakeLockControl() => "groupUnionCompetition:takeLockControl";
        public static string LockedHash() => "groupUnionCompetition:lockedHash";
        public static string LockPrefix() => "lock:groupUnionCompetition";
        public static class LockKey
        {
            public static string GroupUnionCompetition() => LockPrefix();
        }
    }
}