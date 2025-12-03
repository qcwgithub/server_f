using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_UnionSeasonInfo_unionId_all
    {
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_UnionSeasonInfo_unionId_all
    {
        [Key(0)]
        public List<longid> result;
    }
}
