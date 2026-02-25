using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSearch_SceneRoomInfo
    {
        [Key(0)]
        public string keyword;
    }

    [MessagePackObject]
    public class ResSearch_SceneRoomInfo
    {
        [Key(0)]
        public List<SceneRoomInfo> roomInfos;
    }
}