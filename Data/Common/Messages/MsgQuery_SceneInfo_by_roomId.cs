using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_SceneInfo_by_roomId
    {
        [Key(0)]
        public long roomId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_SceneInfo_by_roomId
    {
        [Key(0)]
        public SceneInfo? result;
    }
}
