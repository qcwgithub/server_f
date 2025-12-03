using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ExpeditionPartyPlayerInfo_by_playerId
    {
        [Key(0)]
        public longid playerId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ExpeditionPartyPlayerInfo_by_playerId
    {
        [Key(0)]
        public ExpeditionPartyPlayerInfo result;
    }
}
