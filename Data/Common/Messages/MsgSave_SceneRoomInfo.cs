using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSave_SceneRoomInfo
    {
        [Key(0)]
        public long roomId;
        [Key(1)]
        required public SceneRoomInfoNullable roomInfoNullable;
        [Key(2)]
        public SceneRoomInfo? roomInfo_debug;
    }

    [MessagePackObject]
    public class ResSave_SceneRoomInfo
    {
    }
}