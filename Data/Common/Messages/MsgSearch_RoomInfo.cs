using System.Collections.Generic;
using MessagePack;
namespace Data
{
    [MessagePackObject]
    public class MsgSearch_RoomInfo
    {
        [Key(0)]
        public string keyword;
    }

    [MessagePackObject]
    public class ResSearch_RoomInfo
    {
        [Key(0)]
        public List<RoomInfo> roomInfos;
    }
}