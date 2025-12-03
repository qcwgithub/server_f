using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ExpeditionPartyTeamInfo = Data.TeamInfo;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ExpeditionPartyTeamInfo_by_teamId
    {
        [Key(0)]
        public long teamId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ExpeditionPartyTeamInfo_by_teamId
    {
        [Key(0)]
        public ExpeditionPartyTeamInfo result;
    }
}
