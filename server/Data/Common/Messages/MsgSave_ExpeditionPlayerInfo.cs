using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ExpeditionPlayerInfo
    {
        [Key(0)]
        public ExpeditionPlayerInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ExpeditionPlayerInfo
    {
    }
}
