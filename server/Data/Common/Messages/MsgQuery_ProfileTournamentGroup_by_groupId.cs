using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ProfileTournamentGroup_by_groupId
    {
        [Key(0)]
        public longid groupId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ProfileTournamentGroup_by_groupId
    {
        [Key(0)]
        public ProfileTournamentGroup result;
    }
}
