using System.Collections.Generic;
using MessagePack;

namespace Data
{
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class MsgQuery_SceneRoomInfo_by_roomId
    {
        [Key(0)]
        public long roomId;
    }
    
    //// AUTO CREATED ////
    [MessagePackObject]
    public sealed class ResQuery_SceneRoomInfo_by_roomId
    {
        [Key(0)]
        public SceneRoomInfo? result;
    }
}
