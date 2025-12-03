using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_ProfileApexGroup_by_groupId
    {
        [Key(0)]
        public longid groupId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_ProfileApexGroup_by_groupId
    {
        [Key(0)]
        public ProfileApexGroup result;
    }
}
