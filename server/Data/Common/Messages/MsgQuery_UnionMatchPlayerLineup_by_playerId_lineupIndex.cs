using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex
    {
        [Key(0)]
        public longid playerId;
        [Key(1)]
        public int lineupIndex;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UnionMatchPlayerLineup_by_playerId_lineupIndex
    {
        [Key(0)]
        public UnionMatchPlayerLineup result;
    }
}
