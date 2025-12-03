using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ExpeditionPlayerInfo_by_playerId
    {
        [Key(0)]
        public longid playerId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ExpeditionPlayerInfo_by_playerId
    {
        [Key(0)]
        public ExpeditionPlayerInfo result;
    }
}
