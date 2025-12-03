using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ProfileUnionMatch_by_matchId
    {
        [Key(0)]
        public longid matchId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ProfileUnionMatch_by_matchId
    {
        [Key(0)]
        public ProfileUnionMatch result;
    }
}
