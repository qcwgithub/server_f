using System.Collections.Generic;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class MsgGetUserCount
    {

    }

    [MessagePackObject]
    public class ResGetUserCount
    {
        public const string online_10min = "online_10min";
        public const string online_10min_total = "online_10min_total";

        public const string online = "online";
        public const string online_total = "online_total";

        public static string active_players(int serverId) => $"s{serverId}.active_players";
        public const string active_players_total = "active_players_total";
        [Key(0)]
        public Dictionary<string, int> dict;
    }
}