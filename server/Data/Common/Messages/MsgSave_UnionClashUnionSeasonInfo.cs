using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_UnionClashUnionSeasonInfo
    {
        [Key(0)]
        public UnionClashUnionSeasonInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_UnionClashUnionSeasonInfo
    {
    }
}
