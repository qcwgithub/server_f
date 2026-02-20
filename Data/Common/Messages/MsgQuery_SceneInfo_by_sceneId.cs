using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_SceneInfo_by_sceneId
    {
        [Key(0)]
        public long sceneId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_SceneInfo_by_sceneId
    {
        [Key(0)]
        public SceneInfo? result;
    }
}
