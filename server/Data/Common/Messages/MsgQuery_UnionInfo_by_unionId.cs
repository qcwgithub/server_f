using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_UnionInfo_by_unionId
    {
        [Key(0)]
        public longid unionId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_UnionInfo_by_unionId
    {
        [Key(0)]
        public UnionInfo result;
    }
}
