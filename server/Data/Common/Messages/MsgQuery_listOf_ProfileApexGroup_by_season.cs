using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_ProfileApexGroup_by_season
    {
        [Key(0)]
        public int season;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_ProfileApexGroup_by_season
    {
        [Key(0)]
        public List<ProfileApexGroup> result;
    }
}
