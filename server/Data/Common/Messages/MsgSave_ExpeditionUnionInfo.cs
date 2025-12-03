using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ExpeditionUnionInfo
    {
        [Key(0)]
        public ExpeditionUnionInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ExpeditionUnionInfo
    {
    }
}
