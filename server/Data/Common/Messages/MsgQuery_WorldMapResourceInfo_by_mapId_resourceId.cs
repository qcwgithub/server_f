using System.Collections.Generic;
using MessagePack;
using longid = System.Int64;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_WorldMapResourceInfo_by_mapId_resourceId
    {
        [Key(0)]
        public string mapId;
        [Key(1)]
        public string resourceId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_WorldMapResourceInfo_by_mapId_resourceId
    {
        [Key(0)]
        public WorldMapResourceInfo result;
    }
}
