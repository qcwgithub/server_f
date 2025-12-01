using longid = System.Int64;

namespace Script
{
    public static class ExpeditionPartyKey
    {
        public static string Profile() => "expeditionParty:info";

        public static string TakeLockControl() => "expeditionParty:takeLockControl";
        public static string LockedHash() => "expeditionParty:lockedHash";
        public static string LockPrefix() => "lock:expeditionParty";
        public static class LockKey
        {
            public static string ExpeditionParty() => LockPrefix();
            public static string Player(longid playerId) => LockPrefix() + ":player:" + playerId;

            public static string TeamPlayer(longid playerId) => LockPrefix() + ":player:" + playerId;
            public static string Team(long teamId) => LockPrefix() + ":team:" + teamId;
        }

        public static class Team
        {
            public static string Info(long teamId) => $"expeditionParty:team:{teamId}:info";
            public static string MaxTeamId() => $"expeditionParty:team:maxTeamId";
            public static string PlayerToTeamId() => $"expeditionParty:team:playerToTeamId";
            public static string AllTeamIds(int groupId) => $"expeditionParty:team:allIds:{groupId}";
        }
        public static string TeamName() => $"expeditionParty:name:team";

        public static class Player
        {
            public static string Info(longid playerId) => "expeditionParty:" + "player:" + playerId + ":info";
        }
        public static string RankingList(int groupId) => $"expeditionParty:rankingList:{groupId}";
        public static string TeamRankingList(int groupId) => $"expeditionParty:teamRankingList:{groupId}";
        public static string PlayerOpponent(int groupId) => $"expeditionParty:playerOpponent:{groupId}";
    }
    public static class GroupExpeditionPartyKey
    {
        public static string Info() => "groupExpeditionParty:info";

        public static string TakeLockControl() => "groupExpeditionParty:takeLockControl";
        public static string LockedHash() => "groupExpeditionParty:lockedHash";
        public static string LockPrefix() => "lock:groupExpeditionParty";
        public static class LockKey
        {
            public static string GroupExpeditionParty() => LockPrefix();
        }
    }
}