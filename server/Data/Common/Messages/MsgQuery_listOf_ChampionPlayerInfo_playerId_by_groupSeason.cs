using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_ChampionPlayerInfo_playerId_by_groupSeason
    {
        [Key(0)]
        public int groupSeason;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_ChampionPlayerInfo_playerId_by_groupSeason
    {
        [Key(0)]
        public List<longid> result;
    }
}
