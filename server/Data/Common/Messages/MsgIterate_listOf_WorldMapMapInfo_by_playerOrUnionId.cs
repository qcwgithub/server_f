using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgIterate_listOf_WorldMapMapInfo_by_playerOrUnionId
    {
        [Key(0)]
        public longid start_playerOrUnionId;
        [Key(1)]
        public longid end_playerOrUnionId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResIterate_listOf_WorldMapMapInfo_by_playerOrUnionId
    {
        [Key(0)]
        public List<WorldMapMapInfo> result;
    }
}
