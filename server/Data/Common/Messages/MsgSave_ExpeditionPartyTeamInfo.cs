using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    using ExpeditionPartyTeamInfo = Data.TeamInfo;
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ExpeditionPartyTeamInfo
    {
        [Key(0)]
        public ExpeditionPartyTeamInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ExpeditionPartyTeamInfo
    {
    }
}
