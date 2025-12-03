using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_TournamentRankPlayerInfo_by_playerId
    {
        [Key(0)]
        public longid start_playerId;
        [Key(1)]
        public longid end_playerId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_TournamentRankPlayerInfo_by_playerId
    {
        [Key(0)]
        public List<TournamentRankPlayerInfo> result;
    }
}
