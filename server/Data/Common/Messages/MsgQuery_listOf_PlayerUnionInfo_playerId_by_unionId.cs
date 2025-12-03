using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_PlayerUnionInfo_playerId_by_unionId
    {
        [Key(0)]
        public longid unionId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_PlayerUnionInfo_playerId_by_unionId
    {
        [Key(0)]
        public List<longid> result;
    }
}
