using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgSave_ExpeditionPartyPlayerInfo
    {
        [Key(0)]
        public ExpeditionPartyPlayerInfo info;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResSave_ExpeditionPartyPlayerInfo
    {
    }
}
