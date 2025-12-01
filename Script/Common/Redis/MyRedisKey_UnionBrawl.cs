using System;
using System.Linq;
using System.Threading.Tasks;
using Data;
using longid = System.Int64;

namespace Script
{
    public static class UnionBrawlKey
    {
        public static string TakeLockControl() => "unionb:takeLockControl";
        public static string LockedHash() => "unionb:lockedHash";
        public static string LockPrefix() => "lock:unionb";
        public static class LockKey
        {
            public static string UnionBrawl() => LockPrefix() + ":unionBrawl";
            public static string Union(longid unionId) => LockPrefix() + ":union:" + unionId;
        }
        public static string Profile() => "unionb:info";
        public static string UnionBrawlUnionSeasonInfo(longid unionId) => "unionb:union:" + unionId + ":seasonInfo";
        public static string SignupQueue() => "unionb:signupQueue";
        public static string PlayerToUnionId() => "unionb:playerToUnionId";
        public static string PlayerRankingList(int groupId) => "unionb:group:" + groupId + ":playerRankingList";
        public static string UnionRankingList(int groupId) => "unionb:group:" + groupId + ":unionRankingList";
        public static string GroupUnionIds(int groupId) => "unionb:group:" + groupId + ":unionIds";
        public static string PublicRecords(int groupId) => "unionb:group:" + groupId + ":publicRecords";
        public static string UnionRecords(int groupId, longid unionId) => "unionb:group:" + groupId + ":union:" + unionId + ":records";
        public static string Like(int season) => $"unionb:season:{season}:like";
    }
    public static class GroupUnionBrawlKey
    {
        public static string Info() => "groupUnionB:info";

        public static string TakeLockControl() => "groupUnionB:takeLockControl";
        public static string LockedHash() => "groupUnionB:lockedHash";
        public static string LockPrefix() => "lock:groupUnionB";
        public static class LockKey
        {
            public static string GroupUnionBrawl() => LockPrefix();
        }
    }
}