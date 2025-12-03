using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId
    {
        [Key(0)]
        public longid start_unionId;
        [Key(1)]
        public longid end_unionId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_dictOf_UnionInfo_unionId_name_shortName_by_unionId
    {
        [Key(0)]
        public Dictionary<longid, UnionInfo_name_shortName> result;
    }
}
