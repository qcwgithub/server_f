using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_AccountInfo_byElementOf_playerIds
    {
        [Key(0)]
        public longid ele_playerIds;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_AccountInfo_byElementOf_playerIds
    {
        [Key(0)]
        public List<AccountInfo> result;
    }
}
