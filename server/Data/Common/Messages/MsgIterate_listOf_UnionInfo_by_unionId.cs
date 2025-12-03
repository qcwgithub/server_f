using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_UnionInfo_by_unionId
    {
        [Key(0)]
        public longid start_unionId;
        [Key(1)]
        public longid end_unionId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_UnionInfo_by_unionId
    {
        [Key(0)]
        public List<UnionInfo> result;
    }
}
