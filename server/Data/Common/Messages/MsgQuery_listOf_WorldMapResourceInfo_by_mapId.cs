using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_listOf_WorldMapResourceInfo_by_mapId
    {
        [Key(0)]
        public string mapId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_listOf_WorldMapResourceInfo_by_mapId
    {
        [Key(0)]
        public List<WorldMapResourceInfo> result;
    }
}
