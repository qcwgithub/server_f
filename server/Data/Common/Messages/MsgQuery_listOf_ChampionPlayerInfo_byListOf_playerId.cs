using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_ChampionPlayerInfo_byListOf_playerId
    {
        [Key(0)]
        public List<longid> playerIdList;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_ChampionPlayerInfo_byListOf_playerId
    {
        [Key(0)]
        public List<ChampionPlayerInfo> result;
    }
}
