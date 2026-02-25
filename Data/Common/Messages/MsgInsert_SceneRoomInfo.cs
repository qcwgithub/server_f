using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgInsert_SceneRoomInfo
    {
        [Key(0)]
        public SceneRoomInfo roomInfo;
    }

    [MessagePackObject]
    public class ResInsert_SceneRoomInfo
    {

    }
}