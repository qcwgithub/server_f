using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UnionMatchNpcLineup_by_npcId
    {
        [Key(0)]
        public string npcId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UnionMatchNpcLineup_by_npcId
    {
        [Key(0)]
        public UnionMatchNpcLineup result;
    }
}
