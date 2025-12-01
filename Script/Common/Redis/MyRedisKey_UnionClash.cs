using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using longid = System.Int64;

namespace Script
{
    public static class UnionClashKey
    {
        public static string TakeLockControl() => "unionClash:takeLockControl";
        public static string LockedHash() => "unionClash:lockedHash";
        public static string LockPrefix() => "lock:unionClash";
        public static class LockKey
        {
            public static string UnionClash() => LockPrefix() + ":unionClash";
            public static string Union(longid unionId) => LockPrefix() + ":union:" + unionId;
        }
        public static string Profile() => "unionClash:info";
        public static string UnionClashUnionSeasonInfo(longid unionId) => "unionClash:union:" + unionId + ":seasonInfo";
        public static string SignupQueue() => "unionClash:signupQueue";
        public static string PlayerToUnionId() => "unionClash:playerToUnionId";
        public static string GroupUnionIds(int groupId) => "unionClash:group:" + groupId + ":unionIds";
        public static string WaitGroupUnionIds() => "unionClash:waitGroupIds";
    }
    public static class GroupUnionClashKey
    {
        public static string Info() => "groupUnionClash:info";

        public static string TakeLockControl() => "groupUnionClash:takeLockControl";
        public static string LockedHash() => "groupUnionClash:lockedHash";
        public static string LockPrefix() => "lock:groupUnionClash";
        public static class LockKey
        {
            public static string GroupUnionClash() => LockPrefix();
        }
    }
}