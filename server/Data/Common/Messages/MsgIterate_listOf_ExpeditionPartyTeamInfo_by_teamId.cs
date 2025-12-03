using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ExpeditionPartyTeamInfo = Data.TeamInfo;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_ExpeditionPartyTeamInfo_by_teamId
    {
        [Key(0)]
        public long start_teamId;
        [Key(1)]
        public long end_teamId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_ExpeditionPartyTeamInfo_by_teamId
    {
        [Key(0)]
        public List<ExpeditionPartyTeamInfo> result;
    }
}
