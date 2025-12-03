using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UnionInfo_maxOf_unionId_by_serverId
    {
        [Key(0)]
        public int serverId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UnionInfo_maxOf_unionId_by_serverId
    {
        [Key(0)]
        public longid result;
    }
}
